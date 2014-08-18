using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Support.EfModel
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }

        public virtual List<Post> Posts { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int IntComputedValue { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public byte Rating { get; set; }
        public DateTime CreationDate { get; set; }
        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }
    }

    public class NoColumnsEntity
    {
        public int Id { get; set; }
    }

    public class BloggingContext : DbContext
    {
        public BloggingContext(string connection)
            : base(new CrmAdo.CrmDbConnection(connection), true)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<NoColumnsEntity> NoColumnsEntities { get; set; }
    }
}
