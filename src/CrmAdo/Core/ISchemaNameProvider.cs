using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Core
{

    public abstract class CrmMetadataNamingConvention
    {     
     //   public static string DefaultSchemaPrefix = "new";
        public virtual string GetAttributeSchemaName(string name)
        {
            return name;
        }

        public virtual string GetRelationshipSchemaName(string relationshipName)
        {
            return relationshipName;
        }

        public virtual string GetAttributeDisplayName(string name)
        {
            return name;
        }

        public virtual string GetAttributeDescription(string name)
        {
            return name;
        }

        public virtual string GetEntitySchemaName(string name)
        {
            return name;
        }

        public virtual string GetEntityLogicalName(string name)
        {
            return name.ToLower();
        }

        public virtual string GetEntityIdAttributeLogicalName(string entityname)
        {
            return string.Format("{0}id", entityname);
        }

        internal string GetEntityDisplayName(string entityName)
        {
            return entityName;
        }

        internal string GetEntityDisplayCollectionName(string entityName)
        {
            return entityName;
        }
    }

    public class DefaultAttributeNamingConvention : CrmMetadataNamingConvention
    {    

    }

    /// <summary>
    /// Defines an object responsible for providing the schema names for dynamics crm objects.
    /// </summary>
    public interface ICrmMetadataNamingProvider
    {
        /// <summary>
        /// Gets the naming convention for namiing attributes.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        CrmMetadataNamingConvention GetAttributeNamingConvention();

    }

    public class CrmAdoCrmMetadataNamingProvider : ICrmMetadataNamingProvider
    {
        public static CrmMetadataNamingConvention Instance = new DefaultAttributeNamingConvention();

        public CrmMetadataNamingConvention GetAttributeNamingConvention()
        {
            return Instance;
        }
    }

}
