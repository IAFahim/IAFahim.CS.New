namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Independence
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsIndependentSet(int n, bool* adj, int mask)
        {
            for (int i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) == 0) continue;
                for (int j = i + 1; j < n; j++)
                {
                    if ((mask & (1 << j)) == 0) continue;
                    long index = (long)i * (long)n + (long)j;
                    if (adj[index]) return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PopCount(int mask)
        {
            int bits = 0;
            while (mask > 0) { if ((mask & 1) != 0) bits++; mask >>= 1; }
            return bits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Polynomial(int n, bool* adj, long x, int MOD)
        {
            long* xPowTable = stackalloc long[n + 1];
            xPowTable[0] = 1L;
            for (int i = 1; i <= n; i++) xPowTable[i] = (xPowTable[i - 1] * x) % (long)MOD;

            long result = 0L;
            int size = 1 << n;
            for (int mask = 0; mask < size; mask++)
            {
                if (IsIndependentSet(n, adj, mask))
                {
                    int bits = PopCount(mask);
                    result = (result + xPowTable[bits]) % (long)MOD;
                }
            }
            return result;
        }
    }
}