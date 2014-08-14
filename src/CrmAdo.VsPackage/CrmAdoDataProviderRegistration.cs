using Microsoft.VisualStudio.Data.Services.SupportEntities;
using Microsoft.VisualStudio.Shell;
using System;
using CrmAdo;

namespace CrmAdo.DdexProvider
{
    class CrmAdoDataProviderRegistration : RegistrationAttribute
    {
        public override void Register(RegistrationAttribute.RegistrationContext context)
        {

            Key providerKey = null;
            Key supportedObjectsKey = null;
            Key dataSourceInfoKey = null;
            Key dataSourceKey = null;
            Key supportedProvidersKey = null;
            Key supportedProviderKey = null;

            string resourcesNamespace = this.GetType().Namespace + ".Resources";
            try
            {
                providerKey = context.CreateKey(@"DataProviders\{" + GuidList.guidCrmAdo_DdexProviderDataProviderString + @"}");
                providerKey.SetValue(null, "Crm Ado .NET Framework Data Provider");
                providerKey.SetValue("AssociatedSource", "{" + GuidList.guidCrmAdo_DdexProviderDataSourceString + "}");
                providerKey.SetValue("Description", "Provider_Description, " + resourcesNamespace);
                providerKey.SetValue("DisplayName", "Provider_DisplayName, " + resourcesNamespace);
                providerKey.SetValue("FactoryService", "{" + GuidList.guidCrmAdo_DdexProviderObjectFactoryString + "}");
                providerKey.SetValue("InvariantName", CrmDbProviderFactory.Invariant);
                providerKey.SetValue("PlatformVersion", "2.0");
                providerKey.SetValue("ShortDisplayName", "Provider_ShortDisplayName, " + resourcesNamespace);
                providerKey.SetValue("Technology", "{" + GuidList.guidCrmAdo_DdexProviderTechnologyString + "}");

                supportedObjectsKey = providerKey.CreateSubkey("SupportedObjects");
                supportedObjectsKey.CreateSubkey(typeof(IVsDataConnectionProperties).Name);
                supportedObjectsKey.CreateSubkey(typeof(IVsDataConnectionUIProperties).Name);
                supportedObjectsKey.CreateSubkey(typeof(IVsDataConnectionSupport).Name);
                supportedObjectsKey.CreateSubkey(typeof(IVsDataObjectSupport).Name);
                supportedObjectsKey.CreateSubkey(typeof(IVsDataViewSupport).Name);
                supportedObjectsKey.CreateSubkey(typeof(IDSRefBuilder).Name);

                dataSourceInfoKey = supportedObjectsKey.CreateSubkey(typeof(IVsDataSourceInformation).Name);
                dataSourceInfoKey.SetValue("IdentifierOpenQuote", "[");
                dataSourceInfoKey.SetValue("IdentifierCloseQuote", "]");
                dataSourceInfoKey.SetValue("ParameterPrefix", "@");
                dataSourceInfoKey.SetValue("ParameterPrefixInName", "True");
                dataSourceInfoKey.SetValue("SchemaSeperator", "True");
                dataSourceInfoKey.SetValue("SchemaSupported", "True");
                dataSourceInfoKey.SetValue("SchemaSupportedInDml", "True");
                dataSourceInfoKey.SetValue("ServerSeparator", ".");
                dataSourceInfoKey.SetValue("SupportsAnsi92Sql", "True");
                dataSourceInfoKey.SetValue("SupportsQuotedIdentifierParts", "True");
                //  dataSourceInfoKey.SetValue("CatalogSupportedInDml", "True");            
                //  dataSourceInfoKey.SetValue("CatalogSupported", true);
                dataSourceKey = context.CreateKey(@"DataSources\{" + GuidList.guidCrmAdo_DdexProviderDataSourceString + @"}");
                dataSourceKey.SetValue(null, "Dynamics CRM (CrmAdo)");
                dataSourceKey.SetValue("DefaultProvider", "{" + GuidList.guidCrmAdo_DdexProviderDataProviderString + "}");
                supportedProvidersKey = dataSourceKey.CreateSubkey("SupportingProviders");
                supportedProviderKey = supportedProvidersKey.CreateSubkey("{" + GuidList.guidCrmAdo_DdexProviderDataProviderString + "}");
                supportedProviderKey.SetValue("Description", "Provider_Description, " + resourcesNamespace);
                supportedProviderKey.SetValue("DisplayName", "Provider_DisplayName, " + resourcesNamespace);
            }
            finally
            {
                if (supportedProviderKey != null)
                    providerKey.Close();
                if (supportedProvidersKey != null)
                    providerKey.Close();
                if (dataSourceKey != null)
                    providerKey.Close();
                if (dataSourceInfoKey != null)
                    providerKey.Close();
                if (supportedObjectsKey != null)
                    providerKey.Close();
                if (providerKey != null)
                    providerKey.Close();

            }

            // Register provider in machine config.           
            var factoryType = typeof(CrmDbProviderFactory);
            InstallUtils.RegisterDataProviderInMachineConfig(CrmDbProviderFactory.Invariant, CrmDbProviderFactory.Name, CrmDbProviderFactory.Description, factoryType.FullName, factoryType.Assembly.FullName);
            
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey(@"DataProviders\{" + GuidList.guidCrmAdo_DdexProviderDataProviderString + @"}");
            context.RemoveKey(@"DataSources\{" + GuidList.guidCrmAdo_DdexProviderDataSourceString + @"}");

            // don't unregister provider as other things may be using it! :(
            //WHY DOES DDEX HAVE TO BE THIS LAME AHHHHHHHHHHHHHH!
        }
    }
}
