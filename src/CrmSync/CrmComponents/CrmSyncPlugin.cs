using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CrmSync
{
    public class CrmSyncCreateHandlerPlugin : BasePlugin
    {
        protected override void Execute()
        {
            EnsureTransaction();
            var targetEntity = EnsureTargetEntity();

            if (!targetEntity.Contains("rowversion"))
            {
                Fail("Could not get the RowVersion of the Target Entity");
                return;
            }

            var rowVersion = targetEntity["rowversion"];
            targetEntity["crmsync_createdrowversion"] = rowVersion;

            var orgService = GetOrganisationService();
            orgService.Update(targetEntity);

        }
    }


}
