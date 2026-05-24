namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Octree
    {
        public struct Node { public double X, Y, Z, Size; public int C0, C1, C2, C3, C4, C5, C6, C7; public int FirstIndex, Count; }

        public static int Build(double* xs, double* ys, double* zs, int n, Node* nodes, int maxDepth)
        {
            if (n == 0) return 0;
            FindBounds(xs, ys, zs, n, out double minX, out double minY, out double minZ, out double size);
            int nextFree = 0; BuildRec(xs, ys, zs, 0, n, minX, minY, minZ, size, nodes, ref nextFree, 0, maxDepth); return nextFree;
        }
        private static void FindBounds(double* xs, double* ys, double* zs, int n, out double minX, out double minY, out double minZ, out double size)
        {
            minX = xs[0]; double maxX = xs[0]; minY = ys[0]; double maxY = ys[0]; minZ = zs[0]; double maxZ = zs[0];
            for (int i = 1; i < n; i++) { minX = Math.Min(minX, xs[i]); maxX = Math.Max(maxX, xs[i]); minY = Math.Min(minY, ys[i]); maxY = Math.Max(maxY, ys[i]); minZ = Math.Min(minZ, zs[i]); maxZ = Math.Max(maxZ, zs[i]); }
            size = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));
        }
        private static int BuildRec(double* xs, double* ys, double* zs, int start, int end, double x, double y, double z, double size, Node* nodes, ref int nextFree, int depth, int maxDepth)
        {
            int nodeIdx = nextFree++; InitializeNode(ref nodes[nodeIdx], x, y, z, size, start, end); if (depth >= maxDepth || end - start <= 1) return nodeIdx;
            int* counts = stackalloc int[8]; PartitionOctants(xs, ys, zs, start, end, x + size / 2, y + size / 2, z + size / 2, counts);
            BuildChildren(xs, ys, zs, start, end, x, y, z, size / 2, nodes, nodeIdx, ref nextFree, depth, maxDepth, counts); return nodeIdx;
        }
        private static void InitializeNode(ref Node n, double x, double y, double z, double size, int start, int end) { n.X = x; n.Y = y; n.Z = z; n.Size = size; n.FirstIndex = start; n.Count = end - start; n.C0 = n.C1 = n.C2 = n.C3 = n.C4 = n.C5 = n.C6 = n.C7 = -1; }
        private static void PartitionOctants(double* xs, double* ys, double* zs, int start, int end, double mx, double my, double mz, int* counts)
        {
            int n = end - start; for (int i = 0; i < 8; i++) counts[i] = 0;
            double* tx = stackalloc double[n], ty = stackalloc double[n], tz = stackalloc double[n];
            for (int i = start; i < end; i++) counts[GetOctIdx(xs[i], ys[i], zs[i], mx, my, mz)]++;
            int* p = stackalloc int[8]; p[0] = 0; for (int i = 1; i < 8; i++) p[i] = p[i - 1] + counts[i - 1];
            for (int i = start; i < end; i++) { int idx = GetOctIdx(xs[i], ys[i], zs[i], mx, my, mz), pos = p[idx]++; tx[pos] = xs[i]; ty[pos] = ys[i]; tz[pos] = zs[i]; }
            for (int i = 0; i < n; i++) { xs[start + i] = tx[i]; ys[start + i] = ty[i]; zs[start + i] = tz[i]; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static int GetOctIdx(double x, double y, double z, double mx, double my, double mz) { int i = 0; if (x >= mx) i |= 1; if (y >= my) i |= 2; if (z >= mz) i |= 4; return i; }
        private static void BuildChildren(double* xs, double* ys, double* zs, int start, int end, double x, double y, double z, double half, Node* nodes, int nodeIdx, ref int nextFree, int depth, int maxDepth, int* c)
        {
            double mx = x + half, my = y + half, mz = z + half; int s = start;
            if (c[0] > 0) nodes[nodeIdx].C0 = BuildRec(xs, ys, zs, s, s + c[0], x, y, z, half, nodes, ref nextFree, depth + 1, maxDepth); s += c[0];
            if (c[1] > 0) nodes[nodeIdx].C1 = BuildRec(xs, ys, zs, s, s + c[1], mx, y, z, half, nodes, ref nextFree, depth + 1, maxDepth); s += c[1];
            if (c[2] > 0) nodes[nodeIdx].C2 = BuildRec(xs, ys, zs, s, s + c[2], x, my, z, half, nodes, ref nextFree, depth + 1, maxDepth); s += c[2];
            if (c[3] > 0) nodes[nodeIdx].C3 = BuildRec(xs, ys, zs, s, s + c[3], mx, my, z, half, nodes, ref nextFree, depth + 1, maxDepth); s += c[3];
            if (c[4] > 0) nodes[nodeIdx].C4 = BuildRec(xs, ys, zs, s, s + c[4], x, y, mz, half, nodes, ref nextFree, depth + 1, maxDepth); s += c[4];
            if (c[5] > 0) nodes[nodeIdx].C5 = BuildRec(xs, ys, zs, s, s + c[5], mx, y, mz, half, nodes, ref nextFree, depth + 1, maxDepth); s += c[5];
            if (c[6] > 0) nodes[nodeIdx].C6 = BuildRec(xs, ys, zs, s, s + c[6], x, my, mz, half, nodes, ref nextFree, depth + 1, maxDepth); s += c[6];
            if (c[7] > 0) nodes[nodeIdx].C7 = BuildRec(xs, ys, zs, s, s + c[7], mx, my, mz, half, nodes, ref nextFree, depth + 1, maxDepth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeQuery(Node* nodes, int node, double x1, double y1, double z1, double x2, double y2, double z2, int* outIdx)
        {
            if (node < 0) return 0; Node* n = &nodes[node];
            if (x2 < n->X || x1 > n->X + n->Size || y2 < n->Y || y1 > n->Y + n->Size || z2 < n->Z || z1 > n->Z + n->Size) return 0;
            if (n->C0 < 0 && n->C1 < 0 && n->C2 < 0 && n->C3 < 0 && n->C4 < 0 && n->C5 < 0 && n->C6 < 0 && n->C7 < 0) { for (int i = 0; i < n->Count; i++) outIdx[i] = n->FirstIndex + i; return n->Count; }
            int count = 0;
            if (n->C0 >= 0) count += RangeQuery(nodes, n->C0, x1, y1, z1, x2, y2, z2, outIdx + count);
            if (n->C1 >= 0) count += RangeQuery(nodes, n->C1, x1, y1, z1, x2, y2, z2, outIdx + count);
            if (n->C2 >= 0) count += RangeQuery(nodes, n->C2, x1, y1, z1, x2, y2, z2, outIdx + count);
            if (n->C3 >= 0) count += RangeQuery(nodes, n->C3, x1, y1, z1, x2, y2, z2, outIdx + count);
            if (n->C4 >= 0) count += RangeQuery(nodes, n->C4, x1, y1, z1, x2, y2, z2, outIdx + count);
            if (n->C5 >= 0) count += RangeQuery(nodes, n->C5, x1, y1, z1, x2, y2, z2, outIdx + count);
            if (n->C6 >= 0) count += RangeQuery(nodes, n->C6, x1, y1, z1, x2, y2, z2, outIdx + count);
            if (n->C7 >= 0) count += RangeQuery(nodes, n->C7, x1, y1, z1, x2, y2, z2, outIdx + count);
            return count;
        }
    }
}
