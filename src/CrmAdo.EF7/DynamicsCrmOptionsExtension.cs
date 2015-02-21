using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Data.Entity.DynamicsCrm
{
    public class DynamicsCrmOptionsExtension : RelationalOptionsExtension
    {
        protected override void ApplyServices(EntityFrameworkServicesBuilder builder)
        {
            //  Check.NotNull(builder, "builder");          
            builder.AddDynamicsCrm();
        }      

    }

}
