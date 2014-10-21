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

        public static CustomActions()
        {
           // System.Diagnostics.Debugger.Launch();
        }

        [CustomAction]
        public static ActionResult ConfigureCrmAdoDataProvider(Session session)
        {
            try
            {
                UpdateConfig(session);
                // session.Log("Begin ConfigureCrmAdoDataProvider Custom Action");

                // System.Diagnostics.Debugger.Launch();
                // Debugger.Break();
                //Console.WriteLine("Waiting for debugger to attach");
                //while (!Debugger.IsAttached)
                //{
                //    Thread.Sleep(100);
                //}
                //Console.WriteLine("Debugger attached");

                //            public void EnumerateConstants() {        
                //    FieldInfo[] thisObjectProperties = thisObject.GetFields();
                //    foreach (FieldInfo info in thisObjectProperties) {
                //        if (info.IsLiteral) {
                //            //Constant
                //        }
                //    }    
                //}

            }
            catch (Exception ex)
            {
                session.Log("ERROR in custom action ConfigureCrmAdoDataProvider {0}",
                           ex.ToString());
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        public static void UpdateConfig(Session session)
        {
            string invariant = string.Empty;
            string name = string.Empty;
            string description = string.Empty;
            string typeFullName = string.Empty;
            string assemblyName = string.Empty;

            //var info = string.Format("{0} {1} {2} {3} {4}", invariant, name, description, typeFullName, assemblyName);
            //using (var writer = new StreamWriter(@"G:\\Temp\\MyNotes1.txt"))
            //{
            //    writer.Write(info);
            //    writer.Flush();
            //    writer.Close();
            //}

            Assembly asm = Assembly.ReflectionOnlyLoad("CrmAdo");
            bool asmFound = asm != null;
          
            if (asmFound)
            {
                assemblyName = asm.FullName;
                //  session.Log("Asm codebase: " + asm.CodeBase);
            }


            Type t = asm.GetType("CrmDbProviderFactory");
            bool typeFound = t != null;
            // session.Log("type found: " + typeFound);
            if (typeFound)
            {
                typeFullName = t.FullName;
                //  session.Log("Type full name: " + t.FullName);
            }

            var i = t.GetField("Invariant", BindingFlags.Public);
            bool invariantFound = i != null;
            if (invariantFound)
            {
                invariant = (string)i.GetValue(null);
                //  session.Log("Invariant: " + invariant);
            }

            var n = t.GetField("Name", BindingFlags.Public);
            bool nameFound = n != null;
            if (nameFound)
            {
                name = (string)n.GetValue(null);
                //  session.Log("Name: " + name);
            }

            var d = t.GetField("Description", BindingFlags.Public);
            bool descriptionFound = d != null;
            if (descriptionFound)
            {
                description = (string)d.GetValue(null);
                //  session.Log("Description: " + description);
            }

            // MethodInfo m = t.GetMethod("TestMethod");
            var info2 = string.Format("{0} {1} {2} {3} {4}", invariant, name, description, typeFullName, assemblyName);
            session.Log(info2);
            //using (var writer = new StreamWriter(@"G:\\Temp\\MyNotes2.txt"))
            //{
            //    writer.Write(info2);
            //    writer.Flush();
            //    writer.Close();
            //}

            // TODO: Make changes to config file
            // var factoryType = typeof(CrmDbProviderFactory);
            //  throw new Exception();


            //Utils.RegisterDataProviderInMachineConfig(invariant, name, description, t.FullName, t.Assembly.FullName);

            session.Log("End Configure AddDataProviderToMachineConfig Custom Action");
        }

        [CustomAction]
        public static ActionResult RemoveCrmAdoDataProviderConfiguration(Session session)
        {
            try
            {
                //   session.Log("Begin Configure RemoveCrmAdoDataProviderConfiguration Custom Action");

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
