namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Bsgs
    {
        private const long EmptySlot = -1;

        private const long NoSolution = -1;

        private const long LogOfIdentity = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModMul(long a, long b, long mod)
        {
            return IAFahim.Math.NT.ModMul.Run(a, b, mod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long a, long e, long mod)
        {
            return IAFahim.Math.NT.ModPow.Run(a, e, mod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Gcd(long a, long b)
        {
            if (a < 0) a = -a;
            if (b < 0) b = -b;
            while (b != 0)
            {
                long t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Normalize(long x, long mod)
        {
            x %= mod;
            if (x < 0) x += mod;
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitTable(long* scratchKeys, long* scratchVals, int tableSize)
        {
            for (int i = 0; i < tableSize; i++)
            {
                scratchKeys[i] = EmptySlot;
                scratchVals[i] = EmptySlot;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindSlot(long* scratchKeys, int tableSize, long key)
        {
            int pos = (int)(key % tableSize);
            int probed = 0;
            while (probed < tableSize && scratchKeys[pos] != EmptySlot && scratchKeys[pos] != key)
            {
                pos = (pos + 1) % tableSize;
                probed++;
            }
            return pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long BuildBabySteps(long a, long mod, int tableSize, long* scratchKeys, long* scratchVals)
        {
            long am = 1;
            for (int i = 0; i < tableSize; i++)
            {
                long key = am;
                int pos = FindSlot(scratchKeys, tableSize, key);
                if (scratchKeys[pos] == EmptySlot)
                {
                    scratchKeys[pos] = key;
                    scratchVals[pos] = i;
                }
                am = ModMul(am, a, mod);
            }
            return ModPow(am, mod - 2, mod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ProbeGiantSteps(long b, long mod, int tableSize, long factor, long* scratchKeys, long* scratchVals)
        {
            long cur = b;
            for (int i = 0; i < tableSize; i++)
            {
                int pos = FindSlot(scratchKeys, tableSize, cur);
                if (scratchKeys[pos] == cur)
                {
                    long ans = (long)i * tableSize + scratchVals[pos];
                    if (ans < mod) return ans;
                }
                cur = ModMul(cur, factor, mod);
            }
            return NoSolution;
        }

        public static long Run(long a, long b, long mod, long* scratchKeys, long* scratchVals)
        {
            if (b == 1) return LogOfIdentity;
            a = Normalize(a, mod);
            b = Normalize(b, mod);
            long g = Gcd(a, mod);
            if (b % g != 0) return NoSolution;
            long m = (long)Math.Ceiling(Math.Sqrt(mod));
            int tableSize = (int)m;
            InitTable(scratchKeys, scratchVals, tableSize);
            long factor = BuildBabySteps(a, mod, tableSize, scratchKeys, scratchVals);
            return ProbeGiantSteps(b, mod, tableSize, factor, scratchKeys, scratchVals);
        }
    }
}
