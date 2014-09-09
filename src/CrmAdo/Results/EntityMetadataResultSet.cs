using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{
    public class EntityMetadataResultSet : ResultSet
    {
        private int _ResultCount = -1;

        public EntityMetadataResultSet(CrmDbCommand command, OrganizationRequest request, EntityMetadataCollection results)
            : base(command, request)
        {
            Results = results;

            if (HasResults())
            {
                // Denormalise object heirarchy into a flattened result count.
                int sumTotal = 0;
                foreach (var item in Results)
                {
                    var attCount = item.Attributes != null ? item.Attributes.Count() : 0;
                    var oneToMany = item.OneToManyRelationships != null ? item.OneToManyRelationships.Count() : 0;
                    var manyToOne = item.ManyToOneRelationships != null ? item.ManyToOneRelationships.Count() : 0;
                    var manyToMany = item.ManyToManyRelationships != null ? item.ManyToManyRelationships.Count() : 0;

                    var denormalisedRowCount = Math.Max(1, attCount * oneToMany * manyToOne * manyToMany);
                    sumTotal += denormalisedRowCount;
                }
                _ResultCount = sumTotal;
            }

            throw new NotImplementedException();

            // feild count.
         //   int fieldCount;
         //  var firstResult = 

        }

        public EntityMetadataCollection Results { get; set; }

        public override bool HasResults()
        {
            return Results != null && Results.Any();
        }

        public override int ResultCount()
        {
            return _ResultCount;
        }

        public override DbDataReader GetReader(DbConnection connection = null)
        {
            throw new NotImplementedException();
        }

        public override object GetScalar()
        {
            throw new NotImplementedException();
        }
    }
}
