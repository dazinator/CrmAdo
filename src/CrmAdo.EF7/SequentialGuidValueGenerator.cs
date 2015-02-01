using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework
{
    public class SequentialGuidValueGenerator : SimpleValueGenerator
    {
        private long _counter = DateTime.UtcNow.Ticks;

        public override object Next(IProperty property)
        {
            // Check.NotNull(property, "property");

            var guidBytes = Guid.NewGuid().ToByteArray();
            var counterBytes = BitConverter.GetBytes(Interlocked.Increment(ref _counter));

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(counterBytes);
            }

            guidBytes[08] = counterBytes[1];
            guidBytes[09] = counterBytes[0];
            guidBytes[10] = counterBytes[7];
            guidBytes[11] = counterBytes[6];
            guidBytes[12] = counterBytes[5];
            guidBytes[13] = counterBytes[4];
            guidBytes[14] = counterBytes[3];
            guidBytes[15] = counterBytes[2];

            return new Guid(guidBytes);
        }

        public override bool GeneratesTemporaryValues
        {
            get { return false; }
        }
    }
}
