using CrmAdo.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Core
{
    public interface IOrganisationCommandProvider
    {
        IOrgCommand GetOrganisationCommand(CrmDbCommand command, CommandBehavior behavior);
    }
}
