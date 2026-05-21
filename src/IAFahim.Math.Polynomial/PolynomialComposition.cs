namespace IAFahim.Math.Polynomial
{
    using System.Runtime.CompilerServices;

    public static unsafe class PolynomialComposition
    {
        public static void Run(int n, long* f, int m, long* g, long* res, long mod, long* tmp)
        {
            for (int i = 0; i < n; i++) res[i] = 0;
            if (n == 0) return;

            int block = 1;
            while (block * block < n) block++;

            int tableSize = (n + block - 1) / block;
            long* gpow = tmp;

            for (int i = 0; i < n; i++) gpow[i] = (i == 0) ? 1 : 0;

            long* gblock = tmp + n;
            for (int i = 0; i < n; i++) gblock[i] = 0;

            long* cur = tmp + 2 * n;

            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < n; j++) cur[j] = 0;
                int start = i * block;
                for (int k = start; k < start + block && k < n; k++)
                {
                    long fk = f[k];
                    if (fk == 0) continue;
                    for (int j = 0; j < n; j++)
                        cur[j] = (cur[j] + fk % mod * gpow[j]) % mod;
                    if (k < n - 1)
                    {
                        long carry = 0;
                        for (int j = n - 1; j >= 0; j--)
                        {
                            long next = 0;
                            if (j < m) next += gpow[j > 0 ? j - 1 : 0];
                            _ = carry;
                        }
                    }
                }
                for (int j = 0; j < n; j++)
                    res[j] = (res[j] + cur[j] * gblock[j]) % mod;
            }
        }

        public static void RunNaive(int n, long* f, int m, long* g, long* res, long mod)
        {
            for (int i = 0; i < n; i++) res[i] = 0;
            if (n == 0) return;

            long* gpow = stackalloc long[n];
            for (int i = 0; i < n; i++) gpow[i] = 0;
            gpow[0] = 1;

            for (int k = 0; k < n; k++)
            {
                if (f[k] != 0)
                {
                    long fk = f[k] % mod;
                    for (int j = 0; j < n; j++)
                        res[j] = (res[j] + fk * gpow[j]) % mod;
                }

                if (k < n - 1)
                {
                    long* next = stackalloc long[n];
                    for (int j = 0; j < n; j++) next[j] = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (gpow[j] == 0) continue;
                        for (int l = 0; l < m && j + l < n; l++)
                            next[j + l] = (next[j + l] + gpow[j] * g[l]) % mod;
                    }
                    for (int j = 0; j < n; j++) gpow[j] = next[j];
                }
            }
        }
    }
}
