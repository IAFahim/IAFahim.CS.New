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
                for (int i = 0; i < n; i++) outF[i] = poly[i];
                outL[0] = n;
                return 1;
            }

            long* Q = stackalloc long[n * n];
            long* tmp = stackalloc long[n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++) Q[i * n + j] = 0;
            }

            long* basePoly = stackalloc long[n + 1];
            for (int i = 0; i < n; i++) basePoly[i] = poly[i];
            basePoly[n] = 0;

            for (int i = 0; i < n; i++)
            {
                long* rem = stackalloc long[n];
                for (int j = 0; j < n; j++) rem[j] = 0;
                rem[0] = 1;

                long* power = stackalloc long[n];
                for (int j = 0; j < n; j++) power[j] = 0;
                power[0] = 1;

                int exp = i + 1;
                long* expPoly = stackalloc long[n + 1];
                for (int j = 0; j <= n; j++) expPoly[j] = 0;
                expPoly[1] = 1;

                for (int j = 0; j < n; j++) Q[i * n + j] = power[j];
            }

            for (int i = 0; i < n; i++)
            {
                Q[i * n + 0] = (Q[i * n + 0] - 1 + MOD) % MOD;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    tmp[i] = (tmp[i] + Q[j * n + i]) % MOD;
                }
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++) Q[i * n + j] = tmp[j];
            }

            int rank = 0;
            for (int col = 0; col < n && rank < n; col++)
            {
                int row = rank;
                while (row < n && Q[row * n + col] == 0) row++;
                if (row == n) continue;

                if (row != rank)
                {
                    for (int j = 0; j < n; j++)
                    {
                        long tmpVal = Q[rank * n + j];
                        Q[rank * n + j] = Q[row * n + j];
                        Q[row * n + j] = tmpVal;
                    }
                }

                long inv = ModInv(Q[rank * n + col], MOD);
                for (int j = 0; j < n; j++)
                    Q[rank * n + j] = Q[rank * n + j] * inv % MOD;

                for (int r = 0; r < n; r++)
                {
                    if (r != rank && Q[r * n + col] != 0)
                    {
                        long factor = Q[r * n + col];
                        for (int j = 0; j < n; j++)
                            Q[r * n + j] = (Q[r * n + j] - factor * Q[rank * n + j]) % MOD;
                    }
                }
                rank++;
            }

            int factorCount = 0;
            long* basis = stackalloc long[n];
            for (int i = 0; i < n - rank; i++)
            {
                for (int j = 0; j < n; j++) basis[j] = 0;
                int basisIdx = rank + i;
                basis[basisIdx] = 1;

                int writePos = 0;
                for (int j = 0; j < n; j++) outF[writePos++] = basis[j];
                outL[factorCount++] = n;
            }

            if (factorCount == 0)
            {
                for (int i = 0; i < n; i++) outF[i] = poly[i];
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