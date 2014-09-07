using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Dynamics.Metadata
{
    public class MoneyAttributeInfo : AttributeInfo, IHaveMinMaxAndScaleValues<double>
    {
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public int? Precision { get; set; }
        public int? PrecisionSource { get; set; }

        public int GetNumericPrecision()
        {
            switch (PrecisionSource.GetValueOrDefault())
            {
                // When the precision is set to zero (0), the MoneyAttributeMetadata.Precision value is used.
                case 0:
                    var numericPrecision = Math.Max(MinValue.ToString().Length, MaxValue.ToString().Length) + Precision.GetValueOrDefault();
                    return numericPrecision;

                // When the precision is set to one (1), the Organization.PricingDecimalPrecision value is used.
                case 1:
                    // todo need to support grabbing this info from the organization. For now just always using metadata.
                    var orgPrecision = Math.Max(MinValue.ToString().Length, MaxValue.ToString().Length) + Precision.GetValueOrDefault();
                    return orgPrecision;

                // When the precision is set to two (2), the TransactionCurrency.CurrencyPrecision value is used.
                case 2:
                    // todo: need to grab this information from the transaction currency itself..!
                    var currencyPrecision = Math.Max(MinValue.ToString().Length, MaxValue.ToString().Length) + Precision.GetValueOrDefault();
                    return currencyPrecision;
                default:
                    var defaultPrecision = Math.Max(MinValue.ToString().Length, MaxValue.ToString().Length) + Precision.GetValueOrDefault();
                    return defaultPrecision;
            }
        }

        public int GetNumericScale()
        {
            switch (PrecisionSource.GetValueOrDefault())
            {
                case 0:
                    return Precision.GetValueOrDefault();

                // When the precision is set to one (1), the Organization.PricingDecimalPrecision value is used.
                case 1:
                    // todo need to support grabbing this info from the organization. For now just always using metadata.
                    return Precision.GetValueOrDefault();

                // When the precision is set to two (2), the TransactionCurrency.CurrencyPrecision value is used.
                case 2:
                    // todo: need to grab this information from the transaction currency itself..!
                    return Precision.GetValueOrDefault();
                default:
                    return Precision.GetValueOrDefault();
            }
        }

        object IHaveMinMaxAndScaleValues.MinValue
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

        object IHaveMinMaxAndScaleValues.MaxValue
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
