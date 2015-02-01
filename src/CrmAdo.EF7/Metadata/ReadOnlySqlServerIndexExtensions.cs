using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Metadata
{

    public class ReadOnlyDynamicsCrmIndexExtensions : ReadOnlyRelationalIndexExtensions, IDynamicsCrmIndexExtensions
    {
        protected const string DynamicsCrmNameAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.Name;
        protected const string DynamicsCrmClusteredAnnotation = DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.Clustered;

        public ReadOnlyDynamicsCrmIndexExtensions(IIndex index)
            : base(index)
        {
        }

        public override string Name
        {
            get { return Index[DynamicsCrmNameAnnotation] ?? base.Name; }
        }

        public virtual bool? IsClustered
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                var value = Index[DynamicsCrmClusteredAnnotation];
                return value == null ? null : (bool?)bool.Parse(value);
            }
        }
    }
}

