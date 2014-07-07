using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CrmAdo
{
    /// <summary>
    ///The interface for a provider that will return an appropriate CRM organizationrequest message for a CrmDbCommand.
    /// </summary>
    public interface ICrmRequestProvider
    {
        OrganizationRequest GetOrganizationRequest(CrmDbCommand command);
    }
}