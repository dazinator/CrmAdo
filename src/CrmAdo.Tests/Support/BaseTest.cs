using System;
using NUnit.Framework;
using System.EnterpriseServices.Internal;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.GACManagedAccess;
using CrmAdo.IoC;

namespace CrmAdo.Tests
{
    [TestFixture]
    [Category("Unit Test")]
    public abstract class BaseTest<TTestSubject>
        where TTestSubject : class
    {

        private bool EnsureRemovedFromGac(string assemblyName)
        {

            // var query = AssemblyCache.QueryAssemblyInfo(assemblyName);         
            AssemblyCacheUninstallDisposition disp;
            AssemblyCache.UninstallAssembly(assemblyName, null, out disp);
            switch (disp)
            {
                case AssemblyCacheUninstallDisposition.AlreadyUninstalled:
                case AssemblyCacheUninstallDisposition.ReferenceNotFound:
                case AssemblyCacheUninstallDisposition.Uninstalled:
                    return true;

                default:
                    return false;
            }
        }

        [TestFixtureSetUp]
        public virtual void SetUp()
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

        /// <summary>
        /// Resolves an instance of the Test Subject from the Current IoC Container.
        /// </summary>
        /// <returns></returns>
        protected virtual TTestSubject ResolveTestSubjectInstance()
        {
            var instance = ContainerServices.CurrentContainer().Resolve<TTestSubject>();
            return instance;
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