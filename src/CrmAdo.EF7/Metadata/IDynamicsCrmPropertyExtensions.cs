using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Data.Entity.DynamicsCrm.Metadata
{
    public interface IDynamicsCrmPropertyExtensions : IRelationalPropertyExtensions    {
      
        DynamicsCrmValueGenerationStrategy? ValueGenerationStrategy { get; }

        string ComputedExpression { get; }

        string SequenceName { get; }
      
        string SequenceSchema { get; }
      
        Sequence TryGetSequence();
    }
}
