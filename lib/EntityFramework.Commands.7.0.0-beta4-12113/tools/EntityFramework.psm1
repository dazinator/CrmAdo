$ErrorActionPreference = "Stop"

$EFDefaultParameterValues = @{
    ProjectName = ''
    ContextTypeName = ''
}

#
# Use-DbContext
#

Register-TabExpansion Use-DbContext @{
    Context = { param ($context) GetContextTypes $context.Project }
    Project = { GetProjects }
}

function Use-DbContext {
    [CmdletBinding()]
    param ([Parameter(Mandatory = $true)] [string] $Context, [string] $Project)

    $dteProject = GetProject $Project
    $contextTypeName = InvokeOperation $dteProject GetContextType @{ name = $Context }

    $EFDefaultParameterValues.ContextTypeName = $contextTypeName
    $EFDefaultParameterValues.ProjectName = $dteProject.ProjectName
}

#
# Add-Migration
#

Register-TabExpansion Add-Migration @{
    Context = { param ($context) GetContextTypes $context.Project }
    Project = { GetProjects }
    StartupProject = { GetProjects }
}

function Add-Migration {
    [CmdletBinding()]
    param ([Parameter(Mandatory = $true)] [string] $Name, [string] $Context, [string] $Project, [string] $StartupProject)

    $values = ProcessCommonParameters $Context $Project
    $dteProject = $values.Project
    $contextTypeName = $values.ContextTypeName

    $artifacts = InvokeOperation $dteProject $StartupProject AddMigration @{
        migrationName = $Name
        contextTypeName = $contextTypeName
    }

    $artifacts | %{ $dteProject.ProjectItems.AddFromFile($_) | Out-Null }
    $DTE.ItemOperations.OpenFile($artifacts[0]) | Out-Null
    ShowConsole
}

#
# Apply-Migration
#

Register-TabExpansion Apply-Migration @{
    Migration = { param ($context) GetMigrations $context.Context $context.Project }
    Context = { param ($context) GetContextTypes $context.Project }
    Project = { GetProjects }
    StartupProject = { GetProjects }
}

# TODO: WhatIf
function Apply-Migration {
    [CmdletBinding()]
    param ([string] $Migration, [string] $Context, [string] $Project, [string] $StartupProject)

    $values = ProcessCommonParameters $Context $Project
    $dteProject = $values.Project
    $contextTypeName = $values.ContextTypeName

    $targetFrameworkMoniker = GetProperty $dteProject.Properties TargetFrameworkMoniker
    $frameworkName = New-Object System.Runtime.Versioning.FrameworkName $targetFrameworkMoniker
    if ($frameworkName.Identifier -in '.NETCore', 'WindowsPhoneApp') {
        throw 'Apply-Migration should not be used with Phone/Store apps. Instead, call DbContext.Database.AsMigrationsEnabled().ApplyMigrations() at runtime.'
    }

    InvokeOperation $dteProject $StartupProject ApplyMigration @{
        migrationName = $Migration
        contextTypeName = $contextTypeName
    }
}

#
# Update-Database (Obsolete)
#

Register-TabExpansion Update-Database @{
    Migration = { param ($context) GetMigrations $context.Context $context.Project }
    Context = { param ($context) GetContextTypes $context.Project }
    Project = { GetProjects }
    StartupProject = { GetProjects }
}

function Update-Database {
    [CmdletBinding()]
    param ([string] $Migration, [string] $Context, [string] $Project, [string] $StartupProject)

    Write-Warning 'Update-Database is obsolete. Use Apply-Migration instead.'

    Apply-Migration $Migration -Context $Context -Project $Project -StartupProject $StartupProject
}

#
# Script-Migration
#

Register-TabExpansion Script-Migration @{
    From = { param ($context) GetMigrations $context.Context $context.Project }
    To = { param ($context) GetMigrations $context.Context $context.Project }
    Context = { param ($context) GetContextTypes $context.Project }
    Project = { GetProjects }
    StartupProject = { GetProjects }
}

function Script-Migration {
    [CmdletBinding()]
    param ([string] $From, [string] $To, [switch] $Idempotent, [string] $Context, [string] $Project, [string] $StartupProject)

    $values = ProcessCommonParameters $Context $Project
    $dteProject = $values.Project
    $contextTypeName = $values.ContextTypeName

    $script = InvokeOperation $dteProject $StartupProject ScriptMigration @{
        fromMigrationName = $From
        toMigrationName = $To
        idempotent = [bool]$Idempotent
        contextTypeName = $contextTypeName
    }

    try {
        # NOTE: Certain SKUs cannot create new SQL files
        $window = $DTE.ItemOperations.NewFile('General\Sql File')
        $textDocument = $window.Document.Object('TextDocument')
        $editPoint = $textDocument.StartPoint.CreateEditPoint()
        $editPoint.Insert($script)
    }
    catch {
        $fullPath = GetProperty $dteProject.Properties FullPath
        $intermediatePath = GetProperty $dteProject.ConfigurationManager.ActiveConfiguration.Properties IntermediatePath
        $fullIntermediatePath = Join-Path $fullPath $intermediatePath
        $fileName = [IO.Path]::GetRandomFileName()
        $fileName = [IO.Path]::ChangeExtension($fileName, '.sql')
        $scriptFile = Join-Path $fullIntermediatePath $fileName
        $script | Out-File $scriptFile
        $DTE.ItemOperations.OpenFile($scriptFile) | Out-Null
    }

    ShowConsole
}

