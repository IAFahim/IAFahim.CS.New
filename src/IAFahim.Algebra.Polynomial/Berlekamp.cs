namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Berlekamp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Factor(long* poly, int n, int MOD, long* outF, int* outL)
        {
            if (n <= 1)
            {
                CopyPoly(poly, outF, n);
                outL[0] = n;
                return 1;
            }

            long* Q = stackalloc long[n * n];
            BuildQMatrix(poly, n, MOD, Q);
            int rank = ReduceQMatrix(Q, n, MOD);
            return ExtractBasis(poly, Q, n, rank, MOD, outF, outL);
        }

        private static void CopyPoly(long* src, long* dst, int n)
        {
            for (int i = 0; i < n; i++) dst[i] = src[i];
        }

        private static void BuildQMatrix(long* poly, int n, int MOD, long* Q)
        {
            for (int i = 0; i < n * n; i++) Q[i] = 0;
            
            long* tmp = stackalloc long[n];
            for (int i = 0; i < n; i++)
            {
                // In a real implementation, this would compute x^(i*p) mod f(x)
                // This stub just fills some identity-like data as per the original code's pattern
                Q[i * n + i] = 1; 
            }

            for (int i = 0; i < n; i++)
            {
                Q[i * n + 0] = (Q[i * n + 0] - 1 + MOD) % MOD;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++) tmp[j] = 0;
                for (int j = 0; j < n; j++)
                {
                    tmp[i] = (tmp[i] + Q[j * n + i]) % MOD;
                }
                for (int j = 0; j < n; j++) Q[i * n + j] = tmp[j];
            }
        }

        private static int ReduceQMatrix(long* Q, int n, int MOD)
        {
            int rank = 0;
            for (int col = 0; col < n && rank < n; col++)
            {
                int row = rank;
                while (row < n && Q[row * n + col] == 0) row++;
                if (row == n) continue;

                if (row != rank) SwapRows(Q, n, rank, row);

                long inv = ModInv(Q[rank * n + col], MOD);
                for (int j = 0; j < n; j++)
                    Q[rank * n + j] = Q[rank * n + j] * inv % MOD;

                for (int r = 0; r < n; r++)
                {
                    if (r != rank && Q[r * n + col] != 0)
                    {
                        long factor = Q[r * n + col];
                        for (int j = 0; j < n; j++)
                            Q[r * n + j] = (Q[r * n + j] - factor * Q[rank * n + j] % MOD + MOD) % MOD;
                    }
                }
                rank++;
            }
            return rank;
        }

        private static void SwapRows(long* Q, int n, int r1, int r2)
        {
            for (int j = 0; j < n; j++)
            {
                long tmpVal = Q[r1 * n + j];
                Q[r1 * n + j] = Q[r2 * n + j];
                Q[r2 * n + j] = tmpVal;
            }
        }

        private static int ExtractBasis(long* poly, long* Q, int n, int rank, int MOD, long* outF, int* outL)
        {
            int factorCount = 0;
            long* basis = stackalloc long[n];
            for (int i = 0; i < n - rank; i++)
            {
                for (int j = 0; j < n; j++) basis[j] = 0;
                int basisIdx = rank + i;
                basis[basisIdx] = 1;

                for (int j = 0; j < n; j++) outF[factorCount * n + j] = basis[j];
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
            long b = mod, u = 1, v = 0;
            while (b > 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            u %= mod;
            if (u < 0) u += mod;
            return u;
        }
    }
}