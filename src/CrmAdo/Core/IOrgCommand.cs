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
    public interface IOrgCommand
    {
        CrmOperation OperationType { get; set; }
        List<ColumnMetadata> Columns { get; set; }
       // List<ColumnMetadata> OutputColumns { get; set; }       
        CrmDbCommand DbCommand { get; set; }
        CommandBehavior CommandBehavior { get; set; }
        OrganizationRequest Request { get; set; }
       // OrganizationRequest OutputRequest { get; set; }
    }
}
