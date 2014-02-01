using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Deployment;
using Microsoft.Xrm.Sdk.Discovery;

namespace DynamicsCrmDataProvider.Dynamics
{
    /// <summary>
    /// An interface for defining the Organization management operations.
    /// </summary>
    public interface ICrmOrganisationManager
    {
        IEnumerable<OrganizationDetail> GetOrganisations();
        void CreateOrganization(Organization org, string sysAdminName);
    }
}