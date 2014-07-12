using CrmAdo.Dynamics;
using CrmAdo.Tests.Tests.WIP.Visitors;
using CrmAdo.Tests.WIP;
using Microsoft.Xrm.Sdk;
using SQLGeneration.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Tests.WIP
{
    public class VisitingCrmRequestProvider : ICrmRequestProvider
    {
        public const string ParameterToken = "@";
        private IDynamicsAttributeTypeProvider _TypeProvider;

        public VisitingCrmRequestProvider()
            : this(new DynamicsAttributeTypeProvider())
        {
        }

        public VisitingCrmRequestProvider(IDynamicsAttributeTypeProvider typeProvider)
        {
            _TypeProvider = typeProvider;
        }

        /// <summary>
        /// Creates a QueryExpression from the given Select command.
        /// </summary>
        /// <returns></returns>
        public OrganizationRequest GetOrganizationRequest(CrmDbCommand command)
        {
            var commandText = command.CommandText;
            var commandBuilder = new CommandBuilder();
            var options = new CommandBuilderOptions();
            options.PlaceholderPrefix = ParameterToken;
            var sqlCommandBuilder = commandBuilder.GetCommand(commandText, options);
            var orgRequestVisitingBuilder = new OrganizationRequestBuilderVisitor();
            sqlCommandBuilder.Accept(orgRequestVisitingBuilder);
            var request = orgRequestVisitingBuilder.OrganizationRequest;
            if (request == null)
            {
                throw new NotSupportedException("Could not translate the command into the appropriate Organization Service Request Message");
            }
            return request;
        }
    }
}
