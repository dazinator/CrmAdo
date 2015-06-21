using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.DdexProvider.SupportEntities;
using Microsoft.VisualStudio.Data.Framework;

namespace CrmAdo.DdexProvider
{

    public class CrmAdoConnectionSupport : AdoDotNetConnectionSupport
    {

        public CrmAdoConnectionSupport()
            : base()
        {
            AddDataSourceVersionComparer();
            AddDataMappedObjectConverterService();
            AddDataSourceInformationService();
            AddDataObjectIdentifierResolverService();
            AddDataObjectMemberComparerService();
            AddDataObjectIdentifierConverterService();
        }

        private void AddDataSourceVersionComparer()
        {
            var serviceType = typeof(IVsDataSourceVersionComparer);
            var existingService = this.GetService(serviceType);
            IVsDataConnection existingSite = null;

            if (existingService != null)
            {
                var existingSitable = (DataSiteableObject<IVsDataConnection>)existingService;
                existingSite = existingSitable.Site;
            }

            this.RemoveService(serviceType);
            var dsVersionComparer = CreateNewDataSourceVersionComparer(existingSite);

            this.SiteChanged += (o, e) =>
            {
                dsVersionComparer.Site = this.Site;
            };

            this.AddService(serviceType, dsVersionComparer);
        }

        private void AddDataMappedObjectConverterService()
        {
            var serviceType = typeof(IVsDataMappedObjectConverter);
            var existingService = this.GetService(serviceType);
            IVsDataConnection existingSite = null;

            if (existingService != null)
            {
                var existingSitable = (DataSiteableObject<IVsDataConnection>)existingService;
                existingSite = existingSitable.Site;
            }

            this.RemoveService(serviceType);
            var newService = CreateNewDataMappedObjectConverter(existingSite);

            this.SiteChanged += (o, e) =>
            {
                newService.Site = this.Site;
            };

            this.AddService(serviceType, newService);
        }

        private void AddDataSourceInformationService()
        {
            var serviceType = typeof(IVsDataSourceInformation);
            var existingService = this.GetService(serviceType);
            IVsDataConnection existingSite = null;

            if (existingService != null)
            {
                var existingSitable = (DataSiteableObject<IVsDataConnection>)existingService;
                existingSite = existingSitable.Site;
            }

            this.RemoveService(serviceType);
            var newService = CreateNewDataSourceInformation(existingSite);

            this.SiteChanged += (o, e) =>
            {
                newService.Site = this.Site;
            };

            this.AddService(serviceType, newService);
        }

        private void AddDataObjectIdentifierResolverService()
        {
            var serviceType = typeof(IVsDataObjectIdentifierResolver);
            var existingService = this.GetService(serviceType);
            IVsDataConnection existingSite = null;

            if (existingService != null)
            {
                var existingSitable = (DataSiteableObject<IVsDataConnection>)existingService;
                existingSite = existingSitable.Site;
            }

            this.RemoveService(serviceType);
            var newService = CreateNewDataObjectIdentifierResolver(existingSite);

            this.SiteChanged += (o, e) =>
            {
                newService.Site = this.Site;
            };

            this.AddService(serviceType, newService);
        }

        private void AddDataObjectMemberComparerService()
        {
            var serviceType = typeof(IVsDataObjectMemberComparer);
            var existingService = this.GetService(serviceType);
            IVsDataConnection existingSite = null;

            if (existingService != null)
            {
                var existingSitable = (DataSiteableObject<IVsDataConnection>)existingService;
                existingSite = existingSitable.Site;
            }

            this.RemoveService(serviceType);
            var newService = CreateNewDataObjectMemberComparer(existingSite);

            this.SiteChanged += (o, e) =>
            {
                newService.Site = this.Site;
            };

            this.AddService(serviceType, newService);
        }

