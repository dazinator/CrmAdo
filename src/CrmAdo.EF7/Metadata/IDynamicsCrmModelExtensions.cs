using CrmAdo.EntityFramework.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Metadata
{
    public interface IDynamicsCrmModelExtensions : IRelationalModelExtensions
    {
        // [CanBeNull]
        DynamicsCrmValueGenerationStrategy? ValueGenerationStrategy { get; }

        // [CanBeNull]
        string DefaultSequenceName { get; }

        // [CanBeNull]
        string DefaultSequenceSchema { get; }
    }

}
