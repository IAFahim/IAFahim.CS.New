namespace IAFahim.Linear.Matrix
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KrylovSequence
    {
        public static void Run(int n, long* a, long* v, long* res)
        {
            for (int i = 0; i < n; i++)
            {
                long sum = 0;
                for (int j = 0; j < n; j++)
                    sum += a[i * n + j] * v[j];
                res[i] = sum;
            }
            for (int k = 1; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    long sum = 0;
                    for (int j = 0; j < n; j++)
                        sum += a[i * n + j] * res[(k - 1) * n + j];
                    res[k * n + i] = sum;
                }
            }
        }
    }

    public static unsafe class CharacteristicPolynomial
    {
        public static void Run(int n, long* a, long* poly)
        {
            long* mat = stackalloc long[n * n];
            for (int i = 0; i < n * n; i++) mat[i] = a[i];
            poly[0] = -n;
            for (int k = 1; k <= n; k++)
            {
                long trace = 0;
                for (int i = 0; i < n; i++)
                    trace += mat[i * n + i];
                poly[k] = trace;
                for (int i = 0; i < n * n; i++) mat[i] = a[i];
                for (int i = 0; i < k - 1; i++)
                {
                    long* newMat = stackalloc long[n * n];
                    for (int r = 0; r < n; r++)
                        for (int c = 0; c < n; c++)
                        {
                            long sum = 0;
                            for (int x = 0; x < n; x++)
                                sum += mat[r * n + x] * a[x * n + c];
                            newMat[r * n + c] = sum;
                        }
                    for (int j = 0; j < n * n; j++) mat[j] = newMat[j];
                }
                for (int i = 0; i < n * n; i++) mat[i] = a[i];
            }
        }
    }

    public static unsafe class LinearRecurrence
    {
        public static long Run(int k, long* init, long* trans, long n)
        {
            if (n < k) return init[n];
            long* mat = stackalloc long[k * k];
            for (int i = 0; i < k * k; i++) mat[i] = 0;
            for (int i = 0; i < k; i++) mat[i] = trans[i];
            for (int i = 1; i < k; i++)
                mat[i * k + (i - 1)] = 1;
            long* res = stackalloc long[k * k];
            for (int i = 0; i < k * k; i++) res[i] = 0;
            for (int i = 0; i < k; i++) res[i * k + i] = 1;
            long exp = n - k + 1;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    long* newRes = stackalloc long[k * k];
                    for (int i = 0; i < k; i++)
                        for (int j = 0; j < k; j++)
                        {
                            long sum = 0;
                            for (int x = 0; x < k; x++)
                                sum += res[i * k + x] * mat[x * k + j];
                            newRes[i * k + j] = sum;
                        }
                    for (int i = 0; i < k * k; i++) res[i] = newRes[i];
                }
                long* newMat = stackalloc long[k * k];
                for (int i = 0; i < k; i++)
                    for (int j = 0; j < k; j++)
                    {
                        long sum = 0;
                        for (int x = 0; x < k; x++)
                            sum += mat[i * k + x] * mat[x * k + j];
                        newMat[i * k + j] = sum;
                    }
                for (int i = 0; i < k * k; i++) mat[i] = newMat[i];
                exp >>= 1;
            }
            long ans = 0;
            for (int j = 0; j < k; j++)
                ans += res[0 * k + j] * init[k - 1 - j];
            return ans;
        }
    }

    public static unsafe class BerlekampMassey
    {
        public static int Run(long* s, int n, long* c)
        {
            long* C = stackalloc long[n];
            long* B = stackalloc long[n];
            int L = 0, m = 0;
            long b = 1;
            for (int i = 0; i < n; i++) C[i] = 0;
            for (int i = 0; i < n; i++) B[i] = 0;
            C[0] = 1;
            B[0] = 1;
            for (int i = 0; i < n; i++)
            {
                long d = s[i];
                for (int j = 1; j <= L; j++)
                    d += C[j] * s[i - j];
                if (d == 0) { m++; }
                else if (2 * L <= i)
                {
                    long* T = stackalloc long[n];
                    for (int j = 0; j < n; j++) T[j] = C[j];
                    long coef = d * b;
                    for (int j = 0; j <= n - m; j++)
                        C[m + j] -= coef * B[j];
                    L = i + 1 - L;
                    for (int j = 0; j < n; j++) B[j] = T[j];
                    b = d;
                    m = 1;
                }
                else
                {
                    long coef = d * b;
                    for (int j = 0; j <= n - m; j++)
                        C[m + j] -= coef * B[j];
                    m++;
                }
            }
            for (int i = 0; i <= L; i++) c[i] = C[i];
            return L;
        }
    }

    public static unsafe class LinearRecurrenceNth
    {
        public static long Run(int k, long* init, long* trans, long n)
        {
            if (n < k) return init[n];
            long* mat = stackalloc long[k * k];
            for (int i = 0; i < k * k; i++) mat[i] = 0;
            for (int i = 0; i < k; i++) mat[i] = trans[i];
            for (int i = 1; i < k; i++)
                mat[i * k + (i - 1)] = 1;
            long* res = stackalloc long[k * k];
            for (int i = 0; i < k * k; i++) res[i] = 0;
            for (int i = 0; i < k; i++) res[i * k + i] = 1;
            long exp = n - k + 1;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    long* newRes = stackalloc long[k * k];
                    for (int i = 0; i < k; i++)
                        for (int j = 0; j < k; j++)
                        {
                            long sum = 0;
                            for (int x = 0; x < k; x++)
                                sum += res[i * k + x] * mat[x * k + j];
                            newRes[i * k + j] = sum;
                        }
                    for (int i = 0; i < k * k; i++) res[i] = newRes[i];
                }
                long* newMat = stackalloc long[k * k];
                for (int i = 0; i < k; i++)
                    for (int j = 0; j < k; j++)
                    {
                        long sum = 0;
                        for (int x = 0; x < k; x++)
                            sum += mat[i * k + x] * mat[x * k + j];
                        newMat[i * k + j] = sum;
                    }
                for (int i = 0; i < k * k; i++) mat[i] = newMat[i];
                exp >>= 1;
            }
            long ans = 0;
            for (int j = 0; j < k; j++)
                ans += res[0 * k + j] * init[k - 1 - j];
            return ans;
        }
    }

    public static unsafe class Kitamasa
    {
        public static long Run(int k, long* init, long* trans, long n, long mod)
        {
            if (n < k) return init[n];
            long* pol = stackalloc long[k];
            long* res = stackalloc long[k];
            for (int i = 0; i < k; i++) { pol[i] = 0; res[i] = 0; }
            pol[0] = 1;
            res[1] = 1;
            long exp = n - k + 1;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    long* newRes = stackalloc long[2 * k];
                    for (int i = 0; i < 2 * k; i++) newRes[i] = 0;
                    for (int i = 0; i < k; i++)
                        for (int j = 0; j < k; j++)
                            newRes[i + j] = (newRes[i + j] + (res[i] % mod) * (pol[j] % mod)) % mod;
                    for (int i = 2 * k - 1; i >= k; i--)
                    {
                        for (int j = 1; j <= k; j++)
                            newRes[i - j] = (newRes[i - j] + (newRes[i] % mod) * (trans[k - j] % mod)) % mod;
                    }
                    for (int i = 0; i < k; i++) res[i] = newRes[i];
                }
                long* newPol = stackalloc long[2 * k];
                for (int i = 0; i < 2 * k; i++) newPol[i] = 0;
                for (int i = 0; i < k; i++)
                    for (int j = 0; j < k; j++)
                        newPol[i + j] = (newPol[i + j] + (pol[i] % mod) * (pol[j] % mod)) % mod;
                for (int i = 2 * k - 1; i >= k; i--)
                {
                    for (int j = 1; j <= k; j++)
                        newPol[i - j] = (newPol[i - j] + (newPol[i] % mod) * (trans[k - j] % mod)) % mod;
                }
                for (int i = 0; i < k; i++) pol[i] = newPol[i];
                exp >>= 1;
            }
            long ans = 0;
            for (int i = 0; i < k; i++)
                ans = (ans + (res[i] % mod) * (init[k - 1 - i] % mod)) % mod;
            return ans;
        }
    }
}
