using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Deployment;
using Microsoft.Xrm.Sdk.Discovery;
using System.Diagnostics;
using System.Data.Common;

namespace CrmAdo.Dynamics
{

    public class TargetEntity
    {
        public Guid Id { get; set; }
        public bool Exists()
        {
            return Id != Guid.Empty;
        }
    }

    /// <summary>
    /// Single Responsibility: To provide a repository for Crm publishers.
    /// </summary>
    public class CrmPublisherRepository 
    {      
        public CrmPublisherRepository()
        {          

        }

        /// <summary>
        /// Finds a publisher by the unique name.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public virtual TargetEntity Find(DbConnection conn, string uniqueName)
        {
            TargetEntity result = new TargetEntity();

            var existsSqlFormatString = "SELECT PublisherId FROM Publisher WHERE UniqueName = '{0}'";
            var existsSql = string.Format(existsSqlFormatString, uniqueName);

            conn.Open();

            var existingPublisherCommand = conn.CreateCommand();
            existingPublisherCommand.CommandText = existsSql;

            using (var publisherReader = existingPublisherCommand.ExecuteReader())
            {
                if (publisherReader.HasRows)
                {
                    //  Console.WriteLine("Publisher with unique name: " + name + " exists.");
                    publisherReader.Read();
                    result.Id = (Guid)publisherReader["publisherid"];
                }
            }

            return result;

        }

        /// <summary>
        /// Creates a publisher with the specified unique name, and publisher prefix.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefix"></param>
        public virtual TargetEntity Create(DbConnection conn, string uniqueName, string prefix)
        {

            var sqlFormatString = @"INSERT INTO Publisher (UniqueName,FriendlyName,SupportingWebsiteUrl,CustomizationPrefix,EMailAddress,Description) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}');";
            var sql = string.Format(sqlFormatString, uniqueName, uniqueName, @"http://dazinator.github.io/CrmAdo/", prefix, "darrell.tunnell@crmado.com", "crmado publisher");

            conn.Open();

            // Create publiseher.
            var command = conn.CreateCommand();
            command.CommandText = sql;

            TargetEntity createdEntity = new TargetEntity();

            using (var reader = command.ExecuteReader())
            {
                int resultCount = 0;
                foreach (var result in reader)
                {
                    resultCount++;
                    createdEntity.Id = (Guid)reader["publisherid"];
                    // Console.WriteLine(string.Format("Publisher created, id: {0}", PublisherId));
                }
            }

            return createdEntity;

        }

    }
}