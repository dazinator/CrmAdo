using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using System.Configuration;


namespace CrmAdo.EF7.Tests.Model
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptions builder)
        {
            var connString = ConfigurationManager.ConnectionStrings["CrmConnection"];
            var conn = connString.ConnectionString;
            builder.UseDynamicsCrm(conn);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Blog>()
                .HasMany<Post>(b => b.Posts)
                .WithOne(p => p.Blog)
                .ForeignKey(p => p.BlogId);

        }
    }

    public class Blog
    {
        public Guid BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Guid BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
