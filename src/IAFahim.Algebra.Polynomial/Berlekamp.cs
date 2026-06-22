namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Berlekamp
    {
        private const int NoPivotColumn = -1;

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
        private static int FindPivotRow(long* Q, int n, int startRow, int col)
        {
            int row = startRow;
            int rowBase = row * n + col;
            while (row < n)
            {
                if (Q[rowBase] != 0L) break;
                row++;
                rowBase += n;
            }
            return row;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void NormalizePivotRow(long* Q, int n, int pivotRow, long inv, long mod)
        {
            long* rankRow = Q + pivotRow * n;
            for (int j = 0; j < n; j++)
                rankRow[j] = (rankRow[j] * inv) % mod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EliminateOtherRows(long* Q, int n, int pivotRow, int col, long mod)
        {
            long* rankRow = Q + pivotRow * n;
            for (int r = 0; r < n; r++)
            {
                int rBase = r * n;
                if (r != pivotRow && Q[rBase + col] != 0L)
                {
                    long factor = Q[rBase + col];
                    long* curRow = Q + rBase;
                    for (int j = 0; j < n; j++)
                    {
                        long t = curRow[j] - (factor * rankRow[j]) % mod;
                        t += (t >> 63) & mod;
                        curRow[j] = t;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReduceQMatrix(long* Q, int n, int MOD)
        {
            long mod = (long)MOD;
            int rank = 0;
            for (int col = 0; col < n && rank < n; col++)
            {
                int row = FindPivotRow(Q, n, rank, col);
                if (row == n) continue;
                if (row != rank) SwapRows(Q, n, rank, row);
                long inv = ModInv(Q[rank * n + col], mod);
                NormalizePivotRow(Q, n, rank, inv, mod);
                EliminateOtherRows(Q, n, rank, col, mod);
                rank++;
            }
            return rank;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapRows(long* Q, int n, int r1, int r2)
        {
            long* row1 = Q + r1 * n;
            long* row2 = Q + r2 * n;
            for (int j = 0; j < n; j++)
            {
                long tmpVal = row1[j];
                row1[j] = row2[j];
                row2[j] = tmpVal;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MarkPivotColumns(long* Q, int n, int rank, int* pivotColOfRow, byte* isPivotCol)
        {
            for (int c = 0; c < n; c++) isPivotCol[c] = 0;
            for (int r = 0; r < rank; r++)
            {
                long rowBase = (long)r * (long)n;
                int pivotCol = NoPivotColumn;
                for (int c = 0; c < n; c++)
                {
                    if (Q[rowBase + c] != 0L)
                    {
                        pivotCol = c;
                        break;
                    }
                }
                pivotColOfRow[r] = pivotCol;
                if (pivotCol >= 0) isPivotCol[pivotCol] = 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildBasisVector(long* Q, int n, int rank, int freeCol, int* pivotColOfRow, long mod, long* basis)
        {
            for (int j = 0; j < n; j++) basis[j] = 0L;
            basis[freeCol] = 1L;
            for (int r = 0; r < rank; r++)
            {
                int pivotCol = pivotColOfRow[r];
                if (pivotCol < 0) continue;
                long coeff = Q[(long)r * (long)n + (long)freeCol] % mod;
                if (coeff != 0L) coeff = mod - coeff;
                basis[pivotCol] = coeff;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ExtractBasis(long* poly, long* Q, int n, int rank, int MOD, long* outF, int* outL)
        {
            // Q is in reduced row-echelon form. Identify the pivot column of each
            // pivot row and mark every column as pivot or free. The null space of Q
            // (vectors v with Q v = 0) has dimension n - rank, with one basis vector
            // per free column.
            int* pivotColOfRow = stackalloc int[rank];
            byte* isPivotCol = stackalloc byte[n];
            long mod = (long)MOD;
            MarkPivotColumns(Q, n, rank, pivotColOfRow, isPivotCol);

            int factorCount = 0;
            long* basis = stackalloc long[n];
            for (int freeCol = 0; freeCol < n; freeCol++)
            {
                if (isPivotCol[freeCol] != 0) continue;
                BuildBasisVector(Q, n, rank, freeCol, pivotColOfRow, mod, basis);
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
