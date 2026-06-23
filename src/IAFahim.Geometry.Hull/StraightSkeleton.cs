namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class StraightSkeleton
    {
        public struct Event { public double T; public int V1, V2; }

        public struct Node
        {
            public int Id;
            public double X, Y;
            public double Dx, Dy;
            public double Nx1, Ny1; // Normal of left edge
            public double Nx2, Ny2; // Normal of right edge
            public int Prev, Next;
            public bool Deleted;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double IntersectTime(Node* nodes, int i, int j)
        {
            double xij = nodes[j].X - nodes[i].X;
            double yij = nodes[j].Y - nodes[i].Y;
            double dxij = nodes[j].Dx - nodes[i].Dx;
            double dyij = nodes[j].Dy - nodes[i].Dy;
            
            if (Math.Abs(dxij) < 1e-12 && Math.Abs(dyij) < 1e-12) return double.MaxValue;
            if (Math.Abs(dxij) < 1e-12)
            {
                if (Math.Abs(dyij) < 1e-12) return double.MaxValue;
                double t = -yij / dyij;
                if (Math.Abs(xij + dxij * t) > 1e-9) return double.MaxValue;
                return t > 1e-9 ? t : double.MaxValue;
            }
            double tX = -xij / dxij;
            if (Math.Abs(dyij) < 1e-12)
            {
                if (Math.Abs(yij + dyij * tX) > 1e-9) return double.MaxValue;
                return tX > 1e-9 ? tX : double.MaxValue;
            }
            double tY = -yij / dyij;
            if (Math.Abs(tX - tY) > 1e-9) return double.MaxValue;
            return tX > 1e-9 ? tX : double.MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssignBisector(ref double dx, ref double dy, double bx, double by, double nx1, double ny1)
        {
            double dot = bx * nx1 + by * ny1;
            dx = 0; dy = 0;
            if (Math.Abs(dot) > 1e-12)
            {
                double speed = 1.0 / dot;
                dx = bx * speed;
                dy = by * speed;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitNodes(Node* nodes, double* xs, double* ys, int n)
        {
            for (int i = 0; i < n; i++)
            {
                int prev = (i - 1 + n) % n;
                int next = (i + 1) % n;

                double dx1 = xs[i] - xs[prev], dy1 = ys[i] - ys[prev];
                double len1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);
                dx1 /= len1; dy1 /= len1;

                double dx2 = xs[next] - xs[i], dy2 = ys[next] - ys[i];
                double len2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);
                dx2 /= len2; dy2 /= len2;

                double nx1 = -dy1, ny1 = dx1;
                double nx2 = -dy2, ny2 = dx2;

                double bx = nx1 + nx2, by = ny1 + ny2;

                nodes[i] = new Node
                {
                    Id = i, X = xs[i], Y = ys[i],
                    Nx1 = nx1, Ny1 = ny1, Nx2 = nx2, Ny2 = ny2,
                    Prev = prev, Next = next, Deleted = false
                };

                AssignBisector(ref nodes[i].Dx, ref nodes[i].Dy, bx, by, nx1, ny1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double FindEarliestEvent(Node* nodes, int nextId, out int minI, out int minJ)
        {
            double minT = double.MaxValue;
            minI = -1; minJ = -1;
            for (int i = 0; i < nextId; i++)
            {
                if (nodes[i].Deleted) continue;
                int j = nodes[i].Next;
                if (j == i) continue;
                double t = IntersectTime(nodes, i, j);
                if (t < minT)
                {
                    minT = t;
                    minI = i;
                    minJ = j;
                }
            }
            return minT;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EmitSegment(double* outX, double* outY, ref int outCount, int minI, int minJ, double meetX, double meetY, Node* nodes)
        {
            outX[outCount] = nodes[minI].X; outY[outCount++] = nodes[minI].Y;
            outX[outCount] = meetX; outY[outCount++] = meetY;
            outX[outCount] = nodes[minJ].X; outY[outCount++] = nodes[minJ].Y;
            outX[outCount] = meetX; outY[outCount++] = meetY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CreateMergedNode(Node* nodes, int nextId, int minI, int minJ, int prevI, int nextJ, double meetX, double meetY)
        {
            double nx1 = nodes[minI].Nx1, ny1 = nodes[minI].Ny1;
            double nx2 = nodes[minJ].Nx2, ny2 = nodes[minJ].Ny2;
            nodes[nextId] = new Node
            {
                Id = nextId, X = meetX, Y = meetY,
                Nx1 = nx1, Ny1 = ny1, Nx2 = nx2, Ny2 = ny2,
                Prev = prevI, Next = nextJ, Deleted = false
            };
            double bx = nx1 + nx2, by = ny1 + ny2;
            AssignBisector(ref nodes[nextId].Dx, ref nodes[nextId].Dy, bx, by, nx1, ny1);
            nodes[prevI].Next = nextId;
            nodes[nextJ].Prev = nextId;
            return nextId + 1;
        }

        public static int Build(double* xs, double* ys, int n, double* outX, double* outY, Node* scratchNodes)
        {
            if (n < 3) return 0;
            Node* nodes = scratchNodes;
            int outCount = 0;

            InitNodes(nodes, xs, ys, n);

            int nextId = n;
            int activeCount = n;

            while (activeCount > 2)
            {
                double minT = FindEarliestEvent(nodes, nextId, out int minI, out int minJ);

                if (minI == -1) break;

                double meetX = nodes[minI].X + nodes[minI].Dx * minT;
                double meetY = nodes[minI].Y + nodes[minI].Dy * minT;

                EmitSegment(outX, outY, ref outCount, minI, minJ, meetX, meetY, nodes);

                int prevI = nodes[minI].Prev;
                int nextJ = nodes[minJ].Next;

                nodes[minI].Deleted = true;
                nodes[minJ].Deleted = true;
                activeCount -= 2;

                if (activeCount > 0)
                {
                    nextId = CreateMergedNode(nodes, nextId, minI, minJ, prevI, nextJ, meetX, meetY);
                    activeCount++;
                }
            }

            return outCount / 2; // Returns number of segments
        }
    }
}
