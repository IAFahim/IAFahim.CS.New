namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Graph;
    using IAFahim.Graph.Tree;

    public static unsafe class NetworkRanger
    {
        private static readonly string[] Names = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta" };

        public static void Run()
        {
            DisplayHeader();

            int n = 8, maxEdges = n * 4;
            int* head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* next = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* weight = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* edgeId = (int*)Marshal.AllocHGlobal(sizeof(int));
            long* dist = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));

            try
            {
                InitializeGraph(head, edgeId, n);
                DisplayNetworkMap();
                BuildNetworkEdges(head, to, next, weight, edgeId);

                int src = GetValidatedNode("  Start node (0-7 or name): ", n);
                int dst = GetValidatedNode("  Target node (0-7 or name): ", n);
                if (src < 0 || dst < 0) return;

                ExecutePathfinding(src, dst, n, head, to, next, weight, dist, parent);
                ExecuteNetworkAnalysis(src, n, head, to, next);
            }
            finally
            {
                FreeAllMemory(head, to, next, weight, edgeId, dist, parent);
            }
        }

        private static void DisplayHeader()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🌐  NETWORK RANGER  🌐            ║");
            Console.WriteLine("║  Navigate the network, find the path!    ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝\n");
        }

        private static void InitializeGraph(int* head, int* edgeId, int n)
        {
            for (int i = 0; i < n; i++) head[i] = 0;
            *edgeId = 1;
        }

        private static void DisplayNetworkMap()
        {
            Console.WriteLine("  Network map (8 nodes):");
            Console.WriteLine("  ┌─────┐     ┌─────┐");
            Console.WriteLine("  │ Alpha│────2────│ Beta │");
            Console.WriteLine("  └──┬──┘     └──┬──┘");
            Console.WriteLine("    3│          4│");
            Console.WriteLine("  ┌──┴──┐     ┌──┴──┐");
            Console.WriteLine("  │Gamma │────1────│Delta│");
            Console.WriteLine("  └──┬──┘     └──┬──┘");
            Console.WriteLine("    5│          2│");
            Console.WriteLine("  ┌──┴──┐     ┌──┴──┐");
            Console.WriteLine("  │Epsilon│──3──│ Zeta │");
            Console.WriteLine("  └──┬──┘     └──┬──┘");
            Console.WriteLine("    1│          6│");
            Console.WriteLine("  ┌──┴──┐     ┌──┴──┐");
            Console.WriteLine("  │ Eta  │────4────│Theta│");
            Console.WriteLine("  └─────┘     └─────┘\n");
        }

        private static void BuildNetworkEdges(int* head, int* to, int* next, int* weight, int* edgeId)
        {
            int[,] edges = {
                {0,1,2}, {1,0,2}, {0,2,3}, {2,0,3}, {1,3,4}, {3,1,4}, {2,3,1}, {3,2,1},
                {2,4,5}, {4,2,5}, {3,5,2}, {5,3,2}, {4,5,3}, {5,4,3}, {4,6,1}, {6,4,1},
                {5,7,6}, {7,5,6}, {6,7,4}, {7,6,4}
            };

            for (int i = 0; i < edges.GetLength(0); i++)
                AddWeightedEdge.Run(head, to, next, weight, edgeId, edges[i, 0], edges[i, 1], edges[i, 2], edgeId);
        }

        private static int GetValidatedNode(string prompt, int n)
        {
            Console.Write(prompt);
            int idx = ReadNode(Names);
            if (idx >= 0 && idx < n) return idx;
            Console.WriteLine("  Invalid node.");
            return -1;
        }

        private static void ExecutePathfinding(int src, int dst, int n, int* head, int* to, int* next, int* weight, long* dist, int* parent)
        {
            Dijkstra.Run(n, src, head, to, next, weight, dist, parent);
            Console.WriteLine($"\n  ═══ Pathfinder: {Names[src]} → {Names[dst]} ═══");
            Console.WriteLine($"  Shortest distance: {dist[dst]}");

            RestoreAndDisplayPath(dst, parent, n);
            DisplayAllDistances(src, dist, n);
        }

        private static void RestoreAndDisplayPath(int dst, int* parent, int n)
        {
            int* path = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                int pathLen = DijkstraRestorePath.Run(parent, dst, path);
                Console.Write("  Route: ");
                for (int i = 0; i < pathLen; i++)
                {
                    Console.Write(Names[path[i]]);
                    if (i < pathLen - 1) Console.Write(" → ");
                }
                Console.WriteLine();
            }
            finally { Marshal.FreeHGlobal((nint)path); }
        }

        private static void DisplayAllDistances(int src, long* dist, int n)
        {
            Console.WriteLine($"\n  ═══ All distances from {Names[src]} ═══");
            for (int i = 0; i < n; i++)
            {
                string d = dist[i] == long.MaxValue ? "∞" : dist[i].ToString();
                Console.WriteLine($"    {Names[i],8}: {d}");
            }
        }

        private static void ExecuteNetworkAnalysis(int src, int n, int* head, int* to, int* next)
        {
            Console.WriteLine("\n  ═══ Network Analysis ═══");
            int* bfsDist = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* bfsParent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                Bfs.Run(n, src, head, to, next, bfsDist, bfsParent);
                Console.Write($"  BFS order from {Names[src]}: ");
                for (int i = 0; i < n; i++) Console.Write($"{Names[i]}(d={bfsDist[i]}) ");
                Console.WriteLine();
            }
            finally { Marshal.FreeHGlobal((nint)bfsDist); Marshal.FreeHGlobal((nint)bfsParent); }
        }

        private static int ReadNode(string[] names)
        {
            string input = Console.ReadLine()?.Trim() ?? "";
            if (int.TryParse(input, out int idx)) return idx;
            for (int i = 0; i < names.Length; i++)
                if (names[i].Equals(input, StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }

        private static void FreeAllMemory(params void*[] pointers)
        {
            foreach (var ptr in pointers) Marshal.FreeHGlobal((nint)ptr);
        }
    }
}