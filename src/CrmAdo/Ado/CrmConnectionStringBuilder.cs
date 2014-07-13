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

        private StringComparer _StringComparer = StringComparer.OrdinalIgnoreCase;

        private string _Username;
        private string _Password;
        private string _Domain;
        private string _Url;
        private string _DeviceId;
        private string _DevicePassword;
        private TimeSpan _Timeout;
        private string _HomeRealmUri;
        private bool _ProxyTypesEnabled;
        private string _CallerId;
        private OrganizationServiceInstanceMode _ServiceConfigurationInstanceMode;
        private TimeSpan _UserTokenExpiryWindow;

        //private List<string> _AllowedKeys;

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object this[string key]
        {
            get
            {
                object obj = null;
                if (!this.TryGetValue(key, out obj))
                {
                    throw this.InvalidProperty(key);
                }
                return obj;
            }
            set
            {
                if (!this.LoadProperty(key, value))
                {
                    throw this.InvalidProperty(key);
                }
            }
        }

        [Category("Authentication")]
        [DefaultValue("")]
        [Description("The user name used to authenticate.")]
        [DisplayName("User Name")]
        [RefreshProperties(RefreshProperties.All)]
        public string Username
        {
            get
            {
                return this._Username;
            }
            set
            {
                this._Username = value;
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
                return this._Password;
            }
            set
            {
                this._Password = value;
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
                return this._Domain;
            }
            set
            {
                this._Domain = value;
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
                return this._Url;
            }
            set
            {
                this._Url = value;
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
                return this._DeviceId;
            }
            set
            {
                this._DeviceId = value;
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
                return this._DevicePassword;
            }
            set
            {
                this._DevicePassword = value;
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
                return this._Timeout;
            }
            set
            {
                this._Timeout = value;
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
                return this._HomeRealmUri;
            }
            set
            {
                this._HomeRealmUri = value;
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
                return this._ProxyTypesEnabled;
            }
            set
            {
                this._ProxyTypesEnabled = value;
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
                return this._CallerId;
            }
            set
            {
                this._CallerId = value;
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
                return this._ServiceConfigurationInstanceMode;
            }
            set
            {
                this._ServiceConfigurationInstanceMode = value;
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
                return this._UserTokenExpiryWindow;
            }
            set
            {
                this._UserTokenExpiryWindow = value;
                this.SetProperty("UserTokenExpiryWindow", value);
            }
        }

        /// <summary>
        /// The connection string.
        /// </summary>
        public new string ConnectionString
        {
            get
            {
                return base.ConnectionString;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    base.ConnectionString = value;
                }
                else
                {
                    base.ConnectionString = this.GetDefaultConnectionString();
                }
            }
        }

        /// <summary>
        /// The default connection string.
        /// </summary>
        /// <returns></returns>
        private string GetDefaultConnectionString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Throws an exception for an invalid property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private Exception InvalidProperty(string propertyName)
        {
            return new ArgumentException(string.Format("Invalid connection string property '{0}'", propertyName));
        }

        private bool LoadProperty(string propertyName, object value)
        {
            propertyName = Regex.Replace(propertyName, "\\s+", string.Empty);
            bool loaded = false;

            if (_StringComparer.Compare(propertyName, "username") == 0)
            {
                this.Username = Convert.ToString(value);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "password") == 0)
            {
                this.Password = Convert.ToString(value);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "domain") == 0)
            {
                this.Domain = Convert.ToString(value);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "url") == 0)
            {
                this.Url = Convert.ToString(value);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "deviceid") == 0)
            {
                this.DeviceId = Convert.ToString(value);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "devicepassword") == 0)
            {
                this.DevicePassword = Convert.ToString(value);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "timeout") == 0)
            {
                this.Timeout = TimeSpan.Parse(Convert.ToString(value));
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "homerealmuri") == 0)
            {
                this.HomeRealmUri = Convert.ToString(value);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "proxytypesenabled") == 0)
            {
                this.ProxyTypesEnabled = Convert.ToBoolean(value);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "callerId") == 0)
            {
                this.CallerId = Convert.ToString(value);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "serviceconfigurationinstancemode") == 0)
            {
                this.ServiceConfigurationInstanceMode = (OrganizationServiceInstanceMode)Enum.Parse(typeof(OrganizationServiceInstanceMode), Convert.ToString(value), true);
                loaded = true;
            }
            else if (_StringComparer.Compare(propertyName, "usertokenexpirywindow") == 0)
            {
                this._UserTokenExpiryWindow = TimeSpan.Parse(Convert.ToString(value));
                loaded = true;
            }
            else
            {
                this.SetProperty(propertyName, Convert.ToString(value));
                loaded = true;
            }
            return loaded;

        }

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
