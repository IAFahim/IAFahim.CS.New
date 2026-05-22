namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Graph;
    using IAFahim.Graph.Tree;

    public static unsafe class NetworkRanger
    {
        public static void Run()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🌐  NETWORK RANGER  🌐            ║");
            Console.WriteLine("║  Navigate the network, find the path!    ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();

            int n = 8;
            int maxEdges = n * 4;
            int* head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* next_ = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* weight = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* edgeId = (int*)Marshal.AllocHGlobal(sizeof(int));
            long* dist = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) head[i] = 0;
                *edgeId = 1;

                string[] names = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta" };

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
                Console.WriteLine("  └─────┘     └─────┘");
                Console.WriteLine();

                int[,] edges = {
                    {0,1,2}, {1,0,2},
                    {0,2,3}, {2,0,3},
                    {1,3,4}, {3,1,4},
                    {2,3,1}, {3,2,1},
                    {2,4,5}, {4,2,5},
                    {3,5,2}, {5,3,2},
                    {4,5,3}, {5,4,3},
                    {4,6,1}, {6,4,1},
                    {5,7,6}, {7,5,6},
                    {6,7,4}, {7,6,4}
                };

                for (int i = 0; i < edges.GetLength(0); i++)
                    AddWeightedEdge.Run(head, to, next_, weight, edgeId, edges[i, 0], edges[i, 1], edges[i, 2], edgeId);

                Console.Write("  Start node (0-7 or name): ");
                int src = ReadNode(names);
                Console.Write("  Target node (0-7 or name): ");
                int dst = ReadNode(names);

                if (src < 0 || src >= n || dst < 0 || dst >= n) { Console.WriteLine("  Invalid node."); return; }

                Dijkstra.Run(n, src, head, to, next_, weight, dist, parent);

                Console.WriteLine();
                Console.WriteLine($"  ═══ Pathfinder: {names[src]} → {names[dst]} ═══");
                Console.WriteLine($"  Shortest distance: {dist[dst]}");

                int* path = (int*)Marshal.AllocHGlobal(n * sizeof(int));
                try
                {
                    int pathLen = DijkstraRestorePath.Run(parent, dst, path);
                    Console.Write("  Route: ");
                    for (int i = 0; i < pathLen; i++)
                    {
                        Console.Write(names[path[i]]);
                        if (i < pathLen - 1) Console.Write(" → ");
                    }
                    Console.WriteLine();
                }
                finally { Marshal.FreeHGlobal((nint)path); }

                Console.WriteLine();
                Console.WriteLine("  ═══ All distances from " + names[src] + " ═══");
                for (int i = 0; i < n; i++)
                {
                    string d = dist[i] == long.MaxValue ? "∞" : dist[i].ToString();
                    Console.WriteLine($"    {names[i],8}: {d}");
                }

                Console.WriteLine();
                Console.WriteLine("  ═══ Network Analysis ═══");
                int* depth = (int*)Marshal.AllocHGlobal(n * sizeof(int));
                int* bfsParent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
                try
                {
                    for (int i = 0; i < n; i++) depth[i] = -1;
                    int* bfsDist = (int*)Marshal.AllocHGlobal(n * sizeof(int));
                    try
                    {
                        Bfs.Run(n, src, head, to, next_, bfsDist, bfsParent);
                        Console.Write("  BFS order from " + names[src] + ": ");
                        for (int i = 0; i < n; i++) Console.Write(names[i] + $"(d={bfsDist[i]}) ");
                        Console.WriteLine();
                    }
                    finally { Marshal.FreeHGlobal((nint)bfsDist); }
                }
                finally { Marshal.FreeHGlobal((nint)depth); Marshal.FreeHGlobal((nint)bfsParent); }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)head);
                Marshal.FreeHGlobal((nint)to);
                Marshal.FreeHGlobal((nint)next_);
                Marshal.FreeHGlobal((nint)weight);
                Marshal.FreeHGlobal((nint)edgeId);
                Marshal.FreeHGlobal((nint)dist);
                Marshal.FreeHGlobal((nint)parent);
            }
        }

        private static int ReadNode(string[] names)
        {
            string input = Console.ReadLine()?.Trim() ?? "";
            if (int.TryParse(input, out int idx)) return idx;
            for (int i = 0; i < names.Length; i++)
                if (names[i].Equals(input, StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }
    }
}