using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    /// <summary>
    /// The contract for all Crm Operations. A Crm Operation represents an interaction with the CRM server via the
    /// Organisation Service.
    /// </summary>
    public interface ICrmOperation
    {
        /// <summary>
        /// Column metadata for columns expected in the results.
        /// </summary>
        List<ColumnMetadata> Columns { get; set; }

        /// <summary>
        /// The current DbCommand.
        /// </summary>
        CrmDbCommand DbCommand { get; set; }

        /// <summary>
        /// The command behaviour.
        /// </summary>
        CommandBehavior CommandBehavior { get; set; }

        /// <summary>
        /// The <see cref="OrganizationRequest"/> to be submitted to the CRM Server.
        /// </summary>
        OrganizationRequest Request { get; set; }

        /// <summary>
        /// Executes this command, and returns the <see cref="ICrmOperationResult"/>
        /// </summary>
        /// <returns></returns>
        ICrmOperationResult Execute();

        /// <summary>
        /// The index of this operations Request in the batch - if the operation is submitted as part of a batch. 
        /// </summary>
        int BatchRequestIndex { get; set; }

        /// <summary>
        /// The batch of operations that this operation is part of, otherwise Null if not part of a batch.
        /// </summary>
        BatchOperation BatchOperation { get; set; }

        /// <summary>
        /// Whether this operations is part of a batch of operations, submitted to the CRM server in one trip.
        /// </summary>
        bool IsBatchRequest { get; }
    }
}
