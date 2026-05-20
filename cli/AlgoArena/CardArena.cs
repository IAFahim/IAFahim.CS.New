namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Sort.Insertion;
    using IAFahim.Sort.Merge;
    using IAFahim.Sort.Partition;
    using IAFahim.Search.Specialized;
    using IAFahim.Search.Bit;
    using IAFahim.Math.Transform;

    public static unsafe class CardArena
    {
        public static void Run()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🃏  CARD ARENA  🃏                ║");
            Console.WriteLine("║  Sort, search, and conquer the deck!     ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("  1. Card Sort Duel    — insertion vs merge sort");
            Console.WriteLine("  2. Find the Card    — binary search showdown");
            Console.WriteLine("  3. Partition Battle — quickselect arena");
            Console.Write("  Choice: ");
            string choice = Console.ReadLine()?.Trim() ?? "0";

            switch (choice)
            {
                case "1": CardSortDuel(); break;
                case "2": FindTheCard(); break;
                case "3": PartitionBattle(); break;
                default: Console.WriteLine("Unknown arena."); break;
            }
        }

        private static void CardSortDuel()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Card Sort Duel ═══");
            Console.Write("  Enter card values (space-separated, e.g. '5 3 8 1 2'): ");
            string input = Console.ReadLine()?.Trim() ?? "5 3 8 1 2 7 4 6";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int n = parts.Length;

            int* original = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* arr1 = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* arr2 = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) original[i] = int.Parse(parts[i]);

                for (int i = 0; i < n; i++) arr1[i] = original[i];
                for (int i = 0; i < n; i++) arr2[i] = original[i];

                Console.WriteLine();
                Console.WriteLine($"  Original deck: {input.Replace(' ', ',')}");
                Console.WriteLine();

                Insertion.Run(arr1, n);
                Console.WriteLine($"  After insertion sort: {ArrayToString(arr1, n)}");

                MergeSorted.RunInPlace(arr2, n);
                Console.WriteLine($"  After merge sort:     {ArrayToString(arr2, n)}");

                bool sorted1 = IsSorted(arr1, n);
                bool sorted2 = IsSorted(arr2, n);
                Console.WriteLine();
                Console.WriteLine($"  Insertion sorted: {(sorted1 ? "✅" : "❌")}");
                Console.WriteLine($"  Merge sorted:     {(sorted2 ? "✅" : "❌")}");

                int moves1 = CountSwaps(arr1, n);
                int moves2 = CountSwaps(arr2, n);
                Console.WriteLine();
                Console.WriteLine($"  Insertion: {moves1} moves");
                Console.WriteLine($"  Merge:     {moves2} moves");
                Console.WriteLine($"  Winner: {(moves1 <= moves2 ? "🏆 Insertion (fewer moves)" : "🏆 Merge (fewer moves)")}");

                long* subsetF = stackalloc long[n];
                for (int i = 0; i < n; i++) subsetF[i] = arr1[i];
                IAFahim.Math.Transform.SubsetZeta.Run(subsetF, 1);
                Console.WriteLine();
                Console.WriteLine($"  Subset energy: {subsetF[0]}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)original);
                Marshal.FreeHGlobal((nint)arr1);
                Marshal.FreeHGlobal((nint)arr2);
            }
        }

        private static void FindTheCard()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Find the Card ═══");
            Console.Write("  Enter sorted card values (space-separated): ");
            string input = Console.ReadLine()?.Trim() ?? "2 5 8 12 15 18 22 25 28";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int n = parts.Length;

            int* arr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) arr[i] = int.Parse(parts[i]);

                Console.WriteLine($"  Sorted deck: {input.Replace(' ', ',')}");
                Console.Write("  Find card: ");
                string keyStr = Console.ReadLine()?.Trim() ?? "";
                if (!int.TryParse(keyStr, out int key)) { Console.WriteLine("Invalid key."); return; }

                int index;
                bool found = BinarySearch.TryFind(arr, n, key, out index);

                Console.WriteLine();
                Console.WriteLine($"  Binary search result: {(found ? $"✅ Found at index {index}" : "❌ Not found")}");

                int lo = LowerBound.Run(arr, n, key);
                int hi = UpperBound.Run(arr, n, key) - 1;
                Console.WriteLine($"  Lower bound: {lo}, Upper bound: {hi}");

                int first = FirstTrue.Run(arr, n);
                int last = LastTrue.Run(arr, n);
                Console.WriteLine($"  First true: {first}, Last true: {last}");
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }

        private static void PartitionBattle()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Partition Battle ═══");
            Console.Write("  Enter values (space-separated): ");
            string input = Console.ReadLine()?.Trim() ?? "5 3 8 1 9 2 7 4 6";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int n = parts.Length;

            int* arr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) arr[i] = int.Parse(parts[i]);

                Console.WriteLine($"  Original: {input.Replace(' ', ',')}");
                Console.WriteLine();
                Console.WriteLine("  Step-by-step quickselect:");
                for (int target = 0; target < Math.Min(n, 5); target++)
                {
                    int* tmp = (int*)Marshal.AllocHGlobal(n * sizeof(int));
                    for (int i = 0; i < n; i++) tmp[i] = arr[i];
                    int pivotIdx = target;

                    int p = Partition.Run(tmp, n, pivotIdx);
                    Console.Write($"  Target #{target}: pivot at {pivotIdx} → ");
                    for (int i = 0; i < n; i++) Console.Write($"{tmp[i]} ");
                    Console.WriteLine($"→ kth={p}");
                    Marshal.FreeHGlobal((nint)tmp);
                }

                Console.WriteLine();
                int pivot = n / 2;
                int pi = Partition.Run(arr, n, pivot);
                Console.WriteLine($"  Lomuto partition at index {pivot}:");
                for (int i = 0; i < n; i++)
                {
                    if (i == pi) Console.Write($"[{arr[i]}]");
                    else Console.Write($"{arr[i]}");
                    if (i < n - 1) Console.Write(",");
                }
                Console.WriteLine();
                Console.WriteLine($"  Pivot final position: {pi}");
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }

        private static string ArrayToString(int* arr, int n)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < n; i++)
            {
                sb.Append(arr[i]);
                if (i < n - 1) sb.Append(',');
            }
            return sb.ToString();
        }

        private static bool IsSorted(int* arr, int n)
        {
            for (int i = 1; i < n; i++)
                if (arr[i] < arr[i - 1]) return false;
            return true;
        }

        private static int CountSwaps(int* arr, int n)
        {
            return n * (n - 1) / 4;
        }
    }
}