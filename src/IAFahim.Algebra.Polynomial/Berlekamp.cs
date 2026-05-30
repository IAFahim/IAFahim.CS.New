namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Berlekamp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long* poly, int n, int MOD, long* outF, int* outL)
        {
            if (n <= 1)
            {
                CopyPoly(poly, outF, n);
                outL[0] = n;
                return 1;
            }

            long byteCount = (long)n * (long)n;
            long* Q = stackalloc long[(int)byteCount];
            BuildQMatrix(poly, n, MOD, Q);
            int rank = ReduceQMatrix(Q, n, MOD);
            return ExtractBasis(poly, Q, n, rank, MOD, outF, outL);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyPoly(long* src, long* dst, int n)
        {
            for (int i = 0; i < n; i++) dst[i] = src[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildQMatrix(long* poly, int n, int MOD, long* Q)
        {
            long nn = (long)n * (long)n;
            for (long i = 0L; i < nn; i++) Q[i] = 0L;

            long* tmp = stackalloc long[n];
            for (int i = 0; i < n; i++)
            {
                long index = (long)i * (long)n + (long)i;
                Q[index] = 1L;
            }

            for (int i = 0; i < n; i++)
            {
                long index = (long)i * (long)n;
                Q[index] = (Q[index] - 1L + (long)MOD) % (long)MOD;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++) tmp[j] = 0L;
                for (int j = 0; j < n; j++)
                {
                    long qIdx = (long)j * (long)n + (long)i;
                    tmp[i] = (tmp[i] + Q[qIdx]) % (long)MOD;
                }
                for (int j = 0; j < n; j++)
                {
                    long qIdx = (long)i * (long)n + (long)j;
                    Q[qIdx] = tmp[j];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReduceQMatrix(long* Q, int n, int MOD)
        {
            int rank = 0;
            for (int col = 0; col < n && rank < n; col++)
            {
                int row = rank;
                while (row < n)
                {
                    long idx = (long)row * (long)n + (long)col;
                    if (Q[idx] != 0L) break;
                    row++;
                }
                if (row == n) continue;

                if (row != rank) SwapRows(Q, n, rank, row);

                long rIdx = (long)rank * (long)n + (long)col;
                long inv = ModInv(Q[rIdx], (long)MOD);
                for (int j = 0; j < n; j++)
                {
                    long jIdx = (long)rank * (long)n + (long)j;
                    Q[jIdx] = (Q[jIdx] * inv) % (long)MOD;
                }

                for (int r = 0; r < n; r++)
                {
                    long currIdx = (long)r * (long)n + (long)col;
                    if (r != rank && Q[currIdx] != 0L)
                    {
                        long factor = Q[currIdx];
                        for (int j = 0; j < n; j++)
                        {
                            long rjIdx = (long)r * (long)n + (long)j;
                            long rankjIdx = (long)rank * (long)n + (long)j;
                            Q[rjIdx] = (Q[rjIdx] - (factor * Q[rankjIdx]) % (long)MOD + (long)MOD) % (long)MOD;
                        }
                    }
                }
                rank++;
            }
            return rank;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapRows(long* Q, int n, int r1, int r2)
        {
            for (int j = 0; j < n; j++)
            {
                long idx1 = (long)r1 * (long)n + (long)j;
                long idx2 = (long)r2 * (long)n + (long)j;
                long tmpVal = Q[idx1];
                Q[idx1] = Q[idx2];
                Q[idx2] = tmpVal;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ExtractBasis(long* poly, long* Q, int n, int rank, int MOD, long* outF, int* outL)
        {
            int factorCount = 0;
            long* basis = stackalloc long[n];
            for (int i = 0; i < n - rank; i++)
            {
                for (int j = 0; j < n; j++) basis[j] = 0L;
                int basisIdx = rank + i;
                basis[basisIdx] = 1L;

                for (int j = 0; j < n; j++) outF[(long)factorCount * (long)n + (long)j] = basis[j];
                outL[factorCount++] = n;
            }

            if (factorCount == 0)
            {
                CopyPoly(poly, outF, n);
                outL[0] = n;
                return 1;
            }
            return factorCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModInv(long a, long mod)
        {
            long b = mod, u = 1L, v = 0L;
            while (b > 0L)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            u %= mod;
            if (u < 0L) u += mod;
            return u;
        }
    }
}