using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsCrmDataProvider.Dynamics
{
    public class InMemoryCachedCrmMetaDataProvider
    {
      private readonly ConcurrentDictionary<string, EntityMetadata> _Metadata = new ConcurrentDictionary<string, EntityMetadata>();

        private IEntityMetadataRepository _repository;

        public InMemoryCachedCrmMetaDataProvider(IEntityMetadataRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Returns the metadata for an entity.
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public EntityMetadata GetEntityMetadata(string entityName)
        {
            return _Metadata.GetOrAdd(entityName, p => _repository.GetEntityMetadata(entityName));
        }
     
    }
}
