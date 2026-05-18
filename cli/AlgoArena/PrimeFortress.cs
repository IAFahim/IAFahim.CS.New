namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Math.NT;
    using IAFahim.Math.Modular;

    public static unsafe class PrimeFortress
    {
        public static void Run()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🏰  PRIME FORTRESS  🏰            ║");
            Console.WriteLine("║  Defend your fortress with prime power!  ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Enter a number (2-999999999999) to analyze:");
            Console.Write("> ");
            string input = Console.ReadLine()?.Trim() ?? "0";
            if (!long.TryParse(input, out long n) || n < 2) { Console.WriteLine("Invalid number."); return; }

            Console.WriteLine();
            Console.WriteLine($"═══ Analysis of {n} ═══");
            Console.WriteLine();

            bool prime = MillerRabin.Run(n);
            Console.WriteLine($"  Is prime?     {(prime ? "✅ YES — impregnable!" : "❌ NO — it has weaknesses")}");

            long phi = IAFahim.Math.NT.Phi.Run(n);
            Console.WriteLine($"  Euler's totient φ({n}) = {phi}");

            int mu = IAFahim.Math.NT.Mobius.Run(n);
            string muStr = mu == 1 ? "+1 (squarefree, even primes)" :
                           mu == -1 ? "-1 (squarefree, odd primes)" :
                           "0 (has squared prime factor)";
            Console.WriteLine($"  Möbius μ({n}) = {muStr}");

            long* factors = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            try
            {
                int fCount = Factorize.Run(n, factors);
                Console.Write("  Prime factorization: ");
                int i = 0;
                while (i < fCount)
                {
                    long p = factors[i];
                    int exp = 0;
                    while (i < fCount && factors[i] == p) { exp++; i++; }
                    if (exp == 1) Console.Write($"{p}");
                    else Console.Write($"{p}^{exp}");
                    if (i < fCount) Console.Write(" × ");
                }
                Console.WriteLine();

                long* divs = (long*)Marshal.AllocHGlobal(10000 * sizeof(long));
                try
                {
                    int dCount = Divisors.Run(n, divs);
                    Console.WriteLine($"  Number of divisors: {dCount}");

                    if (dCount <= 30)
                    {
                        Console.Write("  Divisors: ");
                        for (int d = 0; d < dCount; d++)
                        {
                            Console.Write(divs[d]);
                            if (d < dCount - 1) Console.Write(", ");
                        }
                        Console.WriteLine();
                    }
                }
                finally { Marshal.FreeHGlobal((nint)divs); }

                if (prime)
                {
                    long root = PrimitiveRoot.Run(n);
                    Console.WriteLine($"  Primitive root: {root}");

                    long g = Gcd.Run(n, 42);
                    Console.WriteLine($"  GCD({n}, 42) = {g}");

                    Console.WriteLine();
                    Console.WriteLine("  🔮 Primality Oracle — test numbers near your fortress:");
                    Console.Write("  Enter range (-10 to +10): ");
                    for (int r = -10; r <= 10; r++)
                    {
                        long test = n + r;
                        if (test < 2) continue;
                        bool p = MillerRabin.Run(test);
                        if (p) Console.Write($"  ★ {test}");
                    }
                    Console.WriteLine();
                }

                long divSum = IAFahim.Math.NT.DivisorSum.Run(n);
                Console.WriteLine($"  Sum of divisors σ({n}) = {divSum}");

                if (divSum == 2 * n) Console.WriteLine("  🌟 PERFECT NUMBER!");
                else if (divSum < 2 * n) Console.WriteLine("  📉 Deficient number");
                else Console.WriteLine("  📈 Abundant number");

                Console.WriteLine();
                Console.WriteLine("  ⚔  Fortress strength rating: " + (prime ? $"{Math.Min(fCount * 100, 9999)} (prime wall!)" : $"{fCount * 25} (composite armor)"));
            }
            finally { Marshal.FreeHGlobal((nint)factors); }
        }
    }
}