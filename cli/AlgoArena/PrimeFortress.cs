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
            DisplayHeader();
            long n = GetTargetNumber();
            if (n < 2) return;

            DisplayBasicAnalysis(n);
            ExecuteAdvancedAnalysis(n);
        }

        private static void DisplayHeader()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🏰  PRIME FORTRESS  🏰            ║");
            Console.WriteLine("║  Defend your fortress with prime power!  ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝\n");
        }

        private static long GetTargetNumber()
        {
            Console.WriteLine("Enter a number (2-999999999999) to analyze:");
            Console.Write("> ");
            if (long.TryParse(Console.ReadLine(), out long n) && n >= 2) return n;
            Console.WriteLine("Invalid number.");
            return -1;
        }

        private static void DisplayBasicAnalysis(long n)
        {
            Console.WriteLine($"\n═══ Analysis of {n} ═══\n");
            bool isPrime = MillerRabin.Run(n);
            Console.WriteLine($"  Is prime?     {(isPrime ? "✅ YES — impregnable!" : "❌ NO — it has weaknesses")}");
            Console.WriteLine($"  Euler's totient φ({n}) = {Phi.Run(n)}");
            DisplayMobius(n);
        }

        private static void DisplayMobius(long n)
        {
            int mu = Mobius.Run(n);
            string muStr = mu == 1 ? "+1 (squarefree, even primes)" :
                           mu == -1 ? "-1 (squarefree, odd primes)" :
                           "0 (has squared prime factor)";
            Console.WriteLine($"  Möbius μ({n}) = {muStr}");
        }

        private static void ExecuteAdvancedAnalysis(long n)
        {
            long* factors = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            try
            {
                int factorCount = Factorize.Run(n, factors);
                DisplayFactorization(factors, factorCount);
                ExecuteDivisorAnalysis(n);

                if (MillerRabin.Run(n)) ExecutePrimalityOracle(n);

                DisplayAbundance(n);
                DisplayStrengthRating(n, factorCount);
            }
            finally { Marshal.FreeHGlobal((nint)factors); }
        }

        private static void DisplayFactorization(long* factors, int count)
        {
            Console.Write("  Prime factorization: ");
            int i = 0;
            while (i < count)
            {
                long p = factors[i];
                int exponent = 0;
                while (i < count && factors[i] == p) { exponent++; i++; }

                Console.Write(exponent == 1 ? $"{p}" : $"{p}^{exponent}");
                if (i < count) Console.Write(" × ");
            }
            Console.WriteLine();
        }

        private static void ExecuteDivisorAnalysis(long n)
        {
            long* divisors = (long*)Marshal.AllocHGlobal(10000 * sizeof(long));
            try
            {
                int divisorCount = Divisors.Run(n, divisors);
                Console.WriteLine($"  Number of divisors: {divisorCount}");
                if (divisorCount <= 30) DisplayDivisorList(divisors, divisorCount);
            }
            finally { Marshal.FreeHGlobal((nint)divisors); }
        }

        private static void DisplayDivisorList(long* divisors, int count)
        {
            Console.Write("  Divisors: ");
            for (int d = 0; d < count; d++)
            {
                Console.Write(divisors[d]);
                if (d < count - 1) Console.Write(", ");
            }
            Console.WriteLine();
        }

        private static void ExecutePrimalityOracle(long n)
        {
            Console.WriteLine($"  Primitive root: {PrimitiveRoot.Run(n)}");
            Console.WriteLine($"  GCD({n}, 42) = {Gcd.Run(n, 42)}");
            Console.WriteLine("\n  🔮 Primality Oracle — test numbers near your fortress:");
            Console.Write("  Enter range (-10 to +10): ");

            for (int r = -10; r <= 10; r++)
            {
                long testNum = n + r;
                if (testNum >= 2 && MillerRabin.Run(testNum)) Console.Write($"  ★ {testNum}");
            }
            Console.WriteLine();
        }

        private static void DisplayAbundance(long n)
        {
            long sigma = DivisorSum.Run(n);
            Console.WriteLine($"  Sum of divisors σ({n}) = {sigma}");
            if (sigma == 2 * n) Console.WriteLine("  🌟 PERFECT NUMBER!");
            else Console.WriteLine(sigma < 2 * n ? "  📉 Deficient number" : "  📈 Abundant number");
        }

        private static void DisplayStrengthRating(long n, int factorCount)
        {
            bool isPrime = MillerRabin.Run(n);
            string rating = isPrime ? $"{Math.Min(factorCount * 100, 9999)} (prime wall!)" : $"{factorCount * 25} (composite armor)";
            Console.WriteLine($"\n  ⚔  Fortress strength rating: {rating}");
        }
    }
}