using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.ComponentModel
{
    public class CrmConnectionStringBuilderConverter : TypeConverter
    {
        public CrmConnectionStringBuilderConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value as string;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            return value as string;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            TypeConverter.StandardValuesCollection standardValues;
            EnumStandardValuesAttribute item = (EnumStandardValuesAttribute)context.PropertyDescriptor.Attributes[typeof(EnumStandardValuesAttribute)];
            if (item == null)
            {
                standardValues = base.GetStandardValues(context);
            }
            else
            {
                standardValues = new TypeConverter.StandardValuesCollection(item.GetValues());
            }
            return standardValues;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }  
}
