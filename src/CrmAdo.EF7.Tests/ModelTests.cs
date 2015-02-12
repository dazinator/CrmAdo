using CrmAdo.EF7.Tests.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EF7.Tests
{
    [TestFixture]
    public class ModelTests : BaseDbContextTest<BloggingContext>
    {

        public ModelTests()
            : base()
        {

        }

        [Test]
        public void Can_Create_Model()
        {

            using (var bloggingContext = new BloggingContext())
            {
                var bloggs = bloggingContext.Blogs.ToArray();
            }


        }
    }
}
