namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Bit3D
    {
        public struct BIT3D { public int X, Y, Z; public int Size; public long* Tree; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(BIT3D* bit, int x, int y, int z)
        {
            bit->X = x; bit->Y = y; bit->Z = z;
            bit->Size = x * y * z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(BIT3D* bit, int xi, int yi, int zi, long val)
        {
            for (int i = xi; i < bit->X; i += i & -i)
            for (int j = yi; j < bit->Y; j += j & -j)
            for (int k = zi; k < bit->Z; k += k & -k)
            {
                long idx = (long)i * bit->Y * bit->Z + (long)j * bit->Z + k;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sum(BIT3D* bit, int xi, int yi, int zi)
        {
            long res = 0;
            for (int i = xi; i > 0; i -= i & -i)
            for (int j = yi; j > 0; j -= j & -j)
            for (int k = zi; k > 0; k -= k & -k)
            {
                long idx = (long)i * bit->Y * bit->Z + (long)j * bit->Z + k;
                res += idx;
            }
            return res;
        }
    }
}
