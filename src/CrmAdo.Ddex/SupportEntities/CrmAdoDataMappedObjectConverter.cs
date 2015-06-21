using Microsoft.VisualStudio.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
    public class CrmAdoDataMappedObjectConverter : AdoDotNetMappedObjectConverter
    {

        public CrmAdoDataMappedObjectConverter(IVsDataConnection site)
            : base(site)
        {

        }

        public CrmAdoDataMappedObjectConverter()
            : base()
        {

        }

        protected override System.Data.DbType GetDbTypeFromNativeType(string nativeType)
        {
            var result = base.GetDbTypeFromNativeType(nativeType);
            return result;
        }

        protected override Type GetFrameworkTypeFromNativeType(string nativeType)
        {
            var result = base.GetFrameworkTypeFromNativeType(nativeType);
            return result;

        }

        protected override int GetProviderTypeFromNativeType(string nativeType)
        {
            var result = base.GetProviderTypeFromNativeType(nativeType);
            return result;
        }

        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            return result;
        }

        protected override object ConvertToMappedMember(string typeName, string mappedMemberName, object[] underlyingValues, object[] parameters)
        {
            var result = base.ConvertToMappedMember(typeName, mappedMemberName, underlyingValues);
            return result;
        }

        protected override object ConvertToUnderlyingRestriction(string mappedTypeName, int substitutionValueIndex, object[] mappedRestrictions, object[] parameters)
        {
            var result = base.ConvertToUnderlyingRestriction(mappedTypeName, substitutionValueIndex, mappedRestrictions);
            return result;
        }
    }
}
