using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Support
{
    public static class RandomExtensions
    {
        public static long NextLong(this Random random, long min, long max)
        {
            byte[] buf = new byte[8];
            random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (max - min)) + min);
        }

        public static DateTime NextCrmDate(this Random random, DateTime minDate, int dayRange)
        {
            return minDate.AddDays(random.Next(dayRange));
        }

        public static decimal NextDecimal(this Random r, byte scale)
        {
            var s = scale;
            var a = (int)(uint.MaxValue * r.NextDouble());
            var b = (int)(uint.MaxValue * r.NextDouble());
            var c = (int)(uint.MaxValue * r.NextDouble());
            var n = r.NextDouble() >= 0.5;
            return new Decimal(a, b, c, n, s);
        }
    }
}
