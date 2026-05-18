namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HashInt
    {
        private const ulong H1 = 0xBF58476D1CE4E5B9UL;
        private const ulong H2 = 0x94D049BB133111EBUL;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            x = ((x >> 16) ^ x) * 0x45D9F3B;
            x = ((x >> 16) ^ x) * 0x45D9F3B;
            x = (x >> 16) ^ x;
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long x)
        {
            ulong h = (ulong)x;
            h = (h ^ (h >> 30)) * H1;
            h = (h ^ (h >> 27)) * H2;
            h = h ^ (h >> 31);
            return (int)h;
        }
    }

    public static unsafe class XorShift
    {
        private static uint _state = 123456789u;

        public static void SetSeed(uint seed)
        {
            _state = seed;
            if (_state == 0) _state = 123456789u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Next()
        {
            _state ^= _state << 13;
            _state ^= _state >> 17;
            _state ^= _state << 5;
            return _state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Range(int lo, int hi)
        {
            return lo + (int)(Next() % (uint)(hi - lo + 1));
        }
    }

    public static unsafe class RngSeed
    {
        private static int _counter = 0;

        public static int Run()
        {
            long t = DateTime.UtcNow.Ticks;
            int hash1 = (int)(((ulong)t >> 32) ^ (ulong)t);
            int hash2 = (int)((long)t ^ ((long)t >> 32));
            int hash3 = _counter++;
            return hash1 ^ hash2 ^ hash3;
        }
    }
}