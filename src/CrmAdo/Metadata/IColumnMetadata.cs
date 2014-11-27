using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Metadata
{
    public interface IColumnMetadata
    {
        string DataType { get; }
        int IdentitySeed { get; }
        int IdentityIncrement { get; }
        bool IsIdentity { get; }
        int Length { get; set; }
        bool Nullable { get; set; }
        int GetNumericPrecision();
        int GetNumericScale();
    }

    public interface IHaveCrmPrecision
    {
        int? Precision { get; set; }
    }

    public interface IHaveMinMaxValues<T> : IHaveMinAndMaxValues where T : struct
    {
        new T? MinValue { get; set; }
        new T? MaxValue { get; set; }
    }

    public interface IHaveMinAndMaxValues
    {
        object MinValue { get; set; }
        object MaxValue { get; set; }
    }

    public interface IHaveOptionSet
    {
        OptionSetMetadata Options { get; set; }
    }
}
