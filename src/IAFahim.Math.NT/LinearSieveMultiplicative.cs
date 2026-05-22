namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class LinearSieveMultiplicative
    {
        public static void Run(
            long* f,
            int* primes,
            int n,
            out int primeCount,
            delegate* managed<int, int, long> fPower)
        {
            if (n < 1)
            {
                primeCount = 0;
                return;
            }
            f[1] = 1;
            if (n == 1)
            {
                primeCount = 0;
                return;
            }

            int* e = null;
            long* pk = null;
            bool* isPrime = null;
            bool allocated = false;

            if (n > 10000)
            {
                e = (int*)Marshal.AllocHGlobal((nint)(n + 1) * sizeof(int));
                pk = (long*)Marshal.AllocHGlobal((nint)(n + 1) * sizeof(long));
                isPrime = (bool*)Marshal.AllocHGlobal((nint)(n + 1) * sizeof(bool));
                allocated = true;
            }
            else
            {
                int* tempE = stackalloc int[n + 1];
                long* tempPk = stackalloc long[n + 1];
                bool* tempIsPrime = stackalloc bool[n + 1];
                e = tempE;
                pk = tempPk;
                isPrime = tempIsPrime;
            }

            try
            {
                for (int i = 2; i <= n; i++)
                {
                    isPrime[i] = true;
                }
                primeCount = 0;
                e[1] = 0;
                pk[1] = 1;

                for (int i = 2; i <= n; i++)
                {
                    if (isPrime[i])
                    {
                        primes[primeCount++] = i;
                        f[i] = fPower(i, 1);
                        e[i] = 1;
                        pk[i] = (long)i;
                    }
                    for (int j = 0; j < primeCount; j++)
                    {
                        long prod = (long)i * primes[j];
                        if (prod > n)
                        {
                            break;
                        }
                        int ip = (int)prod;
                        int p = primes[j];
                        isPrime[ip] = false;

                        if (i % p == 0)
                        {
                            e[ip] = e[i] + 1;
                            pk[ip] = pk[i] * (long)p;
                            long rem = (long)ip / pk[ip];
                            f[ip] = fPower(p, e[ip]) * (rem == 1 ? 1L : f[rem]);
                            break;
                        }
                        else
                        {
                            e[ip] = 1;
                            pk[ip] = (long)p;
                            f[ip] = f[i] * f[p];
                        }
                    }
                }
            }
            finally
            {
                if (allocated)
                {
                    Marshal.FreeHGlobal((nint)e);
                    Marshal.FreeHGlobal((nint)pk);
                    Marshal.FreeHGlobal((nint)isPrime);
                }
            }
        }
    }
}
