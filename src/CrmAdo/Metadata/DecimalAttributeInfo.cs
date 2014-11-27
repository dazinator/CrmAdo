using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Metadata
{
    public class DecimalAttributeInfo : AttributeInfo, IHaveMinMaxValues<decimal>
    {
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }

        public int? Precision { get; set; }

        protected override int GetNumericPrecision()
        {
            var numericPrecision = Math.Max(MinValue.ToString().Length, MaxValue.ToString().Length) + Precision.GetValueOrDefault();
            return numericPrecision;
        }

        protected override int GetNumericScale()
        {
            return Precision.GetValueOrDefault();
        }

        object IHaveMinAndMaxValues.MinValue
        {
            get
            {
                return MinValue;
            }
            set
            {
                if (value == null)
                {
                    MinValue = null;
                }
                else
                {
                    MinValue = (decimal)value;
                }
            }
        }

        object IHaveMinAndMaxValues.MaxValue
        {
            get
            {
                return MaxValue;
            }
            set
            {
                if (value == null)
                {
                    MaxValue = null;
                }
                else
                {
                    MaxValue = (decimal)value;
                }
            }
        }

    }
}
