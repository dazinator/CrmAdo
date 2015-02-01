using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework.Utils
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendJoin(
            this StringBuilder stringBuilder, IEnumerable<string> values, string separator = ", ")
        {
            return stringBuilder.AppendJoin(values, (sb, value) => sb.Append(value), separator);
        }

        public static StringBuilder AppendJoin<T>(
            this StringBuilder stringBuilder, IEnumerable<T> values, Action<StringBuilder, T> joinAction,
            string separator)
        {
            var appended = false;

            foreach (var value in values)
            {
                joinAction(stringBuilder, value);
                stringBuilder.Append(separator);
                appended = true;
            }

            if (appended)
            {
                stringBuilder.Length -= separator.Length;
            }

            return stringBuilder;
        }
    }
}
