namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class ShortestPath
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PQNode { public int V; public double Dist; }

        public static double Run(double* ox, double* oy, int n, int src, int dst, int* from, int* to, double* w, int e, double* dist, PQNode* pq, int* head, int* next, int* toEdge, double* weight)
        {
            if (src == dst) return 0;
            InitializeGraph(n, e, from, to, w, head, next, toEdge, weight, dist);
            
            int pqSize = 0; dist[src] = 0; Push(pq, ref pqSize, src, 0);
            while (pqSize > 0)
            {
                PQNode top = Pop(pq, ref pqSize);
                if (top.Dist > dist[top.V]) continue;
                if (top.V == dst) return top.Dist;
                RelaxEdges(top.V, top.Dist, head, next, toEdge, weight, dist, pq, ref pqSize);
            }
            return dist[dst];
        }

        private static void InitializeGraph(int n, int e, int* from, int* to, double* w, int* head, int* next, int* toEdge, double* weight, double* dist)
        {
            for (int i = 0; i < n; i++) { dist[i] = double.MaxValue; head[i] = -1; }
            int ec = 0;
            for (int i = 0; i < e; i++)
            {
                int u = from[i], v = to[i];
                toEdge[ec] = v; weight[ec] = w[i]; next[ec] = head[u]; head[u] = ec++;
                toEdge[ec] = u; weight[ec] = w[i]; next[ec] = head[v]; head[v] = ec++;
            }
        }

        private static void RelaxEdges(int u, double d, int* head, int* next, int* toEdge, double* weight, double* dist, PQNode* pq, ref int pqSize)
        {
            for (int e = head[u]; e != -1; e = next[e])
            {
                int v = toEdge[e]; double nd = d + weight[e];
                if (nd < dist[v]) { dist[v] = nd; Push(pq, ref pqSize, v, nd); }
            }
        }

        private static void Push(PQNode* pq, ref int size, int v, double d)
        {
            int i = size++; pq[i] = new PQNode { V = v, Dist = d };
            while (i > 0)
            {
                int p = (i - 1) / 2; if (pq[p].Dist <= pq[i].Dist) break;
                Swap(pq, i, p); i = p;
            }
        }

        private static PQNode Pop(PQNode* pq, ref int size)
        {
            PQNode res = pq[0]; pq[0] = pq[--size]; int i = 0;
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, m = l; if (l >= size) break;
                if (r < size && pq[r].Dist < pq[l].Dist) m = r;
                if (pq[i].Dist <= pq[m].Dist) break;
                Swap(pq, i, m); i = m;
            }
            return res;
        }
        private static void Swap(PQNode* pq, int i, int j) { PQNode t = pq[i]; pq[i] = pq[j]; pq[j] = t; }
    }
}
