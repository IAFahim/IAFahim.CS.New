namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PosetTransforms
    {
        public static void ZetaTransform(long* f, long* g, int* topOrder, bool* relation, int n, long mod)
        {
            for (int i = 0; i < n; i++)
            {
                int y = topOrder[i]; long sum = 0;
                for (int j = 0; j <= i; j++) { int x = topOrder[j]; if (relation[x * n + y]) sum = (sum + f[x]) % mod; }
                g[y] = sum;
            }
        }

        public static void MobiusTransform(long* g, long* f, int* topOrder, bool* relation, int n, long mod, long* mu)
        {
            ComputeMuMatrix(n, topOrder, relation, mod, mu);
            ApplyMuMatrix(n, topOrder, relation, mod, mu, g, f);
        }

        private static void ComputeMuMatrix(int n, int* topOrder, bool* relation, long mod, long* mu)
        {
            for (int i = 0; i < (long)n * n; i++) mu[i] = 0;
            for (int i = 0; i < n; i++)
            {
                int x = topOrder[i]; mu[(long)x * n + x] = 1;
                for (int j = i + 1; j < n; j++)
                {
                    int y = topOrder[j];
                    if (relation[x * n + y])
                    {
                        long sum = ComputeMuSum(i, j, y, topOrder, relation, mod, mu, x, n);
                        mu[(long)x * n + y] = (mod - sum) % mod;
                    }
                }
            }
        }

        private static long ComputeMuSum(int i, int j, int y, int* topOrder, bool* relation, long mod, long* mu, int x, int n)
        {
            long sum = 0;
            for (int k = i; k < j; k++)
            {
                int z = topOrder[k];
                if (relation[z * n + y]) sum = (sum + mu[(long)x * n + z]) % mod;
            }
            return sum;
        }

        private static void ApplyMuMatrix(int n, int* topOrder, bool* relation, long mod, long* mu, long* g, long* f)
        {
            for (int i = 0; i < n; i++)
            {
                int y = topOrder[i]; long sum = 0;
                for (int j = 0; j <= i; j++)
                {
                    int x = topOrder[j];
                    if (relation[x * n + y]) sum = (sum + mu[(long)x * n + y] * g[x]) % mod;
                }
                f[y] = sum;
            }
        }

        public static int LatticeMeet(int x, int y, bool* relation, int n)
        {
            for (int z = 0; z < n; z++)
            {
                if (relation[z * n + x] && relation[z * n + y])
                {
                    bool ok = true;
                    for (int w = 0; w < n; w++) if (relation[w * n + x] && relation[w * n + y] && !relation[w * n + z]) { ok = false; break; }
                    if (ok) return z;
                }
            }
            return -1;
        }

        public static int LatticeJoin(int x, int y, bool* relation, int n)
        {
            for (int z = 0; z < n; z++)
            {
                if (relation[x * n + z] && relation[y * n + z])
                {
                    bool ok = true;
                    for (int w = 0; w < n; w++) if (relation[x * n + w] && relation[y * n + w] && !relation[z * n + w]) { ok = false; break; }
                    if (ok) return z;
                }
            }
            return -1;
        }
        public static int BooleanLatticeRank(int x) { int c = 0; while (x > 0) { x &= (x - 1); c++; } return c; }
    }
}
