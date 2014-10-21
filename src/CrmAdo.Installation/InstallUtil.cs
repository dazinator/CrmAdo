using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
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

        private static string GetMachineConfigFilePath(Version clrVersion, bool x64)
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

        public static void RegisterDataProviderInMachineConfig(string invariant, string name, string description, string typeName, string assemblyName)
        {
            string configFilePath = GetCurrentEnvironmentMachineConfigFilePath(true);
            string assemblyQualifiedName = string.Format("{0}, {1}", typeName, assemblyName);

            if (!string.IsNullOrEmpty(configFilePath))
            {
                RegisterDataProviderInConfigFile(configFilePath, invariant, name, description, assemblyQualifiedName);
            }

            configFilePath = GetCurrentEnvironmentMachineConfigFilePath(false);
            if (!string.IsNullOrEmpty(configFilePath))
            {
                RegisterDataProviderInConfigFile(configFilePath, invariant, name, description, assemblyQualifiedName);
            }
        }

        public static void UnregisterDataProviderFromMachineConfig(string invariant)
        {
            string configFilePath = GetCurrentEnvironmentMachineConfigFilePath(true);          

            if (!string.IsNullOrEmpty(configFilePath))
            {
                RemoveDataProviderFromMachineConfig(configFilePath, invariant);
            }

            configFilePath = GetCurrentEnvironmentMachineConfigFilePath(false);
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

    }
}
