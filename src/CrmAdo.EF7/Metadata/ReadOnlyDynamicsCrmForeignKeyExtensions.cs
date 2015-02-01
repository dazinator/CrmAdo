using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Metadata
{
    public class ReadOnlyDynamicsCrmForeignKeyExtensions : ReadOnlyRelationalForeignKeyExtensions, IDynamicsCrmForeignKeyExtensions
    {
        protected const string DynamicsCrmNameAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.Name;

        public ReadOnlyDynamicsCrmForeignKeyExtensions(IForeignKey foreignKey)
            : base(foreignKey)
        {
        }

        public override string Name
        {
            get { return ForeignKey[DynamicsCrmNameAnnotation] ?? base.Name; }
        }
    }
}
