using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider.SupportEntities
{
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
            : base(site)
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
    
}
