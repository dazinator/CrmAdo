using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Metadata
{
    public class DateTimeAttributeInfo : AttributeInfo
    {
        protected override int? GetNumericPrecision()
        {
            return 23;
        }

        protected override int? GetNumericScale()
        {
            return 3;
        }

        public override int Length
        {
            get
            {
                return 8;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

  
}
