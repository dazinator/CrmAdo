using System;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CrmAdo.Tests
{
    [TestFixture(Category = "Select Statement")]
    public class SelectStatementTests : CrmQueryExpressionProviderTestsBase
    {
        [Category("TOP")]
        [Test(Description = "Should support TOP X")]
        public void Should_Support_Select_TOP_X()
        {
            // Arrange
            var sql = "Select TOP 15 contactid, firstname, lastname From contact";
            // Act
            var queryExpression = GetQueryExpression(sql);
            // Assert
            Assert.That(queryExpression.TopCount == 15);
        }

        [Category("TOP")]
        [ExpectedException(typeof(NotSupportedException))]
        [Test(Description = "Should not support TOP X PERCENT")]
        public void Should_Not_Support_Select_TOP_X_PERCENT()
        {
            // Arrange
            var sql = "Select TOP 10 PERCENT contactid, firstname, lastname From contact";
            // Act
            var queryExpression = GetQueryExpression(sql);
        }

        [Category("Order By")]
        [Test(Description = "Should support Order By ASC single column")]
        public void Should_Support_Order_By_Ascending_Single_Column()
        {
            // Arrange
            var sql = "SELECT TOP 10 firstname, lastname FROM contact ORDER BY lastname ASC ";
            // Act
            var queryExpression = GetQueryExpression(sql);
            // Assert

            Assert.That(queryExpression.Orders, Is.Not.Null);
            Assert.That(queryExpression.Orders.Count, Is.EqualTo(1));
            var order = queryExpression.Orders.Single();
            Assert.That(order.AttributeName, Is.EqualTo("lastname"));
            Assert.That(order.OrderType, Is.EqualTo(OrderType.Ascending));
        }

        [Category("Order By")]
        [Test(Description = "Should support Order By ASC multiple columns")]
        public void Should_Support_Order_By_Ascending_Multiple_Columns()
        {
            // Arrange
            var sql = "SELECT TOP 10 firstname, lastname FROM contact ORDER BY lastname ASC, firstname ASC ";
            // Act
            var queryExpression = GetQueryExpression(sql);
            // Assert

            Assert.That(queryExpression.Orders, Is.Not.Null);
            Assert.That(queryExpression.Orders.Count, Is.EqualTo(2));
            var order = queryExpression.Orders[0];
            Assert.That(order.AttributeName, Is.EqualTo("lastname"));
            Assert.That(order.OrderType, Is.EqualTo(OrderType.Ascending));

            var orderFirstname = queryExpression.Orders[1];
            Assert.That(orderFirstname.AttributeName, Is.EqualTo("firstname"));
            Assert.That(orderFirstname.OrderType, Is.EqualTo(OrderType.Ascending));
        }

        [Category("Order By")]
        [Test(Description = "Should support Order By DESC single column")]
        public void Should_Support_Order_By_Descending_Single_Column()
        {
            // Arrange
            var sql = "SELECT TOP 10 firstname, lastname FROM contact ORDER BY lastname DESC ";
            // Act
            var queryExpression = GetQueryExpression(sql);
            // Assert

            Assert.That(queryExpression.Orders, Is.Not.Null);
            Assert.That(queryExpression.Orders.Count, Is.EqualTo(1));
            var order = queryExpression.Orders.Single();
            Assert.That(order.AttributeName, Is.EqualTo("lastname"));
            Assert.That(order.OrderType, Is.EqualTo(OrderType.Descending));
        }

        [Category("Order By")]
        [Test(Description = "Should support Order By DESC multiple columns")]
        public void Should_Support_Order_By_Descending_Multiple_Columns()
        {
            // Arrange
            var sql = "SELECT TOP 10 firstname, lastname FROM contact ORDER BY lastname DESC, firstname DESC ";
            // Act
            var queryExpression = GetQueryExpression(sql);
            // Assert

            Assert.That(queryExpression.Orders, Is.Not.Null);
            Assert.That(queryExpression.Orders.Count, Is.EqualTo(2));
            var order = queryExpression.Orders[0];
            Assert.That(order.AttributeName, Is.EqualTo("lastname"));
            Assert.That(order.OrderType, Is.EqualTo(OrderType.Descending));

            var orderFirstname = queryExpression.Orders[1];
            Assert.That(orderFirstname.AttributeName, Is.EqualTo("firstname"));
            Assert.That(orderFirstname.OrderType, Is.EqualTo(OrderType.Descending));
        }

        [Category("Order By")]
        [Test(Description = "Should support Order By DESC and ASC multiple columns")]
        public void Should_Support_Order_By_Descending_and_Ascending_Multiple_Columns()
        {
            // Arrange
            var sql = "SELECT TOP 10 firstname, lastname FROM contact ORDER BY lastname DESC, firstname ASC ";
            // Act
            var queryExpression = GetQueryExpression(sql);
            // Assert

            Assert.That(queryExpression.Orders, Is.Not.Null);
            Assert.That(queryExpression.Orders.Count, Is.EqualTo(2));
            var order = queryExpression.Orders[0];
            Assert.That(order.AttributeName, Is.EqualTo("lastname"));
            Assert.That(order.OrderType, Is.EqualTo(OrderType.Descending));

            var orderFirstname = queryExpression.Orders[1];
            Assert.That(orderFirstname.AttributeName, Is.EqualTo("firstname"));
            Assert.That(orderFirstname.OrderType, Is.EqualTo(OrderType.Ascending));
        }


    }
}