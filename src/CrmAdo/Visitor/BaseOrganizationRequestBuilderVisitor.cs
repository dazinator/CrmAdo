using System;
using System.Collections.Generic;
using SQLGeneration.Builders;
using System.Data.Common;

namespace CrmAdo.Visitor
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


    }

}
