using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CrmAdo.Installation
{
    public class DataProviderConfigInstaller
    {
        public virtual void UpdateConfig(string assemblyname, Version assemblyVersion, string culture, string publicKeyToken, string codeBaseLocation = null)
        {
            string invariant = CrmAdoConstants.Invariant;
            string name = CrmAdoConstants.Name;
            string description = CrmAdoConstants.Description;
            string typeName = CrmAdoConstants.DbProviderFactorTypeName;
            string fullyQualifiedAssemblyName = string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", assemblyname, assemblyVersion.ToString(), culture, publicKeyToken);
            Utils.RegisterDataProviderInMachineConfig(Utils.GetNET40Version(), invariant, name, description, typeName, fullyQualifiedAssemblyName);
            if(!string.IsNullOrEmpty(codeBaseLocation))
            {
                Utils.RegisterAssemblyCodeBaseInMachineConfig(Utils.GetNET40Version(), assemblyname, publicKeyToken, culture, assemblyVersion.ToString(), codeBaseLocation);
            }
        }

        public virtual void RemoveConfig()
        {
            string invariant = CrmAdoConstants.Invariant;
            Utils.UnregisterDataProviderFromMachineConfig(Utils.GetNET40Version(), invariant);
        }

    }
}
