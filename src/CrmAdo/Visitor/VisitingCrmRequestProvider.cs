using CrmAdo.Dynamics;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk;
using SQLGeneration.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Visitor
{
    /// <summary>
    /// An <see cref="ICrmRequestProvider"/> implementation that uses the Visitor pattern and SQL Generation to build an <see cref="OrganizationRequest"/> for the <see cref="CrmDbCommand"/>
    /// </summary>
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
        /// Returns the <see cref="OrganizationRequest"/> for the <see cref="CrmDbCommand"/>
        /// </summary>
        /// <returns></returns>
        public OrganizationRequest GetOrganizationRequest(CrmDbCommand command)
        {
            var commandText = command.CommandText;
            var commandBuilder = new CommandBuilder();
            var options = new CommandBuilderOptions();
            options.PlaceholderPrefix = ParameterToken;
            var sqlCommandBuilder = commandBuilder.GetCommand(commandText, options);

            ICrmMetaDataProvider metadataProvider = null;
            if (command.CrmDbConnection != null)
            {
                metadataProvider = command.CrmDbConnection.MetadataProvider;
            }

            var orgRequestVisitingBuilder = new OrganizationRequestBuilderVisitor(metadataProvider, command.Parameters, _TypeProvider);
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
