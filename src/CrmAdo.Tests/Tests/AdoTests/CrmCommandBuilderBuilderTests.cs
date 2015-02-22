using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.Tests.Support;
using Microsoft.Xrm.Sdk.Messages;

namespace CrmAdo.Tests
{
    [Category("ADO")]
    [Category("CommandBuilder")]
    [TestFixture()]
    public class CrmCommandBuilderTests : BaseTest<CrmCommandBuilder>
    {
        [Test]
        public void Should_Generate_Insert_Command()
        {

            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                using (var dbConnection = sandbox.Container.Resolve<CrmDbConnection>())
                {
                    var selectCommand = new CrmDbCommand(dbConnection, "SELECT contactid, firstname, lastname FROM contact");

                    using (var adapter = new CrmDataAdapter(selectCommand))
                    {
                        adapter.SelectCommand = selectCommand;

                        using (var sut = new CrmCommandBuilder(adapter))
                        {

                            // verify that the crmcommand builder generates appropriate insert / udapte and delete commands.
                            // Act
                            var insertCommand = sut.GetInsertCommand();

                            // Assert                      
                            Assert.That(insertCommand, Is.Not.Null);
                            Assert.That(insertCommand.CommandText, Is.Not.Null);
                            Assert.That(insertCommand.CommandText, Is.Not.EqualTo(""));

                            Console.WriteLine(insertCommand.CommandText);

                        }

                    }
                }
            }
        }

        [Test]
        public void Should_Generate_Update_Command_Using_Conflict_Option_Overwrite_Changes()
        {

            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                using (var dbConnection = sandbox.Container.Resolve<CrmDbConnection>())
                {
                    var selectCommand = new CrmDbCommand(dbConnection, "SELECT contactid, firstname, lastname FROM contact");

                    using (var adapter = new CrmDataAdapter(selectCommand))
                    {
                        adapter.SelectCommand = selectCommand;

                        using (var sut = new CrmCommandBuilder(adapter))
                        {
                            sut.ConflictOption = ConflictOption.OverwriteChanges;
                            // verify that the crmcommand builder generates appropriate insert / udapte and delete commands.
                            // Act
                            var updateCommand = sut.GetUpdateCommand();

                            // Assert                      
                            Assert.That(updateCommand, Is.Not.Null);
                            Assert.That(updateCommand.CommandText, Is.Not.Null);
                            Assert.That(updateCommand.CommandText, Is.Not.EqualTo(""));

                            Console.WriteLine(updateCommand.CommandText);

                        }

                    }
                }
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Should_Not_Support_Generation_Of_Update_Command_Using_Conflict_Option_Compare_All_Searchable_Values()
        {

            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                using (var dbConnection = sandbox.Container.Resolve<CrmDbConnection>())
                {
                    var selectCommand = new CrmDbCommand(dbConnection, "SELECT contactid, firstname, lastname FROM contact");

                    using (var adapter = new CrmDataAdapter(selectCommand))
                    {
                        adapter.SelectCommand = selectCommand;

                        using (var sut = new CrmCommandBuilder(adapter))
                        {
                            sut.ConflictOption = ConflictOption.CompareAllSearchableValues;
                            // verify that the crmcommand builder generates appropriate insert / udapte and delete commands.
                            // Act
                            var updateCommand = sut.GetUpdateCommand();

                            // Assert                      
                            Assert.That(updateCommand, Is.Not.Null);
                            Assert.That(updateCommand.CommandText, Is.Not.Null);
                            Assert.That(updateCommand.CommandText, Is.Not.EqualTo(""));

                            Console.WriteLine(updateCommand.CommandText);

                        }

                    }
                }
            }
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Should_Not_Support_Generation_Of_Update_Command_Using_Conflict_Option_Compare_Row_Version()
        {

            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                using (var dbConnection = sandbox.Container.Resolve<CrmDbConnection>())
                {
                    var selectCommand = new CrmDbCommand(dbConnection, "SELECT contactid, firstname, lastname FROM contact");

                    using (var adapter = new CrmDataAdapter(selectCommand))
                    {
                        adapter.SelectCommand = selectCommand;

                        using (var sut = new CrmCommandBuilder(adapter))
                        {
                            sut.ConflictOption = ConflictOption.CompareRowVersion;
                            // verify that the crmcommand builder generates appropriate insert / udapte and delete commands.
                            // Act
                            var updateCommand = sut.GetUpdateCommand();

                            // Assert                      
                            Assert.That(updateCommand, Is.Not.Null);
                            Assert.That(updateCommand.CommandText, Is.Not.Null);
                            Assert.That(updateCommand.CommandText, Is.Not.EqualTo(""));

                            Console.WriteLine(updateCommand.CommandText);

                        }

                    }
                }
            }
        }

        [Test]
        public void Should_Generate_Delete_Command()
        {

            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                using (var dbConnection = sandbox.Container.Resolve<CrmDbConnection>())
                {
                    var selectCommand = new CrmDbCommand(dbConnection, "SELECT contactid, firstname, lastname, versionnumber FROM contact");

                    using (var adapter = new CrmDataAdapter(selectCommand))
                    {
                        adapter.SelectCommand = selectCommand;

                        using (var sut = new CrmCommandBuilder(adapter))
                        {
                            // verify that the crmcommand builder generates appropriate insert / udapte and delete commands.
                            // Act
                            var deleteCommand = sut.GetDeleteCommand();

                            // Assert                      
                            Assert.That(deleteCommand, Is.Not.Null);
                            Assert.That(deleteCommand.CommandText, Is.Not.Null);
                            Assert.That(deleteCommand.CommandText, Is.Not.EqualTo(""));

                            Console.WriteLine(deleteCommand.CommandText);
                        }

                    }
                }
            }
        }


    }

}

