using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Metadata
{
    public class LookupAttributeInfo : AttributeInfo
    {
        public override int GetNumericPrecision()
        {
            return 0;
        }

        public override int GetNumericScale()
        {
            return 0;
        }

        public override int Length
        {
            get
            {
                return 16;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
