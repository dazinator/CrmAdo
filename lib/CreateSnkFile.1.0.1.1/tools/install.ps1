param($installPath, $toolsPath, $package, $project)

$targetFileName = 'CreateSnkFile.targets'
$targetFullFileName = [IO.Path]::Combine($toolsPath, $targetFileName)
$targetUri = New-Object Uri -ArgumentList $targetFullFileName, [UriKind]::Absolute

$msBuildV4Name = 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a';
$msBuildV4 = [System.Reflection.Assembly]::LoadWithPartialName($msBuildV4Name)

if(!$msBuildV4) {
    throw New-Object System.IO.FileNotFoundException("Could not load $msBuildV4Name.");
}


$projectItemExists = $false;
try {
    $dummyProjectItem = $project.ProjectItems.Item("SnkNugetWorkaround.txt");
    $projectItemExists = $true;
} catch [Exception] { 
    $projectDir = [System.IO.Path]::GetDirectoryName($project.FullName);
    $dummyPath = [System.IO.Path]::Combine($projectDir, "SnkNugetWorkaround.txt");

    if([System.IO.File]::Exists($dummyPath)) {
        [System.IO.File]::Delete($dummyPath);
        Write-Host "Removed dummy file $dummyPath.";
    }
}

if($projectItemExists) {    
    if($dummyProjectItem) {
        $dummyProjectItem.Delete();
    }
}

$msBuildV4.GetType('Microsoft.Build.Evaluation.ProjectCollection')::GlobalProjectCollection.GetLoadedProjects($project.FullName) | % {
	$currentProject = $_

	# remove imports from this project 
	$currentProject.Xml.Imports | ? {
		return ($targetFileName -ieq [IO.Path]::GetFileName($_.Project))
	}  | % {  
		$currentProject.Xml.RemoveChild($_);
	}

	$projectUri = New-Object Uri -ArgumentList $currentProject.FullPath, [UriKind]::Absolute
	$relativeUrl = $projectUri.MakeRelative($targetUri)
	$import = $currentProject.Xml.AddImport($relativeUrl)
	$import.Condition = "Exists('$relativeUrl')";
}

Import-Module (Join-Path $toolsPath "mark-as-developmentDependency.psm1");

# Set this NuGet Package to be installed as a Development Dependency.
Set-PackageToBeDevelopmentDependency -PackageId $package.Id -ProjectDirectoryPath ([System.IO.Directory]::GetParent($project.FullName))