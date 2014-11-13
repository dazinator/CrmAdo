using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Xml;

namespace CrmAdo.Installation
{
    public class Utils
    {

        private static string GetFrameworkFolder(bool x64)
        {
            if (x64)
            {
                return Environment.ExpandEnvironmentVariables("%WINDIR%\\Microsoft.NET\\Framework64");
            }
            else
            {
                return Environment.ExpandEnvironmentVariables("%WINDIR%\\Microsoft.NET\\Framework");
            }
        }

        private static string GetCurrentEnvironmentMachineConfigFilePath(bool x64)
        {
            Version version = Environment.Version;
            return GetMachineConfigFilePath(version, x64);
        }

        private static string GetNET40MachineConfigFilePath(bool x64)
        {
            Version version = GetNET40Version();
            return GetMachineConfigFilePath(version, x64);
        }

        public static string GetMachineConfigFilePath(Version clrVersion, bool x64)
        {
            string frameworkVersionFolder = string.Format("v{0}.{1}.{2}", clrVersion.Major, clrVersion.Minor, clrVersion.Build);
            string frameworkBaseFolder = GetFrameworkFolder(x64);
            string frameworkVersionPath = Path.Combine(frameworkBaseFolder, frameworkVersionFolder);
            var frameworkMachineConfigFilePath = Path.Combine(frameworkVersionPath, "CONFIG\\machine.config");
            if (!File.Exists(frameworkMachineConfigFilePath))
            {
                return null;
            }
            return frameworkMachineConfigFilePath;
        }

        public static Version GetNET40Version()
        {
            Version version = new Version("4.0.30319");
            return version;
        }

        public static void RegisterDataProviderInMachineConfig(Version clrVersion, string invariant, string name, string description, string typeName, string assemblyName)
        {
            string configFilePath = GetMachineConfigFilePath(clrVersion, true);
            string assemblyQualifiedName = string.Format("{0}, {1}", typeName, assemblyName);

            if (!string.IsNullOrEmpty(configFilePath))
            {
                RegisterDataProviderInConfigFile(configFilePath, invariant, name, description, assemblyQualifiedName);
            }

            configFilePath = GetMachineConfigFilePath(clrVersion, false);
            if (!string.IsNullOrEmpty(configFilePath))
            {
                RegisterDataProviderInConfigFile(configFilePath, invariant, name, description, assemblyQualifiedName);
            }
        }

        public static void UnregisterDataProviderFromMachineConfig(Version clrVersion, string invariant)
        {
            string configFilePath = GetMachineConfigFilePath(clrVersion, true);
            if (!string.IsNullOrEmpty(configFilePath))
            {
                RemoveDataProviderFromMachineConfig(configFilePath, invariant);
            }

            configFilePath = GetMachineConfigFilePath(clrVersion, false);
            if (!string.IsNullOrEmpty(configFilePath))
            {
                RemoveDataProviderFromMachineConfig(configFilePath, invariant);
            }
        }

        private static void RegisterDataProviderInConfigFile(string configFilePath, string invariant, string name, string description, string assemblyQualifiedTypeName)
        {
            XmlDocument configXmlDocument = new XmlDocument();
            configXmlDocument.Load(configFilePath);

            // Locate the DbProviderFactories element.
            XmlNode dbProviderFactoriesNodes = configXmlDocument.SelectSingleNode("//DbProviderFactories");
            if (dbProviderFactoriesNodes != null)
            {
                XmlNode crmAdoProviderNode = dbProviderFactoriesNodes.SelectSingleNode(string.Format("add[@invariant='{0}']", invariant));
                if (crmAdoProviderNode == null)
                {
                    // Need to create the crm ado provider node because it doesn't already exist.
                    crmAdoProviderNode = configXmlDocument.CreateElement("add");
                    dbProviderFactoriesNodes.AppendChild(crmAdoProviderNode);
                }
                // Clear the existing node, and set it from scratch incase we have amended it since last deployment
                crmAdoProviderNode.RemoveAll();

                //  Assembly assembly = Assembly.LoadFrom(assemblyFile);
                //  ResourceManager resourceManager = new ResourceManager("Resources", assembly);

                // Set name
                XmlAttribute currentAttribute = configXmlDocument.CreateAttribute("name");
                currentAttribute.Value = name;
                crmAdoProviderNode.Attributes.Append(currentAttribute);

                // Set invariant
                currentAttribute = configXmlDocument.CreateAttribute("invariant");
                currentAttribute.Value = invariant;
                crmAdoProviderNode.Attributes.Append(currentAttribute);

                // Set description
                currentAttribute = configXmlDocument.CreateAttribute("description");
                currentAttribute.Value = description;
                crmAdoProviderNode.Attributes.Append(currentAttribute);

                // Set type
                currentAttribute = configXmlDocument.CreateAttribute("type");
                currentAttribute.Value = assemblyQualifiedTypeName; //typename, assemblyname.fullname
                crmAdoProviderNode.Attributes.Append(currentAttribute);

                // Save changes.
                configXmlDocument.Save(configFilePath);
                Debug.WriteLine("{0} updated successfully.", configFilePath);
            }

        }

