namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RollingHash
    {
        private const ulong Base = 131;
        private const ulong MOD = ulong.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Compute(byte* ptr, int len)
        {
            ulong hash = 0;
            for (int i = 0; i < len; i++)
                hash = hash * Base + ptr[i];
            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Build(byte* ptr, int len, ulong* prefix, ulong* power)
        {
            prefix[0] = 0;
            power[0] = 1;
            for (int i = 0; i < len; i++)
            {
                prefix[i + 1] = prefix[i] * Base + ptr[i];
                power[i + 1] = power[i] * Base;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Query(ulong* prefix, ulong* power, int l, int r)
        {
            return prefix[r] - prefix[l] * power[r - l];
        }
    }
}
