namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RandomInt
    {
        private const int HashHighShift = 32;

        private static long _seed = 123456789L;

        public static void SetSeed(long seed)
        {
            _seed = seed;
        }

        // Stateless, pure, thread-safe core. Threads the RNG state explicitly so
        // Burst jobs can hold per-thread state with no contention. Maps uniformly
        // into [0, bound) via Lemire's multiply-shift reduction (no division, and
        // never negative). Caller must pass bound > 0 (unchecked Run-style contract).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Next(ref long state, int bound)
        {
            state = SplitMix64.Run(state);
            uint hash = (uint)((ulong)state >> HashHighShift);
            return (int)(((ulong)hash * (ulong)(uint)bound) >> HashHighShift);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Range(ref long state, int lo, int hi)
        {
            return lo + Next(ref state, hi - lo + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Next(int bound)
        {
            return Next(ref _seed, bound);
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

        // Stateless, pure, thread-safe core. Reduces in unsigned space so the
        // result is always in [0, bound) (the signed remainder of a SplitMix64
        // output is negative ~50% of the time). Caller must pass bound > 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Next(ref long state, long bound)
        {
            state = SplitMix64.Run(state);
            return (long)((ulong)state % (ulong)bound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Range(ref long state, long lo, long hi)
        {
            return lo + Next(ref state, hi - lo + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Next(long bound)
        {
            return Next(ref _seed, bound);
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
