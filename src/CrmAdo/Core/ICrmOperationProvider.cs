using CrmAdo.Operations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Core
{
    public interface ICrmOperationProvider
    {
        ICrmOperation GetOperation(CrmDbCommand command, CommandBehavior behavior);
    }
}
