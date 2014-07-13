using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EnumStandardValuesAttribute : Attribute
    {

        public Type EnumType { get; set; }

        public EnumStandardValuesAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");

            }
            this.EnumType = enumType;
        }

        public string[] GetValues()
        {
            return Enum.GetNames(EnumType);
        }
    }

}
