namespace IAFahim.Linear.Matrix
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LinearRecurrence
    {
        public static long Run(int k, long* init, long* trans, long n)
        {
            if (n < k) return init[n];
            long* result = stackalloc long[k];
            long* temp = stackalloc long[k];
            for (int i = 0; i < k; i++) result[i] = init[i];
            for (int i = 0; i < k; i++) temp[i] = trans[i];
            long* baseMat = stackalloc long[k * k];
            for (int i = 0; i < k; i++)
                for (int j = 0; j < k; j++)
                    baseMat[i * k + j] = 0;
            for (int i = 0; i < k; i++) baseMat[i] = temp[i];
            for (int i = 1; i < k; i++)
            {
                for (int j = 0; j < k - 1; j++)
                    baseMat[i * k + j] = (i == j + 1) ? 1 : 0;
            }
            long* mat = stackalloc long[k * k];
            for (int i = 0; i < k * k; i++) mat[i] = baseMat[i];
            long* res = stackalloc long[k * k];
            for (int i = 0; i < k; i++)
                for (int j = 0; j < k; j++)
                    res[i * k + j] = (i == j) ? 1 : 0;
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
                ans += res[j] * init[k - 1 - j];
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
            for (int i = 0; i < k; i++)
                for (int j = 0; j < k; j++)
                    mat[i * k + j] = 0;
            for (int i = 0; i < k; i++) mat[i] = trans[i];
            for (int i = 1; i < k; i++)
            {
                for (int j = 0; j < k - 1; j++)
                    mat[i * k + j] = (i == j + 1) ? 1 : 0;
            }
            long* res = stackalloc long[k * k];
            for (int i = 0; i < k; i++)
                for (int j = 0; j < k; j++)
                    res[i * k + j] = (i == j) ? 1 : 0;
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
                ans += res[j] * init[k - 1 - j];
            return ans;
        }
    }

    public static unsafe class Kitamasa
    {
        public static long Run(int k, long* init, long* trans, long n)
        {
            if (n < k) return init[n];
            long* pol = stackalloc long[k];
            long* res = stackalloc long[k];
            for (int i = 0; i < k; i++) pol[i] = 0;
            pol[1] = 1;
            for (int i = 0; i < k; i++) res[i] = (i == 0) ? 1 : 0;
            long exp = n - k + 1;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    long* newRes = stackalloc long[k];
                    for (int i = 0; i < k; i++) newRes[i] = 0;
                    for (int i = 0; i < k; i++)
                        if (res[i] != 0)
                            for (int j = 0; j < k; j++)
                                if (pol[j] != 0)
                                {
                                    int ni = i + j;
                                    if (ni >= k) ni = ni % k + k;
                                    newRes[ni] += res[i] * pol[j];
                                }
                    for (int i = 0; i < k; i++) res[i] = newRes[i] % 1000000007;
                }
                long* newPol = stackalloc long[k];
                for (int i = 0; i < k; i++) newPol[i] = 0;
                for (int i = 0; i < k; i++)
                    if (pol[i] != 0)
                        for (int j = 0; j < k; j++)
                            if (pol[j] != 0)
                            {
                                int ni = i + j;
                                if (ni >= k) ni = ni % k + k;
                                newPol[ni] += pol[i] * pol[j];
                            }
                for (int i = 0; i < k; i++) pol[i] = newPol[i] % 1000000007;
                exp >>= 1;
            }
            long ans = 0;
            for (int i = 0; i < k; i++)
                ans += res[i] * init[k - 1 - i];
            return ans % 1000000007;
        }
    }
}
