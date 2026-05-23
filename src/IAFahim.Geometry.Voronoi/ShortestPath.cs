namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class ShortestPath
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PQNode
        {
            public int V;
            public double Dist;
        }

        private static void Swap(PQNode* pq, int i, int j)
        {
            PQNode t = pq[i];
            pq[i] = pq[j];
            pq[j] = t;
        }

        private static void Push(PQNode* pq, ref int size, int v, double d)
        {
            int i = size++;
            pq[i] = new PQNode { V = v, Dist = d };
            while (i > 0)
            {
                int p = (i - 1) / 2;
                if (pq[p].Dist <= pq[i].Dist) break;
                Swap(pq, i, p);
                i = p;
            }
        }

        private static PQNode Pop(PQNode* pq, ref int size)
        {
            PQNode res = pq[0];
            pq[0] = pq[--size];
            int i = 0;
            while (true)
            {
                int l = 2 * i + 1;
                int r = 2 * i + 2;
                if (l >= size) break;
                int min = l;
                if (r < size && pq[r].Dist < pq[l].Dist) min = r;
                if (pq[i].Dist <= pq[min].Dist) break;
                Swap(pq, i, min);
                i = min;
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Run(double* ox, double* oy, int n, int src, int dst, int* from, int* to, double* w, int e, double* dist, PQNode* pq, int* head, int* next, int* toEdge, double* weight)
        {
            if (src == dst) return 0;

            for (int i = 0; i < n; i++)
            {
                dist[i] = double.MaxValue;
                head[i] = -1;
            }

            int edgeCount = 0;
            for (int i = 0; i < e; i++)
            {
                int u = from[i], v = to[i];
                toEdge[edgeCount] = v; weight[edgeCount] = w[i]; next[edgeCount] = head[u]; head[u] = edgeCount++;
                toEdge[edgeCount] = u; weight[edgeCount] = w[i]; next[edgeCount] = head[v]; head[v] = edgeCount++;
            }

            int pqSize = 0;
            dist[src] = 0;
            Push(pq, ref pqSize, src, 0);

            while (pqSize > 0)
            {
                PQNode top = Pop(pq, ref pqSize);
                int u = top.V;
                double d = top.Dist;

                if (d > dist[u]) continue;
                if (u == dst) return d;

                for (int curr = head[u]; curr != -1; curr = next[curr])
                {
                    int v = toEdge[curr];
                    double nd = d + weight[curr];
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        Push(pq, ref pqSize, v, nd);
                    }
                }
            }
            return dist[dst];
        }
    }
}
