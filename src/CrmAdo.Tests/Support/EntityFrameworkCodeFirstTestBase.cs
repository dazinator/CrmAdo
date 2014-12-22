using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Support
{
    public class EntityFrameworkCodeFirstTestBase<T> : BaseTest<T> where T : DbContext
    {
        public EntityFrameworkCodeFirstTestBase(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        protected override T ResolveTestSubjectInstance()
        {
            return (T)Activator.CreateInstance(typeof(T), this.ConnectionString);
        }


    }
}
