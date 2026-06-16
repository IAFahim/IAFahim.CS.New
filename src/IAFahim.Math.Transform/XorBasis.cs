namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class XorBasisInsert
    {
        public static void Run(long* basis, int* size, long x)
        {
            for (int i = 63; i >= 0; i--)
            {
                if ((x & (1L << i)) == 0) continue;
                if (basis[i] == 0)
                {
                    basis[i] = x;
                    (*size)++;
                    return;
                }
                x ^= basis[i];
            }
        }
    }

    public static unsafe class XorBasisMax
    {
        public static long Run(long* basis)
        {
            long result = 0;
            for (int i = 63; i >= 0; i--)
            {
                if ((result ^ basis[i]) > result)
                    result ^= basis[i];
            }
            return result;
        }
    }

    public static unsafe class XorBasisMin
    {
        public static long Run(long* basis)
        {
            long result = long.MaxValue;
            for (int i = 0; i < 64; i++)
            {
                if (basis[i] != 0 && basis[i] < result)
                    result = basis[i];
            }
            return result == long.MaxValue ? 0 : result;
        }
    }

    public static unsafe class XorBasisRank
    {
        public static int Run(long* basis, long x)
        {
            int rank = 0;
            for (int i = 63; i >= 0; i--)
            {
                if ((x & (1L << i)) == 0) continue;
                if (basis[i] == 0) return -1;
                x ^= basis[i];
                rank++;
            }
            return rank;
        }
    }

    public static unsafe class XorBasisKth
    {
        public static long Run(long* basis, int k, int size)
        {
            long* vec = stackalloc long[64];
            int* pivot = stackalloc int[64];
            int cnt = 0;
            for (int i = 0; i < 64; i++)
            {
                if (basis[i] != 0)
                {
                    vec[cnt] = basis[i];
                    pivot[cnt] = i;
                    cnt++;
                }
            }
            // vec is collected with strictly increasing pivot, so each vec[i]'s highest
            // set bit is pivot[i] and the rows are already in row-echelon form.
            // Back-substitute to reduced echelon: clear every lower pivot bit from each
            // row using the (already reduced) rows below it, so each pivot bit pivot[i]
            // ends up set in exactly one row, vec[i]. Processing i ascending is safe
            // because vec[j] for j < i is fully canonical when used as the eliminator.
            for (int i = 1; i < cnt; i++)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (((vec[i] >> pivot[j]) & 1L) != 0)
                        vec[i] ^= vec[j];
                }
            }
            // k-th smallest: bit i of k selects the i-th smallest pivot vector (vec[i]).
            long result = 0;
            for (int i = 0; i < cnt; i++)
            {
                if ((k & (1 << i)) != 0)
                    result ^= vec[i];
            }
            return result;
        }
    }
}