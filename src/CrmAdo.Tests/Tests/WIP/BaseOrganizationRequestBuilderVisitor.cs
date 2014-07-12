using System;
using System.Collections.Generic;
using SQLGeneration.Builders;
using System.Data.Common;

namespace CrmAdo.Tests.WIP
{
    /// <summary>
    /// Serves as a base <see cref="BuilderVisitor"/> class, for visitors that will build Dynamics Xrm objects from Sql Generation <see cref="IVisitableBuilder"/>'s 
    /// </summary>
    public class BaseOrganizationRequestBuilderVisitor : BuilderVisitor
    {
        private int _Level;
        public int Level
        {
            get { return _Level; }
            protected set { _Level = value; }
        }

        protected readonly CommandType _CommandType;
        protected enum CommandType
        {
            Unknown,
            Select,
            Insert,
            Update,
            Delete,
            Batch
        }

        protected class VisitorSubCommandContext : IDisposable
        {
            public VisitorSubCommandContext(BaseOrganizationRequestBuilderVisitor visitor)
            {
                Visitor = visitor;
                Visitor.Level = Visitor.Level + 1;
            }

            public void Dispose()
            {
                Visitor.Level = Visitor.Level - 1;
            }

            public BaseOrganizationRequestBuilderVisitor Visitor { get; set; }

        }

        protected VisitorSubCommandContext GetSubCommand()
        {
            return new VisitorSubCommandContext(this);
        }

        /// <summary>
        /// Visits each of the <see cref="IVisitableBuilder"/> instances, and while visiting each one, the current Level property is incremented for the duration of the visit.
        /// </summary>
        /// <param name="builders"></param>
        protected void VisitEach(IEnumerable<IVisitableBuilder> builders)
        {
            foreach (var item in builders)
            {
                using (var ctx = GetSubCommand())
                {
                    // IVisitableBuilder first = builders.First();
                    item.Accept(ctx.Visitor);
                }
            }
        }

        /// <summary>
        /// Returns the entity logical name for the table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        protected string GetTableLogicalEntityName(Table table)
        {
            return table.Name.ToLower();
        }

        /// <summary>
        /// Returns the attribute logical name for the Column.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        protected string GetColumnLogicalAttributeName(Column column)
        {
            return column.Name.ToLower();
        }

        /// <summary>
        /// Returns the object value of the literal.
        /// </summary>
        /// <param name="lit"></param>
        /// <returns></returns>
        protected object GitLiteralValue(Literal lit)
        {
            // Support string literals.
            StringLiteral stringLit = null;
            NumericLiteral numberLiteral = null;

            stringLit = lit as StringLiteral;
            if (stringLit != null)
            {
                // cast to GUID?
                Guid val;
                if (Guid.TryParse(stringLit.Value, out val))
                {
                    return val;
                }
                return stringLit.Value;
            }

            numberLiteral = lit as NumericLiteral;
            if (numberLiteral != null)
            {
                // cast down from double to int if possible..
                checked
                {
                    try
                    {
                        if ((numberLiteral.Value % 1) == 0)
                        {
                            int intValue = (int)numberLiteral.Value;
                            if (intValue == numberLiteral.Value)
                            {
                                return intValue;
                            }
                        }

                        // can we return a decimal instead?
                        var decVal = Convert.ToDecimal(numberLiteral.Value);
                        return decVal;
                    }
                    catch (OverflowException)
                    {
                        //   can't down cast to int so remain as double.
                        return numberLiteral.Value;
                    }
                }

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
        protected object ParseStringLiteralValue(StringLiteral literal)
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
        protected object ParseNumericLiteralValue(NumericLiteral literal)
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
