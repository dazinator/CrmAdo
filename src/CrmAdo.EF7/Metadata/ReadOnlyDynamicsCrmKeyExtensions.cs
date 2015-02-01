using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework.Metadata
{

    public class ReadOnlyDynamicsCrmKeyExtensions : ReadOnlyRelationalKeyExtensions, IDynamicsCrmKeyExtensions
    {
        protected const string DynamicsCrmNameAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.Name;
        protected const string DynamicsCrmClusteredAnnotation = DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.Clustered;

        public ReadOnlyDynamicsCrmKeyExtensions(IKey key)
            : base(key)
        {
        }

        public override string Name
        {
            get { return Key[DynamicsCrmNameAnnotation] ?? base.Name; }
        }

        public virtual bool? IsClustered
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                // TODO: Issue #700: Annotate associated index object instead
                var value = Key[DynamicsCrmClusteredAnnotation];
                return value == null ? null : (bool?)bool.Parse(value);
            }
        }

    }
}
