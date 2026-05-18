namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;
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
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🏔  MAZE EXPEDITION  🏔           ║");
            Console.WriteLine("║  Navigate challenges with DSU & SegTree! ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("  1. Island Explorer     — DSU on a grid");
            Console.WriteLine("  2. Range Query Arena   — segment tree showdown");
            Console.WriteLine("  3. BIT Treasure Hunt   — fenwick tree adventure");
            Console.Write("  Choice: ");
            string choice = Console.ReadLine()?.Trim() ?? "0";

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
            Console.WriteLine();
            Console.WriteLine("  ═══ Island Explorer ═══");
            Console.Write("  Grid size (n x n, max 10): ");
            string nStr = Console.ReadLine()?.Trim() ?? "5";
            if (!int.TryParse(nStr, out int n) || n < 1 || n > 10) n = 5;
            Console.WriteLine($"  Grid: {n}x{n}");
            Console.WriteLine("  Enter grid (1=land, 0=water), row by row:");

            int[,] grid = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                Console.Write($"  Row {i + 1}: ");
                string row = Console.ReadLine()?.Trim() ?? "";
                string[] parts = row.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < n && j < parts.Length; j++)
                    int.TryParse(parts[j], out grid[i, j]);
            }

            int total = n * n;
            int* parent = (int*)Marshal.AllocHGlobal(total * sizeof(int));
            int* size = (int*)Marshal.AllocHGlobal(total * sizeof(int));
            try
            {
                IAFahim.DS.Dsu.DsuInit.Run(parent, size, total);

                for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    if (grid[i, j] == 0) continue;
                    int idx = i * n + j;
                    int[] di = { -1, 0, 1, 0 };
                    int[] dj = { 0, 1, 0, -1 };
                    for (int d = 0; d < 4; d++)
                    {
                        int ni = i + di[d];
                        int nj = j + dj[d];
                        if (ni < 0 || ni >= n || nj < 0 || nj >= n) continue;
                        if (grid[ni, nj] == 0) continue;
                        int nidx = ni * n + nj;
                        IAFahim.DS.Dsu.DsuUnion.Run(parent, size, idx, nidx);
                    }
                }

                Console.WriteLine();
                Console.WriteLine("  Grid visualization:");
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        int idx = i * n + j;
                        int root = IAFahim.DS.Dsu.DsuFind.Run(parent, idx);
                        int cluster = root % 16;
                        char c = grid[i, j] == 1 ? "0123456789ABCDEF"[cluster] : '~';
                        Console.Write($" {c}");
                    }
                    Console.WriteLine();
                }

                int islands = 0;
                for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    int idx = i * n + j;
                    if (grid[i, j] == 1 && IAFahim.DS.Dsu.DsuFind.Run(parent, idx) == idx) islands++;
                }
                Console.WriteLine();
                Console.WriteLine($"  Number of islands: {islands}");

                int maxSize = 0;
                for (int i = 0; i < total; i++)
                {
                    int ri = IAFahim.DS.Dsu.DsuFind.Run(parent, i);
                    if (ri == i && grid[i / n, i % n] == 1)
                        maxSize = Math.Max(maxSize, size[ri]);
                }
                Console.WriteLine($"  Largest island size: {maxSize}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)parent);
                Marshal.FreeHGlobal((nint)size);
            }
        }

        private static void RangeQueryArena()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Range Query Arena ═══");
            Console.WriteLine("  Enter array values (space-separated):");
            string input = Console.ReadLine()?.Trim() ?? "5 2 8 1 9 3 7 4 6";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int n = parts.Length;

            int* arr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* segTree = (int*)Marshal.AllocHGlobal(n * 4 * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) arr[i] = int.Parse(parts[i]);

                Console.WriteLine();
                Console.WriteLine($"  Array: {input}");
                Console.WriteLine();

                IAFahim.DS.SegmentTree.SegmentTreeBuild.RunInt32(arr, segTree, 1, 0, n - 1);

                int* sparse = (int*)Marshal.AllocHGlobal(n * n * sizeof(int));
                IAFahim.Search.Range.RangeMax.BuildSparse(sparse, arr, n);
                try
                {
                    Console.WriteLine("  Range queries (min):");
                    for (int l = 0; l < n; l++)
                    {
                        for (int r = l; r < n && r < l + 5; r++)
                        {
                            int minSparse = IAFahim.Search.Range.RangeMin.Query(sparse, arr, n, l, r);
                            int minSeg = IAFahim.DS.SegmentTree.SegmentTreeQuery.RunInt32(segTree, 1, 0, n - 1, l, r);
                            Console.WriteLine($"    [{l},{r}]: sparse={minSparse}, segtree={minSeg} {(minSparse == minSeg ? "✅" : "❌")}");
                        }
                    }
                }
                finally { Marshal.FreeHGlobal((nint)sparse); }

                Console.WriteLine();
                Console.WriteLine("  Updates and re-queries:");
                IAFahim.DS.SegmentTree.SegmentTreeSet.RunInt32(segTree, 1, 0, n - 1, 2, 99);
                Console.WriteLine("  Updated index 2 to 99");
                Console.WriteLine($"  Query [0,4] after update: {IAFahim.DS.SegmentTree.SegmentTreeQuery.RunInt32(segTree, 1, 0, n - 1, 0, 4)}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)arr);
                Marshal.FreeHGlobal((nint)segTree);
            }
        }

        private static void BitTreasureHunt()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ BIT Treasure Hunt ═══");
            Console.WriteLine("  Enter values (space-separated):");
            string input = Console.ReadLine()?.Trim() ?? "1 3 5 2 8 7 4 6";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int n = parts.Length;

            long* bit = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            try
            {
                for (int i = 0; i <= n; i++) bit[i] = 0;

                Console.WriteLine();
                Console.WriteLine($"  Initial values: {input}");
                Console.WriteLine("  Building BIT...");
                for (int i = 0; i < n; i++)
                {
                    int val = int.Parse(parts[i]);
                    BitSafe.Add(bit, i + 1, n, val);
                }

                Console.WriteLine();
                Console.WriteLine("  Prefix sums:");
                for (int i = 1; i <= n; i++)
                {
                    long sum = BitSafe.Sum(bit, i);
                    Console.WriteLine($"    [1..{i}]: sum = {sum}");
                }

                Console.WriteLine();
                Console.WriteLine("  Range query [l,r]:");
                Console.Write("  Enter l: ");
                string lStr = Console.ReadLine()?.Trim() ?? "2";
                Console.Write("  Enter r: ");
                string rStr = Console.ReadLine()?.Trim() ?? "5";
                if (!int.TryParse(lStr, out int l)) l = 2;
                if (!int.TryParse(rStr, out int r)) r = 5;
                l = Math.Max(1, Math.Min(l, n));
                r = Math.Max(1, Math.Min(r, n));
                if (l > r) { int t = l; l = r; r = t; }
                long rangeSum = BitSafe.RangeSum(bit, l, r);
                Console.WriteLine($"  Range sum [{l}..{r}] = {rangeSum}");

                long target = rangeSum / 2;
                int idx = BitSafe.LowerBound(bit, n, target);
                Console.WriteLine();
                Console.WriteLine($"  Lower bound of {target}: index {idx}");

                int limit = Math.Min(n, 32);
                long* f = stackalloc long[limit];
                for (int i = 0; i < limit; i++) f[i] = int.Parse(parts[i]);
                IAFahim.Math.Transform.SubsetZeta.Run(f, 1);
                Console.WriteLine($"  Subset ζ transform: f[0] = {f[0]}");
            }
            finally { Marshal.FreeHGlobal((nint)bit); }
        }
    }
}