using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{
    public class CrmConnectionInfo
    {
        public Guid OrganisationId { get; set; }
        public Guid UserId { get; set; }
        public Guid BusinessUnitId { get; set; }
        public string OrganisationName { get; set; }
        public string ServerVersion { get; set; }
    }
}
