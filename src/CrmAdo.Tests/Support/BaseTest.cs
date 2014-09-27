using System;
using NUnit.Framework;
using System.EnterpriseServices.Internal;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace CrmAdo.Tests
{
    [TestFixture]
    [Category("Unit Test")]
    public abstract class BaseTest<TTestSubject>
    {

        [TestFixtureSetUp]
        public virtual void SetUp()
        {

            var directory = AssemblyDirectory;

            List<string> assembliesList = new List<string>();
            assembliesList.Add("CrmAdo.dll");
            assembliesList.Add("Microsoft.Xrm.Sdk.dll");
            assembliesList.Add("SQLGeneration.dll");

            Publish objPub = new Publish();
            foreach (var item in assembliesList)
            {
                var assemblyDir = System.IO.Path.Combine(directory, item);
                objPub.GacRemove(assemblyDir);
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