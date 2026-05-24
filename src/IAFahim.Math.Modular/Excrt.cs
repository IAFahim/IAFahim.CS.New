using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class Excrt
    {
        public static long Run(long* remainders, long* moduli, int len)
        {
            if (len == 0) return 0;
            long r = (remainders[0] % moduli[0] + moduli[0]) % moduli[0], m = moduli[0];
            if (m <= 0) return -1;
            for (int i = 1; i < len; i++)
                if (!TryMerge(ref r, ref m, remainders[i], moduli[i])) return -1;
            return r;
        }

        private static bool TryMerge(ref long r, ref long m, long r2, long m2)
        {
            if (m2 <= 0) return false;
            r2 = (r2 % m2 + m2) % m2;
            long x, y; long g = ExtendedGcd.Run(m, m2, out x, out y);
            if ((r2 - r) % g != 0) return false;
            long m2_g = m2 / g; if (long.MaxValue / m < m2_g) return false;
            long t = ModMul.Run(x, (r2 - r) / g, m2_g);
            r += t * m; m *= m2_g; r = (r % m + m) % m;
            return true;
        }
    }
}
