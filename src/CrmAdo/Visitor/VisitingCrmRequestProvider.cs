using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using SQLGeneration.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Visitor
{
    ///// <summary>
    ///// An <see cref="ICrmRequestProvider"/> implementation that uses the Visitor pattern and SQL Generation to build an <see cref="OrganizationRequest"/> for the <see cref="CrmDbCommand"/>
    ///// </summary>
    //public class VisitingCrmRequestProvider : IOrganisationCommandProvider
    //{
    //    public const string ParameterToken = "@";
    //    private IDynamicsAttributeTypeProvider _TypeProvider;
    //    private ICrmMetaDataProvider _MetadataProvider;


    //    public VisitingCrmRequestProvider()
    //        : this(new DynamicsAttributeTypeProvider())
    //    {
    //    }

    //    public VisitingCrmRequestProvider(IDynamicsAttributeTypeProvider typeProvider)
    //    {
    //        _TypeProvider = typeProvider;
    //        _MetadataProvider = null;
    //    }

    //    public VisitingCrmRequestProvider(IDynamicsAttributeTypeProvider typeProvider, ICrmMetaDataProvider metadataProvider)
    //    {
    //        _MetadataProvider = metadataProvider;
    //        _TypeProvider = typeProvider;
    //    }
        

    //    /// <summary>
    //    /// Returns the <see cref="OrganizationRequest"/> for the <see cref="CrmDbCommand"/>
    //    /// </summary>
    //    /// <returns></returns>
    //    public OrganizationRequest GetOrganizationRequest(CrmDbCommand command, out List<ColumnMetadata> columnMetadata)
    //    {
    //        var commandText = command.CommandText;
    //        var commandBuilder = new CommandBuilder();
    //        var options = new CommandBuilderOptions();
    //        options.PlaceholderPrefix = ParameterToken;
    //        var sqlCommandBuilder = commandBuilder.GetCommand(commandText, options);
    //        ICrmMetaDataProvider metadataProvider = _MetadataProvider;
    //        if (_MetadataProvider == null)
    //        {
    //            if (command.CrmDbConnection != null)
    //            {
    //                metadataProvider = command.CrmDbConnection.MetadataProvider;
    //            }
    //        }

    //        var orgRequestVisitingBuilder = new OrganizationRequestBuilderVisitor(metadataProvider, command.Parameters, _TypeProvider);
    //        sqlCommandBuilder.Accept(orgRequestVisitingBuilder);
    //        var request = orgRequestVisitingBuilder.OrganizationRequest;
    //        if (request == null)
    //        {
    //            throw new NotSupportedException("Could not translate the command into the appropriate Organization Service Request Message");
    //        }
    //        columnMetadata = orgRequestVisitingBuilder.ColumnMetadata;
    //        return request;
    //    }
                

    //    public OrgCommand GetOrganizationCommand(CrmDbCommand command, out List<ColumnMetadata> ColumnMetadata)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
