using CrmAdo.Core;
using CrmAdo.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Sandbox
{
    public class DataReaderTestsSandbox : UnitTestSandboxContainer
    {

        public DataReaderTestsSandbox()
            : base()
        {
            // Arrange by registering our fake services into the test container.
            FakeCrmDbConnection = this.RegisterMockInstance<CrmDbConnection>();
            this.Container.Register<DbConnection>(FakeCrmDbConnection);

            SchemaTableProvider = new SchemaTableProvider();
            this.Container.Register<ISchemaTableProvider>(SchemaTableProvider); // Singleton.
            
            // Create some fake results data
            FakeResultSet = new EntityResultSet(null, null, null);
            FakeResultSet.ColumnMetadata = new List<ColumnMetadata>();

            var firstNameAttInfo = new StringAttributeInfo();
            firstNameAttInfo.AttributeType = AttributeTypeCode.String;
            firstNameAttInfo.LogicalName = "firstname";
            var firstNameC = new ColumnMetadata(firstNameAttInfo);

            var lastNameAttInfo = new StringAttributeInfo();
            lastNameAttInfo.AttributeType = AttributeTypeCode.String;
            lastNameAttInfo.LogicalName = "lastname";
            var lastnameC = new ColumnMetadata(lastNameAttInfo);

            FakeResultSet.ColumnMetadata.Add(firstNameC);
            FakeResultSet.ColumnMetadata.Add(lastnameC);
            FakeResultSet.Results = new EntityCollection(new List<Entity>());
            var result = new Entity("contact");
            result.Id = Guid.NewGuid();
            result["firstname"] = "joe";
            result["lastname"] = "schmoe";
            FakeResultSet.Results.Entities.Add(result);

            this.Container.Register<EntityResultSet>(FakeResultSet);
          

        }


        public CrmDbConnection FakeCrmDbConnection { get; private set; }

        public EntityResultSet FakeResultSet { get; private set; }

        public ISchemaTableProvider SchemaTableProvider { get; private set; }


        public static DataReaderTestsSandbox Create()
        {
            return new DataReaderTestsSandbox();
        }
    }
}
