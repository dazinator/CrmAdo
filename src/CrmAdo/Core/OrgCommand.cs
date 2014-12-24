using CrmAdo.Core;
using CrmAdo.Enums;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Core
{
    public class OrgCommand : IOrgCommand
    {
        public CrmOperation OperationType { get; set; }
        public List<ColumnMetadata> Columns { get; set; }
        public List<ColumnMetadata> OutputColumns { get; set; }
        public OrganizationRequest Request { get; set; }
        public CrmDbCommand DbCommand { get; set; }
        public CommandBehavior CommandBehavior { get; set; }

    }
}
