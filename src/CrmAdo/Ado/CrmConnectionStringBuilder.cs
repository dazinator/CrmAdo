using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.ComponentModel;
using System.Collections;
using CrmAdo.Util;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Client.Configuration;

namespace CrmAdo.Ado
{
    /// <summary>
    /// A Connection string builder for Dynamics Crm connection strings.
    /// </summary>
    public class CrmConnectionStringBuilder : DbConnectionStringBuilder
    {      

        [Category("Authentication")]
        [DefaultValue("")]
        [Description("The user name used to authenticate.")]
        [DisplayName("User Name")]
        [RefreshProperties(RefreshProperties.All)]
        public string Username
        {
            get
            {
                return this.GetPropertyValue<string>("Username");
            }
            set
            {
                this.SetProperty("Username", value);
            }
        }

        [Category("Authentication")]
        [DefaultValue("")]
        [Description("The password used to authenticate.")]
        [DisplayName("Password")]
        [RefreshProperties(RefreshProperties.All)]
        public string Password
        {
            get
            {
                return this.GetPropertyValue<string>("Password");
            }
            set
            {
                this.SetProperty("Password", value);
            }
        }

        [Category("Authentication")]
        [DefaultValue("")]
        [Description("The domain used to authenticate.")]
        [DisplayName("Domain")]
        [RefreshProperties(RefreshProperties.All)]
        public string Domain
        {
            get
            {
                return this.GetPropertyValue<string>("Domain");
            }
            set
            {
                this.SetProperty("Domain", value);
            }
        }

        [Category("Authentication")]
        [DefaultValue("")]
        [Description("The url of the CRM organisation to connect to.")]
        [DisplayName("Url")]
        [RefreshProperties(RefreshProperties.All)]
        public string Url
        {
            get
            {
                return this.GetPropertyValue<string>("Url");
            }
            set
            {
                this.SetProperty("Url", value);
            }
        }

        [Category("Authentication")]
        [DefaultValue("")]
        [Description("The device id used to authenticate.")]
        [DisplayName("Device Id")]
        [RefreshProperties(RefreshProperties.All)]
        public string DeviceId
        {
            get
            {
                return this.GetPropertyValue<string>("DeviceId");
            }
            set
            {
                this.SetProperty("DeviceId", value);
            }
        }

        [Category("Authentication")]
        [DefaultValue("")]
        [Description("The device password used to authenticate.")]
        [DisplayName("Device Password")]
        [RefreshProperties(RefreshProperties.All)]
        public string DevicePassword
        {
            get
            {
                return this.GetPropertyValue<string>("DevicePassword");
            }
            set
            {
                this.SetProperty("DevicePassword", value);
            }
        }

        [Category("Other Options")]
        [DefaultValue("00:02:00")]
        [Description("The timeout for the Crm connection.")]
        [DisplayName("Timeout")]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(TimeSpanConverter))]
        public TimeSpan Timeout
        {
            get
            {
                return this.GetPropertyValue<TimeSpan>("Timeout");
            }
            set
            {
                this.SetProperty("Timeout", value);
            }
        }

        [Category("Authentication")]
        [DefaultValue("")]
        [Description("The Home Realm Uri for the Crm connection.")]
        [DisplayName("Home Realm Uri")]
        [RefreshProperties(RefreshProperties.All)]
        public string HomeRealmUri
        {
            get
            {
                return this.GetPropertyValue<string>("HomeRealmUri");
            }
            set
            {
                this.SetProperty("HomeRealmUri", value);
            }
        }

        [Category("Other Options")]
        [DefaultValue(true)]
        [Description("True by default, this parameter enables the OrganizationService to return proxy types instead of just the base Entity type.")]
        [DisplayName("Proxy Types Enabled")]
        [RefreshProperties(RefreshProperties.All)]
        public bool ProxyTypesEnabled
        {
            get
            {
                return this.GetPropertyValue<bool>("ProxyTypesEnabled");
            }
            set
            {
                this.SetProperty("ProxyTypesEnabled", value);
            }
        }

        [Category("Authentication")]
        [DefaultValue("")]
        [Description("Gets passed to the CallerId property of the OrganizationServiceProxy class.")]
        [DisplayName("Caller Id")]
        [RefreshProperties(RefreshProperties.All)]
        public string CallerId
        {
            get
            {
                return this.GetPropertyValue<string>("CallerId");
            }
            set
            {
                this.SetProperty("CallerId", value);
            }
        }

        [EnumStandardValuesAttribute(typeof(OrganizationServiceInstanceMode))]
        [Category("Other Options")]
        [DefaultValue(OrganizationServiceInstanceMode.PerName)]
        [Description("Can be set to Static, PerName, PerRequest, or PerInstance. PerName is default; this optimizes the number of times an IServiceConfiguration will be created for an OrganizationService, so that only one is created for each connection string.")]
        [DisplayName("Service Configuration Instance Mode")]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(CrmConnectionStringBuilderConverter))]
        public OrganizationServiceInstanceMode ServiceConfigurationInstanceMode
        {
            get
            {
                return this.GetPropertyValue<OrganizationServiceInstanceMode>("ServiceConfigurationInstanceMode");
            }
            set
            {
                this.SetProperty("ServiceConfigurationInstanceMode", value);
            }
        }

        [Category("Other Options")]
        [DefaultValue("00:00:30")]
        [Description("Specifies a TimeSpan, formatted hh:mm:ss, used as an offset for when a new user token is retrieved. For example, if this parameter is set to '00:10:00', a new user token will be retrieved 10 minutes before the token actually expires.")]
        [DisplayName("User Token Expiry Window")]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(TimeSpanConverter))]
        public TimeSpan UserTokenExpiryWindow
        {
            get
            {
                return this.GetPropertyValue<TimeSpan>("UserTokenExpiryWindow");
            }
            set
            {
                this.SetProperty("UserTokenExpiryWindow", value);
            }
        }               

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private T GetPropertyValue<T>(string key)
        {
            object val;
            var type = typeof(T);
            bool loaded = TryGetValue(key, out val);

            if (loaded)
            {
                if (type.IsEnum)
                {
                    if (val is string)
                    {
                        return (T)Enum.Parse(type, val as string);
                    }
                }
                if (type == typeof(TimeSpan))
                {
                    if (val is string)
                    {                        
                        return (T)System.Convert.ChangeType(TimeSpan.Parse(val as string), type);
                    }
                }
                return (T)System.Convert.ChangeType(val, type);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Sets the property value as a string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void SetProperty(string key, object value)
        {
            base[key] = Convert.ToString(value);
        }

        public enum ConnectionProperties
        {
            [Description("User Name")]
            Username,
            [Description("Password")]
            Password,
            [Description("Domain")]
            Domain,
            [Description("Url")]
            Url,
            [Description("Device Id")]
            DeviceId,
            [Description("Device Password")]
            DevicePassword,
            [Description("Timeout")]
            Timeout,
            [Description("Home Realm Uri")]
            HomeRealmUri,
            [Description("Proxy Types Enabled")]
            ProxyTypesEnabled,
            [Description("Caller Id")]
            CallerId,
            [Description("Service Configuration Instance Mode")]
            ServiceConfigurationInstanceMode
        }



    }


}
