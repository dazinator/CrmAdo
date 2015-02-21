using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm.Metadata
{
    public class ReadOnlyDynamicsCrmEntityTypeExtensions : ReadOnlyRelationalEntityTypeExtensions, IDynamicsCrmEntityTypeExtensions
    {
        protected const string DynamicsCrmTableAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.TableName;
        protected const string DynamicsCrmSchemaAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.Schema;

        public ReadOnlyDynamicsCrmEntityTypeExtensions(IEntityType entityType)
            : base(entityType)
        {
        }

        public override string Table
        {
            get { return EntityType[DynamicsCrmTableAnnotation] ?? base.Table; }
        }

        public override string Schema
        {
            get { return EntityType[DynamicsCrmSchemaAnnotation] ?? base.Schema; }
        }
    }

}
