using System;
using System.Collections.Generic;
using SQLGeneration.Builders;
using System.Data.Common;
using CrmAdo.Core;
using CrmAdo.Metadata;
using System.Linq;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace CrmAdo.Visitor
{
    /// <summary>
    /// Serves as a base <see cref="BuilderVisitor"/> class, for visitors that will build Dynamics Xrm objects from Sql Generation <see cref="IVisitableBuilder"/>'s 
    /// </summary>
    public class BaseOrganizationRequestBuilderVisitor<TOrgRequest> : BuilderVisitor
        where TOrgRequest : OrganizationRequest, new()
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
            public VisitorSubCommandContext(BaseOrganizationRequestBuilderVisitor<TOrgRequest> visitor)
            {
                Visitor = visitor;
                Visitor.Level = Visitor.Level + 1;
            }

            public void Dispose()
            {
                Visitor.Level = Visitor.Level - 1;
            }

            public BaseOrganizationRequestBuilderVisitor<TOrgRequest> Visitor { get; set; }

        }

        protected BaseOrganizationRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider)
        {
            MetadataProvider = metadataProvider;
            ResultColumnMetadata = new List<ColumnMetadata>();
            EntityMetadata = new Dictionary<string, CrmEntityMetadata>();
            var req = new TOrgRequest();
            CurrentRequest = req;
            Request = req;
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

        public ICrmMetaDataProvider MetadataProvider { get; set; }

        #region Metadata Helper Methods

        /// <summary>
        /// The Columns expected in the result.
        /// </summary>
        public List<ColumnMetadata> ResultColumnMetadata { get; set; }

        protected Dictionary<string, CrmEntityMetadata> EntityMetadata { get; set; }

        protected void AddAllColumnMetadata(string entityName, string entityAlias)
        {
            // Add the metadata for this column.
            var entityMetadata = GetEntityMetadata(entityName);
            if (entityMetadata != null)
            {
                // Populate metadata for these columns.
                ResultColumnMetadata.AddRange((from c in entityMetadata.Attributes orderby c.LogicalName select new ColumnMetadata(c, entityAlias)));
            }
            else
            {
                // Could throw an exceptiton as no metadata found for this entity.
            }
        }

        protected void AddColumnMetadata(string entityName, string entityAlias, string attributeName)
        {
            // Add the metadata for this column.
            var entityMetadata = GetEntityMetadata(entityName);
            if (entityMetadata != null)
            {
                var colMeta = entityMetadata.Attributes.FirstOrDefault(c => c.LogicalName == attributeName);
                ColumnMetadata columnMetadata = null;
                if (colMeta == null)
                {
                    // could throw an exception as no metadata found for this attribute?
                    //  throw new ArgumentException("Unknown column: " + columnAttributeName);
                    columnMetadata = new ColumnMetadata(attributeName, entityAlias);

                }
                else
                {
                    columnMetadata = new ColumnMetadata(colMeta, entityAlias);
                }
                ResultColumnMetadata.Add(columnMetadata);
            }
            else
            {
                // Could throw an exceptiton as no metadata found for this entity.
            }
        }

        //protected void AddPrimaryIdMetadata(string entityName, string entityAlias)
        //{
        //    // Add the metadata for this column.
        //    var entityMetadata = GetEntityMetadata(entityName);
        //    if (entityMetadata != null)
        //    {
        //        var idMeta = entityMetadata.Attributes.FirstOrDefault(c => c.IsPrimaryId == true);
        //        ColumnMetadata columnMetadata = null;
        //        if (idMeta == null)
        //        {
        //            throw new MissingMetadataException(entityName, null, null);
        //            // could throw an exception as no metadata found for this attribute?
        //            //  throw new ArgumentException("Unknown column: " + columnAttributeName);
        //            // columnMetadata = new ColumnMetadata(entityName + "id", entityAlias);
        //        }
        //        else
        //        {
        //            columnMetadata = new ColumnMetadata(idMeta, entityAlias);
        //        }
        //        ResultColumnMetadata.Add(columnMetadata);
        //    }
        //    else
        //    {
        //        // Could throw an exceptiton as no metadata found for this entity.
        //        throw new MissingMetadataException(entityName, null, null);
        //    }
        //}

        protected bool IsPrimaryIdColumn(string entityName, string attributeName)
        {
            var entityMetadata = GetEntityMetadata(entityName);
            if (entityMetadata != null)
            {
                //var attName = column.GetColumnLogicalAttributeName();
                if (entityMetadata.PrimaryIdAttribute == attributeName)
                {
                    return true;
                }
                return false;
            }
            else
            {
                // Could throw an exceptiton as no metadata found for this entity.
                throw new MissingMetadataException(entityName, null, null);
            }
        }

        protected CrmEntityMetadata GetEntityMetadata(string entityName)
        {
            if (!EntityMetadata.ContainsKey(entityName))
            {
                if (MetadataProvider == null)
                {
                    return null;
                }
                EntityMetadata[entityName] = MetadataProvider.GetEntityMetadata(entityName);
            }
            var entMeta = EntityMetadata[entityName];
            return entMeta;
        }

        #endregion

        /// <summary>
        /// Whether the Request was upgraded to an ExecuteMultiple Request. This can happen if there are
        /// OUTPUT values that need to be retreived after the initial Request has been processed.
        /// </summary>
        public bool IsExecuteMultiple { get; set; }

        /// <summary>
        /// The Organization Request that will be executed.
        /// </summary>
        public OrganizationRequest Request { get; set; }

        /// <summary>
        /// The current request that the builder is building.
        /// </summary>
        public TOrgRequest CurrentRequest { get; set; }

        /// <summary>
        /// A seperate request that is used to retrieve any values specified by an OUTPUT clause.
        /// </summary>
        public RetrieveRequest RetrieveOutputRequest { get; set; }

        /// <summary>
        /// The items contained within the OUTPUT clause.
        /// </summary>
        protected AliasedProjection[] OutputColumns { get; set; }

        protected void UpgradeRequestToExecuteMultipleWithRetrieve(string entityName, Guid id)
        {
            // If there are output columns for anything that isn't part of the Create Response, then
            // we have to upgrade to an executemultiplerequest, with an additional Retrieve to get the extra values.
            RetrieveOutputRequest = new RetrieveRequest();
            RetrieveOutputRequest.Target = new EntityReference(entityName, id);

            // Upgrade to an ExecuteMultiple.
            var executeMultipleRequest = new ExecuteMultipleRequest();
            executeMultipleRequest.Settings = new ExecuteMultipleSettings();
            executeMultipleRequest.Settings.ContinueOnError = false;
            executeMultipleRequest.Settings.ReturnResponses = true;
            executeMultipleRequest.Requests = new OrganizationRequestCollection();
            executeMultipleRequest.Requests.Add(CurrentRequest);
            executeMultipleRequest.Requests.Add(RetrieveOutputRequest);
            RetrieveOutputRequest.ColumnSet = new ColumnSet();
            foreach (var c in OutputColumns)
            {
                c.ProjectionItem.Accept(this);
                // c.Accept(this);
            }
            Request = executeMultipleRequest;
            IsExecuteMultiple = true;

        }


    }

}
