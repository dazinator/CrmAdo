using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Metadata
{
    public class BigIntAttributeInfo : AttributeInfo, IHaveMinMaxValues<long>
    {

        private long? _MinValue;
        public long? MinValue
        {
            get
            {
                return _MinValue ?? long.MinValue;
            }
            set
            {
                _MinValue = value;
            }
        }

        private long? _MaxValue;
        public long? MaxValue
        {
            get
            {
                return _MaxValue ?? long.MaxValue;
            }
            set
            {
                _MaxValue = value;
            }
        }

        object IHaveMinAndMaxValues.MinValue
        {
            get
            {
                return this.MinValue;
            }
            set
            {
                if (value == null)
                {
                    this.MinValue = null;
                }
                else
                {
                    this.MinValue = (long)value;
                }

            }
        }

        object IHaveMinAndMaxValues.MaxValue
        {
            get
            {
                return this.MaxValue;
            }
            set
            {
                if (value == null)
                {
                    this.MaxValue = null;
                }
                else
                {
                    this.MaxValue = (long)value;
                }

            }
        }

        public override int GetNumericPrecision()
        {
            return 19;
        }

        public override int GetNumericScale()
        {
            return 0;
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
