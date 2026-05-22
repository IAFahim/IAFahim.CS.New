namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Octree
    {
        public struct Node
        {
            public double X, Y, Z, Size;
            public int C0, C1, C2, C3, C4, C5, C6, C7;
            public int FirstIndex;
            public int Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, double* zs, int n, Node* nodes, int maxDepth)
        {
            if (n == 0) return 0;
            double minX = xs[0], maxX = xs[0], minY = ys[0], maxY = ys[0], minZ = zs[0], maxZ = zs[0];
            for (int i = 1; i < n; i++)
            {
                if (xs[i] < minX) minX = xs[i];
                if (xs[i] > maxX) maxX = xs[i];
                if (ys[i] < minY) minY = ys[i];
                if (ys[i] > maxY) maxY = ys[i];
                if (zs[i] < minZ) minZ = zs[i];
                if (zs[i] > maxZ) maxZ = zs[i];
            }

            double w = maxX - minX;
            double h = maxY - minY;
            double d = maxZ - minZ;
            double size = w;
            if (h > size) size = h;
            if (d > size) size = d;

            int nextFree = 0;
            BuildRec(xs, ys, zs, 0, n, minX, minY, minZ, size, nodes, ref nextFree, 0, maxDepth);
            return nextFree;
        }

        private static int BuildRec(double* xs, double* ys, double* zs, int start, int end, double x, double y, double z, double size, Node* nodes, ref int nextFree, int depth, int maxDepth)
        {
            int nodeIdx = nextFree++;
            nodes[nodeIdx].X = x; nodes[nodeIdx].Y = y; nodes[nodeIdx].Z = z; nodes[nodeIdx].Size = size;
            nodes[nodeIdx].C0 = nodes[nodeIdx].C1 = nodes[nodeIdx].C2 = nodes[nodeIdx].C3 = -1;
            nodes[nodeIdx].C4 = nodes[nodeIdx].C5 = nodes[nodeIdx].C6 = nodes[nodeIdx].C7 = -1;
            nodes[nodeIdx].FirstIndex = start;
            nodes[nodeIdx].Count = end - start;

            if (depth >= maxDepth || end - start <= 1)
            {
                return nodeIdx;
            }

            double half = size / 2;
            double midX = x + half, midY = y + half, midZ = z + half;

            int n = end - start;
            double* tx = stackalloc double[n];
            double* ty = stackalloc double[n];
            double* tz = stackalloc double[n];

            int* c = stackalloc int[8];
            for (int i = 0; i < 8; i++) c[i] = 0;

            for (int i = start; i < end; i++)
            {
                int idx = 0;
                if (xs[i] >= midX) idx |= 1;
                if (ys[i] >= midY) idx |= 2;
                if (zs[i] >= midZ) idx |= 4;
                c[idx]++;
            }

            int* p = stackalloc int[8];
            p[0] = 0;
            for (int i = 1; i < 8; i++) p[i] = p[i - 1] + c[i - 1];

            for (int i = start; i < end; i++)
            {
                int idx = 0;
                if (xs[i] >= midX) idx |= 1;
                if (ys[i] >= midY) idx |= 2;
                if (zs[i] >= midZ) idx |= 4;
                int pos = p[idx]++;
                tx[pos] = xs[i]; ty[pos] = ys[i]; tz[pos] = zs[i];
            }

            for (int i = 0; i < n; i++)
            {
                xs[start + i] = tx[i];
                ys[start + i] = ty[i];
                zs[start + i] = tz[i];
            }

            int cSum = 0;
            if (c[0] > 0) nodes[nodeIdx].C0 = BuildRec(xs, ys, zs, start + cSum, start + cSum + c[0], x, y, z, half, nodes, ref nextFree, depth + 1, maxDepth); cSum += c[0];
            if (c[1] > 0) nodes[nodeIdx].C1 = BuildRec(xs, ys, zs, start + cSum, start + cSum + c[1], midX, y, z, half, nodes, ref nextFree, depth + 1, maxDepth); cSum += c[1];
            if (c[2] > 0) nodes[nodeIdx].C2 = BuildRec(xs, ys, zs, start + cSum, start + cSum + c[2], x, midY, z, half, nodes, ref nextFree, depth + 1, maxDepth); cSum += c[2];
            if (c[3] > 0) nodes[nodeIdx].C3 = BuildRec(xs, ys, zs, start + cSum, start + cSum + c[3], midX, midY, z, half, nodes, ref nextFree, depth + 1, maxDepth); cSum += c[3];
            if (c[4] > 0) nodes[nodeIdx].C4 = BuildRec(xs, ys, zs, start + cSum, start + cSum + c[4], x, y, midZ, half, nodes, ref nextFree, depth + 1, maxDepth); cSum += c[4];
            if (c[5] > 0) nodes[nodeIdx].C5 = BuildRec(xs, ys, zs, start + cSum, start + cSum + c[5], midX, y, midZ, half, nodes, ref nextFree, depth + 1, maxDepth); cSum += c[5];
            if (c[6] > 0) nodes[nodeIdx].C6 = BuildRec(xs, ys, zs, start + cSum, start + cSum + c[6], x, midY, midZ, half, nodes, ref nextFree, depth + 1, maxDepth); cSum += c[6];
            if (c[7] > 0) nodes[nodeIdx].C7 = BuildRec(xs, ys, zs, start + cSum, start + cSum + c[7], midX, midY, midZ, half, nodes, ref nextFree, depth + 1, maxDepth); cSum += c[7];

            return nodeIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeQuery(Node* nodes, int node, double x1, double y1, double z1, double x2, double y2, double z2, int* outIdx)
        {
            if (node < 0) return 0;
            Node* n = &nodes[node];
            if (x2 < n->X || x1 > n->X + n->Size || y2 < n->Y || y1 > n->Y + n->Size || z2 < n->Z || z1 > n->Z + n->Size) return 0;

            if (n->C0 < 0 && n->C1 < 0 && n->C2 < 0 && n->C3 < 0 && n->C4 < 0 && n->C5 < 0 && n->C6 < 0 && n->C7 < 0)
            {
                for (int i = 0; i < n->Count; i++) outIdx[i] = n->FirstIndex + i;
                return n->Count;
            }

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
