using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Sandbox
{
    public class CommandTestsSandbox : UnitTestSandboxContainer
    {

        public CommandTestsSandbox()
            : base()
        {
            // Arrange by registering our fake services into the test container.
            FakeCrmDbConnection = this.RegisterMockInstance<CrmDbConnection>();           
        }


        public CrmDbConnection FakeCrmDbConnection { get; private set; }


        public static CommandTestsSandbox Create()
        {
            return new CommandTestsSandbox();
        }


    }
}
