using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace CrmAdo
{
    public class EntityResultSet
    {
        //TODO: Consider return datatable and using datatable reader?
        public EntityCollection Results { get; set; }
        public List<ColumnMetadata> ColumnMetadata { get; set; }

        public bool HasResults()
        {
            return Results.Entities != null && Results.Entities.Any();
        }

        public int ResultCount()
        {
            if (HasResults())
            {
                return Results.Entities.Count;
            }
            return -1;
        }

    }
}