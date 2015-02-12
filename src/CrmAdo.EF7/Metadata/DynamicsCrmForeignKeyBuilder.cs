using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace CrmAdo.EntityFramework.Metadata
{
    
    public class DynamicsCrmForeignKeyBuilder
    {
        private readonly ForeignKey _foreignKey;

        public DynamicsCrmForeignKeyBuilder(ForeignKey foreignKey)
        {
           // Check.NotNull(foreignKey, "foreignKey");

            _foreignKey = foreignKey;
        }

        public virtual DynamicsCrmForeignKeyBuilder Name(string name)
        {
            //Check.NullButNotEmpty(name, "name");
            _foreignKey.DynamicsCrm().Name = name;

            return this;
        }
    }
}
