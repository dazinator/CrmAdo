using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CrmAdo
{
    public interface ICrmRequestProvider
    {
        OrganizationRequest GetOrganizationRequest(CrmDbCommand command);
    }
}