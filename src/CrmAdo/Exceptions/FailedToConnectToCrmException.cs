using System;
using Microsoft.Xrm.Client;

namespace CrmAdo.Dynamics
{
    /// <summary>
    /// Single responsibility: To represent an exception connecting to a Crm service.
    /// </summary>
    [Serializable()]
    public class FailedToConnectToCrmException : Exception
    {
        public const string FailedToConnectErrorMessage = "Failed to connect to a required CRM service.";

        public CrmConnection Connection { get; set; }

        public FailedToConnectToCrmException(CrmConnection connection, Exception innerException)
            : base(FailedToConnectErrorMessage, innerException)
        {

        }
        public override string Message
        {
            get
            {
                var builder = new System.Text.StringBuilder();
                builder.AppendLine(FailedToConnectErrorMessage);
                if (Connection != null)
                {
                    builder.AppendFormat("Crm Connection String was: {0}", Connection.ServiceUri.ToString());
                    builder.AppendLine();
                }
                builder.Append(base.Message);
                return builder.ToString();

            }
        }


    }

    /// <summary>
    /// Single responsibility: To represent an exception for missing metadata.
    /// </summary>
    [Serializable()]
    public class MissingMetadataException : Exception
    {
        public const string FailedToConnectErrorMessage = "Metadata for the following Entity / Attribute was not found.";

        public string EntityName { get; set; }

        public string AttributeName { get; set; }

        public MissingMetadataException(string entityName, string attributeName, Exception innerException)
            : base(FailedToConnectErrorMessage, innerException)
        {
            EntityName = entityName;
            AttributeName = attributeName;
        }

        public override string Message
        {
            get
            {
                var builder = new System.Text.StringBuilder();
                builder.AppendLine(FailedToConnectErrorMessage);
                if (!string.IsNullOrWhiteSpace(EntityName))
                {
                    builder.AppendFormat("Entity Named: {0}", EntityName);
                    builder.AppendLine();
                }
                if (!string.IsNullOrWhiteSpace(AttributeName))
                {
                    builder.AppendFormat("Attribute Named: {0}", AttributeName);
                    builder.AppendLine();
                }
                builder.Append(base.Message);
                return builder.ToString();
            }
        }


    }
}