using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    public interface IMultipartOperation
    {
        bool HasMultipleRequests { get; }
        int RequestCount { get; }
    }
}
