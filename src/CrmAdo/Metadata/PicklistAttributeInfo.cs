using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Metadata
{
    public class PicklistAttributeInfo : AttributeInfo, IHaveMinMaxValues<int>, IHaveOptionSet
    {
        public PicklistAttributeInfo(OptionSetMetadata options)
        {
            this.Options = Options;
        }

        private int? _MinValue;
        public int? MinValue
        {
            get
            {
                return _MinValue ?? int.MinValue;
            }
            set
            {
                _MinValue = value;
            }
        }

        private int? _MaxValue;
        public int? MaxValue
        {
            get
            {
                return _MaxValue ?? int.MaxValue;
            }
            set
            {
                _MaxValue = value;
            }
        }

        protected override int GetNumericPrecision()
        {
            return 10;
        }

        protected override int GetNumericScale()
        {
            return 0;
        }

        public override int Length
        {
            get
            {
                return 4;
            }
            set
            {
                throw new NotImplementedException();
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
                    this.MinValue = (int)value;
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
                    this.MaxValue = (int)value;
                }

            }
        }

        public OptionSetMetadata Options { get; set; }

    }
}
