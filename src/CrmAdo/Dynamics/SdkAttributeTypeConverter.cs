using System;
using Microsoft.Xrm.Sdk;

namespace CrmAdo.Dynamics
{
    public class SdkAttributeTypeConverter : ISdkAttributeTypeConverter
    {
        public long GetBigInt(object val)
        {
            var typeCode = Type.GetTypeCode(val.GetType());
            if (typeCode == TypeCode.Int64)
            {
                return (long)val;
            }
            if (typeCode == TypeCode.String)
            {
                long sdkVal;
                if (long.TryParse((string)val, out sdkVal))
                {
                    return sdkVal;
                }
            }
            throw new InvalidOperationException("unable to convert value of type: " + typeCode + " to dynamics " + "BigInt");
        }

        public bool GetBoolean(object value)
        {
            var typeCode = Type.GetTypeCode(value.GetType());
            if (typeCode == TypeCode.Boolean)
            {
                return (bool)value;
            }
            if (typeCode == TypeCode.Int32)
            {
                // bool sdkVal = false;
                var intVal = (int)value;
                if (intVal == 1)
                {
                    return true;
                }
                if (intVal == 0)
                {
                    return false;
                }
            }
            if (typeCode == TypeCode.String)
            {
                bool sdkVal;
                if (bool.TryParse((string)value, out sdkVal))
                {
                    return sdkVal;
                }
            }
           
            throw new InvalidOperationException("unable to convert value of type: " + typeCode + " to dynamics " + "Boolean");
        }

        public EntityReference GetCustomer(object value)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(object value)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(object value)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(object value)
        {
            throw new NotImplementedException();
        }

        public string GetEntityName(object value)
        {
            throw new NotImplementedException();
        }

        public int GetInteger(object value)
        {
            throw new NotImplementedException();
        }

        public EntityReference GetLookup(object value)
        {
            throw new NotImplementedException();
        }

        public object GetManagedProperty(object value)
        {
            throw new NotImplementedException();
        }

        public string GetMemo(object value)
        {
            throw new NotImplementedException();
        }

        public Money GetMoney(object value)
        {
            throw new NotImplementedException();
        }

        public EntityReference GetOwner(object value)
        {
            throw new NotImplementedException();
        }

        public OptionSetValue GetPicklist(object value)
        {
            throw new NotImplementedException();
        }

        public OptionSetValue GetState(object value)
        {
            throw new NotImplementedException();
        }

        public OptionSetValue GetStatus(object value)
        {
            throw new NotImplementedException();
        }

        public string GetString(object value)
        {
            throw new NotImplementedException();
        }

        public Guid GetUniqueIdentifier(object value)
        {
            throw new NotImplementedException();
        }

        #region Not Supported

        public object GetCalendarRules(object value)
        {
            throw new NotImplementedException();
        }

        public object GetPartyList(object value)
        {
            throw new NotImplementedException();
        }

        public object GetVirtual(object value)
        {
            throw new NotImplementedException();
        }

        #endregion

    
    }
}