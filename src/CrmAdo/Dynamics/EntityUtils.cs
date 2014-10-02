using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata.Query;

namespace CrmAdo.Dynamics
{ // ReSharper disable CheckNamespace 
// Do not change the namespace as we want anyone impprting the dynamics Sdk to get these handy utility extension methods without having
// to set up additional Using / Imports statements.
    public static class EntityUtils
    {

        /// <summary>Serialize an entity</summary>
        /// <param name="entity">Entity to serialize</param>
        /// <param name="formatting">Formatting, determines if indentation and line feeds are used in the file</param>
        public static string Serialize(this Entity entity, Formatting formatting)
        {

            using (var stringWriter = new StringWriter())
            {

                var serializer = new DataContractSerializer(typeof(Entity), null, int.MaxValue, false, false, null, new KnownTypesResolver());
                var writer = new XmlTextWriter(stringWriter)
                    {
                        Formatting = formatting
                    };
                serializer.WriteObject(writer, entity);
                writer.Close();
                return stringWriter.ToString();
            }

        }

        /// <summary>
        /// Deserialises the xml into the Entity object.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Entity Deserialize(XmlReader reader)
        {
            var serializer = new DataContractSerializer(typeof(Entity), null, int.MaxValue, false, false, null, new KnownTypesResolver());
            var entity = (Entity)serializer.ReadObject(reader);
            return entity;
        }

        public static void NegateOperator(this ConditionExpression conditionOperator)
        {
            var negated = GetNegatedOperator(conditionOperator.Operator);
            conditionOperator.Operator = negated;
        }

        private static ConditionOperator GetNegatedOperator(this ConditionOperator conditionOperator)
        {
            switch (conditionOperator)
            {
                case ConditionOperator.BeginsWith:
                    return ConditionOperator.DoesNotBeginWith;
                case ConditionOperator.DoesNotBeginWith:
                    return ConditionOperator.BeginsWith;
                case ConditionOperator.Between:
                    return ConditionOperator.NotBetween;
                case ConditionOperator.NotBetween:
                    return ConditionOperator.Between;
                case ConditionOperator.Contains:
                    return ConditionOperator.DoesNotContain;
                case ConditionOperator.DoesNotContain:
                    return ConditionOperator.Contains;
                case ConditionOperator.EndsWith:
                    return ConditionOperator.DoesNotEndWith;
                case ConditionOperator.DoesNotEndWith:
                    return ConditionOperator.EndsWith;
                case ConditionOperator.Equal:
                    return ConditionOperator.NotEqual;
                case ConditionOperator.NotEqual:
                    return ConditionOperator.Equal;
                case ConditionOperator.GreaterEqual:
                    return ConditionOperator.LessThan;
                case ConditionOperator.LessThan:
                    return ConditionOperator.GreaterEqual;
                case ConditionOperator.GreaterThan:
                    return ConditionOperator.LessEqual;
                case ConditionOperator.LessEqual:
                    return ConditionOperator.GreaterThan;
                case ConditionOperator.In:
                    return ConditionOperator.NotIn;
                case ConditionOperator.NotIn:
                    return ConditionOperator.In;
                case ConditionOperator.Like:
                    return ConditionOperator.NotLike;
                case ConditionOperator.NotLike:
                    return ConditionOperator.Like;
                case ConditionOperator.Null:
                    return ConditionOperator.NotNull;
                case ConditionOperator.NotNull:
                    return ConditionOperator.Null;
                case ConditionOperator.On:
                    return ConditionOperator.NotOn;
                case ConditionOperator.NotOn:
                    return ConditionOperator.On;
                default:
                    throw new NotSupportedException("Can not negate condition operator: " + conditionOperator);

            }
        }

        public static void NegateOperator(this MetadataConditionExpression conditionOperator)
        {
            var negated = GetNegatedOperator(conditionOperator.ConditionOperator);
            conditionOperator.ConditionOperator = negated;
        }

        private static MetadataConditionOperator GetNegatedOperator(this MetadataConditionOperator conditionOperator)
        {
            switch (conditionOperator)
            {

                case MetadataConditionOperator.Equals:
                    return MetadataConditionOperator.NotEquals;
                case MetadataConditionOperator.NotEquals:
                    return MetadataConditionOperator.Equals;            
                case MetadataConditionOperator.LessThan:
                    throw new NotSupportedException("Can not negate a 'Less Than' filter when performing metadata queries, as the Crm Sdk does not allow for 'Greater Than Equals' condition operator.");
                case MetadataConditionOperator.GreaterThan:
                    throw new NotSupportedException("Can not negate a 'Greater Than' filter when performing metadata queries, as the Crm Sdk does not allow for 'Less Than Equals' condition operator.");
                case MetadataConditionOperator.In:
                    return MetadataConditionOperator.NotIn;
                case MetadataConditionOperator.NotIn:
                    return MetadataConditionOperator.In;              
                default:
                    throw new NotSupportedException("Can not negate condition operator: " + conditionOperator);

            }
        }
     

    }

// ReSharper disable CheckNamespace 
// Do not change the namespace as we want anyone impprting the dynamics Sdk to get these handy utility extension methods without having
// to set up additional Using / Imports statements.
}