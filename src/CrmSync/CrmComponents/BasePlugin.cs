using System;
using Microsoft.Xrm.Sdk;

namespace CrmSync
{
    public abstract class BasePlugin : IPlugin
    {
        private ITracingService TracingService { get; set; }

        protected IPluginExecutionContext Context { get; set; }

        protected IOrganizationServiceFactory OrganisationServiceFactory { get; set; }

        protected IServiceProvider ServiceProvider { get; set; }

        protected IOrganizationService GetOrganisationService()
        {
            return GetOrganisationService(Context.UserId);
        }

        protected IOrganizationService GetOrganisationService(Guid userId)
        {
            if (OrganisationServiceFactory != null)
            {
                return OrganisationServiceFactory.CreateOrganizationService(userId);
            }
            throw new InvalidOperationException("Organisation Service Factory is not initialised.");
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            OrganisationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            try
            {
                this.Execute();
            }
            catch (InvalidPluginExecutionException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }

        }

        protected abstract void Execute();

        protected void Trace(string format, params object[] args)
        {
            TracingService.Trace(format, args);
        }

        protected Entity EnsureTargetEntity()
        {
            var targetEntity = GetTargetEntity();
            if (targetEntity == null)
            {
                Fail("Could not get Target Entity");
            }
            return targetEntity;
        }

        protected void EnsureTransaction()
        {
            if (!Context.IsInTransaction)
            {
                Fail("The plugin detected that it was not running within a database transaction. The plugin requires a database transaction.");
                return;
            } 
        }

        protected Entity GetTargetEntity()
        {
            if (Context.InputParameters.Contains("Target") && Context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                var entity = (Entity)Context.InputParameters["Target"];
                return entity;

            }
            return null;
        }

        protected EntityReference GetTargetEntityReference()
        {
            // The InputParameters collection contains all the data passed in the message request.
            if (Context.InputParameters.Contains("Target") && Context.InputParameters["Target"] is EntityReference)
            {
                // Obtain the target entity from the input parameters.
                EntityReference entity = (EntityReference)Context.InputParameters["Target"];
                return entity;
            }
            return null;
        }

        protected void Fail(string message)
        {
            throw new InvalidPluginExecutionException(message);
        }

    }
}