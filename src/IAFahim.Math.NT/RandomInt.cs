namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RandomInt
    {
        private static long _seed = 123456789L;

        public static void SetSeed(long seed)
        {
            _seed = seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Next(int bound)
        {
            _seed = SplitMix64.Run(_seed);
            return (int)(_seed % bound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Range(int lo, int hi)
        {
            return lo + Next(hi - lo + 1);
        }
    }

    public static unsafe class RandomInt64
    {
        private static long _seed = 123456789L;

        public static void SetSeed(long seed)
        {
            _seed = seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Next(long bound)
        {
            _seed = SplitMix64.Run(_seed);
            return _seed % bound;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Range(long lo, long hi)
        {
            return lo + Next(hi - lo + 1);
        }
    }

    public static unsafe class SplitMix64
    {
        private const ulong C1 = 0x9E3779B97F4A7C15UL;
        private const ulong C2 = 0xBF58476D1CE4E5B9UL;
        private const ulong C3 = 0x94D049BB133111EBUL;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x)
        {
            ulong u = (ulong)x;
            u += C1;
            u = (u ^ (u >> 30)) * C2;
            u = (u ^ (u >> 27)) * C3;
            u = u ^ (u >> 31);
            return (long)u;
        }
    }
}