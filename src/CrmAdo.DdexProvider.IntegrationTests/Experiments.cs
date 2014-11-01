using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using CrmAdo.Dynamics;
using CrmAdo.Dynamics.Metadata;
using NUnit.Framework;
using System.IO;

namespace CrmAdo.DdexProvider.IntegrationTests
{
    [TestFixture()]
    [Category("Experimental")]
    public class Experiments : BaseTest
    {        
        [Category("Experimentation")]
        [Test]
        [TestCase(TestName = "Experiment for loading icon resource")]
        public void Experiment_For_Loading_Manifest_Resource()
        {

            var ddexAssy = typeof(CrmObjectSelector).Assembly;


            // Get the stream that holds the resource
            // NOTE1: Make sure not to close this stream!
            // NOTE2: Also be very careful to match the case
            //        on the resource name itself

            var names = ddexAssy.GetManifestResourceNames();

            foreach (var item in names)
            {
                Console.WriteLine(item);
            }




        }

    }
}