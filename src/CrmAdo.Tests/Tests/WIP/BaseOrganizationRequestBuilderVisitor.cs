using System;
using System.Collections.Generic;
using SQLGeneration.Builders;
using System.Data.Common;

namespace CrmAdo.Tests.WIP
{

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

        protected string GetTableLogicalEntityName(Table table)
        {
            return table.Name.ToLower();
        }

        protected string GetColumnLogicalAttributeName(Column column)
        {
            return column.Name.ToLower();
        }       

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



    }
    
}
