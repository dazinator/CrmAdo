using Microsoft.Data.Entity.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm
{    
    public class DynamicsCrmSequenceValueGeneratorState : HiLoValueGeneratorState
    {
        public DynamicsCrmSequenceValueGeneratorState(string sequenceName, int blockSize, int poolSize)
            : base(blockSize, poolSize)
        {
            //Check.NotEmpty(sequenceName, nameof(sequenceName));
            SequenceName = sequenceName;
        }

        public virtual string SequenceName { get; private set; }
    }
}
