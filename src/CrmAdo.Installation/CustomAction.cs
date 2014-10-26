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

        //[CustomAction]
        //public static ActionResult AddToGAC(Session session)
        //{
        //    try
        //    {
        //        var installdirectory = session.CustomActionData["InstallPath"];

        //        var directory = installdirectory;

        //        List<string> assembliesList = new List<string>();
        //        assembliesList.Add("CrmAdo");
        //        assembliesList.Add("Microsoft.Xrm.Sdk");
        //        assembliesList.Add("SQLGeneration");

        //        foreach (var item in assembliesList)
        //        {
        //            bool notInGac = EnsureRemovedFromGac(item);
        //            if (!notInGac)
        //            {
        //                Console.WriteLine("Could not remove: " + item + " from GAC. This will effect unit tests and so SetUp will now fail. ");
        //                throw new Exception("Could not remove: " + item + " from GAC");
        //            }
        //        }
        //        return ActionResult.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        session.Log("ERROR in custom action RemoveCrmAdoDataProviderConfiguration {0}", ex.Message);
        //        return ActionResult.Failure;
        //    }


        //}

    }
}