#
# Remove-Migration
#

Register-TabExpansion Remove-Migration @{
    Context = { param ($context) GetContextTypes $context.Project }
    Project = { GetProjects }
    StartupProject = { GetProjects }
}

function Remove-Migration {
    [CmdletBinding()]
    param ([string] $Context, [string] $Project, [string] $StartupProject)

    $values = ProcessCommonParameters $Context $Project
    $dteProject = $values.Project
    $contextTypeName = $values.ContextTypeName

    $filesToDelete = InvokeOperation $dteProject $StartupProject RemoveMigration @{ contextTypeName = $contextTypeName }

	$filesToDelete | ?{ Test-Path $_ } | %{ (GetProjectItem $dteProject $_).Delete() }
}

#
# (Private Helpers)
#

function GetProjects {
    $projects = Get-Project -All
    $groups = $projects | group Name

    return $projects | %{
        if ($groups | ? Name -eq $_.Name | ? Count -eq 1) {
            return $_.Name
        }

        return $_.ProjectName
    }
}

function GetContextTypes($projectName) {
    $project = GetProject $projectName

    $contextTypes = InvokeOperation $project GetContextTypes -skipBuild

    return $contextTypes | %{ $_.SafeName }
}

function GetMigrations($contextTypeName, $projectName) {
    $values = ProcessCommonParameters $contextTypeName $projectName
    $project = $values.Project
    $contextTypeName = $values.ContextTypeName

    $migrations = InvokeOperation $project GetMigrations @{ contextTypeName = $contextTypeName } -skipBuild

    return $migrations | %{ $_.SafeName }
}

function ProcessCommonParameters($contextTypeName, $projectName) {
    $project = GetProject $projectName

    if (!$contextTypeName -and $project.ProjectName -eq $EFDefaultParameterValues.ProjectName) {
        $contextTypeName = $EFDefaultParameterValues.ContextTypeName
    }

    return @{
        Project = $project
        ContextTypeName = $contextTypeName
    }
}

function GetProject($projectName) {
    if ($projectName) {
        return Get-Project $projectName
    }

    return Get-Project
}

function ShowConsole {
    $componentModel = Get-VSComponentModel
    $powerConsoleWindow = $componentModel.GetService([NuGetConsole.IPowerConsoleWindow])
    $powerConsoleWindow.Show()
}

