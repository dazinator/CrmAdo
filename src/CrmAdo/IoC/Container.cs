using CrmAdo;
using CrmAdo.Core;
using CrmAdo.Dynamics;
using CrmAdo.Dynamics.Metadata;
using CrmAdo.IoC;
using CrmAdo.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;

namespace CrmAdo.IoC
{

    public interface IContainer
    {
        void BuildUp(object instance);
    }
}


