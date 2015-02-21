using Microsoft.Data.Entity.Relational.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm.Migrations
{   
    public class CreateDatabaseOperation : MigrationOperation
    {
        public CreateDatabaseOperation(
            string name,
            IReadOnlyDictionary<string, string> annotations = null)
            : base(annotations)
        {
            //Check.NotEmpty(name, nameof(name));

            Name = name;
        }

        public virtual string Name { get; private set; }
    }
}
