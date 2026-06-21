namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;
    using IAFahim.Graph.Flow;

    public static unsafe class Planar
    {
        public static void GomoryHuBuild(int n, int m, int* head, int* to, int* next, int* cap, int* parent, int* weight)
        {
            InitializeGomoryHu(n, parent, weight);
            int flowSize = FindMaxEdgeIdx(n, head, next) + 2;
            int* flow = stackalloc int[flowSize];

            for (int i = 1; i < n; i++)
            {
                int s = i, t = parent[i];
                ClearFlow(flow, flowSize);
                long cutVal = DinicMaxFlow.Run(n, s, t, head, to, next, cap, flow);
                
                byte* visited = stackalloc byte[n];
                FindCutNodes(n, s, head, to, next, cap, flow, visited);
                
                weight[i] = (int)cutVal;
                UpdateParentTree(n, i, t, visited, parent);
            }
        }

        private static void InitializeGomoryHu(int n, int* parent, int* weight)
        {
            for (int i = 0; i < n; i++) { parent[i] = 0; weight[i] = 0; }
        }

        private static int FindMaxEdgeIdx(int n, int* head, int* next)
        {
            int maxIdx = 0;
            for (int i = 0; i < n; i++)
                for (int e = head[i]; e != 0; e = next[e])
                    if (e > maxIdx) maxIdx = e;
            return maxIdx;
        }

        private static void ClearFlow(int* flow, int size)
        {
            for (int i = 0; i < size; i++) flow[i] = 0;
        }

        private static void FindCutNodes(int n, int s, int* head, int* to, int* next, int* cap, int* flow, byte* visited)
        {
            for (int j = 0; j < n; j++) visited[j] = 0;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            visited[s] = 1; q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                    if (visited[to[e]] == 0 && cap[e] - flow[e] > 0) { visited[to[e]] = 1; q[qt++] = to[e]; }
            }
        }

        private static void UpdateParentTree(int n, int i, int t, byte* visited, int* parent)
        {
            for (int j = i + 1; j < n; j++)
                if (parent[j] == t && visited[j] == 1) parent[j] = i;
        }

        public static int GomoryHuQuery(int n, int* parent, int* weight, int u, int v)
        {
            int* q = stackalloc int[n], ew = stackalloc int[n];
            byte* vis = stackalloc byte[n];
            for (int i = 0; i < n; i++) { vis[i] = 0; ew[i] = int.MaxValue; }

            int qh = 0, qt = 0;
            vis[u] = 1; q[qt++] = u;
            while (qh < qt)
            {
                int curr = q[qh++];
                if (curr == v) return ew[v];
                ProcessGomoryHuNeighbors(n, curr, parent, weight, q, ref qt, vis, ew);
            }
            return 0;
        }

        private static void ProcessGomoryHuNeighbors(int n, int curr, int* parent, int* weight, int* q, ref int qt, byte* vis, int* ew)
        {
            if (curr > 0 && vis[parent[curr]] == 0)
                AddGomoryNeighbor(parent[curr], curr, weight[curr], q, ref qt, vis, ew);
            for (int j = 1; j < n; j++)
                if (parent[j] == curr && vis[j] == 0)
                    AddGomoryNeighbor(j, curr, weight[j], q, ref qt, vis, ew);
        }

        private static void AddGomoryNeighbor(int next, int curr, int w, int* q, ref int qt, byte* vis, int* ew)
        {
            vis[next] = 1; ew[next] = Math.Min(ew[curr], w); q[qt++] = next;
        }

        public static bool SplittingOff(int n, int* m, int s, int* u, int* v, int* resultU, int* resultV, int* resultCount)
        {
            *resultCount = 0;
            int initialMin = GetMinConnectivity(n, *m, u, v, s);
            while (TryFindSplitEdge(n, m, s, u, v, initialMin, resultU, resultV, resultCount)) { }
            return true;
        }

        private static bool TryFindSplitEdge(int n, int* m, int s, int* u, int* v, int initialMin, int* resultU, int* resultV, int* resultCount)
        {
            int incidentCount = 0;
            int* incident = stackalloc int[*m];
            for (int i = 0; i < *m; i++) if (u[i] == s || v[i] == s) incident[incidentCount++] = i;
            if (incidentCount < 2) return false;

            for (int i = 0; i < incidentCount; i++)
            for (int j = i + 1; j < incidentCount; j++)
            {
                int e1 = incident[i], e2 = incident[j];
                int a = u[e1] == s ? v[e1] : u[e1], b = u[e2] == s ? v[e2] : u[e2];
                if (CheckSplitValidity(n, m, s, u, v, initialMin, e1, e2, a, b))
                {
                    PerformSplit(m, u, v, e1, e2, a, b, resultU, resultV, resultCount);
                    return true;
                }
            }
            return false;
        }

        private static bool CheckSplitValidity(int n, int* m, int s, int* u, int* v, int initialMin, int e1, int e2, int a, int b)
        {
            int tempM = *m - 1;
            int* tu = stackalloc int[tempM], tv = stackalloc int[tempM];
            int tidx = 0;
            for (int k = 0; k < *m; k++) if (k != e1 && k != e2) { tu[tidx] = u[k]; tv[tidx++] = v[k]; }
            tu[tidx] = a; tv[tidx] = b;
            return GetMinConnectivity(n, tempM, tu, tv, s) >= initialMin;
        }

        private static void PerformSplit(int* m, int* u, int* v, int e1, int e2, int a, int b, int* resultU, int* resultV, int* resultCount)
        {
            u[e1] = a; v[e1] = b; u[e2] = u[*m - 1]; v[e2] = v[*m - 1]; (*m)--;
            resultU[*resultCount] = a; resultV[*resultCount] = b; (*resultCount)++;
        }

        private static int GetMinConnectivity(int n, int m, int* u, int* v, int s)
        {
            int minConn = 999999;
            int* head = stackalloc int[n], to = stackalloc int[m * 2 + 2], nxt = stackalloc int[m * 2 + 2], cap = stackalloc int[m * 2 + 2], flow = stackalloc int[m * 2 + 2];
            for (int i = 0; i < n; i++) head[i] = -1;
            int ec = 0;
            for (int i = 0; i < m; i++)
            {
                int ui = u[i], vi = v[i];
                to[ec] = vi; cap[ec] = 1; nxt[ec] = head[ui]; head[ui] = ec++;
                to[ec] = ui; cap[ec] = 1; nxt[ec] = head[vi]; head[vi] = ec++;
            }
            int* active = stackalloc int[n]; int ac = 0;
            for (int i = 0; i < n; i++) if (i != s) active[ac++] = i;
            if (ac < 2) return 0;
            for (int i = 0; i < ac; i++)
            for (int j = i + 1; j < ac; j++)
            {
                for (int k = 0; k < ec; k++) flow[k] = 0;
                minConn = Math.Min(minConn, (int)DinicMaxFlow.Run(n, active[i], active[j], head, to, nxt, cap, flow));
            }
            return minConn;
        }

        public static bool EarDecomposition(int n, int m, int* u, int* v, int* earEdges, int* earLengths, int* earCount)
        {
            *earCount = 0;
            byte* eu = stackalloc byte[m], vv = stackalloc byte[n];
            for (int i = 0; i < m; i++) eu[i] = 0; for (int i = 0; i < n; i++) vv[i] = 0;
            int* p = stackalloc int[n], pe = stackalloc int[n];
            for (int i = 0; i < n; i++) { p[i] = -1; pe[i] = -1; }

            if (!FindInitialCycle(n, m, u, v, p, pe, vv, out int cs, out int ce, out int ced)) return false;
            
            AddInitialEar(p, pe, cs, ce, ced, earEdges, earLengths, earCount, eu, vv);
            while (TryAddEar(n, m, u, v, earEdges, earLengths, earCount, eu, vv)) { }
            
            for (int i = 0; i < m; i++) if (eu[i] == 0) return false;
            return true;
        }

        private static bool FindInitialCycle(int n, int m, int* u, int* v, int* p, int* pe, byte* vv, out int cs, out int ce, out int ced)
        {
            cs = ce = ced = -1;
            for (int i = 0; i < n; i++)
                if (vv[i] == 0 && DfsCycle(i, -1, -1, m, u, v, p, pe, vv, ref cs, ref ce, ref ced)) return true;
            return false;
        }

        private static bool DfsCycle(int curr, int par, int pe_idx, int m, int* u, int* v, int* p, int* pe, byte* vv, ref int cs, ref int ce, ref int ced)
        {
            vv[curr] = 1;
            for (int e = 0; e < m; e++)
            {
                if (u[e] != curr && v[e] != curr) continue;
                if (e == pe_idx) continue;
                int nbr = u[e] == curr ? v[e] : u[e];
                if (vv[nbr] == 1) { cs = nbr; ce = curr; ced = e; return true; }
                if (vv[nbr] == 0) { p[nbr] = curr; pe[nbr] = e; if (DfsCycle(nbr, curr, e, m, u, v, p, pe, vv, ref cs, ref ce, ref ced)) return true; }
            }
            vv[curr] = 2; return false;
        }

        private static void AddInitialEar(int* p, int* pe, int cs, int ce, int ced, int* earEdges, int* earLengths, int* earCount, byte* eu, byte* vv)
        {
            int idx = 0; earEdges[idx++] = ced; eu[ced] = 1; vv[cs] = vv[ce] = 1;
            int cur = ce, len = 1;
            while (cur != cs) { int e = pe[cur]; earEdges[idx++] = e; eu[e] = 1; vv[cur] = 1; cur = p[cur]; len++; }
            earLengths[*earCount] = len; (*earCount)++;
        }

        private static bool TryAddEar(int n, int m, int* u, int* v, int* earEdges, int* earLengths, int* earCount, byte* eu, byte* vv)
        {
            int startEdge = -1;
            for (int e = 0; e < m; e++) if (eu[e] == 0 && (vv[u[e]] == 1 || vv[v[e]] == 1)) { startEdge = e; break; }
            if (startEdge == -1) return false;

            int sn = vv[u[startEdge]] == 1 ? u[startEdge] : v[startEdge];
            int nn = u[startEdge] == sn ? v[startEdge] : u[startEdge];
            
            if (vv[nn] == 1) { AddSingleEdgeEar(startEdge, earEdges, earLengths, earCount, eu); return true; }
            return FindAndAddPathEar(n, m, u, v, startEdge, sn, nn, earEdges, earLengths, earCount, eu, vv);
        }

        private static void AddSingleEdgeEar(int e, int* earEdges, int* earLengths, int* earCount, byte* eu)
        {
            int offset = 0; for (int i = 0; i < *earCount; i++) offset += earLengths[i];
            earEdges[offset] = e; eu[e] = 1; earLengths[*earCount] = 1; (*earCount)++;
        }

        private static bool FindAndAddPathEar(int n, int m, int* u, int* v, int se, int sn, int nn, int* earEdges, int* earLengths, int* earCount, byte* eu, byte* vv)
        {
            int* q = stackalloc int[n], bp = stackalloc int[n], bpe = stackalloc int[n];
            byte* tv = stackalloc byte[n];
            for (int i = 0; i < n; i++) { bp[i] = bpe[i] = -1; tv[i] = 0; }
            int qh = 0, qt = 0; tv[nn] = 1; q[qt++] = nn;
            int target = -1;
            while (qh < qt)
            {
                int c = q[qh++]; if (vv[c] == 1) { target = c; break; }
                for (int e = 0; e < m; e++)
                {
                    if (eu[e] == 1 || e == se) continue;
                    if (u[e] != c && v[e] != c) continue;
                    int nbr = u[e] == c ? v[e] : u[e];
                    if (tv[nbr] == 0) { tv[nbr] = 1; bp[nbr] = c; bpe[nbr] = e; q[qt++] = nbr; }
                }
            }
            if (target == -1) return false;
            int offset = 0; for (int i = 0; i < *earCount; i++) offset += earLengths[i];
            earEdges[offset++] = se; eu[se] = 1; vv[nn] = 1;
            int cur = target, pathLen = 1;
            while (cur != nn) { int e = bpe[cur]; eu[e] = 1; vv[u[e]] = vv[v[e]] = 1; cur = bp[cur]; pathLen++; }
            earLengths[*earCount] = pathLen; (*earCount)++;
            return true;
        }

        public static bool StNumbering(int n, int m, int* u, int* v, int s, int t, int* stOrder)
        {
            int* head = stackalloc int[n], to = stackalloc int[m * 2], nxt = stackalloc int[m * 2];
            BuildAdjacency(n, m, u, v, head, to, nxt);
            int* dfn = stackalloc int[n], low = stackalloc int[n], p = stackalloc int[n], order = stackalloc int[n];
            for (int i = 0; i < n; i++) dfn[i] = 0;
            int timer = 0; StDfs(s, s, head, to, nxt, dfn, low, p, order, &timer, t);
            for (int i = 0; i < n; i++) if (dfn[i] == 0) return false;

            byte* sign = stackalloc byte[n]; int* nl = stackalloc int[n], pl = stackalloc int[n];
            for (int i = 0; i < n; i++) { sign[i] = 0; nl[i] = pl[i] = -1; }
            nl[s] = t; pl[t] = s;
            sign[s] = 0; sign[t] = 1;
            for (int i = 0; i < n; i++)
            {
                int curr = order[i]; if (curr == s || curr == t) continue;
                int par = p[curr], lvn = order[low[curr] - 1];
                if (sign[lvn] == 0) { LinkBefore(par, curr, nl, pl); sign[curr] = 0; }
                else { LinkAfter(par, curr, nl, pl); sign[curr] = 1; }
            }
            return FinalizeStOrder(s, n, nl, stOrder);
        }

        private static void BuildAdjacency(int n, int m, int* u, int* v, int* head, int* to, int* nxt)
        {
            for (int i = 0; i < n; i++) head[i] = -1;
            for (int i = 0, ec = 0; i < m; i++)
            {
                to[ec] = v[i]; nxt[ec] = head[u[i]]; head[u[i]] = ec++;
                to[ec] = u[i]; nxt[ec] = head[v[i]]; head[v[i]] = ec++;
            }
        }

        private static void LinkAfter(int p, int c, int* nl, int* pl)
        {
            int nxt = nl[p]; nl[p] = c; pl[c] = p; nl[c] = nxt; if (nxt != -1) pl[nxt] = c;
        }

        private static void LinkBefore(int p, int c, int* nl, int* pl)
        {
            int prev = pl[p]; pl[p] = c; nl[c] = p; pl[c] = prev; if (prev != -1) nl[prev] = c;
        }

        private static bool FinalizeStOrder(int s, int n, int* nl, int* stOrder)
        {
            int cnt = 0, node = s; while (node != -1) { stOrder[cnt++] = node; node = nl[node]; }
            return cnt == n;
        }

        private static void StDfs(int u, int par, int* head, int* to, int* next, int* dfn, int* low, int* p, int* order, int* timer, int t)
        {
            dfn[u] = low[u] = ++(*timer); p[u] = par; order[*timer - 1] = u;
            if (u == p[u])
            {
                for (int e = head[u]; e != -1; e = next[e])
                    if (to[e] == t && dfn[to[e]] == 0) { StDfs(to[e], u, head, to, next, dfn, low, p, order, timer, t); low[u] = Math.Min(low[u], low[to[e]]); }
            }
            for (int e = head[u]; e != -1; e = next[e])
            {
                int v = to[e]; if ((u == p[u] && v == t) || v == par) continue;
                if (dfn[v] == 0) { StDfs(v, u, head, to, next, dfn, low, p, order, timer, t); low[u] = Math.Min(low[u], low[v]); }
                else low[u] = Math.Min(low[u], dfn[v]);
            }
        }

        public static bool PlanarEmbedding(int n, int m, int* u, int* v, int* embeddingHead, int* embeddingNext, int* embeddingTo)
        {
            int* x = stackalloc int[n], y = stackalloc int[n];
            if (!PlaceVertex(0, n, m, u, v, x, y, n * 2 + 1, n * 2 + 1)) return false;
            BuildEmbeddingFromCoords(n, m, u, v, x, y, embeddingHead, embeddingNext, embeddingTo);
            return true;
        }

        private static void BuildEmbeddingFromCoords(int n, int m, int* u, int* v, int* x, int* y, int* eh, int* en, int* et)
        {
            for (int i = 0; i < n; i++) eh[i] = -1;
            int eIdx = 0;
            for (int i = 0; i < n; i++)
            {
                int deg = 0;
                int* nbrs = stackalloc int[n];
                for (int e = 0; e < m; e++)
                {
                    if (u[e] == i) nbrs[deg++] = v[e];
                    else if (v[e] == i) nbrs[deg++] = u[e];
                }
                SortNeighborsByAngle(i, nbrs, deg, x, y);
                for (int k = 0; k < deg; k++) { et[eIdx] = nbrs[k]; en[eIdx] = eh[i]; eh[i] = eIdx++; }
            }
        }

        private static void SortNeighborsByAngle(int i, int* nbrs, int deg, int* x, int* y)
        {
            double* angles = stackalloc double[deg];
            for (int k = 0; k < deg; k++) angles[k] = Math.Atan2(y[nbrs[k]] - y[i], x[nbrs[k]] - x[i]);
            for (int k = (deg >> 1) - 1; k >= 0; k--) SiftDownAngleNbr(angles, nbrs, k, deg);
            for (int end = deg - 1; end > 0; end--)
            {
                double ta = angles[0]; angles[0] = angles[end]; angles[end] = ta;
                int tn = nbrs[0]; nbrs[0] = nbrs[end]; nbrs[end] = tn;
                SiftDownAngleNbr(angles, nbrs, 0, end);
            }
        }

        private static void SiftDownAngleNbr(double* angles, int* nbrs, int i, int n)
        {
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, m = i;
                if (l < n && angles[l] > angles[m]) m = l;
                if (r < n && angles[r] > angles[m]) m = r;
                if (m == i) break;
                double ta = angles[i]; angles[i] = angles[m]; angles[m] = ta;
                int tn = nbrs[i]; nbrs[i] = nbrs[m]; nbrs[m] = tn;
                i = m;
            }
        }

        private static bool PlaceVertex(int idx, int n, int m, int* u, int* v, int* x, int* y, int w, int h)
        {
            if (idx == n) return true;
            for (int cx = 0; cx < w; cx++)
            for (int cy = 0; cy < h; cy++)
            {
                if (IsPointTaken(idx, cx, cy, x, y)) continue;
                x[idx] = cx; y[idx] = cy;
                if (ValidatePlacement(idx, m, u, v, x, y))
                    if (PlaceVertex(idx + 1, n, m, u, v, x, y, w, h)) return true;
            }
            return false;
        }

        private static bool IsPointTaken(int idx, int cx, int cy, int* x, int* y)
        {
            for (int j = 0; j < idx; j++) if (x[j] == cx && y[j] == cy) return true;
            return false;
        }

        private static bool ValidatePlacement(int maxIdx, int m, int* u, int* v, int* x, int* y)
        {
            for (int e = 0; e < m; e++)
            {
                int uNode = u[e], vNode = v[e];
                if (uNode <= maxIdx && vNode <= maxIdx)
                {
                    for (int i = 0; i <= maxIdx; i++)
                        if (i != uNode && i != vNode && Math.Abs(CrossProduct(x[uNode], y[uNode], x[vNode], y[vNode], x[i], y[i])) < 1e-9) return false;
                }
            }
            for (int e1 = 0; e1 < m; e1++)
            {
                int u1 = u[e1], v1 = v[e1];
                if (u1 <= maxIdx && v1 <= maxIdx)
                    for (int e2 = e1 + 1; e2 < m; e2++)
                    {
                        int u2 = u[e2], v2 = v[e2];
                        if (u2 <= maxIdx && v2 <= maxIdx && u1 != u2 && u1 != v2 && v1 != u2 && v1 != v2)
                            if (SegmentsIntersect(x[u1], y[u1], x[v1], y[v1], x[u2], y[u2], x[v2], y[v2])) return false;
                    }
            }
            return true;
        }

        private static double CrossProduct(double ax, double ay, double bx, double by, double cx, double cy) => (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);

        private static bool SegmentsIntersect(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
        {
            double cp1 = CrossProduct(ax, ay, bx, by, cx, cy), cp2 = CrossProduct(ax, ay, bx, by, dx, dy);
            double cp3 = CrossProduct(cx, cy, dx, dy, ax, ay), cp4 = CrossProduct(cx, cy, dx, dy, bx, by);
            return ((cp1 > 0 && cp2 < 0) || (cp1 < 0 && cp2 > 0)) && ((cp3 > 0 && cp4 < 0) || (cp3 < 0 && cp4 > 0));
        }

        public static bool PlanarDualBuild(int n, int m, int* u, int* v, int* embeddingHead, int* embeddingNext, int* embeddingTo, int* dualN, int* dualM, int* dualU, int* dualV, int* faceSizes)
        {
            byte* hvv = stackalloc byte[2 * m]; int* hf = stackalloc int[2 * m];
            for (int i = 0; i < 2 * m; i++) { hvv[i] = 0; hf[i] = -1; }
            int fc = 0;
            for (int i = 0; i < 2 * m; i++)
                if (hvv[i] == 0)
                {
                    int ce = i, fs = 0;
                    while (hvv[ce] == 0)
                    {
                        hvv[ce] = 1; hf[ce] = fc; fs++;
                        ce = GetNextHalfEdge(n, m, u, v, embeddingHead, embeddingNext, embeddingTo, ce);
                        if (ce == -1) return false;
                    }
                    faceSizes[fc++] = fs;
                }
            *dualN = fc; int dm = 0;
            for (int e = 0; e < m; e++)
            {
                int f1 = hf[2 * e], f2 = hf[2 * e + 1];
                if (f1 != -1 && f2 != -1) { dualU[dm] = f1; dualV[dm++] = f2; }
            }
            *dualM = dm; return true;
        }

        private static int GetNextHalfEdge(int n, int m, int* u, int* v, int* eh, int* en, int* et, int ce)
        {
            int src = ce % 2 == 0 ? u[ce / 2] : v[ce / 2], dst = ce % 2 == 0 ? v[ce / 2] : u[ce / 2];
            int pni = -1, cur = eh[dst];
            while (cur != -1) { if (et[cur] == src) { pni = cur; break; } cur = en[cur]; }
            if (pni == -1) return -1;
            int nni = en[pni] == -1 ? eh[dst] : en[pni], nn = et[nni];
            for (int e = 0; e < m; e++)
            {
                if (u[e] == dst && v[e] == nn) return e * 2;
                if (v[e] == dst && u[e] == nn) return e * 2 + 1;
            }
            return -1;
        }

        public static void PlanarShortestPath(int n, int m, int* u, int* v, long* w, int src, int dest, long* dist)
        {
            byte* vis = stackalloc byte[n];
            for (int i = 0; i < n; i++) { dist[i] = 999999999999999; vis[i] = 0; }
            dist[src] = 0;
            for (int it = 0; it < n; it++)
            {
                int nn = -1; long md = 999999999999999;
                for (int i = 0; i < n; i++) if (vis[i] == 0 && dist[i] < md) { md = dist[i]; nn = i; }
                if (nn == -1 || nn == dest) break;
                vis[nn] = 1;
                for (int e = 0; e < m; e++)
                {
                    int nbr = -1;
                    if (u[e] == nn) nbr = v[e]; else if (v[e] == nn) nbr = u[e];
                    if (nbr != -1 && vis[nbr] == 0 && dist[nn] + w[e] < dist[nbr]) dist[nbr] = dist[nn] + w[e];
                }
            }
        }

        public static bool PlanarSeparator(int n, int m, int* u, int* v, int* separator, int* separatorCount, int* partA, int* partACount, int* partB, int* partBCount)
        {
            int* ass = stackalloc int[n], bass = stackalloc int[n];
            for (int i = 0; i < n; i++) ass[i] = bass[i] = -1;
            int bss = n + 1;
            SeparatorBacktrack(0, n, m, u, v, ass, bass, &bss, 0, 0, 0);
            if (bss > n) return false;
            *separatorCount = *partACount = *partBCount = 0;
            for (int i = 0; i < n; i++)
                if (bass[i] == 2) separator[(*separatorCount)++] = i;
                else if (bass[i] == 0) partA[(*partACount)++] = i;
                else if (bass[i] == 1) partB[(*partBCount)++] = i;
            return true;
        }

        private static void SeparatorBacktrack(int idx, int n, int m, int* u, int* v, int* ass, int* bass, int* bss, int ca, int cb, int cc)
        {
            if (cc >= *bss || ca > (2 * n) / 3 || cb > (2 * n) / 3) return;
            if (idx == n)
            {
                for (int e = 0; e < m; e++) if ((ass[u[e]] == 0 && ass[v[e]] == 1) || (ass[u[e]] == 1 && ass[v[e]] == 0)) return;
                *bss = cc; for (int i = 0; i < n; i++) bass[i] = ass[i];
                return;
            }
            ass[idx] = 0; SeparatorBacktrack(idx + 1, n, m, u, v, ass, bass, bss, ca + 1, cb, cc);
            ass[idx] = 1; SeparatorBacktrack(idx + 1, n, m, u, v, ass, bass, bss, ca, cb + 1, cc);
            ass[idx] = 2; SeparatorBacktrack(idx + 1, n, m, u, v, ass, bass, bss, ca, cb, cc + 1);
            ass[idx] = -1;
        }

        public static long PlanarMaxFlow(int n, int s, int t, int* head, int* to, int* next, int* cap, int m)
        {
            int* tc = stackalloc int[m * 2 + 2]; for (int i = 0; i < m * 2 + 2; i++) tc[i] = cap[i];
            int* tf = stackalloc int[m * 2 + 2]; return DinicMaxFlow.Run(n, s, t, head, to, next, tc, tf);
        }

        public static long PlanarMinCut(int n, int s, int t, int* head, int* to, int* next, int* cap, int m, int* cutEdges, int* cutCount)
        {
            int* tc = stackalloc int[m * 2 + 2]; for (int i = 0; i < m * 2 + 2; i++) tc[i] = cap[i];
            int* flow = stackalloc int[m * 2 + 2]; for (int i = 0; i < m * 2 + 2; i++) flow[i] = 0;
            long mf = DinicMaxFlow.Run(n, s, t, head, to, next, tc, flow);
            byte* vis = stackalloc byte[n]; for (int i = 0; i < n; i++) vis[i] = 0;
            int* q = stackalloc int[n]; int qh = 0, qt = 0; vis[s] = 1; q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e]) if (vis[to[e]] == 0 && tc[e] - flow[e] > 0) { vis[to[e]] = 1; q[qt++] = to[e]; }
            }
            *cutCount = 0; byte* ec = stackalloc byte[m + 1]; for (int i = 0; i <= m; i++) ec[i] = 0;
            for (int un = 0; un < n; un++) if (vis[un] == 1)
                for (int e = head[un]; e != 0; e = next[e]) if (vis[to[e]] == 0) { int oe = e / 2; if (ec[oe] == 0) { ec[oe] = 1; cutEdges[(*cutCount)++] = oe; } }
            return mf;
        }

        public static void FacePotentialSolve(int nf, int nde, int* du, int* dv, long* dw, double* pots)
        {
            byte* vis = stackalloc byte[nf]; for (int i = 0; i < nf; i++) { pots[i] = 99999999999999; vis[i] = 0; }
            pots[0] = 0;
            for (int it = 0; it < nf; it++)
            {
                int nn = -1; double md = 99999999999999;
                for (int i = 0; i < nf; i++) if (vis[i] == 0 && pots[i] < md) { md = pots[i]; nn = i; }
                if (nn == -1) break;
                vis[nn] = 1;
                for (int e = 0; e < nde; e++)
                {
                    int nbr = du[e] == nn ? dv[e] : (dv[e] == nn ? du[e] : -1);
                    if (nbr != -1 && vis[nbr] == 0 && pots[nn] + dw[e] < pots[nbr]) pots[nbr] = pots[nn] + dw[e];
                }
            }
        }

        public static bool KuratowskiSubgraph(int n, int m, int* u, int* v, int* ku, int* kv, int* kc)
        {
            *kc = 0; byte* ea = stackalloc byte[m]; for (int i = 0; i < m; i++) ea[i] = 0;
            bool found = false; SearchKuratowski(0, 0, n, m, u, v, ea, ref found, ku, kv, kc);
            return found;
        }

        private static void SearchKuratowski(int ei, int ac, int n, int m, int* u, int* v, byte* ea, ref bool found, int* ku, int* kv, int* kc)
        {
            if (found) return;
            if (ac >= 9 && IsKuratowskiSubdivision(n, m, u, v, ea))
            {
                found = true; for (int i = 0; i < m; i++) if (ea[i] == 1) { ku[*kc] = u[i]; kv[*kc] = v[i]; (*kc)++; }
                return;
            }
            if (ei == m) return;
            ea[ei] = 1; SearchKuratowski(ei + 1, ac + 1, n, m, u, v, ea, ref found, ku, kv, kc);
            if (found) return;
            ea[ei] = 0; SearchKuratowski(ei + 1, ac, n, m, u, v, ea, ref found, ku, kv, kc);
        }

        private static bool IsKuratowskiSubdivision(int n, int m, int* u, int* v, byte* ea)
        {
            int activeM = 0; int* tu = stackalloc int[m], tv = stackalloc int[m];
            for (int i = 0; i < m; i++) if (ea[i] == 1) { tu[activeM] = u[i]; tv[activeM++] = v[i]; }
            int tempM = activeM; byte* nr = stackalloc byte[n]; for (int i = 0; i < n; i++) nr[i] = 0;
            while (true)
            {
                int* deg = stackalloc int[n]; for (int i = 0; i < n; i++) deg[i] = 0;
                for (int i = 0; i < tempM; i++) { deg[tu[i]]++; deg[tv[i]]++; }
                int d2n = -1; for (int i = 0; i < n; i++) if (nr[i] == 0 && deg[i] == 2) { d2n = i; break; }
                if (d2n == -1) break;
                int n1 = -1, n2 = -1, e1 = -1, e2 = -1;
                for (int i = 0; i < tempM; i++) if (tu[i] == d2n || tv[i] == d2n)
                    { if (n1 == -1) { n1 = tu[i] == d2n ? tv[i] : tu[i]; e1 = i; } else { n2 = tu[i] == d2n ? tv[i] : tu[i]; e2 = i; } }
                if (n1 == -1 || n2 == -1) break;
                nr[d2n] = 1; tu[e1] = n1; tv[e1] = n2; tu[e2] = tu[tempM - 1]; tv[e2] = tv[tempM - 1]; tempM--;
            }
            int rc = 0; int* rn = stackalloc int[n], degF = stackalloc int[n]; for (int i = 0; i < n; i++) degF[i] = 0;
            for (int i = 0; i < tempM; i++) { degF[tu[i]]++; degF[tv[i]]++; }
            for (int i = 0; i < n; i++) if (degF[i] > 0) rn[rc++] = i;
            if (rc == 5) { for (int i = 0; i < 5; i++) if (degF[rn[i]] != 4) return false; return true; }
            if (rc == 6) { for (int i = 0; i < 6; i++) if (degF[rn[i]] != 3) return false; return IsK33(n, tempM, tu, tv, rn); }
            return false;
        }

        private static bool IsK33(int n, int m, int* tu, int* tv, int* rn)
        {
            int* col = stackalloc int[n]; for (int i = 0; i < n; i++) col[i] = -1;
            int* q = stackalloc int[n]; int qh = 0, qt = 0; col[rn[0]] = 0; q[qt++] = rn[0];
            while (qh < qt)
            {
                int c = q[qh++];
                for (int i = 0; i < m; i++)
                {
                    int nbr = tu[i] == c ? tv[i] : (tv[i] == c ? tu[i] : -1);
                    if (nbr != -1) { if (col[nbr] == -1) { col[nbr] = 1 - col[c]; q[qt++] = nbr; } else if (col[nbr] == col[c]) return false; }
                }
            }
            int c0 = 0, c1 = 0; for (int i = 0; i < 6; i++) { if (col[rn[i]] == 0) c0++; else if (col[rn[i]] == 1) c1++; }
            return c0 == 3 && c1 == 3;
        }

        public static bool OuterplanarCheck(int n, int m, int* u, int* v)
        {
            byte* ea = stackalloc byte[m]; for (int i = 0; i < m; i++) ea[i] = 0;
            bool hf = false; SearchOuterplanar(0, 0, n, m, u, v, ea, ref hf);
            return !hf;
        }

        private static void SearchOuterplanar(int ei, int ac, int n, int m, int* u, int* v, byte* ea, ref bool hf)
        {
            if (hf) return;
            if (ac >= 6 && IsOuterplanarForbiddenSubdivision(n, m, u, v, ea)) { hf = true; return; }
            if (ei == m) return;
            ea[ei] = 1; SearchOuterplanar(ei + 1, ac + 1, n, m, u, v, ea, ref hf);
            if (hf) return;
            ea[ei] = 0; SearchOuterplanar(ei + 1, ac, n, m, u, v, ea, ref hf);
        }

        private static bool IsOuterplanarForbiddenSubdivision(int n, int m, int* u, int* v, byte* ea)
        {
            int am = 0; int* tu = stackalloc int[m], tv = stackalloc int[m];
            for (int i = 0; i < m; i++) if (ea[i] == 1) { tu[am] = u[i]; tv[am++] = v[i]; }
            int tm = am; byte* nr = stackalloc byte[n]; for (int i = 0; i < n; i++) nr[i] = 0;
            while (true)
            {
                int* deg = stackalloc int[n]; for (int i = 0; i < n; i++) deg[i] = 0;
                for (int i = 0; i < tm; i++) { deg[tu[i]]++; deg[tv[i]]++; }
                int d2n = -1; for (int i = 0; i < n; i++) if (nr[i] == 0 && deg[i] == 2) { d2n = i; break; }
                if (d2n == -1) break;
                int n1 = -1, n2 = -1, e1 = -1, e2 = -1;
                for (int i = 0; i < tm; i++) if (tu[i] == d2n || tv[i] == d2n)
                    { if (n1 == -1) { n1 = tu[i] == d2n ? tv[i] : tu[i]; e1 = i; } else { n2 = tu[i] == d2n ? tv[i] : tu[i]; e2 = i; } }
                if (n1 == -1 || n2 == -1) break;
                nr[d2n] = 1; tu[e1] = n1; tv[e1] = n2; tu[e2] = tu[tm - 1]; tv[e2] = tv[tm - 1]; tm--;
            }
            int rc = 0; int* rn = stackalloc int[n], degF = stackalloc int[n]; for (int i = 0; i < n; i++) degF[i] = 0;
            for (int i = 0; i < tm; i++) { degF[tu[i]]++; degF[tv[i]]++; }
            for (int i = 0; i < n; i++) if (degF[i] > 0) rn[rc++] = i;
            if (rc == 4) { for (int i = 0; i < 4; i++) if (degF[rn[i]] != 3) return false; return true; }
            if (rc == 5) { int c3 = 0, c2 = 0; for (int i = 0; i < 5; i++) { if (degF[rn[i]] == 3) c3++; else if (degF[rn[i]] == 2) c2++; } return c3 == 2 && c2 == 3; }
            return false;
        }

        public static bool SeriesParallelDecompose(int n, int m, int* u, int* v, int s, int t)
        {
            int tm = m; int* tu = stackalloc int[m], tv = stackalloc int[m];
            for (int i = 0; i < m; i++) { tu[i] = u[i]; tv[i] = v[i]; }
            byte* nr = stackalloc byte[n]; for (int i = 0; i < n; i++) nr[i] = 0;
            while (true)
            {
                if (ReduceParallel(ref tm, tu, tv)) continue;
                if (ReduceSeries(n, ref tm, tu, tv, s, t, nr)) continue;
                break;
            }
            return tm == 1 && ((tu[0] == s && tv[0] == t) || (tu[0] == t && tv[0] == s));
        }

        private static bool ReduceParallel(ref int tm, int* tu, int* tv)
        {
            for (int i = 0; i < tm; i++)
            for (int j = i + 1; j < tm; j++)
                if ((tu[i] == tu[j] && tv[i] == tv[j]) || (tu[i] == tv[j] && tv[i] == tu[j]))
                {
                    tu[j] = tu[tm - 1]; tv[j] = tv[tm - 1]; tm--; return true;
                }
            return false;
        }

        private static bool ReduceSeries(int n, ref int tm, int* tu, int* tv, int s, int t, byte* nr)
        {
            int* deg = stackalloc int[n]; for (int i = 0; i < n; i++) deg[i] = 0;
            for (int i = 0; i < tm; i++) { deg[tu[i]]++; deg[tv[i]]++; }
            int d2n = -1; for (int i = 0; i < n; i++) if (i != s && i != t && nr[i] == 0 && deg[i] == 2) { d2n = i; break; }
            if (d2n == -1) return false;
            int n1 = -1, n2 = -1, e1 = -1, e2 = -1;
            for (int i = 0; i < tm; i++) if (tu[i] == d2n || tv[i] == d2n)
                { if (n1 == -1) { n1 = tu[i] == d2n ? tv[i] : tu[i]; e1 = i; } else { n2 = tu[i] == d2n ? tv[i] : tu[i]; e2 = i; } }
            if (n1 == -1 || n2 == -1) return false;
            nr[d2n] = 1; tu[e1] = n1; tv[e1] = n2; tu[e2] = tu[tm - 1]; tv[e2] = tv[tm - 1]; tm--;
            return true;
        }

        public static int TriconnectedComponents(int n, int m, int* u, int* v, int* ct)
        {
            int cc = 0; byte* ea = stackalloc byte[m]; for (int i = 0; i < m; i++) ea[i] = 1;
            DecomposeTriconnected(n, m, u, v, ea, &cc, ct); return cc;
        }

        public static int SpqrTreeBuild(int n, int m, int* u, int* v, int* ct) => TriconnectedComponents(n, m, u, v, ct);

        private static void DecomposeTriconnected(int n, int m, int* u, int* v, byte* ea, int* cc, int* ct)
        {
            int sx = -1, sy = -1;
            if (FindSeparationPair(n, m, u, v, ea, &sx, &sy))
            {
                int* ci = stackalloc int[n]; for (int i = 0; i < n; i++) ci[i] = -1;
                int ncc = 0; for (int i = 0; i < n; i++) if (i != sx && i != sy && ci[i] == -1) { MarkComp(i, sx, sy, n, m, u, v, ea, ci, ncc++); }
                for (int c = 0; c < ncc; c++)
                {
                    byte* sea = stackalloc byte[m]; for (int i = 0; i < m; i++) sea[i] = 0;
                    for (int i = 0; i < m; i++) if (ea[i] == 1 && ((u[i] != sx && u[i] != sy && ci[u[i]] == c) || (v[i] != sx && v[i] != sy && ci[v[i]] == c))) sea[i] = 1;
                    DecomposeTriconnected(n, m, u, v, sea, cc, ct);
                }
            }
            else
            {
                int ae = 0; for (int i = 0; i < m; i++) if (ea[i] == 1) ae++;
                if (ae > 0) { ct[*cc] = ae == 3 ? 0 : 2; (*cc)++; }
            }
        }

        private static void MarkComp(int start, int sx, int sy, int n, int m, int* u, int* v, byte* ea, int* ci, int c)
        {
            int* q = stackalloc int[n]; int qh = 0, qt = 0; ci[start] = c; q[qt++] = start;
            while (qh < qt)
            {
                int cur = q[qh++];
                for (int e = 0; e < m; e++)
                    if (ea[e] == 1)
                    {
                        int nbr = u[e] == cur ? v[e] : (v[e] == cur ? u[e] : -1);
                        if (nbr != -1 && nbr != sx && nbr != sy && ci[nbr] == -1) { ci[nbr] = c; q[qt++] = nbr; }
                    }
            }
        }

        private static bool FindSeparationPair(int n, int m, int* u, int* v, byte* ea, int* sx, int* sy)
        {
            for (int x = 0; x < n; x++)
            for (int y = x + 1; y < n; y++)
            {
                byte* vis = stackalloc byte[n]; for (int i = 0; i < n; i++) vis[i] = 0;
                vis[x] = vis[y] = 1;
                int start = -1;
                for (int i = 0; i < n; i++) if (i != x && i != y) { for (int e = 0; e < m; e++) if (ea[e] == 1 && (u[e] == i || v[e] == i)) { start = i; break; } if (start != -1) break; }
                if (start == -1) continue;
                int* q = stackalloc int[n]; int qh = 0, qt = 0; vis[start] = 1; q[qt++] = start;
                while (qh < qt)
                {
                    int cur = q[qh++];
                    for (int e = 0; e < m; e++) if (ea[e] == 1) { int nbr = u[e] == cur ? v[e] : (v[e] == cur ? u[e] : -1); if (nbr != -1 && vis[nbr] == 0) { vis[nbr] = 1; q[qt++] = nbr; } }
                }
                for (int i = 0; i < n; i++) if (i != x && i != y && vis[i] == 0) { for (int e = 0; e < m; e++) if (ea[e] == 1 && (u[e] == i || v[e] == i)) { *sx = x; *sy = y; return true; } }
            }
            return false;
        }

        public static void MaximumPlanarMatching(int n, int m, int* u, int* v, int* mu, int* mv, int* mc)
        {
            int* match = stackalloc int[n]; GeneralMatchingBlossom.Run(n, m, u, v, match);
            *mc = 0; for (int i = 0; i < n; i++) if (match[i] != -1 && i < match[i]) { mu[*mc] = i; mv[*mc] = match[i]; (*mc)++; }
        }
    }
}