using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CrmAdo.IntegrationTests
{
    [TestFixture()]
    [Category("Alter Statement")]
    public class AlterEntityIntegrationTests : BaseTest
    {

        public string TestEntityName { get; set; }

        [TestFixtureSetUp]
        public override void SetUp()
        {
            base.SetUp();
            // Create a new Entity for these tests to be carried out against.
            TestEntityName = base.CreateTestEntity();
        }

        [Test(Description = "Integration test that creates a new type of entity in CRM.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyBool BIT", TestName = "Can Add Boolean Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyBoolTrue BIT DEFAULT 1", TestName = "Can Add Boolean Attribute with Default True.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyBoolFalse BIT DEFAULT 0", TestName = "Can Add Boolean Attribute with Default False.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyDateTime DATETIME", TestName = "Can Add DateTime Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyDate DATE", TestName = "Can Add Date Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyDecimal DECIMAL", TestName = "Can Add Decimal Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyDecimalP DECIMAL(12)", TestName = "Can Add Decimal Attribute With Precision.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyDecimalPS DECIMAL(12,4)", TestName = "Can Add Decimal Attribute With Precision and Scale.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyDouble FLOAT", TestName = "Can Add Double Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyDoubleP FLOAT(12)", TestName = "Can Add Double Attribute With Precision.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyDoublePS FLOAT(12,5)", TestName = "Can Add Double Attribute With Precision and Scale.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyInt Int", TestName = "Can Add Integer Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyFirstContactId UNIQUEIDENTIFIER REFERENCES {2}", TestName = "Can Add Lookup Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MySecondContactId UNIQUEIDENTIFIER CONSTRAINT {0}_{2} REFERENCES {2}", TestName = "Can Add Lookup Attribute With Named Constraint.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyMemo NVARCHAR(MAX)", TestName = "Can Add Memo Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyMoney MONEY", TestName = "Can Add Money Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyMoneyP MONEY(4)", TestName = "Can Add Money Attribute With Precision.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyMoneyPS MONEY(4,2)", TestName = "Can Add Money Attribute With Precision and Scale.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyString NVARCHAR", TestName = "Can Add String Attribute.")]
        [TestCase("ALTER TABLE {0} ADD {1}MyStringMaxSize NVARCHAR(300)", TestName = "Can Add String Attribute With MaxSize.")]
        public void Should_Be_Able_To_Add_A_New_Column(string sqlFormatString)
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            //  string attributeSchemaName = "boolField";
            string lookupToEntity = "contact";
            var sql = string.Format(sqlFormatString, TestEntityName, DefaultPublisherPrefix, lookupToEntity);
          //  Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

             //   Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                base.ExecuteCrmNonQuery(sql, -1);
            }

        }





    }





}