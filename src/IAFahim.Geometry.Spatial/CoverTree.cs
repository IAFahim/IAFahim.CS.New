namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class CoverTree
    {
        public struct Node
        {
            public double X, Y;
            public int Level;
            public int Next;
        }

        private const double Base = 2.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Dist2(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return dx * dx + dy * dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Dist(Node* nodes, int a, int b)
        {
            return Math.Sqrt(Dist2(nodes[a].X, nodes[a].Y, nodes[b].X, nodes[b].Y));
        }

        public static int Build(double* xs, double* ys, int n, Node* nodes)
        {
            if (n <= 0) return 0;
            for (int i = 0; i < n; i++)
            {
                nodes[i].X = xs[i];
                nodes[i].Y = ys[i];
                nodes[i].Level = 0;
                nodes[i].Next = -1;
            }
            if (n == 1) return 1;

            double maxD = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double d = Dist(nodes, i, j);
                    if (d > maxD) maxD = d;
                }
            }
            int maxLevel = 0;
            double scale = 1.0;
            while (scale < maxD && maxLevel < 62)
            {
                scale *= Base;
                maxLevel++;
            }
            nodes[0].Level = maxLevel;
            nodes[0].Next = -1;

            for (int i = 1; i < n; i++)
            {
                int parent = 0;
                double bestD = Dist(nodes, 0, i);
                for (int j = 1; j < i; j++)
                {
                    double d = Dist(nodes, j, i);
                    if (d < bestD)
                    {
                        bestD = d;
                        parent = j;
                    }
                }
                nodes[i].Next = parent;
                int childLevel = nodes[parent].Level - 1;
                if (childLevel < 0) childLevel = 0;
                nodes[i].Level = childLevel;
                if (nodes[parent].Level <= nodes[i].Level)
                {
                    nodes[parent].Level = nodes[i].Level + 1;
                }
            }
            return n;
        }

        public static int Nearest(Node* nodes, int n, double qx, double qy)
        {
            if (n <= 0) return -1;
            int root = 0;
            for (int i = 0; i < n; i++)
            {
                if (nodes[i].Next < 0)
                {
                    root = i;
                    break;
                }
            }
            int best = root;
            double bestD2 = Dist2(nodes[root].X, nodes[root].Y, qx, qy);
            Search(nodes, n, root, qx, qy, ref best, ref bestD2);
            return best;
        }

        private static void Search(Node* nodes, int n, int u, double qx, double qy, ref int best, ref double bestD2)
        {
            double d2 = Dist2(nodes[u].X, nodes[u].Y, qx, qy);
            if (d2 < bestD2)
            {
                bestD2 = d2;
                best = u;
            }

            double bestD = Math.Sqrt(bestD2);
            for (int c = 0; c < n; c++)
            {
                if (nodes[c].Next != u) continue;
                double cover = SubtreeCover(nodes, n, c);
                double cd = Math.Sqrt(Dist2(nodes[c].X, nodes[c].Y, qx, qy));
                if (cd - cover >= bestD) continue;
                Search(nodes, n, c, qx, qy, ref best, ref bestD2);
                bestD = Math.Sqrt(bestD2);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double SubtreeCover(Node* nodes, int n, int u)
        {
            double r = 0;
            for (int i = 0; i < n; i++)
            {
                int p = i;
                while (p >= 0)
                {
                    if (p == u)
                    {
                        double d = Dist(nodes, i, u);
                        if (d > r) r = d;
                        break;
                    }
                    p = nodes[p].Next;
                }
            }
            return r;
        }
    }
}
