namespace IAFahim.Geometry.Subdivision
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class PointQuadtree
    {
        public const int Empty = -1;
        public const int MaxPointsPerLeaf = 4;

        // Node layout in parallel arrays (capacity nodes):
        // x0,x1,y0,y1 bounds; child[4] indices or Empty; first/count point range in leaf list.
        // Points stored as indices into caller xs/ys via pointIdx[pointCap].

        public static int NodeBytes(int nodeCap) => nodeCap * (4 * sizeof(double) + 6 * sizeof(int));

        // Build a pointer quadtree over n points in [bx0,bx1]x[by0,by1].
        // Returns root node index (0) or -1 on failure. Writes nodes into nodeMem.
        public static int Build(
            double* xs, double* ys, int n,
            double bx0, double bx1, double by0, double by1,
            byte* nodeMem, int nodeCap,
            int* pointIdx, int pointCap,
            int* nodeCount)
        {
            *nodeCount = 0;
            if (n <= 0 || nodeCap < 1 || pointCap < n) return -1;
            for (int i = 0; i < n; i++) pointIdx[i] = i;

            double* x0 = (double*)nodeMem;
            double* x1 = x0 + nodeCap;
            double* y0 = x1 + nodeCap;
            double* y1 = y0 + nodeCap;
            int* c0 = (int*)(y1 + nodeCap);
            int* c1 = c0 + nodeCap;
            int* c2 = c1 + nodeCap;
            int* c3 = c2 + nodeCap;
            int* first = c3 + nodeCap;
            int* count = first + nodeCap;

            int root = AllocNode(x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap,
                bx0, bx1, by0, by1, 0, n);
            if (root < 0) return -1;
            Subdivide(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap, pointIdx, root);
            return root;
        }

        public static int QueryCount(
            double* xs, double* ys,
            byte* nodeMem, int nodeCap, int nodeCount,
            int* pointIdx,
            int root,
            double qx0, double qx1, double qy0, double qy1)
        {
            if (root < 0 || nodeCount <= 0) return 0;
            double* x0 = (double*)nodeMem;
            double* x1 = x0 + nodeCap;
            double* y0 = x1 + nodeCap;
            double* y1 = y0 + nodeCap;
            int* c0 = (int*)(y1 + nodeCap);
            int* c1 = c0 + nodeCap;
            int* c2 = c1 + nodeCap;
            int* c3 = c2 + nodeCap;
            int* first = c3 + nodeCap;
            int* count = first + nodeCap;
            return QueryRec(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, pointIdx, root, qx0, qx1, qy0, qy1);
        }

        // Brute force for verification.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeCount(double* xs, double* ys, int n, double x0, double x1, double y0, double y1)
        {
            int c = 0;
            for (int i = 0; i < n; i++)
                if (xs[i] >= x0 && xs[i] <= x1 && ys[i] >= y0 && ys[i] <= y1) c++;
            return c;
        }

        public static void SubdivideBox(double x0, double x1, double y0, double y1, double* outBoxes)
        {
            double mx = (x0 + x1) * 0.5;
            double my = (y0 + y1) * 0.5;
            outBoxes[0] = x0; outBoxes[1] = mx; outBoxes[2] = my; outBoxes[3] = y1;
            outBoxes[4] = mx; outBoxes[5] = x1; outBoxes[6] = my; outBoxes[7] = y1;
            outBoxes[8] = x0; outBoxes[9] = mx; outBoxes[10] = y0; outBoxes[11] = my;
            outBoxes[12] = mx; outBoxes[13] = x1; outBoxes[14] = y0; outBoxes[15] = my;
        }

        private static int AllocNode(
            double* x0, double* x1, double* y0, double* y1,
            int* c0, int* c1, int* c2, int* c3, int* first, int* count,
            int* nodeCount, int nodeCap,
            double bx0, double bx1, double by0, double by1, int ptFirst, int ptCount)
        {
            int id = *nodeCount;
            if (id >= nodeCap) return -1;
            (*nodeCount)++;
            x0[id] = bx0; x1[id] = bx1; y0[id] = by0; y1[id] = by1;
            c0[id] = c1[id] = c2[id] = c3[id] = Empty;
            first[id] = ptFirst;
            count[id] = ptCount;
            return id;
        }

        private static void Subdivide(
            double* xs, double* ys,
            double* x0, double* x1, double* y0, double* y1,
            int* c0, int* c1, int* c2, int* c3, int* first, int* count,
            int* nodeCount, int nodeCap, int* pointIdx, int node)
        {
            if (count[node] <= MaxPointsPerLeaf) return;
            double mx = (x0[node] + x1[node]) * 0.5;
            double my = (y0[node] + y1[node]) * 0.5;
            int f = first[node];
            int n = count[node];

            // Partition pointIdx[f..f+n) into 4 buckets in-place via counts then scatter.
            int* tmp = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int n0 = 0, n1 = 0, n2 = 0, n3 = 0;
            for (int i = 0; i < n; i++)
            {
                int pi = pointIdx[f + i];
                int q = ChildQuad(xs[pi], ys[pi], mx, my);
                if (q == 0) n0++;
                else if (q == 1) n1++;
                else if (q == 2) n2++;
                else n3++;
            }
            int o0 = 0, o1 = n0, o2 = n0 + n1, o3 = n0 + n1 + n2;
            int w0 = o0, w1 = o1, w2 = o2, w3 = o3;
            for (int i = 0; i < n; i++)
            {
                int pi = pointIdx[f + i];
                int q = ChildQuad(xs[pi], ys[pi], mx, my);
                if (q == 0) tmp[w0++] = pi;
                else if (q == 1) tmp[w1++] = pi;
                else if (q == 2) tmp[w2++] = pi;
                else tmp[w3++] = pi;
            }
            for (int i = 0; i < n; i++) pointIdx[f + i] = tmp[i];
            Marshal.FreeHGlobal((nint)tmp);

            if (n0 > 0)
            {
                c0[node] = AllocNode(x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap,
                    x0[node], mx, my, y1[node], f + o0, n0);
                if (c0[node] >= 0) Subdivide(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap, pointIdx, c0[node]);
            }
            if (n1 > 0)
            {
                c1[node] = AllocNode(x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap,
                    mx, x1[node], my, y1[node], f + o1, n1);
                if (c1[node] >= 0) Subdivide(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap, pointIdx, c1[node]);
            }
            if (n2 > 0)
            {
                c2[node] = AllocNode(x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap,
                    x0[node], mx, y0[node], my, f + o2, n2);
                if (c2[node] >= 0) Subdivide(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap, pointIdx, c2[node]);
            }
            if (n3 > 0)
            {
                c3[node] = AllocNode(x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap,
                    mx, x1[node], y0[node], my, f + o3, n3);
                if (c3[node] >= 0) Subdivide(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, nodeCount, nodeCap, pointIdx, c3[node]);
            }
            // Internal: clear leaf range
            count[node] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ChildQuad(double x, double y, double mx, double my)
        {
            // 0 NW, 1 NE, 2 SW, 3 SE
            if (y >= my) return x < mx ? 0 : 1;
            return x < mx ? 2 : 3;
        }

        private static int QueryRec(
            double* xs, double* ys,
            double* x0, double* x1, double* y0, double* y1,
            int* c0, int* c1, int* c2, int* c3, int* first, int* count, int* pointIdx,
            int node, double qx0, double qx1, double qy0, double qy1)
        {
            if (x1[node] < qx0 || x0[node] > qx1 || y1[node] < qy0 || y0[node] > qy1) return 0;
            bool leaf = c0[node] == Empty && c1[node] == Empty && c2[node] == Empty && c3[node] == Empty;
            if (leaf || count[node] > 0)
            {
                int c = 0;
                int f = first[node];
                int n = count[node];
                // If internal with empty count but children exist, skip leaf scan
                if (leaf)
                {
                    for (int i = 0; i < n; i++)
                    {
                        int pi = pointIdx[f + i];
                        if (xs[pi] >= qx0 && xs[pi] <= qx1 && ys[pi] >= qy0 && ys[pi] <= qy1) c++;
                    }
                    return c;
                }
            }
            int total = 0;
            if (c0[node] != Empty) total += QueryRec(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, pointIdx, c0[node], qx0, qx1, qy0, qy1);
            if (c1[node] != Empty) total += QueryRec(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, pointIdx, c1[node], qx0, qx1, qy0, qy1);
            if (c2[node] != Empty) total += QueryRec(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, pointIdx, c2[node], qx0, qx1, qy0, qy1);
            if (c3[node] != Empty) total += QueryRec(xs, ys, x0, x1, y0, y1, c0, c1, c2, c3, first, count, pointIdx, c3[node], qx0, qx1, qy0, qy1);
            return total;
        }
    }
}
