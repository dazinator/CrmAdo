using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Relational.Migrations.Operations;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.Relational.Migrations.MigrationsModel;

namespace CrmAdo.EntityFramework.Migrations
{

    public class DropDatabaseOperation : MigrationOperation
    {
        public DropDatabaseOperation(
            string name,
            IReadOnlyDictionary<string, string> annotations = null)
            : base(annotations)
        {
            //Check.NotEmpty(name, nameof(name));               
            Name = name;
        }

        public virtual string Name { get; }
    }

}
