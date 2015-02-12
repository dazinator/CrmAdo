using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace CrmAdo.EntityFramework.Metadata
{   
    public class DynamicsCrmEntityBuilder
    {
        private readonly EntityType _entityType;

        public DynamicsCrmEntityBuilder(EntityType entityType)
        {
            //Check.NotNull(entityType, "entityType");

            _entityType = entityType;
        }

        public virtual DynamicsCrmEntityBuilder Table(string tableName)
        {
            //.NullButNotEmpty(tableName, "tableName");

            _entityType.DynamicsCrm().Table = tableName;

            return this;
        }

        public virtual DynamicsCrmEntityBuilder Table(string tableName, string schemaName)
        {
            //Check.NullButNotEmpty(tableName, "tableName");
           // Check.NullButNotEmpty(schemaName, "schemaName");

            _entityType.DynamicsCrm().Table = tableName;
            _entityType.DynamicsCrm().Schema = schemaName;

            return this;
        }
    }
}
