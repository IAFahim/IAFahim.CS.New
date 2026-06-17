namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Tournament
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TournamentHamiltonianPath(int n, byte* adj, int* path)
        {
            for (int i = 0; i < n; i++) path[i] = i;
            for (int i = 1; i < n; i++)
            {
                int val = path[i], j = i - 1;
                while (j >= 0 && adj[val * n + path[j]] == 1) { path[j + 1] = path[j]; j--; }
                path[j + 1] = val;
            }
        }

        public static bool TournamentHamiltonianCycle(int n, byte* adj, int* cycle)
        {
            if (n < 3) return n == 1 ? (cycle[0] = 0) == 0 : false;
            int* path = stackalloc int[n]; TournamentHamiltonianPath(n, adj, path);

            int firstIdx = FindFirstEdgeBack(n, adj, path);
            if (firstIdx == -1) return false;

            int cycleLen = BuildInitialCycle(n, path, firstIdx, cycle);
            byte* inCycle = stackalloc byte[n]; MarkInCycle(n, cycle, cycleLen, inCycle);

            for (int i = 0; i < n; i++)
            {
                if (inCycle[i] == 1) continue;
                if (!TryInsertIntoCycle(n, i, adj, cycle, ref cycleLen, inCycle)) return false;
            }
            return cycleLen == n;
        }

        private static int FindFirstEdgeBack(int n, byte* adj, int* path)
        {
            int last = path[n - 1];
            for (int i = 0; i < n - 1; i++)
                if (adj[last * n + path[i]] == 1) return i;
            return -1;
        }

        private static int BuildInitialCycle(int n, int* path, int firstIdx, int* cycle)
        {
            int len = 0;
            for (int i = firstIdx; i < n; i++) cycle[len++] = path[i];
            return len;
        }

        private static void MarkInCycle(int n, int* cycle, int len, byte* inCycle)
        {
            for (int i = 0; i < n; i++) inCycle[i] = 0;
            for (int i = 0; i < len; i++) inCycle[cycle[i]] = 1;
        }

        private static bool TryInsertIntoCycle(int n, int u, byte* adj, int* cycle, ref int len, byte* inCycle)
        {
            int pos = -1;
            for (int j = 0; j < len; j++)
                if (adj[cycle[j] * n + u] == 1 && adj[u * n + cycle[(j + 1) % len]] == 1) { pos = j + 1; break; }
            if (pos == -1) return false;
            for (int j = len; j > pos; j--) cycle[j] = cycle[j - 1];
            cycle[pos] = u; len++; inCycle[u] = 1; return true;
        }

        public static void TournamentMedianOrder(int n, byte* adj, int* bestOrder)
        {
            int* cur = stackalloc int[n], best = stackalloc int[n];
            byte* used = stackalloc byte[n]; int maxS = -1;
            for (int i = 0; i < n; i++) { cur[i] = best[i] = i; used[i] = 0; }
            MedianOrderBacktrack(0, n, adj, cur, used, best, &maxS);
            for (int i = 0; i < n; i++) bestOrder[i] = best[i];
        }

        private static void MedianOrderBacktrack(int step, int n, byte* adj, int* cur, byte* used, int* best, int* maxS)
        {
            if (step == n) { UpdateBestOrder(n, adj, cur, best, maxS); return; }
            for (int i = 0; i < n; i++)
                if (used[i] == 0) { used[i] = 1; cur[step] = i; MedianOrderBacktrack(step + 1, n, adj, cur, used, best, maxS); used[i] = 0; }
        }

        private static void UpdateBestOrder(int n, byte* adj, int* cur, int* best, int* maxS)
        {
            int s = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++) if (adj[cur[i] * n + cur[j]] == 1) s++;
            if (s > *maxS) { *maxS = s; for (int i = 0; i < n; i++) best[i] = cur[i]; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TournamentKingFind(int n, byte* adj)
        {
            for (int u = 0; u < n; u++)
                if (IsKing(n, u, adj)) return u;
            return 0;
        }

        private static bool IsKing(int n, int u, byte* adj)
        {
            for (int v = 0; v < n; v++)
            {
                if (u == v || adj[u * n + v] == 1) continue;
                bool reach2 = false;
                for (int w = 0; w < n; w++) if (adj[u * n + w] == 1 && adj[w * n + v] == 1) { reach2 = true; break; }
                if (!reach2) return false;
            }
            return true;
        }

        public static bool EulerianOrientation(int n, int m, int* head, int* next, int* to, int* eu, int* ev, int* ou, int* ov)
        {
            if (!CheckEulerianPossible(n, eu, ev, m)) return false;
            byte* ve = stackalloc byte[m]; for (int i = 0; i < m; i++) ve[i] = 0;
            int* ce = stackalloc int[n]; for (int i = 0; i < n; i++) ce[i] = head[i];
            for (int s = 0; s < n; s++)
                if (ce[s] != -1) ProcessEulerianCircuit(s, m, ce, next, to, ve, ou, ov);
            return true;
        }

        private static bool CheckEulerianPossible(int n, int* eu, int* ev, int m)
        {
            int* deg = stackalloc int[n]; for (int i = 0; i < n; i++) deg[i] = 0;
            for (int i = 0; i < m; i++) { deg[eu[i]]++; deg[ev[i]]++; }
            for (int i = 0; i < n; i++) if (deg[i] % 2 != 0) return false;
            return true;
        }

        private static void ProcessEulerianCircuit(int s, int m, int* ce, int* next, int* to, byte* ve, int* ou, int* ov)
        {
            int* stack = stackalloc int[m + 2];
            int ss = 0; stack[ss++] = s;
            while (ss > 0)
            {
                int u = stack[ss - 1];
                int e = ce[u];
                while (e != -1 && ve[e / 2] == 1) e = next[e];
                ce[u] = e;
                if (e != -1) { int v = to[e]; ve[e / 2] = 1; ou[e / 2] = u; ov[e / 2] = v; stack[ss++] = v; ce[u] = next[e]; }
                else ss--;
            }
        }

        public static bool StrongOrientation(int n, int m, int* head, int* next, int* to, int* eu, int* ev, int* ou, int* ov)
        {
            byte* vis = stackalloc byte[n], ue = stackalloc byte[m];
            int* p = stackalloc int[n];
            for (int i = 0; i < n; i++) { vis[i] = 0; p[i] = -1; }
            for (int i = 0; i < m; i++) ue[i] = 0;
            StrongDfs(0, head, next, to, vis, p, ou, ov, ue);
            return CheckStronglyConnected(n, m, ou, ov);
        }

        private static void StrongDfs(int u, int* head, int* next, int* to, byte* vis, int* p, int* ou, int* ov, byte* ue)
        {
            vis[u] = 1;
            for (int e = head[u]; e != -1; e = next[e])
            {
                int ei = e / 2; if (ue[ei] == 1) continue;
                int v = to[e];
                ou[ei] = u; ov[ei] = v; ue[ei] = 1;
                if (vis[v] == 0) { p[v] = u; StrongDfs(v, head, next, to, vis, p, ou, ov, ue); }
            }
        }

        private static bool CheckStronglyConnected(int n, int m, int* ou, int* ov)
        {
            int* h = stackalloc int[n], nxt = stackalloc int[m], t = stackalloc int[m];
            int* rh = stackalloc int[n], rnxt = stackalloc int[m], rt = stackalloc int[m];
            for (int i = 0; i < n; i++) h[i] = rh[i] = -1;
            for (int i = 0; i < m; i++) { t[i] = ov[i]; nxt[i] = h[ou[i]]; h[ou[i]] = i; rt[i] = ou[i]; rnxt[i] = rh[ov[i]]; rh[ov[i]] = i; }
            byte* v1 = stackalloc byte[n], v2 = stackalloc byte[n];
            int c1 = 0, c2 = 0;
            for (int i = 0; i < n; i++) v1[i] = v2[i] = 0;
            DfsReach(0, h, nxt, t, v1, &c1); DfsReach(0, rh, rnxt, rt, v2, &c2);
            return c1 == n && c2 == n;
        }

        private static void DfsReach(int u, int* h, int* nxt, int* t, byte* vis, int* count)
        {
            vis[u] = 1; (*count)++;
            for (int e = h[u]; e != -1; e = nxt[e]) if (vis[t[e]] == 0) DfsReach(t[e], h, nxt, t, vis, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OrientEdgesAcyclic(int m, int* eu, int* ev, int* ou, int* ov)
        {
            for (int i = 0; i < m; i++) { if (eu[i] < ev[i]) { ou[i] = eu[i]; ov[i] = ev[i]; } else { ou[i] = ev[i]; ov[i] = eu[i]; } }
        }

        public static int FeedbackArcTournament(int n, byte* adj, int* ru, int* rv, int* rc)
        {
            int* ord = stackalloc int[n], bord = stackalloc int[n];
            for (int i = 0; i < n; i++) ord[i] = bord[i] = i;
            byte* used = stackalloc byte[n]; for (int i = 0; i < n; i++) used[i] = 0;
            int minR = 999999; FeedbackArcBacktrack(0, n, adj, ord, used, bord, &minR);
            *rc = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if (adj[bord[j] * n + bord[i]] == 1) { ru[*rc] = bord[j]; rv[*rc] = bord[i]; (*rc)++; }
            return minR;
        }

        private static void FeedbackArcBacktrack(int step, int n, byte* adj, int* ord, byte* used, int* bord, int* minR)
        {
            if (step == n) { UpdateBestFeedbackOrder(n, adj, ord, bord, minR); return; }
            for (int i = 0; i < n; i++)
                if (used[i] == 0) { used[i] = 1; ord[step] = i; FeedbackArcBacktrack(step + 1, n, adj, ord, used, bord, minR); used[i] = 0; }
        }

        private static void UpdateBestFeedbackOrder(int n, byte* adj, int* ord, int* bord, int* minR)
        {
            int r = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++) if (adj[ord[j] * n + ord[i]] == 1) r++;
            if (r < *minR) { *minR = r; for (int i = 0; i < n; i++) bord[i] = ord[i]; }
        }
    }
}
