using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Metadata
{

    public class DynamicsCrmMetadataExtensionProvider : IRelationalMetadataExtensionProvider
    {
        private RelationalNameBuilder _nameBuilder;

        public virtual IDynamicsCrmModelExtensions Extensions(IModel model)
        {
            return model.DynamicsCrm();
        }

        public virtual IDynamicsCrmEntityTypeExtensions Extensions(IEntityType entityType)
        {
            return entityType.DynamicsCrm();
        }

        public virtual IDynamicsCrmPropertyExtensions Extensions(IProperty property)
        {
            return property.DynamicsCrm();
        }

        public virtual IDynamicsCrmKeyExtensions Extensions(IKey key)
        {
            return key.DynamicsCrm();
        }

        public virtual IDynamicsCrmForeignKeyExtensions Extensions(IForeignKey foreignKey)
        {
            return foreignKey.DynamicsCrm();
        }

        public virtual IDynamicsCrmIndexExtensions Extensions(IIndex index)
        {
            return index.DynamicsCrm();
        }

        public virtual RelationalNameBuilder NameBuilder
        {
            get { return _nameBuilder ?? (_nameBuilder = new RelationalNameBuilder(this)); }

            //[param: NotNull]
            protected set { _nameBuilder = value; }
        }

        IRelationalModelExtensions IRelationalMetadataExtensionProvider.Extensions(IModel model)
        {
            return Extensions(model);
        }

        IRelationalEntityTypeExtensions IRelationalMetadataExtensionProvider.Extensions(IEntityType entityType)
        {
            return Extensions(entityType);
        }

        IRelationalPropertyExtensions IRelationalMetadataExtensionProvider.Extensions(IProperty property)
        {
            return Extensions(property);
        }

        IRelationalKeyExtensions IRelationalMetadataExtensionProvider.Extensions(IKey key)
        {
            return Extensions(key);
        }

        IRelationalForeignKeyExtensions IRelationalMetadataExtensionProvider.Extensions(IForeignKey foreignKey)
        {
            return Extensions(foreignKey);
        }

        IRelationalIndexExtensions IRelationalMetadataExtensionProvider.Extensions(IIndex index)
        {
            return Extensions(index);
        }
    }

}
