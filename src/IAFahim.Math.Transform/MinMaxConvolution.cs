namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinMaxConvolution
    {
        public static void MinIndex(long* a, long* b, long* c, int n, long mod, long* sa, long* sb, long* sc)
        {
            sa[n - 1] = a[n - 1] % mod;
            sb[n - 1] = b[n - 1] % mod;
            for (int i = n - 2; i >= 0; i--)
            {
                sa[i] = (sa[i + 1] + a[i]) % mod;
                sb[i] = (sb[i + 1] + b[i]) % mod;
            }
            for (int i = 0; i < n; i++)
            {
                sc[i] = sa[i] * sb[i] % mod;
            }
            for (int i = 0; i < n - 1; i++)
            {
                c[i] = (sc[i] - sc[i + 1] + mod) % mod;
            }
            c[n - 1] = sc[n - 1];
        }

        public static void MaxIndex(long* a, long* b, long* c, int n, long mod, long* pa, long* pb, long* pc)
        {
            pa[0] = a[0] % mod;
            pb[0] = b[0] % mod;
            for (int i = 1; i < n; i++)
            {
                pa[i] = (pa[i - 1] + a[i]) % mod;
                pb[i] = (pb[i - 1] + b[i]) % mod;
            }
            for (int i = 0; i < n; i++)
            {
                pc[i] = pa[i] * pb[i] % mod;
            }
            c[0] = pc[0];
            for (int i = 1; i < n; i++)
            {
                c[i] = (pc[i] - pc[i - 1] + mod) % mod;
            }
        }

        public static void MinPlusGeneral(long* a, int aLen, long* b, int bLen, long* c)
        {
            int limit = aLen + bLen - 1;
            for (int k = 0; k < limit; k++)
            {
                long best = long.MaxValue;
                for (int i = 0; i < aLen; i++)
                {
                    int j = k - i;
                    if (j >= 0 && j < bLen)
                    {
                        long sum = a[i] + b[j];
                        if (sum < best)
                        {
                            best = sum;
                        }
                    }
                }
                c[k] = best;
            }
        }

        public static void MaxPlusGeneral(long* a, int aLen, long* b, int bLen, long* c)
        {
            int limit = aLen + bLen - 1;
            for (int k = 0; k < limit; k++)
            {
                long best = long.MinValue;
                for (int i = 0; i < aLen; i++)
                {
                    int j = k - i;
                    if (j >= 0 && j < bLen)
                    {
                        long sum = a[i] + b[j];
                        if (sum > best)
                        {
                            best = sum;
                        }
                    }
                }
                c[k] = best;
            }
        }

        public static void MinPlusConvexArbitrary(long* a, int n, long* b, int m, long* c)
        {
            SolveMinPlus(0, n + m - 2, 0, m - 1, a, n, b, m, c);
        }

        public static void MaxPlusConcaveArbitrary(long* a, int n, long* b, int m, long* c)
        {
            SolveMaxPlus(0, n + m - 2, 0, m - 1, a, n, b, m, c);
        }

        public static void MinPlusConvexConvex(long* a, int n, long* b, int m, long* c)
        {
            c[0] = a[0] + b[0];
            int i = 0;
            int j = 0;
            int k = 1;
            while (i < n - 1 && j < m - 1)
            {
                long diffA = a[i + 1] - a[i];
                long diffB = b[j + 1] - b[j];
                if (diffA < diffB)
                {
                    c[k] = c[k - 1] + diffA;
                    i++;
                }
                else
                {
                    c[k] = c[k - 1] + diffB;
                    j++;
                }
                k++;
            }
            while (i < n - 1)
            {
                c[k] = c[k - 1] + (a[i + 1] - a[i]);
                i++;
                k++;
            }
            while (j < m - 1)
            {
                c[k] = c[k - 1] + (b[j + 1] - b[j]);
                j++;
                k++;
            }
        }

        public static void MaxPlusConcaveConcave(long* a, int n, long* b, int m, long* c)
        {
            c[0] = a[0] + b[0];
            int i = 0;
            int j = 0;
            int k = 1;
            while (i < n - 1 && j < m - 1)
            {
                long diffA = a[i + 1] - a[i];
                long diffB = b[j + 1] - b[j];
                if (diffA > diffB)
                {
                    c[k] = c[k - 1] + diffA;
                    i++;
                }
                else
                {
                    c[k] = c[k - 1] + diffB;
                    j++;
                }
                k++;
            }
            while (i < n - 1)
            {
                c[k] = c[k - 1] + (a[i + 1] - a[i]);
                i++;
                k++;
            }
            while (j < m - 1)
            {
                c[k] = c[k - 1] + (b[j + 1] - b[j]);
                j++;
                k++;
            }
        }

        private static void SolveMinPlus(
            int l, int r,
            int optL, int optR,
            long* a, int n,
            long* b, int m,
            long* c)
        {
            if (l > r)
            {
                return;
            }
            int mid = (l + r) >> 1;
            long bestVal = long.MaxValue;
            int bestJ = -1;
            int startJ = Math.Max(optL, mid - n + 1);
            int endJ = Math.Min(optR, Math.Min(mid, m - 1));
            for (int j = startJ; j <= endJ; j++)
            {
                int i = mid - j;
                if (i >= 0 && i < n)
                {
                    long candA = a[i];
                    long candB = b[j];
                    if (candA != long.MaxValue && candB != long.MaxValue)
                    {
                        long val = candA + candB;
                        if (val < bestVal)
                        {
                            bestVal = val;
                            bestJ = j;
                        }
                    }
                }
            }
            c[mid] = bestVal;
            if (bestJ == -1)
            {
                bestJ = optL;
            }
            SolveMinPlus(l, mid - 1, optL, bestJ, a, n, b, m, c);
            SolveMinPlus(mid + 1, r, bestJ, optR, a, n, b, m, c);
        }

        private static void SolveMaxPlus(
            int l, int r,
            int optL, int optR,
            long* a, int n,
            long* b, int m,
            long* c)
        {
            if (l > r)
            {
                return;
            }
            int mid = (l + r) >> 1;
            long bestVal = long.MinValue;
            int bestJ = -1;
            int startJ = Math.Max(optL, mid - n + 1);
            int endJ = Math.Min(optR, Math.Min(mid, m - 1));
            for (int j = startJ; j <= endJ; j++)
            {
                int i = mid - j;
                if (i >= 0 && i < n)
                {
                    long candA = a[i];
                    long candB = b[j];
                    if (candA != long.MinValue && candB != long.MinValue)
                    {
                        long val = candA + candB;
                        if (val > bestVal)
                        {
                            bestVal = val;
                            bestJ = j;
                        }
                    }
                }
            }
            c[mid] = bestVal;
            if (bestJ == -1)
            {
                bestJ = optL;
            }
            SolveMaxPlus(l, mid - 1, optL, bestJ, a, n, b, m, c);
            SolveMaxPlus(mid + 1, r, bestJ, optR, a, n, b, m, c);
        }
    }
}
