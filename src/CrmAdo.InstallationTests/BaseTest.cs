using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.GACManagedAccess;
using CrmAdo.Installation.GAC;

namespace CrmAdo.InstallationTests
{
    [TestFixture]
    [Category("Unit Test")]
    public abstract class BaseTest<TTestSubject>
    {

        private bool EnsureRemovedFromGac(string assemblyName)
        {

            // var query = AssemblyCache.QueryAssemblyInfo(assemblyName);         
            CrmAdo.Installation.GAC.AssemblyCacheUninstallDisposition disp;
            CrmAdo.Installation.GAC.AssemblyCache.UninstallAssembly(assemblyName, null, out disp);
            switch (disp)
            {
                case CrmAdo.Installation.GAC.AssemblyCacheUninstallDisposition.AlreadyUninstalled:
                case CrmAdo.Installation.GAC.AssemblyCacheUninstallDisposition.ReferenceNotFound:
                case CrmAdo.Installation.GAC.AssemblyCacheUninstallDisposition.Uninstalled:
                    return true;

                default:
                    return false;
            }
        }

        [TestFixtureSetUp]
        public virtual void SetUp()
        {
            RemoveCrmAdoFromGac();
        }

        [TestFixtureTearDown]
        public virtual void TearDown()
        {
            RemoveCrmAdoFromGac();
        }

        protected virtual void AddCrmAdoToGac()
        {
            var directory = AssemblyDirectory;

            List<string> assembliesList = new List<string>();
            assembliesList.Add("CrmAdo");
            assembliesList.Add("Microsoft.Xrm.Sdk");
            assembliesList.Add("SQLGeneration");

            foreach (var item in assembliesList)
            {
                var crmAdoDllPath = System.IO.Path.Combine(CrmAdo.Installation.GAC.GacHelper.CurrentAssemblyDirectory, item + ".dll");
                GacHelper.AddToGac(crmAdoDllPath);
            }
        }

        protected virtual void RemoveCrmAdoFromGac()
        {
            var directory = AssemblyDirectory;

            List<string> assembliesList = new List<string>();
            assembliesList.Add("CrmAdo");
            assembliesList.Add("Microsoft.Xrm.Sdk");
            assembliesList.Add("SQLGeneration");

            foreach (var item in assembliesList)
            {
                bool notInGac = EnsureRemovedFromGac(item);
                if (!notInGac)
                {
                    Console.WriteLine("Could not remove: " + item + " from GAC. This will effect unit tests and so SetUp will now fail. ");
                    throw new Exception("Could not remove: " + item + " from GAC");
                }
            }
        }


        protected virtual TTestSubject CreateTestSubject()
        {
            return Activator.CreateInstance<TTestSubject>();
        }

        protected virtual TTestSubject CreateTestSubject(params object[] args)
        {
            return (TTestSubject)Activator.CreateInstance(typeof(TTestSubject), args);
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