function InvokeOperation($project, $startupProjectName, $operation, $arguments = @{}, [switch] $skipBuild) {
    $projectName = $project.ProjectName

    Write-Verbose "Using project '$projectName'"

    if (!$skipBuild) {
        Write-Verbose "Build started..."

        $solutionBuild = $DTE.Solution.SolutionBuild
        $solutionBuild.BuildProject($solutionBuild.ActiveConfiguration.Name, $project.UniqueName, $true)
        if ($solutionBuild.LastBuildInfo) {
            throw "Build failed for project '$projectName'."
        }

        Write-Verbose "Build succeeded."
    }

    #Get startup project
    $startupProject = Get-MigrationsStartUpProject $startupProjectName $project
    $startupProjectDirectoryObject = Get-ChildItem $startupProject.FileName | Select-Object Directory

    $startupProjectName = $startupProject.ProjectName
    $startupProjectDirectory = $startupProjectDirectoryObject.Directory.FullName

    if (![Type]::GetType('Microsoft.Data.Entity.Commands.ILogHandler')) {
        $componentModel = Get-VSComponentModel
        $packageInstaller = $componentModel.GetService([NuGet.VisualStudio.IVsPackageInstallerServices])
        $package = $packageInstaller.GetInstalledPackages() | ? Id -eq EntityFramework.Commands |
            sort Version -Descending | select -First 1
        $installPath = $package.InstallPath
        $toolsPath = Join-Path $installPath tools

        Add-Type @(
            Join-Path $toolsPath IHandlers.cs
            Join-Path $toolsPath Handlers.cs
        )
    }

    $logHandler = New-Object Microsoft.Data.Entity.Commands.LogHandler @(
        { param ($message) Write-Warning $message }
        { param ($message) Write-Host $message }
        { param ($message) Write-Verbose $message }
    )

    $outputPath = GetProperty $project.ConfigurationManager.ActiveConfiguration.Properties OutputPath
    $properties = $project.Properties
    $fullPath = GetProperty $properties FullPath
    $targetDir = Join-Path $fullPath $outputPath

    $startupOutputPath = GetProperty $startupProject.ConfigurationManager.ActiveConfiguration.Properties OutputPath
    $startupProperties = $startupProject.Properties
    $startupFullPath = GetProperty $startupProperties FullPath
    $startupTargetDir = Join-Path $startupFullPath $startupOutputPath

    $webConfig = GetProjectItemByString $startupProject 'Web.Config'
    $appConfig = GetProjectItemByString $startupProject 'App.Config'

    Write-Verbose "Using application base '$targetDir'."

    if ($webConfig)
    {
        $configurationFile = $webConfig.Properties.Item('LocalPath').Value
        $dataDirectory = Join-Path $startupProjectDirectory 'App_Data'
        Write-Verbose "Using application configuration '$configurationFile'"
    }
    elseif ($appConfig)
    {
        $configurationFile = $appConfig.Properties.Item('LocalPath').Value
        $dataDirectory = $outputPath
        Write-Verbose "Using application configuration '$configurationFile'"
    }
    else
    {
        Write-Verbose "No configuration file found."
        $dataDirectory = $outputPath
    }

    Write-Verbose "Using data directory '$dataDirectory'"

    $info = New-Object AppDomainSetup -Property @{
        ApplicationBase = $targetDir
        ShadowCopyFiles = 'true'
        ConfigurationFile = $configurationFile
    }

    $domain = [AppDomain]::CreateDomain('EntityFrameworkDesignDomain', $null, $info)
    $domain.SetData("DataDirectory", $dataDirectory)
    try {
        $assemblyName = 'EntityFramework.Commands'
        $typeName = 'Microsoft.Data.Entity.Commands.Executor'
        $targetFileName = GetProperty $properties OutputFileName
        $targetPath = Join-Path $targetDir $targetFileName
        $rootNamespace = GetProperty $properties RootNamespace

        Write-Verbose "Using assembly '$targetFileName'."
        $executor = $domain.CreateInstanceAndUnwrap(
            $assemblyName,
            $typeName,
            $false,
            0,
            $null,
            @(
                [MarshalByRefObject]$logHandler,
                @{
                    targetPath = [string]$targetPath
                    projectDir = $fullPath
                    rootNamespace = $rootNamespace
                }
            ),
            $null,
            $null)

        $resultHandler = New-Object Microsoft.Data.Entity.Commands.ResultHandler
        $currentDirectory = [IO.Directory]::GetCurrentDirectory()

        Write-Verbose "Using current directory '$currentDirectory'."

        [IO.Directory]::SetCurrentDirectory($startupTargetDir)
        try {
            $domain.CreateInstance(
                $assemblyName,
                "$typeName+$operation",
                $false,
                0,
                $null,
                ($executor, [MarshalByRefObject]$resultHandler, $arguments),
                $null,
                $null) | Out-Null
        }
        finally {
            [IO.Directory]::SetCurrentDirectory($currentDirectory)
        }
    }
    finally {
        [AppDomain]::Unload($domain)
    }

    if ($resultHandler.ErrorType) {
        Write-Verbose $resultHandler.ErrorStackTrace

        throw $resultHandler.ErrorMessage
    }
    if ($resultHandler.HasResult) {
        return $resultHandler.Result
    }
}

function GetProperty($properties, $propertyName) {
    $property = $properties.Item($propertyName)
    if (!$property) {
        return $null
    }

    return $property.Value
}

