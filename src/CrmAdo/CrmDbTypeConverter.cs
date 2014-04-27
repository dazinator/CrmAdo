using System;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmAdo
{
    public class CrmDbTypeConverter
    {

        //  private IDynamicsAttributeTypeProvider _AttributeTypeProvider;

        public static object ToDbType(object val)
        {
            if (val == null)
            {
                return DBNull.Value;
            }
            //if (metadata == null)
            //{
            //    // possibly throw an exception?
            //    return val;
            //}
            return val;
        }

        public static object ToDbType(EntityReference val)
        {
            if (val == null)
            {
                return DBNull.Value;
            }

            return val.Id;
        }

      






    }
}