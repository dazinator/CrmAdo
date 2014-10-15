using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{
    public class CrmConnectionCache
    {
        private static ConcurrentDictionary<string, CrmConnectionInfo> _Cache = new ConcurrentDictionary<string, CrmConnectionInfo>();

        public CrmConnectionInfo GetConnectionInfo(CrmDbConnection connection)
        {
            var connInfo = _Cache.GetOrAdd(connection.ConnectionString, f =>
            {

                WhoAmIRequest whoRequest = new WhoAmIRequest();
                WhoAmIResponse whoResponse = (WhoAmIResponse)connection.OrganizationService.Execute(whoRequest);

                var info = new CrmConnectionInfo();
                if (whoResponse != null)
                {
                    info.OrganisationId = whoResponse.OrganizationId;
                    info.UserId = whoResponse.UserId;
                    info.BusinessUnitId = whoResponse.BusinessUnitId;
                }               

                var orgEntity = connection.OrganizationService.Retrieve("organization", info.OrganisationId, new ColumnSet("name"));
                if (orgEntity != null)
                {
                    info.OrganisationName = (string)orgEntity["name"];
                }              

                var versionReq = new RetrieveVersionRequest();
                var versionResponse = (RetrieveVersionResponse)connection.OrganizationService.Execute(versionReq);
                if (versionResponse != null)
                {
                    info.ServerVersion = versionResponse.Version;
                }             

                return info;

            });

            return connInfo;
        }

    }
}
