using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CrmAdo.DdexProvider;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Framework;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using CrmAdo.DdexProvider.SupportEntities;

namespace CrmAdo.DdexProvider
{


    [Guid(GuidList.guidCrmAdo_DdexProviderObjectFactoryString)]
    class CrmAdoProviderObjectFactory : DataProviderObjectFactory
    {

        public Dictionary<Type, Func<object>> TypeMappings { get; set; }

        public CrmAdoProviderObjectFactory()
        {
            TypeMappings = new Dictionary<Type, Func<object>>();
            TypeMappings.Add(typeof(IVsDataConnectionProperties), () => { return CreateNewConnectionProperties(); });
            TypeMappings.Add(typeof(IVsDataConnectionUIProperties), () => { return CreateNewConnectionProperties(); });
            TypeMappings.Add(typeof(IVsDataConnectionSupport), () => { return CreateNewConnectionSupport(); });
            TypeMappings.Add(typeof(IVsDataObjectSupport), () => { return CreateNewDataObjectSupport(); });
            TypeMappings.Add(typeof(IVsDataViewSupport), () => { return CreateNewDataViewSupport(); });
            //   TypeMappings.Add(typeof(IVsDataObjectSelector), () => { return CreateNewDataObjectSelector(); });
            TypeMappings.Add(typeof(IVsDataConnectionEquivalencyComparer), () => { return CreateNewDataConnectionEquivalencyComparer(null); });
        }



        public override object CreateObject(Type objType)
        {

            var hasMapping = TypeMappings.ContainsKey(objType);
            if (hasMapping)
            {
                var factory = TypeMappings[objType];
                if (factory != null)
                {
                    var instance = factory();
                    return instance;
                }
            }


            //else if (objType == typeof(IVsDataConnectionUIControl))
            ////   return new Microsoft.VisualStudio.Data.Framework.AdoDotNet.data();
            //{

            //}
            //else if (objType == typeof(IVsDataConnectionUIProperties))
            //{

            //}
            //else if (objType == typeof(IVsDataObjectMemberComparer))
            //{

            //}
            //else if (objType == typeof(IVsDataMappedObjectConverter))
            //{
            //    return new CrmAdoDataMappedObjectConverter();
            //}

            return null;

        }

 

        //private object CreateNewDataObjectSelector()
        //{
        //    return new CrmObjectSelector();
        //}

        //private object CreateNewDsRefBuilder()
        //{
        //    return new CrmAdoDsRefBuilder();
        //}    

        private object CreateNewConnectionProperties()
        {
            // ok
            return new CrmAdoConnectionProperties();
        }

        private object CreateNewConnectionSupport()
        {
            // ok
            var connSupport = new CrmAdoConnectionSupport();
            return connSupport;
        }

        private object CreateNewDataObjectSupport()
        {
            // ok
            var dataObjectSupport = new DataObjectSupport(this.GetType().Namespace + ".CrmObjectSupport", System.Reflection.Assembly.GetExecutingAssembly());
            return dataObjectSupport;
        }

        private object CreateNewDataViewSupport()
        {
            // ok
            return new DataViewSupport(this.GetType().Namespace + ".CrmViewSupport", System.Reflection.Assembly.GetExecutingAssembly());
        }

        private object CreateNewDataConnectionEquivalencyComparer(object p)
        {
            // ok
            return new CrmAdoDataConnectionEquivalencyComparer();
        }
    }





}
