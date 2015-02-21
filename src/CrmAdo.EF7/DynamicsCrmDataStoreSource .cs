﻿using System;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity;

namespace Microsoft.Data.Entity.DynamicsCrm
{

    public class DynamicsCrmDataStoreSource : DataStoreSource<DynamicsCrmDataStoreServices, DynamicsCrmOptionsExtension>
    {
        public DynamicsCrmDataStoreSource(DbContextServices services, DbContextService<IDbContextOptions> options)
            : base(services, options)
        {
        }

        public override string Name
        {
            get { return typeof(DynamicsCrmDataStore).Name; }
        }

        public override void AutoConfigure()
        {
            ContextOptions.UseDynamicsCrm();
        }
    }

   
}
