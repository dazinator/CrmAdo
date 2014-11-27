using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Metadata
{
    public class DoubleAttributeInfo : AttributeInfo, IHaveMinMaxValues<double>
    {
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
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
                    MinValue = (double)value;
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
                    MaxValue = (double)value;
                }
            }
        }
    }
}