        private static void RemoveDataProviderFromMachineConfig(string invariant)
        {
            string configFilePath = GetCurrentEnvironmentMachineConfigFilePath(false);
            if (!string.IsNullOrEmpty(configFilePath))
            {
                RemoveDataProviderFromMachineConfig(configFilePath, invariant);
            }

            configFilePath = GetCurrentEnvironmentMachineConfigFilePath(true);
            if (!string.IsNullOrEmpty(configFilePath))
            {
                RemoveDataProviderFromMachineConfig(configFilePath, invariant);
            }
        }

        private static void RemoveDataProviderFromMachineConfig(string path, string invariant)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            Console.WriteLine("Loaded machine.config");
            XmlNode xmlNodes = xmlDocument.SelectSingleNode("//DbProviderFactories");
            if (xmlNodes != null)
            {
                XmlNode xmlNodes1 = xmlNodes.SelectSingleNode(string.Format("add[@invariant='{0}']", invariant));
                if (xmlNodes1 != null)
                {
                    xmlNodes.RemoveChild(xmlNodes1);
                }
                xmlDocument.Save(path);
                Console.WriteLine("{0} updated successfully.", path);
            }
        }

        public static void RegisterAssemblyCodeBaseInMachineConfig(Version clrVersion, string assemblyName, string publicKeyToken, string culture, string version, string hrefLocation)
        {
            string configFilePath = GetMachineConfigFilePath(clrVersion, true);          
            if (!string.IsNullOrEmpty(configFilePath))
            {
                RegisterAssemblyCodeBaseInMachineConfig(configFilePath, assemblyName, publicKeyToken, culture, version, hrefLocation);
            }

            configFilePath = GetMachineConfigFilePath(clrVersion, false);
            if (!string.IsNullOrEmpty(configFilePath))
            {
                RegisterAssemblyCodeBaseInMachineConfig(configFilePath, assemblyName, publicKeyToken, culture, version, hrefLocation);
            }

        }

        public static void RegisterAssemblyCodeBaseInMachineConfig(string configFilePath, string assemblyName, string publicKeyToken, string culture, string version, string hrefLocation)
        {
            XmlDocument configXmlDocument = new XmlDocument();
            configXmlDocument.Load(configFilePath);

            XmlNode configNode = configXmlDocument.SelectSingleNode("/configuration");
            XmlNode runtimeNode = configXmlDocument.SelectSingleNode("/configuration/runtime");
            if (runtimeNode == null)
            {
                // Need to create the runtime node because it doesn't already exist.
                runtimeNode = configXmlDocument.CreateElement("runtime");
                configNode.AppendChild(runtimeNode);
            }

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(configXmlDocument.NameTable);
            string namespaceUri = "urn:schemas-microsoft-com:asm.v1";
            nsmgr.AddNamespace("ab", namespaceUri);

            var xpathSelector = string.Format("/configuration/runtime/ab:assemblyBinding/ab:dependentAssembly/ab:assemblyIdentity[@name='{0}' and @publicKeyToken='{1}']", assemblyName, publicKeyToken);
            XmlNode assemblyIdentityNode = configXmlDocument.SelectSingleNode(xpathSelector);

            if (assemblyIdentityNode == null)
            {
                var abNode = configXmlDocument.CreateElement("assemblyBinding", namespaceUri);
                runtimeNode.AppendChild(abNode);

                var daNode = configXmlDocument.CreateElement("dependentAssembly", namespaceUri);
                abNode.AppendChild(daNode);

                var aiNode = configXmlDocument.CreateElement("assemblyIdentity ", namespaceUri);
                aiNode.SetAttribute("name", assemblyName);
                aiNode.SetAttribute("publicKeyToken", publicKeyToken);
                aiNode.SetAttribute("culture", culture);

                daNode.AppendChild(aiNode);
                assemblyIdentityNode = aiNode;
            }

            XmlElement codeBaseElement = null;
            var parent = assemblyIdentityNode.ParentNode;
            foreach (var item in parent.ChildNodes)
            {
                var ele = item as XmlElement;
                if (ele != null)
                {
                    if (ele.Name.ToLowerInvariant() == "codebase")
                    {
                        codeBaseElement = ele;
                    }
                }
            }

            if (codeBaseElement == null)
            {
                codeBaseElement = configXmlDocument.CreateElement("codeBase", namespaceUri);
                assemblyIdentityNode.AppendChild(codeBaseElement);
            }

            codeBaseElement.SetAttribute("version", version);
            codeBaseElement.SetAttribute("href", hrefLocation);           




            //            <configuration>
            //   <runtime>
            //      <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
            //         <dependentAssembly>
            //            <assemblyIdentity name="myAssembly"
            //                              publicKeyToken="32ab4ba45e0a69a1"
            //                              culture="neutral" />
            //            <codeBase version="2.0.0.0"
            //                      href="http://www.litwareinc.com/myAssembly.dll"/>
            //         </dependentAssembly>
            //      </assemblyBinding>
            //   </runtime>
            //</configuration>



        }



    }
}
