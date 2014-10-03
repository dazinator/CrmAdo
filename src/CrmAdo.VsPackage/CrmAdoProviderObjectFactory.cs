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
using EnvDTE80;

namespace CrmAdo.DdexProvider
{
    public class CrmAdoDataSourceVersionNumberComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return x.CompareTo(y);
        }
    }

    public class CrmAdoDataSourceVersionComparer : DataSiteableObject<Microsoft.VisualStudio.Data.Services.IVsDataConnection>, IVsDataSourceVersionComparer, IComparable<string>
    {
        private bool _HasSourceVersion = false;

        public CrmAdoDataSourceVersionComparer()
            : this(null)
        {          
        }

        public CrmAdoDataSourceVersionComparer(IVsDataConnection site)
            : this(site, new CrmAdoDataSourceVersionNumberComparer())
        {            
        }

        public CrmAdoDataSourceVersionComparer(IVsDataConnection site, IComparer<string> comparer)
            :base(site)
        {
            Comparer = comparer;
        }

        protected IComparer<string> Comparer { get; set; }

        private string _SourceVersion = string.Empty;
        public string SourceVersion
        {
            get
            {
                if (!this._HasSourceVersion && base.Site != null)
                {
                    IVsDataSourceInformation service = base.Site.GetService(typeof(IVsDataSourceInformation)) as IVsDataSourceInformation;
                    if (service != null)
                    {
                        this._SourceVersion = service["DataSourceVersion"] as string;
                    }
                    this._HasSourceVersion = true;
                }
                return this._SourceVersion;

            }
            set
            {
                _SourceVersion = value;
            }
        }

        public int CompareTo(string other)
        {
            if (this.SourceVersion == null)
            {
                return 0;
            }
            var result = Comparer.Compare(this.SourceVersion, other);

            return result;
        }

    }

    [Guid(GuidList.guidCrmAdo_DdexProviderObjectFactoryString)]
    class CrmAdoProviderObjectFactory : DataProviderObjectFactory
    {
        public override object CreateObject(Type objType)
        {                      

            if (objType == typeof(IVsDataConnectionProperties) || objType == typeof(IVsDataConnectionUIProperties))
                return new AdoDotNetConnectionProperties();
            else if (objType == typeof(IVsDataConnectionSupport))
            {
                var connSupport = new AdoDotNetConnectionSupport();

                //var comparer = new CrmAdoDataSourceVersionComparer();              

                var serviceType = typeof(IVsDataSourceVersionComparer);
                var existingService = connSupport.GetService(serviceType);
                IVsDataConnection existingSite = null;

                if (existingService != null)
                {
                    var existingSitable = (DataSiteableObject<IVsDataConnection>)existingService;
                    existingSite = existingSitable.Site;
                }

                connSupport.RemoveService(serviceType);
                var dsVersionComparer = new CrmAdoDataSourceVersionComparer(existingSite);

                connSupport.SiteChanged += (o, e) =>
                {
                    dsVersionComparer.Site = connSupport.Site;
                };

                connSupport.AddService(serviceType, dsVersionComparer);

                //IServiceProvider provider =  EnvDTE.DTE.

                //connSupport.RemoveService(serviceType);

                //connSupport.AddService(serviceType, (container, type) =>
                //{


                //    // container.AddService(type, dsVersionComparer);
                //    return dsVersionComparer;

                //});


                return connSupport;
            }
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
