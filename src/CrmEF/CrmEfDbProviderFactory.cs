using CrmAdo;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmEf
{
    public class CrmEfDbProviderFactory : CrmDbProviderFactory,  IServiceProvider
    {
        //Implement IServiceProvider
        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(DbProviderServices))
                return CrmDbProviderFactory.Instance;
            else
                return null;
        }
    }
  
}
