using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace CrmAdo.Installation
{

    public class CustomActions
    {      

        [CustomAction]
        public static ActionResult ConfigureCrmAdoDataProvider(Session session)
        {
            try
            {              
                DataProviderConfigInstaller configInstaller = new DataProviderConfigInstaller();

                // string msiPath = session["OriginalDatabase"];
                string assemblyName = session.CustomActionData["CrmAdoAssemblyName"];
                string culture = session.CustomActionData["CrmAdoAssemblyCulture"];
                string publicKeyToken = session.CustomActionData["CrmAdoPublicKeyToken"];
                string versionstring = session.CustomActionData["CrmAdoVersion"];
            
                var version = new Version(versionstring);               
                configInstaller.UpdateConfig(assemblyName, version, culture, publicKeyToken);           

            }
            catch (Exception ex)
            {
                session.Log("ERROR in custom action ConfigureCrmAdoDataProvider {0}", ex.Message);        
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }        

        [CustomAction]
        public static ActionResult RemoveCrmAdoDataProviderConfiguration(Session session)
        {
            try
            {               
                DataProviderConfigInstaller configInstaller = new DataProviderConfigInstaller();
                configInstaller.RemoveConfig();
                return ActionResult.Success;             
            }
            catch (Exception ex)
            {
                session.Log("ERROR in custom action RemoveCrmAdoDataProviderConfiguration {0}", ex.Message);              
                return ActionResult.Failure;
            }


        }        

    }
}
