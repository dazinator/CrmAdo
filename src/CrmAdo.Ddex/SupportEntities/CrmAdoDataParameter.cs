using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
    public class CrmAdoDataParameter : AdoDotNetParameter
    {
        public CrmAdoDataParameter(DbParameter parameter)
            : base(parameter)
        {

        }

        public CrmAdoDataParameter(string providerInvariantName)
            : base(providerInvariantName)
        {

        }

        public CrmAdoDataParameter(DbParameter parameter, bool isDerived)
            : base(parameter, isDerived)
        {

        }

        public CrmAdoDataParameter(string providerInvariantName, bool isDerived)
            : base(providerInvariantName, isDerived)
        {

        }




        protected override int DefaultSize
        {
            get
            {
                var result = base.DefaultSize;
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            return result;
        }

        protected override DataParameterDirection GetDirectionCore()
        {
            var result = base.GetDirectionCore();
            return result;
        }

        protected override bool GetIsNullableCore()
        {
            var result = base.GetIsNullableCore();
            return result;
        }

        protected override bool GetIsOptionalCore()
        {
            var result = base.GetIsOptionalCore();
            return result;
        }

        protected override string GetNameCore()
        {
            var result = base.GetNameCore();
            return result;
        }

        protected override int GetSizeCore()
        {
            var result = base.GetSizeCore();
            return result;
        }

        protected override string GetTypeCore()
        {
            var result = base.GetTypeCore();
            return result;
        }

        protected override string GetTypeFrom(object value)
        {
            var result = base.GetTypeFrom(value);
            return result;
        }

        protected override object GetValueCore()
        {
            var result = base.GetValueCore();
            return result;
        }

        protected override bool HasDescriptor
        {
            get
            {
                var result = base.HasDescriptor;
                return result;
            }
        }

        protected override bool IsFixedSize
        {
            get
            {
                var result = base.IsFixedSize;
                return result;
            }
        }

        protected override bool IsSupportedDirection(DataParameterDirection direction)
        {
            var result = base.IsSupportedDirection(direction);
            return result;
        }

        protected override bool IsValidType(string type)
        {
            var result = base.IsValidType(type);
            return result;
        }

        public override void Parse(string value)
        {
            base.Parse(value);
        }

        protected override object TryConvertValue(object value, string type)
        {
            var result = base.TryConvertValue(value, type);
            return result;
        }
    }
}
