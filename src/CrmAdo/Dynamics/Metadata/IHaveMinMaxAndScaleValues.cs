using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Dynamics.Metadata
{
    public interface IHaveMinMaxAndScaleValues
    {
        object MinValue { get; set; }
        object MaxValue { get; set; }
        int? Precision { get; set; }
        int GetNumericPrecision();
        int GetNumericScale();

    }

    public interface IHaveMinMaxAndScaleValues<T> : IHaveMinMaxAndScaleValues where T : struct
    {
        new T? MinValue { get; set; }
        new T? MaxValue { get; set; }
    }
}
