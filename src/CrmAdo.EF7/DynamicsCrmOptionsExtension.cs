using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Relational;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrmAdo.EntityFramework.Extensions;

namespace CrmAdo.EntityFramework
{
    public class DynamicsCrmOptionsExtension : RelationalOptionsExtension
    {
        protected override void ApplyServices(EntityServicesBuilder builder)
        {
            //  Check.NotNull(builder, "builder");          
            builder.AddDynamicsCrm();
        }

        //todo: can remove this when next releae of ef comes out as it will be in base class.
        public virtual string MigrationsAssembly { get; set; }

    }

}
