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
            if (objType == typeof(IVsDataConnectionProperties) || objType == typeof(IVsDataConnectionUIProperties))
                return new AdoDotNetConnectionProperties();
            else if (objType == typeof(IVsDataConnectionSupport))
                return new AdoDotNetConnectionSupport();
            else if (objType == typeof(IVsDataObjectSupport))
                return new DataObjectSupport(this.GetType().Namespace + ".CrmObjectSupport", System.Reflection.Assembly.GetExecutingAssembly());
            else if (objType == typeof(IVsDataViewSupport))
                return new DataViewSupport(this.GetType().Namespace + ".CrmViewSupport", System.Reflection.Assembly.GetExecutingAssembly());
            else if (objType == typeof(IVsDataObjectSelector))
                return new CrmObjectSelector();
            else if (objType == typeof(IVsDataSourceInformation))
                return new CrmSourceInformation();
            else if (objType == typeof(IDSRefBuilder))
                return new Microsoft.VisualStudio.Data.Framework.DSRefBuilder();
            return null;

        }

    }
}
