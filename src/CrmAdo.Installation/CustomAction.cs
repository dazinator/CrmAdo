using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace CrmAdo.Installation
{



    public class CustomActions
    {

        //public static CustomActions()
        //{
        //   // System.Diagnostics.Debugger.Launch();
        //}



        [CustomAction]
        public static ActionResult ConfigureCrmAdoDataProvider(Session session)
        {
            try
            {
                // return ActionResult.Success;
                DataProviderConfigInstaller configInstaller = new DataProviderConfigInstaller();

                // string msiPath = session["OriginalDatabase"];
                string assemblyName = session["CrmAdoAssemblyName"];
                string culture = session["CrmAdoAssemblyCulture"];
                string publicKeyToken = session["CrmAdoPublicKeyToken"];

                // We share the same versioninfo file as CrmAdo so our version is always the same.
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                configInstaller.UpdateConfig(assemblyName, currentVersion, culture, publicKeyToken);

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("ERROR in custom action ConfigureCrmAdoDataProvider {0}", ex.ToString());
                return ActionResult.Failure;
            }

        }



        [CustomAction]
        public static ActionResult RemoveCrmAdoDataProviderConfiguration(Session session)
        {
            try
            {
                //   session.Log("Begin Configure RemoveCrmAdoDataProviderConfiguration Custom Action");
                DataProviderConfigInstaller configInstaller = new DataProviderConfigInstaller();
                configInstaller.RemoveConfig();
                return ActionResult.Success;
                // TODO: Make changes to config file
                //  var factoryType = typeof(CrmDbProviderFactory);
                // Utils.UnregisterDataProviderFromMachineConfig(CrmDbProviderFactory.Invariant);

                //  session.Log("End Configure RemoveCrmAdoDataProviderConfiguration Custom Action");
            }
            catch (Exception ex)
            {
                // session.Log("ERROR in custom action RemoveCrmAdoDataProviderConfiguration {0}",
                //  ex.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

    }
}