function GetProjectItem($project, $path) {
    $fullPath = GetProperty $project.Properties FullPath
    $itemDirectory = (Split-Path $path.Substring($fullPath.Length) -Parent)

    $projectItems = $project.ProjectItems
    if ($itemDirectory) {
        $directories = $itemDirectory.Split('\')
        $directories | %{
            $projectItems = $projectItems.Item($_).ProjectItems
        }
    }

    $itemName = Split-Path $path -Leaf

    return $projectItems.Item($itemName)
}

function GetProjectItemByString($project, $itemName){
    try
    {
        return $project.ProjectItems.Item($itemName)
    }
    catch [Exception]
    {
    }
}

function Get-MigrationsStartUpProject($name, $fallbackProject)
{    
    $startUpProject = $null

    if ($name)
    {
        $startupProject = Get-Project $name
    }
    else
    {
        $startupProjectPaths = $DTE.Solution.SolutionBuild.StartupProjects

        if ($startupProjectPaths)
        {
            if ($startupProjectPaths.Length -eq 1)
            {
                $startupProjectPath = $startupProjectPaths[0]

                if (!(Split-Path -IsAbsolute $startupProjectPath))
                {
                    $solutionPath = Split-Path $DTE.Solution.Properties.Item('Path').Value
                    $startupProjectPath = Join-Path $solutionPath $startupProjectPath -Resolve
                }

                $startupProject = Get-SolutionProjects | ?{
                    try
                    {
                        $fullName = $_.FullName
                    }
                    catch [NotImplementedException]
                    {
                        return $false
                    }

                    if ($fullName -and $fullName.EndsWith('\'))
                    {
                        $fullName = $fullName.Substring(0, $fullName.Length - 1)
                    }

                    return $fullName -eq $startupProjectPath
                }
            }
            else
            {
                $errorMessage = 'More than one start-up project found.'
            }
        }
        else
        {
            $errorMessage = 'No start-up project found.'
        }
    }

    if (!$startUpProject)
    {
        $fallbackProjectName = $fallbackProject.Name
        Write-Verbose "$errorMessage Using '$fallbackProjectName' instead."

        return $fallbackProject
    }

    $startUpProjectName = $startUpProject.Name
    Write-Verbose "Using start-up project '$startUpProjectName'."

    return $startUpProject
}

function Get-SolutionProjects()
{
    $projects = New-Object System.Collections.Stack
    
    $DTE.Solution.Projects | %{
        $projects.Push($_)
    }
    
    while ($projects.Count -ne 0)
    {
        $project = $projects.Pop();
        
        # NOTE: This line is similar to doing a "yield return" in C#
        $project

        if ($project.ProjectItems)
        {
            $project.ProjectItems | ?{ $_.SubProject } | %{
                $projects.Push($_.SubProject)
            }
        }
    }
}

# SIG # Begin signature block
# MIIj8AYJKoZIhvcNAQcCoIIj4TCCI90CAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCDDz9QgsQpHCJUN
# 0pe5CVhS+6M1SI3p8sKjCKR5iPwpQqCCDZIwggYQMIID+KADAgECAhMzAAAAOI0j
# bRYnoybgAAAAAAA4MA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTEwHhcNMTQxMDAxMTgxMTE2WhcNMTYwMTAxMTgxMTE2WjCBgzEL
# MAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1v
# bmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjENMAsGA1UECxMETU9Q
# UjEeMBwGA1UEAxMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMIIBIjANBgkqhkiG9w0B
# AQEFAAOCAQ8AMIIBCgKCAQEAwt7Wz+K3fxFl/7NjqfNyufEk61+kHLJEWetvnPtw
# 22VpmquQMV7/3itkEfXtbOkAIYLDkMyCGaPjmWNlir3T1fsgo+AZf7iNPGr+yBKN
# 5dM5701OPoaWTBGxEYSbJ5iIOy3UfRjzBeCtSwQ+Q3UZ5kbEjJ3bidgkh770Rye/
# bY3ceLnDZaFvN+q8caadrI6PjYiRfqg3JdmBJKmI9GNG6rsgyQEv2I4M2dnt4Db7
# ZGhN/EIvkSCpCJooSkeo8P7Zsnr92Og4AbyBRas66Boq3TmDPwfb2OGP/DksNp4B
# n+9od8h4bz74IP+WGhC+8arQYZ6omoS/Pq6vygpZ5Y2LBQIDAQABo4IBfzCCAXsw
# HwYDVR0lBBgwFgYIKwYBBQUHAwMGCisGAQQBgjdMCAEwHQYDVR0OBBYEFMbxyhgS
# CySlRfWC5HUl0C8w12JzMFEGA1UdEQRKMEikRjBEMQ0wCwYDVQQLEwRNT1BSMTMw
# MQYDVQQFEyozMTY0MitjMjJjOTkzNi1iM2M3LTQyNzEtYTRiZC1mZTAzZmE3MmMz
# ZjAwHwYDVR0jBBgwFoAUSG5k5VAF04KqFzc3IrVtqMp1ApUwVAYDVR0fBE0wSzBJ
# oEegRYZDaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9jcmwvTWljQ29k
# U2lnUENBMjAxMV8yMDExLTA3LTA4LmNybDBhBggrBgEFBQcBAQRVMFMwUQYIKwYB
# BQUHMAKGRWh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2lvcHMvY2VydHMvTWlj
# Q29kU2lnUENBMjAxMV8yMDExLTA3LTA4LmNydDAMBgNVHRMBAf8EAjAAMA0GCSqG
# SIb3DQEBCwUAA4ICAQCecm6ourY1Go2EsDqVN+I0zXvsz1Pk7qvGGDEWM3tPIv6T
# dVZHTXRrmYdcLnSIcKVGb7ScG5hZEk00vtDcdbNdDDPW2AX2NRt+iUjB5YmlLTo3
# J0ce7mjTaFpGoqyF+//Q6OjVYFXnRGtNz73epdy71XqL0+NIx0Z7dZhz+cPI7IgQ
# C/cqLRN4Eo/+a6iYXhxJzjqmNJZi2+7m4wzZG2PH+hhh7LkACKvkzHwSpbamvWVg
# Dh0zWTjfFuEyXH7QexIHgbR+uKld20T/ZkyeQCapTP5OiT+W0WzF2K7LJmbhv2Xj
# 97tj+qhtKSodJ8pOJ8q28Uzq5qdtCrCRLsOEfXKAsfg+DmDZzLsbgJBPixGIXncI
# u+OKq39vCT4rrGfBR+2yqF16PLAF9WCK1UbwVlzypyuwLhEWr+KR0t8orebVlT/4
# uPVr/wLnudvNvP2zQMBxrkadjG7k9gVd7O4AJ4PIRnvmwjrh7xy796E3RuWGq5eu
# dXp27p5LOwbKH6hcrI0VOSHmveHCd5mh9yTx2TgeTAv57v+RbbSKSheIKGPYUGNc
# 56r7VYvEQYM3A0ABcGOfuLD5aEdfonKLCVMOP7uNQqATOUvCQYMvMPhbJvgfuS1O
# eQy77Hpdnzdq2Uitdp0v6b5sNlga1ZL87N/zsV4yFKkTE/Upk/XJOBbXNedrODCC
# B3owggVioAMCAQICCmEOkNIAAAAAAAMwDQYJKoZIhvcNAQELBQAwgYgxCzAJBgNV
# BAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4w
# HAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xMjAwBgNVBAMTKU1pY3Jvc29m
# dCBSb290IENlcnRpZmljYXRlIEF1dGhvcml0eSAyMDExMB4XDTExMDcwODIwNTkw
# OVoXDTI2MDcwODIxMDkwOVowfjELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hp
# bmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jw
# b3JhdGlvbjEoMCYGA1UEAxMfTWljcm9zb2Z0IENvZGUgU2lnbmluZyBQQ0EgMjAx
# MTCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAKvw+nIQHC6t2G6qghBN
# NLrytlghn0IbKmvpWlCquAY4GgRJun/DDB7dN2vGEtgL8DjCmQawyDnVARQxQtOJ
# DXlkh36UYCRsr55JnOloXtLfm1OyCizDr9mpK656Ca/XllnKYBoF6WZ26DJSJhIv
# 56sIUM+zRLdd2MQuA3WraPPLbfM6XKEW9Ea64DhkrG5kNXimoGMPLdNAk/jj3gcN
# 1Vx5pUkp5w2+oBN3vpQ97/vjK1oQH01WKKJ6cuASOrdJXtjt7UORg9l7snuGG9k+
# sYxd6IlPhBryoS9Z5JA7La4zWMW3Pv4y07MDPbGyr5I4ftKdgCz1TlaRITUlwzlu
# ZH9TupwPrRkjhMv0ugOGjfdf8NBSv4yUh7zAIXQlXxgotswnKDglmDlKNs98sZKu
# HCOnqWbsYR9q4ShJnV+I4iVd0yFLPlLEtVc/JAPw0XpbL9Uj43BdD1FGd7P4AOG8
# rAKCX9vAFbO9G9RVS+c5oQ/pI0m8GLhEfEXkwcNyeuBy5yTfv0aZxe/CHFfbg43s
# TUkwp6uO3+xbn6/83bBm4sGXgXvt1u1L50kppxMopqd9Z4DmimJ4X7IvhNdXnFy/
# dygo8e1twyiPLI9AN0/B4YVEicQJTMXUpUMvdJX3bvh4IFgsE11glZo+TzOE2rCI
# F96eTvSWsLxGoGyY0uDWiIwLAgMBAAGjggHtMIIB6TAQBgkrBgEEAYI3FQEEAwIB
# ADAdBgNVHQ4EFgQUSG5k5VAF04KqFzc3IrVtqMp1ApUwGQYJKwYBBAGCNxQCBAwe
# CgBTAHUAYgBDAEEwCwYDVR0PBAQDAgGGMA8GA1UdEwEB/wQFMAMBAf8wHwYDVR0j
# BBgwFoAUci06AjGQQ7kUBU7h6qfHMdEjiTQwWgYDVR0fBFMwUTBPoE2gS4ZJaHR0
# cDovL2NybC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMvTWljUm9vQ2Vy
# QXV0MjAxMV8yMDExXzAzXzIyLmNybDBeBggrBgEFBQcBAQRSMFAwTgYIKwYBBQUH
# MAKGQmh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljUm9vQ2Vy
# QXV0MjAxMV8yMDExXzAzXzIyLmNydDCBnwYDVR0gBIGXMIGUMIGRBgkrBgEEAYI3
# LgMwgYMwPwYIKwYBBQUHAgEWM2h0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2lv
# cHMvZG9jcy9wcmltYXJ5Y3BzLmh0bTBABggrBgEFBQcCAjA0HjIgHQBMAGUAZwBh
# AGwAXwBwAG8AbABpAGMAeQBfAHMAdABhAHQAZQBtAGUAbgB0AC4gHTANBgkqhkiG
# 9w0BAQsFAAOCAgEAZ/KGpZjgVHkaLtPYdGcimwuWEeFjkplCln3SeQyQwWVfLiw+
# +MNy0W2D/r4/6ArKO79HqaPzadtjvyI1pZddZYSQfYtGUFXYDJJ80hpLHPM8QotS
# 0LD9a+M+By4pm+Y9G6XUtR13lDni6WTJRD14eiPzE32mkHSDjfTLJgJGKsKKELuk
# qQUMm+1o+mgulaAqPyprWEljHwlpblqYluSD9MCP80Yr3vw70L01724lruWvJ+3Q
# 3fMOr5kol5hNDj0L8giJ1h/DMhji8MUtzluetEk5CsYKwsatruWy2dsViFFFWDgy
# cScaf7H0J/jeLDogaZiyWYlobm+nt3TDQAUGpgEqKD6CPxNNZgvAs0314Y9/HG8V
# fUWnduVAKmWjw11SYobDHWM2l4bf2vP48hahmifhzaWX0O5dY0HjWwechz4GdwbR
# BrF1HxS+YWG18NzGGwS+30HHDiju3mUv7Jf2oVyW2ADWoUa9WfOXpQlLSBCZgB/Q
# ACnFsZulP0V3HjXG0qKin3p6IvpIlR+r+0cjgPWe+L9rt0uX4ut1eBrs6jeZeRhL
# /9azI2h15q/6/IvrC4DqaTuv/DDtBEyO3991bWORPdGdVk5Pv4BXIqF4ETIheu9B
# CrE/+6jMpF3BoYibV3FWTkhFwELJm3ZbCoBIa/15n8G9bW1qyVJzEw16UM0xghW0
# MIIVsAIBATCBlTB+MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQ
# MA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9u
# MSgwJgYDVQQDEx9NaWNyb3NvZnQgQ29kZSBTaWduaW5nIFBDQSAyMDExAhMzAAAA
# OI0jbRYnoybgAAAAAAA4MA0GCWCGSAFlAwQCAQUAoIG6MBkGCSqGSIb3DQEJAzEM
# BgorBgEEAYI3AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEVMC8GCSqG
# SIb3DQEJBDEiBCBcYMXQDQ5Vttr2FIBCr8yIZM2DG4zkJbvq2nVF8PDdtDBOBgor
# BgEEAYI3AgEMMUAwPqAkgCIATQBpAGMAcgBvAHMAbwBmAHQAIABBAFMAUAAuAE4A
# RQBUoRaAFGh0dHA6Ly93d3cuYXNwLm5ldC8gMA0GCSqGSIb3DQEBAQUABIIBAG2u
# r8GwOl8zvgcMJaxI01KbSsPWzsCTWMzZ4CW7Vz6pUNWefJbu1XpTO7pHKVdG6pcE
# rmYLp8CEIRTWBJP3o94bsBrNCKFLFQUZLjmTNn57esP45gXTTcbIr7SdCeejgjxl
# rQ8Lj+y9EVH4xLrW1QZ+UVfw6ead+sW1ulesAl1Kiutz9hsko5TZJcIjEAfTk3yU
# VtoKLOKpBm+fcHc2jrG+wkQxF4CPeKkw6CPuSto+xdlxNFGMKczxNl8md2Ee1esj
# iCqnsYvYzjVYdj2S5mjjunKR/Kg6qxz8u6iu+IWLhx7E00el6qspx/NfRyWScU0G
# PqXfYDzcz0lEsf4dqrahghMyMIITLgYKKwYBBAGCNwMDATGCEx4wghMaBgkqhkiG
# 9w0BBwKgghMLMIITBwIBAzEPMA0GCWCGSAFlAwQCAQUAMIIBNQYLKoZIhvcNAQkQ
# AQSgggEkBIIBIDCCARwCAQEGCisGAQQBhFkKAwEwMTANBglghkgBZQMEAgEFAAQg
# arVZsuh4o9avwHLw3+Dudtsto4b6THaHu+pb29fLxhcCBlTBJ+uj9xgTMjAxNTAx
# MzEyMDUyNDEuNzM1WjAHAgEBgAIB9KCBsaSBrjCBqzELMAkGA1UEBhMCVVMxCzAJ
# BgNVBAgTAldBMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQg
# Q29ycG9yYXRpb24xDTALBgNVBAsTBE1PUFIxJzAlBgNVBAsTHm5DaXBoZXIgRFNF
# IEVTTjpCQkVDLTMwQ0EtMkRCRTElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3Rh
# bXAgU2VydmljZaCCDr0wggZxMIIEWaADAgECAgphCYEqAAAAAAACMA0GCSqGSIb3
# DQEBCwUAMIGIMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4G
# A1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMTIw
# MAYDVQQDEylNaWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkgMjAx
# MDAeFw0xMDA3MDEyMTM2NTVaFw0yNTA3MDEyMTQ2NTVaMHwxCzAJBgNVBAYTAlVT
# MRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQK
# ExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1l
# LVN0YW1wIFBDQSAyMDEwMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA
# qR0NvHcRijog7PwTl/X6f2mUa3RUENWlCgCChfvtfGhLLF/Fw+Vhwna3PmYrW/AV
# UycEMR9BGxqVHc4JE458YTBZsTBED/FgiIRUQwzXTbg4CLNC3ZOs1nMwVyaCo0UN
# 0Or1R4HNvyRgMlhgRvJYR4YyhB50YWeRX4FUsc+TTJLBxKZd0WETbijGGvmGgLvf
# YfxGwScdJGcSchohiq9LZIlQYrFd/XcfPfBXday9ikJNQFHRD5wGPmd/9WbAA5ZE
# fu/QS/1u5ZrKsajyeioKMfDaTgaRtogINeh4HLDpmc085y9Euqf03GS9pAHBIAmT
# eM38vMDJRF1eFpwBBU8iTQIDAQABo4IB5jCCAeIwEAYJKwYBBAGCNxUBBAMCAQAw
# HQYDVR0OBBYEFNVjOlyKMZDzQ3t8RhvFM2hahW1VMBkGCSsGAQQBgjcUAgQMHgoA
# UwB1AGIAQwBBMAsGA1UdDwQEAwIBhjAPBgNVHRMBAf8EBTADAQH/MB8GA1UdIwQY
# MBaAFNX2VsuP6KJcYmjRPZSQW9fOmhjEMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6
# Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY1Jvb0NlckF1
# dF8yMDEwLTA2LTIzLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0
# dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljUm9vQ2VyQXV0XzIw
# MTAtMDYtMjMuY3J0MIGgBgNVHSABAf8EgZUwgZIwgY8GCSsGAQQBgjcuAzCBgTA9
# BggrBgEFBQcCARYxaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL1BLSS9kb2NzL0NQ
# Uy9kZWZhdWx0Lmh0bTBABggrBgEFBQcCAjA0HjIgHQBMAGUAZwBhAGwAXwBQAG8A
# bABpAGMAeQBfAFMAdABhAHQAZQBtAGUAbgB0AC4gHTANBgkqhkiG9w0BAQsFAAOC
# AgEAB+aIUQ3ixuCYP4FxAz2do6Ehb7Prpsz1Mb7PBeKp/vpXbRkws8LFZslq3/Xn
# 8Hi9x6ieJeP5vO1rVFcIK1GCRBL7uVOMzPRgEop2zEBAQZvcXBf/XPleFzWYJFZL
# dO9CEMivv3/Gf/I3fVo/HPKZeUqRUgCvOA8X9S95gWXZqbVr5MfO9sp6AG9LMEQk
# IjzP7QOllo9ZKby2/QThcJ8ySif9Va8v/rbljjO7Yl+a21dA6fHOmWaQjP9qYn/d
# xUoLkSbiOewZSnFjnXshbcOco6I8+n99lmqQeKZt0uGc+R38ONiU9MalCpaGpL2e
# Gq4EQoO4tYCbIjggtSXlZOz39L9+Y1klD3ouOVd2onGqBooPiRa6YacRy5rYDkea
# gMXQzafQ732D8OE7cQnfXXSYIghh2rBQHm+98eEA3+cxB6STOvdlR3jo+KhIq/fe
# cn5ha293qYHLpwmsObvsxsvYgrRyzR30uIUBHoD7G4kqVDmyW9rIDVWZeodzOwjm
# mC3qjeAzLhIp9cAvVCch98isTtoouLGp25ayp0Kiyc8ZQU3ghvkqmqMRZjDTu3Qy
# S99je/WZii8bxyGvWbWu3EQ8l1Bx16HSxVXjad5XwdHeMMD9zOZN+w2/XU/pnR4Z
# OC+8z1gFLu8NoFA12u8JJxzVs341Hgi62jbb01+P3nSISRIwggTSMIIDuqADAgEC
# AhMzAAAAUGczu9f74RcaAAAAAABQMA0GCSqGSIb3DQEBCwUAMHwxCzAJBgNVBAYT
# AlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYD
# VQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBU
# aW1lLVN0YW1wIFBDQSAyMDEwMB4XDTE0MDUyMzE3MjAwOVoXDTE1MDgyMzE3MjAw
# OVowgasxCzAJBgNVBAYTAlVTMQswCQYDVQQIEwJXQTEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMQ0wCwYDVQQLEwRNT1BS
# MScwJQYDVQQLEx5uQ2lwaGVyIERTRSBFU046QkJFQy0zMENBLTJEQkUxJTAjBgNV
# BAMTHE1pY3Jvc29mdCBUaW1lLVN0YW1wIFNlcnZpY2UwggEiMA0GCSqGSIb3DQEB
# AQUAA4IBDwAwggEKAoIBAQCRIY8TvquljNudp7VisqDCXFVCo8HCEyCKb4joAJRm
# p2ur6chlKuuE34p8aq2CHsD7bNzEMeB0NOZuEPLP+w/x7c9lJOV1N5JWPfu3sQgN
# Vgfvru3QPfGCl9En/oE4sPhrsOh9JKJMF+AAy+yPno34hhmDkMd/NqSXEsBKsDs9
# qK5Xg699Dfv2m34W9YJr8zZ0LYU3engn/BthrOqAO/JUt22o78D7/gw1/Xvdu6Az
# uMGBdmmSYpTpEAKzA+7wG8B+4m+oGrGaJLcG/H0YwzI8wKJ/XqtUhZazjf/Dh/fZ
# BQtQVIjoEx1M37QBb1QoVURqEMXPoZ3yCbjbplTKPOkbAgMBAAGjggEbMIIBFzAd
# BgNVHQ4EFgQUCARZ9qAvtyG0aAYBtudmgbD9QdswHwYDVR0jBBgwFoAU1WM6XIox
# kPNDe3xGG8UzaFqFbVUwVgYDVR0fBE8wTTBLoEmgR4ZFaHR0cDovL2NybC5taWNy
# b3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMvTWljVGltU3RhUENBXzIwMTAtMDct
# MDEuY3JsMFoGCCsGAQUFBwEBBE4wTDBKBggrBgEFBQcwAoY+aHR0cDovL3d3dy5t
# aWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNUaW1TdGFQQ0FfMjAxMC0wNy0wMS5j
# cnQwDAYDVR0TAQH/BAIwADATBgNVHSUEDDAKBggrBgEFBQcDCDANBgkqhkiG9w0B
# AQsFAAOCAQEAXlD7H173mgNrKxVGBtVH6VTn8VvVlxd6nn7b1OhXVtk3x/otxVfI
# 68ssK3k+fSAXwqsvEHvd3PkW+J0sG0MLf5737fR3AZwv2L16XKUwOfO0cTNtsqQg
# yApyjQZNac+AUxjjvqTSZKw3H4TQEQ5WAyUxTuiClBzoDVcz26zR9BwwgNSlM7z2
# M2tlxKIWgDApn0dYQNdnEpugbCjJ0a9w+pRriRHqkxTP9ISdcQEQ2eZ0MMHPZ4IB
# x7rZ0CAPBL6MKdrim34IhHBTWGj5RzVu57TazbRvUUWUue5+/tzG5Yf+sIo/ING5
# ZZd4QguRrZO5cmbW9tMA2CfVEmnIUPokkaGCA24wggJWAgEBMIHboYGxpIGuMIGr
# MQswCQYDVQQGEwJVUzELMAkGA1UECBMCV0ExEDAOBgNVBAcTB1JlZG1vbmQxHjAc
# BgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjENMAsGA1UECxMETU9QUjEnMCUG
# A1UECxMebkNpcGhlciBEU0UgRVNOOkJCRUMtMzBDQS0yREJFMSUwIwYDVQQDExxN
# aWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2aWNloiUKAQEwCQYFKw4DAhoFAAMVADIm
# 2/E358tdnLwjkN9z70yROwvFoIHCMIG/pIG8MIG5MQswCQYDVQQGEwJVUzETMBEG
# A1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWlj
# cm9zb2Z0IENvcnBvcmF0aW9uMQ0wCwYDVQQLEwRNT1BSMScwJQYDVQQLEx5uQ2lw
# aGVyIE5UUyBFU046NTdGNi1DMUUwLTU1NEMxKzApBgNVBAMTIk1pY3Jvc29mdCBU
# aW1lIFNvdXJjZSBNYXN0ZXIgQ2xvY2swDQYJKoZIhvcNAQEFBQACBQDYd0h+MCIY
# DzIwMTUwMTMxMTIyNjM4WhgPMjAxNTAyMDExMjI2MzhaMHQwOgYKKwYBBAGEWQoE
# ATEsMCowCgIFANh3SH4CAQAwBwIBAAICD38wBwIBAAICGJAwCgIFANh4mf4CAQAw
# NgYKKwYBBAGEWQoEAjEoMCYwDAYKKwYBBAGEWQoDAaAKMAgCAQACAxbjYKEKMAgC
# AQACAwehIDANBgkqhkiG9w0BAQUFAAOCAQEAi4ZLEiTPPbLExiFXfdJaj5Bc4eoH
# nNcbRjb7q4O0Mm3jkjhrYZbPJH2DzJ+nRzHZu9IGAsf5H4Ds4dDlBm+/l7tQ4t65
# 1AzLiHTNjdackA6K4Go9YC1nsuJ+hcRR3N1KMSYJ3anPXWzTSMzAnmkkzxXl+C8v
# dpEJ5QNFUQQ+9Y/pqq/T7A7WI8PQx7OquqgK/OW0Wix+/azlsm0UKswQnAetL7NV
# PdKnCbtpeKpjEYYbzBccL+5KWFBf9WFbBSTqWlPM7EhHQ1Xwe0DBO7at6SM/DAM8
# aNiJGA4RXnyrzf6qgj1RJwKJ7CEnkW5tvfBUw+SiSzW0FZz7pMkeoOGvNjGCAvUw
# ggLxAgEBMIGTMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAw
# DgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24x
# JjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMzAAAAUGcz
# u9f74RcaAAAAAABQMA0GCWCGSAFlAwQCAQUAoIIBMjAaBgkqhkiG9w0BCQMxDQYL
# KoZIhvcNAQkQAQQwLwYJKoZIhvcNAQkEMSIEILQ2UkLaR4bizK0e27sW250UjI0B
# MPfbZ+bSV0NTtk9kMIHiBgsqhkiG9w0BCRACDDGB0jCBzzCBzDCBsQQUMibb8Tfn
# y12cvCOQ33PvTJE7C8UwgZgwgYCkfjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMK
# V2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0
# IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0Eg
# MjAxMAITMwAAAFBnM7vX++EXGgAAAAAAUDAWBBTwYMvZwUBbTr/WrJqLClaMwSFH
# 7TANBgkqhkiG9w0BAQsFAASCAQAzHAw4UNjs+3YlNS3RG3vFSLGoYH6UKWrIHsC8
# 7NGUdMhbjAa6ymVO/fpmXh7XXLE8DWxmgjPc6AA8/xUCetzahmSlCZhXEDHxzVgf
# Pa5DQCSeo7gRnI/GqpDXRWNoEDggTwFcabsNhTrFhzuHAN9XHZqIAtE0JlgIB6LL
# d9aiw9dP+GG/U6ob01HGQcFv7fN3agFjaUvhb6up2OyiQOhiVvnNlBqRUR2hwtDD
# jAfNiZrWVs8livhPL76996hc5IBoOV6iHbVxlRCCjWZMiRPrH/Hst2vcgXQ7dw6W
# m1FqAlFiHP01BRoSOEZ5nTTLl2gsWDPfjoVUvzNWAvA4gZjH
# SIG # End signature block
