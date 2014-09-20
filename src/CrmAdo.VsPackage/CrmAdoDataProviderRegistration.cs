﻿using Microsoft.VisualStudio.Data.Services.SupportEntities;
using Microsoft.VisualStudio.Shell;
using System;
using CrmAdo;
using System.Reflection;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Collections.Generic;

namespace CrmAdo.DdexProvider
{
    class CrmAdoDataProviderRegistration : RegistrationAttribute
    {
        public override void Register(RegistrationAttribute.RegistrationContext context)
        {

        //    AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

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


                // Register assemblies in GAC - REQUIRED FOR DDEX - which sucks by the way.
                var directory = AssemblyDirectory;

                List<string> assembliesList = new List<string>();
                assembliesList.Add("CrmAdo.dll");
                assembliesList.Add("Microsoft.Xrm.Sdk.dll");
                assembliesList.Add("SQLGeneration.dll");
                
                Publish objPub = new Publish();
                foreach (var item in assembliesList)
                {
                    var assemblyDir = System.IO.Path.Combine(directory, item);
                    objPub.GacInstall(assemblyDir);
                }
               
                //   var assembly = File.ReadAllBytes(crmAdoDiretcory);
                //  AppDomain.CurrentDomain.Load(assembly);
          
               
               
                //to add the assembly - use full path with file name
               


            }
            finally
            {

                if (supportedProviderKey != null)
                    supportedProviderKey.Close();
                if (supportedProvidersKey != null)
                    supportedProvidersKey.Close();
                if (dataSourceKey != null)
                    dataSourceKey.Close();
                if (dataSourceInfoKey != null)
                    dataSourceInfoKey.Close();
                if (supportedObjectsKey != null)
                    supportedObjectsKey.Close();
                if (providerKey != null)
                    providerKey.Close();

            }

            // Register provider in machine config.           
            var factoryType = typeof(CrmDbProviderFactory);
            InstallUtils.RegisterDataProviderInMachineConfig(CrmDbProviderFactory.Invariant, CrmDbProviderFactory.Name, CrmDbProviderFactory.Description, factoryType.FullName, factoryType.Assembly.FullName);

        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName askedAssembly = new AssemblyName(args.Name);

            lock (this)
            {
                Assembly assembly = null;

                //string resourceName = string.Format("Assets.Assemblies.{0}.dll", askedAssembly.Name);
                //using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                //{
                //    if (stream == null)
                //    {
                //        LogManager.Instance.Write(LogMessageTypes.Fatal, string.Format("Can not resolve asked assembly: {0}", askedAssembly.Name));
                //        MessageBox.Show(i18n.CanNotLoadRequiredAssembliesMessage, i18n.CanNotLoadRequiredAssembliesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        Environment.Exit(-1);
                //    }

                //    byte[] assemblyData = new byte[stream.Length];
                //    stream.Read(assemblyData, 0, assemblyData.Length);
                //    assembly = Assembly.Load(assemblyData);
                //}

                //LogManager.Instance.Write(LogMessageTypes.Trace, "Loaded embedded assembly: " + askedAssembly.Name);

                return assembly;
            }
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey(@"DataProviders\{" + GuidList.guidCrmAdo_DdexProviderDataProviderString + @"}");
            context.RemoveKey(@"DataSources\{" + GuidList.guidCrmAdo_DdexProviderDataSourceString + @"}");

            // don't unregister provider as other things may be using it! :(
            //WHY DOES DDEX HAVE TO BE THIS LAME AHHHHHHHHHHHHHH!
            //to remove the assembly - use full path with the file name
            //objPub.GacRemove("AssemblyPath");
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

    }
}
