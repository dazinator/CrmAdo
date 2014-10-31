using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using System.Reflection;
using CrmAdo.Installation;

namespace CrmAdo.InstallationTests
{
    [Category("Installation")]
    [TestFixture()]
    public class DataProviderConfigInstallerTests
    {
        [Test]
        public void Should_Be_Able_To_Install_Machine_Config_File_Changes()
        {
            // Ensure crmado dll is in gac.
            //  AddCrmAdoToGac();

            //  var crmAdoDllPath = System.IO.Path.Combine(CrmAdo.Installation.GAC.GacHelper.CurrentAssemblyDirectory, "CrmAdo.dll");
            // CrmAdo.Installation.GAC.GacHelper.AddToGac(crmAdoDllPath);         
            // Now run config updater.
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var sut = new CrmAdo.Installation.DataProviderConfigInstaller();
            sut.UpdateConfig("CrmAdo", currentVersion, "en-gb", "abc");
        }

        [Test]
        public void Should_Be_Able_To_Uninstall_Machine_Config_File_Changes()
        {
            var sut = new DataProviderConfigInstaller();
            sut.RemoveConfig();
        }


    }
}
