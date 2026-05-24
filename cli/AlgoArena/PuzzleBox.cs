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
        private const int Mod = 1000000007;

        public static void Run()
        {
            DisplayHeader();
            string choice = GetUserChoice();
            RouteChoice(choice);
        }

        private static void DisplayHeader()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🧩  PUZZLE BOX  🧩                ║");
            Console.WriteLine("║  Master combinatorics and permutations! ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝\n");
            Console.WriteLine("  1. Binom Calculator  — choose your path");
            Console.WriteLine("  2. Permutation Forge — generate permutations");
            Console.WriteLine("  3. Gray Code Lab     — binary puzzles");
        }

        private static string GetUserChoice()
        {
            Console.Write("  Choice: ");
            return Console.ReadLine()?.Trim() ?? "0";
        }

        private static void RouteChoice(string choice)
        {
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
            Console.WriteLine("\n  ═══ Binom Calculator ═══");
            if (!GetNK(out int n, out int k)) return;

            Console.WriteLine($"\n  C({n},{k}) = {Binom.Run(n, k, Mod)}");
            ExecuteCombinatoricsLogic(n, k);
        }

        private static bool GetNK(out int n, out int k)
        {
            n = 10; k = 5;
            Console.Write("  Enter n: ");
            if (!int.TryParse(Console.ReadLine(), out n)) { Console.WriteLine("Invalid n."); return false; }
            Console.Write("  Enter k: ");
            if (!int.TryParse(Console.ReadLine(), out k)) { Console.WriteLine("Invalid k."); return false; }
            return true;
        }

        private static void ExecuteCombinatoricsLogic(int n, int k)
        {
            long* fact = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            long* invFact = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            try
            {
                Factorial.Run(fact, invFact, n, Mod);
                DisplaySpecialNumbers();
                DisplaySubsetMetrics(n, k);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)fact);
                Marshal.FreeHGlobal((nint)invFact);
            }
        }

        private static void DisplaySpecialNumbers()
        {
            Console.WriteLine($"  Catalan(5) = {Catalan.Run(5, Mod)}");
            Console.WriteLine($"  Stirling2(5,3) = {StirlingSecond.Run(5, 3, Mod)}");
            Console.WriteLine($"  Bell(5) = {BellNumbers.Run(5, Mod)}");
            Console.WriteLine($"  Derangements(5) = {Derangements.Run(5, Mod)}");
        }

        private static void DisplaySubsetMetrics(int n, int k)
        {
            long* all = stackalloc long[n + 1];
            for (int i = 0; i <= n; i++) all[i] = i;
            SubsetZeta.Run(all, 1);

            Console.WriteLine($"\n  Subset sum ζ(C(0..{n},0..k)): {all[0]}");
            Console.WriteLine($"  Subset count: {all[n]}");
            Console.WriteLine($"  Total subsets: {1L << n}");
            Console.WriteLine($"  Subsets of size ≤ {k}: {SumBinom(n, k)}");
        }

        private static long SumBinom(int n, int k)
        {
            long sum = 0;
            for (int i = 0; i <= k && i <= n; i++)
                sum += Binom.Run(n, i, Mod);
            return sum;
        }

        private static void PermutationForge()
        {
            Console.WriteLine("\n  ═══ Permutation Forge ═══");
            int[] values = GetPermutationInput();
            int n = values.Length;

            int* arr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                InitializeBuffer(values, arr);
                ExecutePermutationSteps(arr, n, values);
                ExecuteGrayGeneration(n);
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }

        private static int[] GetPermutationInput()
        {
            Console.Write("  Enter numbers (space-separated): ");
            string input = Console.ReadLine()?.Trim() ?? "1 2 3 4";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int[] result = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++) int.TryParse(parts[i], out result[i]);
            return result;
        }

        private static void InitializeBuffer(int[] source, int* buffer)
        {
            for (int i = 0; i < source.Length; i++) buffer[i] = source[i];
        }

        private static void ExecutePermutationSteps(int* arr, int n, int[] originalValues)
        {
            Console.WriteLine("\n  Next permutation: ");
            NextPermutation.Run(arr, n);
            PrintArray(arr, n);

            if (n <= 8) DisplayFirstTenPermutations(originalValues, n);
        }

        private static void PrintArray(int* arr, int n)
        {
            Console.Write("  ");
            for (int i = 0; i < n; i++) Console.Write($"{arr[i]} ");
            Console.WriteLine();
        }

        private static void DisplayFirstTenPermutations(int[] initial, int n)
        {
            int* arr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                InitializeBuffer(initial, arr);
                Console.WriteLine("\n  First 10 permutations: ");
                int count = 0;
                do
                {
                    Console.Write("(");
                    for (int i = 0; i < n; i++) Console.Write(arr[i]);
                    Console.Write(") ");
                    count++;
                } while (count < 10 && NextPermutation.Run(arr, n));
                Console.WriteLine();
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }

        private static void ExecuteGrayGeneration(int n)
        {
            int* gray = (int*)Marshal.AllocHGlobal((1 << n) * sizeof(int));
            try
            {
                GrayCode.Generate(gray, n);
                Console.WriteLine($"\n  Gray code sequence (n={n}, first 16):");
                int limit = Math.Min(1 << n, 16);
                for (int i = 0; i < limit; i++)
                    Console.WriteLine($"    {Convert.ToString(i, 2).PadLeft(n, '0')} → {Convert.ToString(gray[i], 2).PadLeft(n, '0')}");
            }
            finally { Marshal.FreeHGlobal((nint)gray); }
        }

        private static void GrayCodeLab()
        {
            Console.WriteLine("\n  ═══ Gray Code Lab ═══");
            Console.Write("  Enter number: ");
            if (!int.TryParse(Console.ReadLine(), out int num)) return;

            DisplayGrayConversions(num);
            ExecuteGraySequence(4);
            ExecuteBasisLogic(num);
        }

        private static void DisplayGrayConversions(int num)
        {
            int gray = GrayCode.ToGray(num);
            int back = GrayCode.FromGray(gray);
            Console.WriteLine($"\n  Binary: {Convert.ToString(num, 2)}");
            Console.WriteLine($"  Gray:   {Convert.ToString(gray, 2)}");
            Console.WriteLine($"  Match:  {(num == back ? "✅" : "❌")}");
        }

        private static void ExecuteGraySequence(int bits)
        {
            Console.WriteLine($"\n  Gray code sequence for {bits} bits:");
            for (int i = 0; i < (1 << bits); i++)
            {
                int g = GrayCode.ToGray(i);
                Console.WriteLine($"    {Convert.ToString(i, 2).PadLeft(bits, '0')} → {Convert.ToString(g, 2).PadLeft(bits, '0')}");
            }
        }

        private static void ExecuteBasisLogic(int num)
        {
            long* basis = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            int* basisSize = (int*)Marshal.AllocHGlobal(sizeof(int));
            *basisSize = 0;
            try
            {
                XorBasisInsert.Run(basis, basisSize, num);
                Console.WriteLine($"\n  Max XOR from basis: {XorBasisMax.Run(basis)}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)basis);
                Marshal.FreeHGlobal((nint)basisSize);
            }
        }
    }
}