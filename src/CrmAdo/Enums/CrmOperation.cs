using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Enums
{
    public enum CrmOperation
    {
        Unknown = 0,
        RetrieveMetadataChanges = 1,
        RetrieveMultiple = 2,
        Create = 3,
        Update = 4,
        Delete = 5,
        CreateEntity = 6,
        CreateAttribute = 7,
        CreateOneToMany = 8,
        CreateWithOutput = 9 // Uses an ExecuteMultiple
    }
}
