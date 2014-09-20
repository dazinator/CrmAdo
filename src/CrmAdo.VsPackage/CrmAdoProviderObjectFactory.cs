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

namespace CrmAdo.DdexProvider
{
    [Guid(GuidList.guidCrmAdo_DdexProviderObjectFactoryString)]
    class CrmAdoProviderObjectFactory : DataProviderObjectFactory
    {
        public override object CreateObject(Type objType)
        {

            //IVsDataConnectionProperties == Microsoft.VisualStudio.Data.Framework.AdoDotNet.AdoDotNetConnectionProperties
            //IVsDataConnectionUIProperties == Microsoft.VisualStudio.Data.Framework.AdoDotNet.AdoDotNetConnectionProperties
            //IVsDataConnectionSupport == Microsoft.VisualStudio.Data.Framework.AdoDotNet.AdoDotNetConnectionSupport

            //IDSRefBuilder == DSRefBuilder
            //IVsDataObjectSupport == Microsoft.VisualStudio.Data.Framework.DataObjectSupport
            //IVsDataViewSupport == Microsoft.VisualStudio.Data.Framework.DataViewSupport


            //IVsDataCommand == 
            //IVsDataConnectionEquivalencyComparer == System.Data.RSSBus.DynamicsCRM.DynamicsCRMConnectionEquivalencyComparer      
            //IVsDataMappedObjectConverter == 
            //IVsDataObjectIdentifierConverter == 
            //IVsDataObjectIdentifierResolver
            //IVsDataObjectMemberComparer
            //IVsDataObjectSelector          
            //IVsDataSourceInformation ==          


            if (objType == typeof(IVsDataConnectionProperties) || objType == typeof(IVsDataConnectionUIProperties))
                return new AdoDotNetConnectionProperties();
            else if (objType == typeof(IVsDataConnectionSupport))
                return new AdoDotNetConnectionSupport();
            else if (objType == typeof(IDSRefBuilder))
                return new Microsoft.VisualStudio.Data.Framework.DSRefBuilder();
            else if (objType == typeof(IVsDataObjectSupport))
                return new DataObjectSupport(this.GetType().Namespace + ".CrmObjectSupport", System.Reflection.Assembly.GetExecutingAssembly());
            else if (objType == typeof(IVsDataViewSupport))
                return new DataViewSupport(this.GetType().Namespace + ".CrmViewSupport", System.Reflection.Assembly.GetExecutingAssembly());
            else if (objType == typeof(IVsDataObjectSelector))
                return new CrmObjectSelector();
            else if (objType == typeof(IVsDataSourceInformation))
                return new CrmSourceInformation();

            else if (objType == typeof(IVsDataConnectionUIControl))
                //   return new Microsoft.VisualStudio.Data.Framework.AdoDotNet.data();
            {

            }
            else if (objType == typeof(IVsDataConnectionUIProperties))
            {

            }

            return null;

        }

        public override Type GetType(string typeName)
        {
            return base.GetType(typeName);
        }

        public override System.Reflection.Assembly GetAssembly(string assemblyString)
        {
            return base.GetAssembly(assemblyString);
        }

    }
}
