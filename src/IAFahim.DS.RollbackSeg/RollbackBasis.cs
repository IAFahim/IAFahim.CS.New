namespace IAFahim.DS.RollbackSeg
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LinearBasisRollbackInsert
    {
        private const int Bits = 64;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(long* basis, int* histSlot, byte* histWasEmpty, int* top, long x)
        {
            for (int i = Bits - 1; i >= 0; i--)
            {
                if (((x >> i) & 1L) == 0L) continue;
                if (basis[i] == 0L)
                {
                    basis[i] = x;
                    int t = *top;
                    histSlot[t] = i;
                    histWasEmpty[t] = 1;
                    *top = t + 1;
                    return true;
                }
                x ^= basis[i];
            }
            int t2 = *top;
            histSlot[t2] = -1;
            histWasEmpty[t2] = 0;
            *top = t2 + 1;
            return false;
        }
    }

    public static unsafe class LinearBasisRollbackMax
    {
        private const int Bits = 64;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* basis, long x)
        {
            for (int i = Bits - 1; i >= 0; i--)
            {
                if (basis[i] != 0L && ((x >> i) & 1L) == 0L) x ^= basis[i];
            }
            return x;
        }
    }

    public static unsafe class RangeBasisQuery
    {
        private const int Bits = 64;

        public static int Run(long* arr, int n, int l, int r, long* basisBuf)
        {
            if (l > r) return 0;
            for (int i = 0; i < Bits; i++) basisBuf[i] = 0L;
            int rank = 0;
            for (int k = l; k <= r; k++)
            {
                long x = arr[k];
                for (int i = Bits - 1; i >= 0; i--)
                {
                    if (((x >> i) & 1L) == 0L) continue;
                    if (basisBuf[i] == 0L) { basisBuf[i] = x; rank++; break; }
                    x ^= basisBuf[i];
                }
            }
            return rank;
        }
    }

    public static unsafe class LinearBasisRollback
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* basis, int* histSlot, byte* histWasEmpty, int* top, int checkpoint)
        {
            while (*top > checkpoint)
            {
                int t = --(*top);
                if (histWasEmpty[t] != 0 && histSlot[t] >= 0) basis[histSlot[t]] = 0L;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCheckpoint(int* top) => *top;
    }
}
