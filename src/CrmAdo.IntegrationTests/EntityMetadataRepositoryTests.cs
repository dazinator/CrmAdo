using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using CrmAdo.Dynamics;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Metadata;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using CrmAdo.Util;
using CrmAdo.Core;

namespace CrmAdo.IntegrationTests
{
    [TestFixture()]
    [Category("EntityMetadataRepository")]
    [Category("Metadata")]
    public class EntityMetadataRepositoryTests : BaseTest
    {
        [Test(Description = "Get entity metadata from CRM using repisitory.")]
        public void Should_Get_Metadata()
        {
            // arrange
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                       new CrmClientCredentialsProvider());

            var sut = new EntityMetadataRepository(serviceProvider);
            // act
            var contactMetadata = sut.GetEntityMetadata("contact");            

            // assert
            Assert.That(contactMetadata, Is.Not.Null);
            Assert.That(contactMetadata, Is.Not.Null);

            Assert.That(contactMetadata.Attributes, Is.Not.Null);
            Assert.That(contactMetadata.Attributes.FirstOrDefault(a => a.LogicalName == "firstname"), Is.Not.Null);
            Assert.That(contactMetadata.Attributes.FirstOrDefault(a => a.LogicalName == "lastname"), Is.Not.Null);
        }

    }
}