        private void AddDataObjectIdentifierConverterService()
        {
            var serviceType = typeof(IVsDataObjectIdentifierConverter);
            var existingService = this.GetService(serviceType);
            IVsDataConnection existingSite = null;

            if (existingService != null)
            {
                var existingSitable = (DataSiteableObject<IVsDataConnection>)existingService;
                existingSite = existingSitable.Site;
            }

            this.RemoveService(serviceType);
            var newService = CreateNewDataObjectIdentifierConverter(existingSite);

            this.SiteChanged += (o, e) =>
            {
                newService.Site = this.Site;
            };

            this.AddService(serviceType, newService);
        }


        private AdoDotNetMappedObjectConverter CreateNewDataMappedObjectConverter(IVsDataConnection site)
        {
            // if (site != null)
            return new AdoDotNetMappedObjectConverter(site);
        }

        private CrmAdoDataSourceVersionComparer CreateNewDataSourceVersionComparer(IVsDataConnection site)
        {
            // ok
            return new CrmAdoDataSourceVersionComparer(site);
        }

        private CrmSourceInformation CreateNewDataSourceInformation(IVsDataConnection site)
        {
            // ok
            return new CrmSourceInformation(site);
        }

        private CrmAdoDataObjectIdentifierResolver CreateNewDataObjectIdentifierResolver(IVsDataConnection site)
        {
            // ok
            return new CrmAdoDataObjectIdentifierResolver(site);
        }

        private CrmAdoDataObjectMemberComparer CreateNewDataObjectMemberComparer(IVsDataConnection site)
        {
            // ok
            return new CrmAdoDataObjectMemberComparer(site);
        }

        private CrmAdoObjectIdentifierConverter CreateNewDataObjectIdentifierConverter(IVsDataConnection site)
        {
            return new CrmAdoObjectIdentifierConverter(site);
        }

        protected override void OnStateChanged(DataConnectionStateChangedEventArgs e)
        {
            base.OnStateChanged(e);
        }
        public override void Initialize(object providerObj)
        {
            base.Initialize(providerObj);
        }
        protected override string PrepareCore(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.PrepareCore(command, commandType, parameters, commandTimeout);
            return result;
        }
        protected override void OnMessageReceived(DataConnectionMessageReceivedEventArgs e)
        {
            base.OnMessageReceived(e);
        }
        public override object GetService(Guid serviceGuid)
        {
            return base.GetService(serviceGuid);
        }
        protected override int ExecuteWithoutResultsCore(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.ExecuteWithoutResultsCore(command, commandType, parameters, commandTimeout);
            return result;
        }
        protected override IVsDataReader ExecuteCore(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.ExecuteCore(command, commandType, parameters, commandTimeout);
            return result;
        }
        protected override IVsDataParameter[] DeriveParametersCore(string command, DataCommandType commandType, int commandTimeout)
        {
            var result = base.DeriveParametersCore(command, commandType, commandTimeout);
            return result;
        }
        protected override object CreateService(System.ComponentModel.Design.IServiceContainer container, Type serviceType)
        {        
            var result = base.CreateService(container, serviceType);
            return result;
        }
        protected override IVsDataParameter CreateParameterFrom(DbParameter parameter)
        {
            var result = base.CreateParameterFrom(parameter);
            return result;
        }
        protected override IVsDataParameter CreateParameterCore()
        {
            var result = base.CreateParameterCore();
            return result;
        }

        public override void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote)
        {
            base.AddService(serviceType, callback, promote);
        }
        public override void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            base.AddService(serviceType, serviceInstance, promote);
        }
        protected override void DeriveParametersOn(DbCommand command)
        {
            base.DeriveParametersOn(command);
        }
        protected override IVsDataReader DeriveSchemaCore(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.DeriveSchemaCore(command, commandType, parameters, commandTimeout);
            return result;
        }

        protected override DbCommand GetCommand(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            var result = base.GetCommand(command, commandType, parameters, commandTimeout);
            return result;
        }



    }
}
