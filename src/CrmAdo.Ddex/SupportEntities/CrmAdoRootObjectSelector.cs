using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{

    //internal sealed class Host : IDisposable
    //{
    //    private static object s_lockObject;

    //    private static Microsoft.VisualStudio.Data.HostServices.System s_system;

    //    private Microsoft.VisualStudio.Data.HostServices.Environment _environment;

    //    private IVsDataHostService _hostService;

    //    private IServiceProvider _serviceProvider;

    //    private static Host _defaultInstance;

    //    public Microsoft.VisualStudio.Data.HostServices.Environment Environment
    //    {
    //        get
    //        {
    //            return this.GetServiceObject<Microsoft.VisualStudio.Data.HostServices.Environment>(ref this._environment);
    //        }
    //    }

    //    private IVsDataHostService HostService
    //    {
    //        get
    //        {
    //            if (this._hostService == null)
    //            {
    //                lock (Host.s_lockObject)
    //                {
    //                    if (this._hostService == null)
    //                    {
    //                        this._hostService = this.ServiceProvider.GetService(typeof(IVsDataHostService)) as IVsDataHostService;
    //                    }
    //                }
    //            }
    //            return this._hostService;
    //        }
    //    }

    //    public IServiceProvider ServiceProvider
    //    {
    //        get
    //        {
    //            return this._serviceProvider;
    //        }
    //    }

    //    public static Microsoft.VisualStudio.Data.HostServices.System System
    //    {
    //        get
    //        {
    //            return Host._defaultInstance.GetServiceObject<Microsoft.VisualStudio.Data.HostServices.System>(ref Host.s_system);
    //        }
    //    }

    //    static Host()
    //    {
    //        Host.s_lockObject = new object();
    //        Host._defaultInstance = new Host();
    //    }

    //    private Host()
    //    {
    //    }

    //    public Host(IServiceProvider serviceProvider)
    //    {
    //        this._serviceProvider = serviceProvider;
    //    }

    //    private void Dispose(bool disposing)
    //    {
    //        if (disposing && this._serviceProvider != null)
    //        {
    //            if (this._environment != null)
    //            {
    //                ((IDisposable)this._environment).Dispose();
    //                this._environment = null;
    //            }
    //            this._hostService = null;
    //            this._serviceProvider = null;
    //        }
    //    }

    //    ~Host()
    //    {
    //        this.Dispose(false);
    //    }

    //    public S GetService<S>()
    //    {
    //        return this.HostService.GetService<S>();
    //    }

    //    public I GetService<S, I>()
    //    {
    //        return this.HostService.GetService<S, I>();
    //    }

    //    public I GetService<I>(Guid serviceGuid)
    //    {
    //        return this.HostService.GetService<I>(serviceGuid);
    //    }

    //    private static T GetServiceObject<T>(IServiceProvider serviceProvider, ref T storage)
    //    where T : Service, new()
    //    {
    //        T service = storage;
    //        if (service == null)
    //        {
    //            lock (Host.s_lockObject)
    //            {
    //                service = storage;
    //                if (service == null)
    //                {
    //                    if (serviceProvider != null)
    //                    {
    //                        service = (T)(serviceProvider.GetService(typeof(T)) as T);
    //                    }
    //                    if (service == null)
    //                    {
    //                        T t = Activator.CreateInstance<T>();
    //                        T t1 = t;
    //                        storage = t;
    //                        service = t1;
    //                    }
    //                }
    //            }
    //        }
    //        return service;
    //    }

    //    private T GetServiceObject<T>(ref T storage)
    //    where T : Service, new()
    //    {
    //        T serviceObject = Host.GetServiceObject<T>(this.ServiceProvider, ref storage);
    //        serviceObject.Host = this;
    //        return serviceObject;
    //    }

    //    void System.IDisposable.Dispose()
    //    {
    //        this.Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    public S TryGetService<S>()
    //    {
    //        return this.HostService.TryGetService<S>();
    //    }

    //    public I TryGetService<S, I>()
    //    {
    //        return this.HostService.TryGetService<S, I>();
    //    }

    //    public I TryGetService<I>(Guid serviceGuid)
    //    {
    //        return this.HostService.TryGetService<I>(serviceGuid);
    //    }
    //}


    //public class AdoDotNetRootObjectSelector : DataObjectSelector
    //{
    //    public AdoDotNetRootObjectSelector()
    //    {
    //    }

    //    protected static void ApplyMappings(DataTable dataTable, IDictionary<string, object> mappings)
    //    {
    //        AdoDotNetProvider.ApplyMappings(dataTable, mappings);
    //    }

    //    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    //    protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
    //    {
    //        IVsDataReader adoDotNetTableReader;
    //        if (typeName == null)
    //        {
    //            throw new ArgumentNullException("typeName");
    //        }
    //        if (typeName.Length > 0)
    //        {
    //            throw new NotSupportedException();
    //        }
    //        if (restrictions != null && (int)restrictions.Length > 0)
    //        {
    //            throw new ArgumentException("Invalid restrictions", "restrictions");
    //        }
    //        if (base.Site == null)
    //        {
    //            throw new InvalidOperationException();
    //        }
    //        object lockedProviderObject = base.Site.GetLockedProviderObject();
    //        if (lockedProviderObject == null)
    //        {
    //            throw new NotImplementedException();
    //        }
    //        try
    //        {
    //            DbConnection dbConnection = lockedProviderObject as DbConnection;
    //            if (dbConnection == null)
    //            {
    //                throw new NotImplementedException();
    //            }
    //            DataTable dataTable = new DataTable();
    //            dataTable.Locale = CultureInfo.CurrentCulture;
    //            dataTable.Columns.Add("DataSource", typeof(string));
    //            dataTable.Columns.Add("ServerVersion", typeof(string));
    //            dataTable.Columns.Add("Database", typeof(string));
    //            IVsDataConnectionProperties vsDataConnectionProperties = null;
    //            IServiceProvider service = base.Site.GetService(typeof(IServiceProvider)) as IServiceProvider;
    //            using (Host host = new Host(service))
    //            {
    //                vsDataConnectionProperties = host.GetService<IVsDataProviderManager>().Providers[base.Site.Provider].TryCreateObject<IVsDataConnectionUIProperties>(base.Site.Source);
    //                if (vsDataConnectionProperties == null)
    //                {
    //                    vsDataConnectionProperties = host.GetService<IVsDataProviderManager>().Providers[base.Site.Provider].TryCreateObject<IVsDataConnectionProperties>(base.Site.Source);
    //                }
    //            }
    //            if (vsDataConnectionProperties != null)
    //            {
    //                vsDataConnectionProperties.Parse(base.Site.SafeConnectionString);
    //                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(vsDataConnectionProperties))
    //                {
    //                    string name = property.Name;
    //                    int num = 1;
    //                    while (dataTable.Columns.Contains(name))
    //                    {
    //                        name = string.Concat(property.Name, num.ToString(CultureInfo.CurrentCulture));
    //                        num++;
    //                    }
    //                    dataTable.Columns.Add(name, property.PropertyType);
    //                }
    //            }
    //            base.Site.EnsureConnected();
    //            List<object> objs = new List<object>();
    //            objs.Add(AdoDotNetRootObjectSelector.ValueOrDBNull(dbConnection.DataSource));
    //            objs.Add(AdoDotNetRootObjectSelector.ValueOrDBNull(dbConnection.ServerVersion));
    //            objs.Add(AdoDotNetRootObjectSelector.ValueOrDBNull(dbConnection.Database));
    //            if (vsDataConnectionProperties != null)
    //            {
    //                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(vsDataConnectionProperties))
    //                {
    //                    object propertyOwner = vsDataConnectionProperties;
    //                    ICustomTypeDescriptor customTypeDescriptor = vsDataConnectionProperties as ICustomTypeDescriptor;
    //                    if (customTypeDescriptor != null)
    //                    {
    //                        propertyOwner = customTypeDescriptor.GetPropertyOwner(propertyDescriptor);
    //                    }
    //                    objs.Add(propertyDescriptor.GetValue(propertyOwner));
    //                }
    //            }
    //            dataTable.Rows.Add(objs.ToArray());
    //            if ((int)parameters.Length == 1 && parameters[0] is DictionaryEntry)
    //            {
    //                object[] value = ((DictionaryEntry)parameters[0]).Value as object[];
    //                if (value != null)
    //                {
    //                    AdoDotNetRootObjectSelector.ApplyMappings(dataTable, DataObjectSelector.GetMappings(value));
    //                }
    //            }
    //            adoDotNetTableReader = new AdoDotNetTableReader(dataTable);
    //        }
    //        finally
    //        {
    //            base.Site.UnlockProviderObject();
    //        }
    //        return adoDotNetTableReader;
    //    }

    //    private static object ValueOrDBNull(object value)
    //    {
    //        if (value == null)
    //        {
    //            return DBNull.Value;
    //        }
    //        return value;
    //    }
    //}

    public class CrmAdoRootObjectSelector : AdoDotNetRootObjectSelector
    {

        protected override Microsoft.VisualStudio.Data.Services.SupportEntities.IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                typeName = "Root";
            }
            var result = base.SelectObjects(typeName, restrictions, properties, parameters);
            return result;
        }
    }
}
