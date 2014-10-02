using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.Ado;

namespace CrmAdo.Tests
{
    [Category("ADO")]
    [Category("Connection")]
    [TestFixture()]
    public class CrmConnectionStringBuilderTests : BaseTest<CrmConnectionStringBuilder>
    {
        [Test]
        public void Should_Be_Able_To_Create_And_Restore_A_Connection_String_With_All_Properties_Set()
        {
            var sut = CreateTestSubject();
            sut.CallerId = "testuser";
            sut.DeviceId = "testdeviceid";
            sut.DevicePassword = "testpassword";
            sut.Domain = "testdomain";
            sut.HomeRealmUri = "http://www.test.com";
            sut.Password = "password";
            sut.ProxyTypesEnabled = true;
            sut.ServiceConfigurationInstanceMode = Microsoft.Xrm.Client.Configuration.OrganizationServiceInstanceMode.PerName;
            sut.Timeout = new TimeSpan(0, 3, 0);
            sut.Url = "www.dyanmicscrm.com/org";
            sut.Username = "testuser";
            sut.UserTokenExpiryWindow = new TimeSpan(0, 1, 30);

            var connectionString = sut.ToString();

            var newSut = new CrmConnectionStringBuilder();
            newSut.ConnectionString = sut.ConnectionString;

            Assert.AreEqual(sut.CallerId, newSut.CallerId);
            Assert.AreEqual(sut.DeviceId, newSut.DeviceId);
            Assert.AreEqual(sut.DevicePassword, newSut.DevicePassword);
            Assert.AreEqual(sut.Domain, newSut.Domain);
            Assert.AreEqual(sut.HomeRealmUri, newSut.HomeRealmUri);
            Assert.AreEqual(sut.Password, newSut.Password);
            Assert.AreEqual(sut.ProxyTypesEnabled, newSut.ProxyTypesEnabled);
            Assert.AreEqual(sut.ServiceConfigurationInstanceMode, newSut.ServiceConfigurationInstanceMode);
            Assert.AreEqual(sut.Timeout, newSut.Timeout);
            Assert.AreEqual(sut.Url, newSut.Url);
            Assert.AreEqual(sut.Username, newSut.Username);
            Assert.AreEqual(sut.UserTokenExpiryWindow, newSut.UserTokenExpiryWindow);

            Console.WriteLine(newSut.ConnectionString);


        }


    }
}
