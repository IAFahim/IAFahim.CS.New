namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class TotientPrefix
    {
        public static void Run(int n, long* result)
        {
            if (n < 0)
            {
                return;
            }
            result[0] = 0;
            if (n == 0)
            {
                return;
            }

            int* phi = null;
            int* primes = null;
            bool* isPrime = null;
            bool allocated = false;

            if (n > 10000)
            {
                phi = (int*)Marshal.AllocHGlobal((nint)(n + 1) * sizeof(int));
                primes = (int*)Marshal.AllocHGlobal((nint)(n + 1) * sizeof(int));
                isPrime = (bool*)Marshal.AllocHGlobal((nint)(n + 1) * sizeof(bool));
                allocated = true;
            }
            else
            {
                int* tempPhi = stackalloc int[n + 1];
                int* tempPrimes = stackalloc int[n + 1];
                bool* tempIsPrime = stackalloc bool[n + 1];
                phi = tempPhi;
                primes = tempPrimes;
                isPrime = tempIsPrime;
            }

            try
            {
                for (int i = 0; i <= n; i++)
                {
                    phi[i] = i;
                }
                for (int i = 2; i <= n; i++)
                {
                    isPrime[i] = true;
                }
                int pCount = 0;
                phi[1] = 1;
                for (int i = 2; i <= n; i++)
                {
                    if (isPrime[i])
                    {
                        primes[pCount++] = i;
                        phi[i] = i - 1;
                    }
                    for (int j = 0; j < pCount && i * primes[j] <= n; j++)
                    {
                        int p = primes[j];
                        isPrime[i * p] = false;
                        if (i % p == 0)
                        {
                            phi[i * p] = phi[i] * p;
                            break;
                        }
                        else
                        {
                            phi[i * p] = phi[i] * (p - 1);
                        }
                    }
                }
                result[0] = 0;
                for (int i = 1; i <= n; i++)
                {
                    result[i] = result[i - 1] + (long)phi[i];
                }
            }
            finally
            {
                if (allocated)
                {
                    Marshal.FreeHGlobal((nint)phi);
                    Marshal.FreeHGlobal((nint)primes);
                    Marshal.FreeHGlobal((nint)isPrime);
                }
            }
        }
    }
}
