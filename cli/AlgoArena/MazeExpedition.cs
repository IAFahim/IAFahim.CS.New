namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.DS.Dsu;
    using IAFahim.DS.SegmentTree;
    using IAFahim.Search.Range;
    using IAFahim.Math.Transform;

    public static unsafe class BitSafe
    {
        public static void Add(long* bit, int idx, int size, long val)
        {
            for (int i = idx; i <= size; i += i & -i)
                bit[i] += val;
        }

        public static long Sum(long* bit, int idx)
        {
            long res = 0;
            for (int i = idx; i >= 0; i -= i & -i)
                res += bit[i];
            return res;
        }

        public static long RangeSum(long* bit, int l, int r)
        {
            if (l > r) return 0;
            return Sum(bit, r) - (l > 0 ? Sum(bit, l - 1) : 0);
        }

        public static int LowerBound(long* bit, int n, long target)
        {
            int idx = 0;
            int bitMask = 1 << (31 - (n - 1).ToString().Length);
            while (bitMask != 0)
            {
                int next = idx + bitMask;
                if (next <= n && bit[next] < target)
                {
                    idx = next;
                    target -= bit[next];
                }
                bitMask >>= 1;
            }
            return idx;
        }
    }

    public static unsafe class MazeExpedition
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
            Console.WriteLine("║        🏔  MAZE EXPEDITION  🏔           ║");
            Console.WriteLine("║  Navigate challenges with DSU & SegTree! ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  1. Island Explorer     — DSU on a grid");
            Console.WriteLine("  2. Range Query Arena   — segment tree showdown");
            Console.WriteLine("  3. BIT Treasure Hunt   — fenwick tree adventure");
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
                case "1": IslandExplorer(); break;
                case "2": RangeQueryArena(); break;
                case "3": BitTreasureHunt(); break;
                default: Console.WriteLine("Unknown expedition."); break;
            }
        }

        private static void IslandExplorer()
        {
            Console.WriteLine("\n  ═══ Island Explorer ═══");
            int n = GetGridSize();
            int[,] grid = GetGridInput(n);

            int total = n * n;
            int* parent = (int*)Marshal.AllocHGlobal(total * sizeof(int));
            int* size = (int*)Marshal.AllocHGlobal(total * sizeof(int));

            try
            {
                DsuInit.Run(parent, size, total);
                ProcessGridConnectivity(grid, parent, size, n);
                VisualizeGrid(grid, parent, n);
                DisplayIslandMetrics(grid, parent, size, n);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)parent);
                Marshal.FreeHGlobal((nint)size);
            }
        }

        private static int GetGridSize()
        {
            Console.Write("  Grid size (n x n, max 10): ");
            if (int.TryParse(Console.ReadLine(), out int n) && n >= 1 && n <= 10) return n;
            return 5;
        }

        private static int[,] GetGridInput(int n)
        {
            Console.WriteLine($"  Grid: {n}x{n}");
            Console.WriteLine("  Enter grid (1=land, 0=water), row by row:");
            int[,] grid = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                Console.Write($"  Row {i + 1}: ");
                string[] parts = (Console.ReadLine() ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < n && j < parts.Length; j++)
                    int.TryParse(parts[j], out grid[i, j]);
            }
            return grid;
        }

        private static void ProcessGridConnectivity(int[,] grid, int* parent, int* size, int n)
        {
            int[] di = { -1, 0, 1, 0 };
            int[] dj = { 0, 1, 0, -1 };

            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                if (grid[i, j] == 0) continue;
                int idx = i * n + j;
                for (int d = 0; d < 4; d++)
                {
                    int ni = i + di[d], nj = j + dj[d];
                    if (ni >= 0 && ni < n && nj >= 0 && nj < n && grid[ni, nj] == 1)
                        DsuUnion.Run(parent, size, idx, ni * n + nj);
                }
            }
        }

        private static void VisualizeGrid(int[,] grid, int* parent, int n)
        {
            Console.WriteLine("\n  Grid visualization:");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (grid[i, j] == 0) { Console.Write(" ~"); continue; }
                    int root = DsuFind.Run(parent, i * n + j);
                    Console.Write($" {"0123456789ABCDEF"[root % 16]}");
                }
                Console.WriteLine();
            }
        }

        private static void DisplayIslandMetrics(int[,] grid, int* parent, int* size, int n)
        {
            int islands = 0, maxSize = 0;
            for (int i = 0; i < n * n; i++)
            {
                if (grid[i / n, i % n] == 0) continue;
                int root = DsuFind.Run(parent, i);
                if (root == i) islands++;
                maxSize = Math.Max(maxSize, size[root]);
            }
            Console.WriteLine($"\n  Number of islands: {islands}");
            Console.WriteLine($"  Largest island size: {maxSize}");
        }

        private static void RangeQueryArena()
        {
            Console.WriteLine("\n  ═══ Range Query Arena ═══");
            int[] values = GetArrayInput();
            int n = values.Length;

            int* arr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* segTree = (int*)Marshal.AllocHGlobal(n * 4 * sizeof(int));
            try
            {
                InitializeBuffer(values, arr);
                SegmentTreeBuild.RunInt32(arr, segTree, 1, 0, n - 1);
                ExecuteRangeQueries(arr, segTree, n);
                ExecuteUpdates(segTree, n);
            }
            finally { FreeMemory(arr, segTree); }
        }

        private static int[] GetArrayInput()
        {
            Console.WriteLine("  Enter array values (space-separated):");
            string input = Console.ReadLine()?.Trim() ?? "5 2 8 1 9 3 7 4 6";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int[] result = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++) int.TryParse(parts[i], out result[i]);
            return result;
        }

        private static void InitializeBuffer(int[] values, int* buffer)
        {
            for (int i = 0; i < values.Length; i++) buffer[i] = values[i];
        }

        private static void ExecuteRangeQueries(int* arr, int* segTree, int n)
        {
            int* sparse = (int*)Marshal.AllocHGlobal(n * n * sizeof(int));
            RangeMax.BuildSparse(sparse, arr, n);
            try
            {
                Console.WriteLine("\n  Range queries (min):");
                for (int l = 0; l < n; l++)
                for (int r = l; r < n && r < l + 5; r++)
                {
                    int minSparse = RangeMin.Query(sparse, arr, n, l, r);
                    int minSeg = SegmentTreeQuery.RunInt32(segTree, 1, 0, n - 1, l, r);
                    Console.WriteLine($"    [{l},{r}]: sparse={minSparse}, segtree={minSeg} {(minSparse == minSeg ? "✅" : "❌")}");
                }
            }
            finally { Marshal.FreeHGlobal((nint)sparse); }
        }

        private static void ExecuteUpdates(int* segTree, int n)
        {
            Console.WriteLine("\n  Updates and re-queries:");
            SegmentTreeSet.RunInt32(segTree, 1, 0, n - 1, 2, 99);
            Console.WriteLine("  Updated index 2 to 99");
            Console.WriteLine($"  Query [0,4] after update: {SegmentTreeQuery.RunInt32(segTree, 1, 0, n - 1, 0, 4)}");
        }

        private static void BitTreasureHunt()
        {
            Console.WriteLine("\n  ═══ BIT Treasure Hunt ═══");
            int[] values = GetArrayInput();
            int n = values.Length;

            long* bit = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            try
            {
                InitializeBit(bit, values, n);
                DisplayPrefixSums(bit, n);
                ExecuteBitQueries(bit, n, values);
            }
            finally { Marshal.FreeHGlobal((nint)bit); }
        }

        private static void InitializeBit(long* bit, int[] values, int n)
        {
            for (int i = 0; i <= n; i++) bit[i] = 0;
            for (int i = 0; i < n; i++) BitSafe.Add(bit, i + 1, n, values[i]);
        }

        private static void DisplayPrefixSums(long* bit, int n)
        {
            Console.WriteLine("\n  Prefix sums:");
            for (int i = 1; i <= n; i++) Console.WriteLine($"    [1..{i}]: sum = {BitSafe.Sum(bit, i)}");
        }

        private static void ExecuteBitQueries(long* bit, int n, int[] originalValues)
        {
            Console.WriteLine("\n  Range query [l,r]:");
            int l = GetInputInt("  Enter l: ", 2, n);
            int r = GetInputInt("  Enter r: ", 5, n);
            if (l > r) { int t = l; l = r; r = t; }

            long rangeSum = BitSafe.RangeSum(bit, l, r);
            Console.WriteLine($"  Range sum [{l}..{r}] = {rangeSum}");

            long target = rangeSum / 2;
            Console.WriteLine($"\n  Lower bound of {target}: index {BitSafe.LowerBound(bit, n, target)}");

            ExecuteSubsetTransform(originalValues);
        }

        private static int GetInputInt(string prompt, int @default, int max)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int result)) return Math.Max(1, Math.Min(result, max));
            return @default;
        }

        private static void ExecuteSubsetTransform(int[] values)
        {
            int limit = Math.Min(values.Length, 32);
            long* f = stackalloc long[limit];
            for (int i = 0; i < limit; i++) f[i] = values[i];
            SubsetZeta.Run(f, 1);
            Console.WriteLine($"  Subset ζ transform: f[0] = {f[0]}");
        }

        private static void FreeMemory(params int*[] pointers)
        {
            foreach (var ptr in pointers) Marshal.FreeHGlobal((nint)ptr);
        }
    }
}