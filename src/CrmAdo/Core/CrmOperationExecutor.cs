using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CrmAdo.Dynamics;
using CrmAdo.Metadata;
using CrmAdo.Util;
using CrmAdo.Visitor;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using SQLGeneration.Generators;
using SQLGeneration.Builders;
using CrmAdo.Operations;

namespace CrmAdo.Core
{

    public class CrmOperationExecutor : IOperationExecutor
    {

        private static readonly CrmOperationExecutor _Instance = new CrmOperationExecutor();

        private CrmOperationExecutor() { }

        public static CrmOperationExecutor Instance
        {
            get
            {
                return _Instance;
            }
        }

        #region ICrmCommandExecutor

        public ICrmOperationResult ExecuteOperation(ICrmOperation command)
        {
            GuardOrgCommand(command);

            ICrmOperationResult result = null;

            // if ((behavior & CommandBehavior.KeyInfo) > 0)
            switch (command.DbCommand.CommandType)
            {
                case CommandType.Text:
                case CommandType.TableDirect:
                    result = Execute(command);
                    break;
                case CommandType.StoredProcedure:
                    result = ExecuteStoredProcedureOperation(command);
                    break;
            }

            return result;
        }

        public int ExecuteNonQueryOperation(ICrmOperation command)
        {
            // You can use ExecuteNonQuery to perform catalog operations (for example, querying the structure of a database or creating database objects such as tables), or to change the data in a database by executing UPDATE, INSERT, or DELETE statements.
            // Although ExecuteNonQuery does not return any rows, any output parameters or return values mapped to parameters are populated with data.
            // For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1.
            GuardOrgCommand(command);

            ICrmOperationResult result = command.Execute();          
            if (result == null)
            {
                throw new NotSupportedException("Sorry, was not able to translate the command into the appropriate CRM SDK Organization Request message.");
            }

            return result.ReturnValue;            
        }
       
        #endregion

        protected virtual ICrmOperationResult Execute(ICrmOperation orgCommand)
        {          
            ICrmOperationResult result = orgCommand.Execute();          
            if (result == null)
            {
                throw new NotSupportedException("Sorry, was not able to translate the command into the appropriate CRM SDK Organization Request message.");
            }

            return result;   
        }

        protected virtual ICrmOperationResult ExecuteStoredProcedureOperation(ICrmOperation command)
        {
            // What would a stored procedure be in terms of Dynamics Crm SDK?
            // Perhaps this could be used for exectuign fetch xml commands...?
            throw new System.NotImplementedException();
        }

        protected void GuardOrgCommand(ICrmOperation orgCommand)
        {
            if (orgCommand == null)
            {
                throw new ArgumentNullException("orgCommand");
            }
        }      


    }

}