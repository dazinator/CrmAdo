using System;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CrmAdo.Tests
{
    public static class AssertUtils
    {
        public static void AssertFilterExpressionContion(string attributeName, string filterOperator, object value, ConditionExpression condition)
        {

            // var condition = filterExpression.Conditions[position];
            switch (filterOperator)
            {
                case "=":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.Equal));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "<>":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.NotEqual));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case ">":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.GreaterThan));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "<":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.LessThan));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case ">=":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.GreaterEqual));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "<=":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.LessEqual));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "IS NULL":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.Null));
                    Assert.That(condition.Values, Is.Empty);
                    break;
                case "IS NOT NULL":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.NotNull));
                    Assert.That(condition.Values, Is.Empty);
                    break;
                case "LIKE":
                    if (value != null)
                    {
                        bool startsWith = false;
                        bool endsWith = false;
                        var stringVal = value.ToString();
                        startsWith = stringVal.EndsWith("%");
                        endsWith = stringVal.StartsWith("%");
                        if (startsWith && !endsWith)
                        {
                            // starts with..
                            var actualValue = stringVal.Remove(stringVal.Length - 1, 1);
                            Assert.That(condition.Operator,
                                        Is.EqualTo(ConditionOperator.BeginsWith));
                            Assert.That(condition.Values, Contains.Item(actualValue));
                        }
                        else if (endsWith && !startsWith)
                        {
                            // ends with
                            var actualValue = stringVal.Remove(0, 1);
                            Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.EndsWith));
                            Assert.That(condition.Values, Contains.Item(actualValue));
                        }
                        else
                        {
                            // like (will also do contains)
                            Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.Like));
                            Assert.That(condition.Values, Contains.Item(value));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case data");
                    }

                    break;
                case "NOT LIKE":
                    if (value != null)
                    {
                        bool startsWith = false;
                        bool endsWith = false;
                        var stringVal = value.ToString();
                        startsWith = stringVal.EndsWith("%");
                        endsWith = stringVal.StartsWith("%");
                        if (startsWith && !endsWith)
                        {
                            // starts with..
                            var actualValue = stringVal.Remove(stringVal.Length - 1, 1);
                            Assert.That(condition.Operator,
                                        Is.EqualTo(ConditionOperator.DoesNotBeginWith));
                            Assert.That(condition.Values, Contains.Item(actualValue));
                        }
                        else if (endsWith && !startsWith)
                        {
                            // ends with
                            var actualValue = stringVal.Remove(0, 1);
                            Assert.That(condition.Operator,
                                        Is.EqualTo(ConditionOperator.DoesNotEndWith));
                            Assert.That(condition.Values, Contains.Item(actualValue));
                        }
                        else
                        {
                            // not like (will also do not contains)
                            Assert.That(condition.Operator,
                                        Is.EqualTo(ConditionOperator.NotLike));
                            Assert.That(condition.Values, Contains.Item(value));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case data");
                    }
                    break;
                case "IN":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.In));
                    if (value != null && value.GetType().IsArray)
                    {
                        foreach (var val in value as Array)
                        {
                            Guid guidVal;
                            if (val is string)
                            {
                                if (Guid.TryParse(val as string, out guidVal))
                                {
                                    Assert.That(condition.Values, Contains.Item(guidVal));
                                    continue;
                                }
                            }

                            Assert.That(condition.Values, Contains.Item(val));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case for IN expression.");
                    }
                    break;
                case "NOT IN":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.NotIn));
                    if (value != null && value.GetType().IsArray)
                    {
                        foreach (var val in value as Array)
                        {
                            Guid guidVal;
                            if (val is string)
                            {
                                if (Guid.TryParse(val as string, out guidVal))
                                {
                                    Assert.That(condition.Values, Contains.Item(guidVal));
                                    continue;
                                }
                            }

                            Assert.That(condition.Values, Contains.Item(val));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case for IN expression.");
                    }

                    break;

                case "CONTAINS":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.Contains));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "NOT CONTAINS":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.DoesNotContain));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                default:
                    Assert.Fail("Unhandled test case.");
                    break;
            }

            Assert.That(condition.AttributeName, Is.EqualTo(attributeName));
        }

        private static ConditionExpression EnsureSingleFilterCondition(QueryExpression query)
        {
            // Assert
            Assert.That(query.Criteria.Filters.Count, Is.EqualTo(1));
            var defaultFilterConditons = query.Criteria.Filters[0].Conditions;
            //var defaultConditons = queryExpression.Criteria.Conditions;
            //queryExpression.Criteria.Filters[0];
            Assert.That(defaultFilterConditons.Count, Is.EqualTo(1));
            var condition = defaultFilterConditons[0];
            return condition;
        }

        public static ConditionExpression AssertQueryContainsSingleFilterCondition(QueryExpression query, string attributeName, string filterOperator, object value)
        {
            var condition = EnsureSingleFilterCondition(query);
            AssertFilterExpressionContion(attributeName, filterOperator, value, condition);
            return condition;
        }

    }
}