namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Math.Combinatorics;
    using IAFahim.Permutation;
    using IAFahim.Unique;
    using IAFahim.Math.Transform;

    public static unsafe class PuzzleBox
    {
        public static void Run()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🧩  PUZZLE BOX  🧩                ║");
            Console.WriteLine("║  Master combinatorics and permutations! ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("  1. Binom Calculator  — choose your path");
            Console.WriteLine("  2. Permutation Forge — generate permutations");
            Console.WriteLine("  3. Gray Code Lab     — binary puzzles");
            Console.Write("  Choice: ");
            string choice = Console.ReadLine()?.Trim() ?? "0";

            switch (choice)
            {
                case "1": BinomCalculator(); break;
                case "2": PermutationForge(); break;
                case "3": GrayCodeLab(); break;
                default: Console.WriteLine("Unknown puzzle."); break;
            }
        }

        private static void BinomCalculator()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Binom Calculator ═══");
            Console.Write("  Enter n: ");
            string nStr = Console.ReadLine()?.Trim() ?? "10";
            Console.Write("  Enter k: ");
            string kStr = Console.ReadLine()?.Trim() ?? "5";

            if (!int.TryParse(nStr, out int n)) { Console.WriteLine("Invalid n."); return; }
            if (!int.TryParse(kStr, out int k)) { Console.WriteLine("Invalid k."); return; }

            Console.WriteLine();
            Console.WriteLine($"  C({n},{k}) = {Binom.Run(n, k, 1000000007)}");

            long* fact = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            long* invFact = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            long* inv = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            try
            {
                Factorial.Run(fact, invFact, n, 1000000007);
                Console.WriteLine($"  Catalan(5) = {Catalan.Run(5, 1000000007)}");
                Console.WriteLine($"  Stirling2(5,3) = {StirlingSecond.Run(5, 3, 1000000007)}");
                Console.WriteLine($"  Bell(5) = {BellNumbers.Run(5, 1000000007)}");
                Console.WriteLine($"  Derangements(5) = {Derangements.Run(5, 1000000007)}");

                long* all = stackalloc long[n + 1];
                for (int i = 0; i <= n; i++) all[i] = i;
                IAFahim.Math.Transform.SubsetZeta.Run(all, 1);
                Console.WriteLine();
                Console.WriteLine($"  Subset sum ζ(C(0..{n},0..k)): {all[0]}");
                Console.WriteLine($"  Subset count: {all[n]}");

                Console.WriteLine();
                Console.WriteLine($"  Number of subsets: {1L << n}");
                Console.WriteLine($"  Number of subsets of size ≤ {k}: {SumBinom(n, k)}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)fact);
                Marshal.FreeHGlobal((nint)invFact);
                Marshal.FreeHGlobal((nint)inv);
            }
        }

        private static long SumBinom(int n, int k)
        {
            long sum = 0;
            for (int i = 0; i <= k && i <= n; i++)
                sum += Binom.Run(n, i, 1000000007);
            return sum;
        }

        private static void PermutationForge()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Permutation Forge ═══");
            Console.Write("  Enter numbers (space-separated): ");
            string input = Console.ReadLine()?.Trim() ?? "1 2 3 4";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int n = parts.Length;

            int* arr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) arr[i] = int.Parse(parts[i]);

                Console.WriteLine();
                Console.WriteLine($"  Original: {input}");
                Console.WriteLine($"  Next permutation: ");
                bool hasNext = NextPermutation.Run(arr, n);
                Console.Write("  ");
                for (int i = 0; i < n; i++) Console.Write($"{arr[i]} ");
                Console.WriteLine();

                if (n <= 8)
                {
                    int total = 1;
                    for (int i = 2; i <= n; i++) total *= i;
                    Console.WriteLine();
                    Console.WriteLine($"  Total permutations: {total}");
                    Console.Write("  First 10 permutations: ");

                    for (int i = 0; i < n; i++) arr[i] = int.Parse(parts[i]);
                    int count = 0;
                    do
                    {
                        Console.Write("(");
                        for (int i = 0; i < n; i++) Console.Write(arr[i]);
                        Console.Write(") ");
                        count++;
                        if (count >= 10) break;
                    } while (NextPermutation.Run(arr, n));
                    Console.WriteLine();
                }

                int* gray = (int*)Marshal.AllocHGlobal(n * sizeof(int));
                GrayCode.Generate(gray, n);
                Console.WriteLine();
                Console.WriteLine($"  Gray code (n={n}):");
                for (int i = 0; i < (1 << n) && i < 16; i++)
                    Console.WriteLine($"    {Convert.ToString(i, 2).PadLeft(n, '0')} → {Convert.ToString(gray[i], 2).PadLeft(n, '0')}");
                Marshal.FreeHGlobal((nint)gray);
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }

        private static void GrayCodeLab()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Gray Code Lab ═══");
            Console.Write("  Enter number to convert: ");
            string input = Console.ReadLine()?.Trim() ?? "13";
            if (!int.TryParse(input, out int num)) { Console.WriteLine("Invalid number."); return; }

            Console.WriteLine();
            Console.WriteLine($"  Binary: {Convert.ToString(num, 2)}");
            int gray = GrayCode.ToGray(num);
            Console.WriteLine($"  Gray:   {Convert.ToString(gray, 2)}");
            int back = GrayCode.FromGray(gray);
            Console.WriteLine($"  Binary again: {Convert.ToString(back, 2)}");
            Console.WriteLine($"  Match: {(num == back ? "✅" : "❌")}");

            Console.WriteLine();
            Console.WriteLine("  Gray code sequence for 4 bits:");
            for (int i = 0; i < 16; i++)
            {
                int g = GrayCode.ToGray(i);
                Console.WriteLine($"    {Convert.ToString(i, 2).PadLeft(4, '0')} → {Convert.ToString(g, 2).PadLeft(4, '0')}");
            }

            long* basis = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            int* basisSize = (int*)Marshal.AllocHGlobal(sizeof(int));
            *basisSize = 0;
            try
            {
                IAFahim.Math.Transform.XorBasisInsert.Run(basis, basisSize, num);
                long maxXor = IAFahim.Math.Transform.XorBasisMax.Run(basis);
                Console.WriteLine();
                Console.WriteLine($"  Max XOR from basis (num={num}): {maxXor}");
            }
            finally { Marshal.FreeHGlobal((nint)basis); Marshal.FreeHGlobal((nint)basisSize); }
        }
    }
}