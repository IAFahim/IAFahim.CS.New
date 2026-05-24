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

            InitializeTonelliShanks(mod, out long q, out int s, out long z);
            return SolveTonelliShanks(a, mod, q, s, z);
        }

        private static void InitializeTonelliShanks(long mod, out long q, out int s, out long z)
        {
            q = mod - 1; s = 0; while ((q & 1) == 0) { q >>= 1; s++; }
            z = 2; while (ModPow.Run(z, (mod - 1) / 2, mod) != mod - 1) z++;
        }

        private static long SolveTonelliShanks(long a, long mod, long q, int s, long z)
        {
            long m = s, c = ModPow.Run(z, q, mod), t = ModPow.Run(a, q, mod), r = ModPow.Run(a, (q + 1) / 2, mod);
            while (true)
            {
                if (t == 0) return 0;
                if (t == 1) return System.Math.Min(r, mod - r);
                int i = FindFirstOne(t, mod);
                long b2 = ComputeB2(c, m, i, mod);
                m = i; c = ModMul.Run(b2, b2, mod); t = ModMul.Run(t, c, mod); r = ModMul.Run(r, b2, mod);
            }
        }

        private static int FindFirstOne(long t, long mod)
        {
            int i = 0; long tmp = t;
            while (tmp != 1) { tmp = ModMul.Run(tmp, tmp, mod); i++; }
            return i;
        }

        private static long ComputeB2(long c, long m, int i, long mod)
        {
            long b2 = c;
            for (int j = 0; j < (int)(m - i - 1); j++) b2 = ModMul.Run(b2, b2, mod);
            return b2;
        }
    }
}
