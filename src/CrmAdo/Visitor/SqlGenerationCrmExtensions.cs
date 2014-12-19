using CrmAdo.Util;
using SQLGeneration.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Visitor
{
    public static class SqlGenerationCrmExtensions
    {
        /// <summary>
        /// Returns the entity logical name for the table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetTableLogicalEntityName(this Table table)
        {
            var tableName = table.Name;
            var unQuoted = SqlUtils.GetUnquotedIdentifier(tableName);
            return unQuoted.ToLower();
        }

        /// <summary>
        /// Returns the attribute logical name for the Column.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetColumnLogicalAttributeName(this Column column)
        {
            var columnName = column.Name;
            var unQuoted = SqlUtils.GetUnquotedIdentifier(columnName);
            return unQuoted.ToLower();
        }

        /// <summary>
        /// Returns the object value of the literal.
        /// </summary>
        /// <param name="lit"></param>
        /// <returns></returns>
        public static object GitLiteralValue(this Literal lit)
        {
            // Support string literals.
            StringLiteral stringLit = null;
            NumericLiteral numberLiteral = null;

            stringLit = lit as StringLiteral;
            if (stringLit != null)
            {
                return stringLit.ParseStringLiteralValue();
            }

            numberLiteral = lit as NumericLiteral;
            if (numberLiteral != null)
            {
                return numberLiteral.ParseNumericLiteralValue();
            }

            var nullLiteral = lit as NullLiteral;
            if (nullLiteral != null)
            {
                return null;
            }

            throw new NotSupportedException("Unknown Literal");

        }

        /// <summary>
        /// Returns the object value of the string literal.
        /// </summary>
        /// <param name="literal"></param>
        /// <returns></returns>
        public static object ParseStringLiteralValue(this StringLiteral literal)
        {
            // cast to GUID?
            Guid val;
            if (Guid.TryParse(literal.Value, out val))
            {
                return val;
            }
            return literal.Value;
        }

        /// <summary>
        /// Returns the object value of the numeric literal.
        /// </summary>
        /// <param name="literal"></param>
        /// <returns></returns>
        public static object ParseNumericLiteralValue(this NumericLiteral literal)
        {
            // cast down from double to int if possible..
            checked
            {
                try
                {
                    if ((literal.Value % 1) == 0)
                    {
                        int intValue = (int)literal.Value;
                        if (intValue == literal.Value)
                        {
                            return intValue;
                        }
                    }

                    // can we return a decimal instead?
                    var decVal = Convert.ToDecimal(literal.Value);
                    return decVal;
                }
                catch (OverflowException)
                {
                    //   can't down cast to int so remain as double.
                    return literal.Value;
                }
            }
        }
    }
}
