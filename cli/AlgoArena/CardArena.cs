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
            DisplayHeader();
            string choice = GetUserChoice();
            RouteChoice(choice);
        }

        private static void DisplayHeader()
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
                case "1": CardSortDuel(); break;
                case "2": FindTheCard(); break;
                case "3": PartitionBattle(); break;
                default: Console.WriteLine("Unknown arena."); break;
            }
        }

        private static void CardSortDuel()
        {
            Console.WriteLine("\n  ═══ Card Sort Duel ═══");
            Console.Write("  Enter card values (space-separated): ");
            string input = Console.ReadLine()?.Trim() ?? "5 3 8 1 2 7 4 6";
            int[] values = ParseIntArray(input);
            int count = values.Length;

            int* original = (int*)Marshal.AllocHGlobal(count * sizeof(int));
            int* insertionArr = (int*)Marshal.AllocHGlobal(count * sizeof(int));
            int* mergeArr = (int*)Marshal.AllocHGlobal(count * sizeof(int));

            try
            {
                CopyValues(values, original, insertionArr, mergeArr);
                ExecuteSortComparison(original, insertionArr, mergeArr, count);
            }
            finally
            {
                FreeMemory(original, insertionArr, mergeArr);
            }
        }

        private static int[] ParseIntArray(string input)
        {
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int[] result = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                int.TryParse(parts[i], out result[i]);
            }
            return result;
        }

        private static void CopyValues(int[] source, int* dest1, int* dest2, int* dest3)
        {
            for (int i = 0; i < source.Length; i++)
            {
                dest1[i] = source[i];
                dest2[i] = source[i];
                dest3[i] = source[i];
            }
        }

        private static void ExecuteSortComparison(int* original, int* insertionArr, int* mergeArr, int count)
        {
            Console.WriteLine($"\n  Original deck: {ArrayToString(original, count)}");

            Insertion.Run(insertionArr, count);
            Console.WriteLine($"  After insertion sort: {ArrayToString(insertionArr, count)}");

            MergeSorted.RunInPlace(mergeArr, count);
            Console.WriteLine($"  After merge sort:     {ArrayToString(mergeArr, count)}");

            DisplayDuelResults(insertionArr, mergeArr, count);
            CalculateSubsetEnergy(insertionArr, count);
        }

        private static void DisplayDuelResults(int* insertionArr, int* mergeArr, int count)
        {
            bool insertionSorted = IsSorted(insertionArr, count);
            bool mergeSorted = IsSorted(mergeArr, count);

            Console.WriteLine($"\n  Insertion sorted: {(insertionSorted ? "✅" : "❌")}");
            Console.WriteLine($"  Merge sorted:     {(mergeSorted ? "✅" : "❌")}");

            int insertionMoves = CountSwaps(insertionArr, count);
            int mergeMoves = CountSwaps(mergeArr, count);

            Console.WriteLine($"\n  Insertion: {insertionMoves} moves");
            Console.WriteLine($"  Merge:     {mergeMoves} moves");
            Console.WriteLine($"  Winner: {(insertionMoves <= mergeMoves ? "🏆 Insertion" : "🏆 Merge")}");
        }

        private static void CalculateSubsetEnergy(int* arr, int count)
        {
            long* subsetF = stackalloc long[count];
            for (int i = 0; i < count; i++) subsetF[i] = arr[i];
            SubsetZeta.Run(subsetF, 1);
            Console.WriteLine($"\n  Subset energy: {subsetF[0]}");
        }

        private static void FreeMemory(params int*[] pointers)
        {
            foreach (var ptr in pointers) Marshal.FreeHGlobal((nint)ptr);
        }

        private static void FindTheCard()
        {
            Console.WriteLine("\n  ═══ Find the Card ═══");
            Console.Write("  Enter sorted card values (space-separated): ");
            string input = Console.ReadLine()?.Trim() ?? "2 5 8 12 15 18 22 25 28";
            int[] values = ParseIntArray(input);
            int count = values.Length;

            int* arr = (int*)Marshal.AllocHGlobal(count * sizeof(int));
            try
            {
                InitializeBuffer(values, arr);
                ProcessSearchQueries(arr, count);
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }

        private static void InitializeBuffer(int[] values, int* buffer)
        {
            for (int i = 0; i < values.Length; i++) buffer[i] = values[i];
        }

        private static void ProcessSearchQueries(int* arr, int count)
        {
            Console.Write("  Find card: ");
            if (!int.TryParse(Console.ReadLine(), out int key)) return;

            bool found = BinarySearch.TryFind(arr, count, key, out int index);
            Console.WriteLine($"\n  Binary search: {(found ? $"✅ Index {index}" : "❌ Not found")}");

            int lo = LowerBound.Run(arr, count, key);
            int hi = UpperBound.Run(arr, count, key) - 1;
            Console.WriteLine($"  Lower bound: {lo}, Upper bound: {hi}");

            int first = FirstTrue.Run(arr, count);
            int last = LastTrue.Run(arr, count);
            Console.WriteLine($"  First true: {first}, Last true: {last}");
        }

        private static void PartitionBattle()
        {
            Console.WriteLine("\n  ═══ Partition Battle ═══");
            Console.Write("  Enter values (space-separated): ");
            string input = Console.ReadLine()?.Trim() ?? "5 3 8 1 9 2 7 4 6";
            int[] values = ParseIntArray(input);
            int count = values.Length;

            int* arr = (int*)Marshal.AllocHGlobal(count * sizeof(int));
            try
            {
                InitializeBuffer(values, arr);
                ExecutePartitionRounds(arr, count);
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }

        private static void ExecutePartitionRounds(int* arr, int count)
        {
            Console.WriteLine("\n  Step-by-step quickselect:");
            int rounds = Math.Min(count, 5);
            for (int target = 0; target < rounds; target++)
            {
                RunSinglePartitionStep(arr, count, target);
            }

            int finalPivotIdx = count / 2;
            int pi = Partition.Run(arr, count, finalPivotIdx);
            DisplayPartitionState(arr, count, pi);
        }

        private static void RunSinglePartitionStep(int* source, int count, int targetIdx)
        {
            int* tmp = (int*)Marshal.AllocHGlobal(count * sizeof(int));
            for (int i = 0; i < count; i++) tmp[i] = source[i];

            int p = Partition.Run(tmp, count, targetIdx);
            Console.WriteLine($"  Target #{targetIdx}: pivot at {targetIdx} → {ArrayToString(tmp, count)} → kth={p}");
            Marshal.FreeHGlobal((nint)tmp);
        }

        private static void DisplayPartitionState(int* arr, int count, int pivotIdx)
        {
            Console.Write("\n  Lomuto partition: ");
            for (int i = 0; i < count; i++)
            {
                Console.Write(i == pivotIdx ? $"[{arr[i]}]" : $"{arr[i]}");
                if (i < count - 1) Console.Write(",");
            }
            Console.WriteLine($"\n  Pivot final position: {pivotIdx}");
        }

        private static string ArrayToString(int* arr, int count)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(arr[i]);
                if (i < count - 1) sb.Append(',');
            }
            return sb.ToString();
        }

        private static bool IsSorted(int* arr, int count)
        {
            for (int i = 1; i < count; i++)
                if (arr[i] < arr[i - 1]) return false;
            return true;
        }

        private static int CountSwaps(int* arr, int count)
        {
            return count * (count - 1) / 4;
        }
    }
}