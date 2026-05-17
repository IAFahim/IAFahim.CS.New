using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModSqrt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long mod)
        {
            a = ModNormalize.Run(a, mod);
            if (a == 0) return 0;
            if (ModPow.Run(a, (mod - 1) / 2, mod) != 1) return -1;
            if (mod % 4 == 3) return ModPow.Run(a, (mod + 1) / 4, mod);

            long q = mod - 1;
            int s = 0;
            while ((q & 1) == 0) { q >>= 1; s++; }

            long z = 2;
            while (ModPow.Run(z, (mod - 1) / 2, mod) != mod - 1) z++;

            long m = s;
            long c = ModPow.Run(z, q, mod);
            long t = ModPow.Run(a, q, mod);
            long r = ModPow.Run(a, (q + 1) / 2, mod);

            while (true)
            {
                if (t == 0) return 0;
                if (t == 1) return r < mod - r ? r : mod - r;

                long tmp = t;
                int i = 0;
                while (tmp != 1) { tmp = ModMul.Run(tmp, tmp, mod); i++; }

                long b2 = c;
                for (int j = 0; j < (int)(m - i - 1); j++) b2 = ModMul.Run(b2, b2, mod);

                m = i;
                c = ModMul.Run(b2, b2, mod);
                t = ModMul.Run(t, c, mod);
                r = ModMul.Run(r, b2, mod);
            }
        }
    }
}