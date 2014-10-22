using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CrmAdo.Installation
{
    public class DataProviderConfigInstaller
    {
        public virtual void UpdateConfig(Version currentVersion)
        {
            string invariant = CrmAdoConstants.Invariant;
            string name = CrmAdoConstants.Name;
            string description = CrmAdoConstants.Description;
            string typeName = CrmAdoConstants.DbProviderFactorTypeName;

            string typeFullName = string.Empty;
            string assemblyName = string.Empty;

            // var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string fullyQualifiedAssemblyName = string.Format("CrmAdo, Version={0}", currentVersion.ToString());


          



           // Assembly asm = Assembly.Load(fullyQualifiedAssemblyName);

            //   Assembly asm = Assembly.GetExecutingAssembly();
            //if (asm == null)
            //{
            //    throw new Exception("CrmAdo Assembly not found");
            //}
            //assemblyName = asm.FullName;

            //Type t = asm.GetType("CrmAdo.CrmDbProviderFactory");
            //if (t == null)
            //{
            //    throw new Exception("CrmDbProviderFactory type not found");
            //}
            //typeFullName = t.FullName;

           // var info2 = string.Format("{0} {1} {2} {3} {4}", invariant, name, description, typeFullName, assemblyName);
            Utils.RegisterDataProviderInMachineConfig(invariant, name, description, typeName, assemblyName);

            #region nonsense

            //var i = t.GetField("Invariant", BindingFlags.Public);
            //bool invariantFound = i != null;
            //if (invariantFound)
            //{
            //    invariant = (string)i.GetValue(null);
            //    //  session.Log("Invariant: " + invariant);
            //}

            //var n = t.GetField("Name", BindingFlags.Public);
            //bool nameFound = n != null;
            //if (nameFound)
            //{
            //    name = (string)n.GetValue(null);
            //    //  session.Log("Name: " + name);
            //}

            //var d = t.GetField("Description", BindingFlags.Public);
            //bool descriptionFound = d != null;
            //if (descriptionFound)
            //{
            //    description = (string)d.GetValue(null);
            //    //  session.Log("Description: " + description);
            //}

            // MethodInfo m = t.GetMethod("TestMethod");

            // session.Log(info2);
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

            // session.Log("End Configure AddDataProviderToMachineConfig Custom Action");
            #endregion
        }

        public virtual void RemoveConfig()
        {
            string invariant = CrmAdoConstants.Invariant;
            Utils.UnregisterDataProviderFromMachineConfig(invariant);
        }

    }
}
