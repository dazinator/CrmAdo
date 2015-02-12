using Microsoft.Data.Entity.Relational.Migrations.MigrationsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework.Migrations
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

        public virtual string Name { get; }
    }
}
