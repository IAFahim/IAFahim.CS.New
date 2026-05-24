namespace IAFahim.Graph.SpanningTrees
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumFeedbackArcSetApprox
    {
        public static int Run(int n, int m, int* eu, int* ev, bool* remove)
        {
            int* inDeg = stackalloc int[n], outDeg = stackalloc int[n];
            for (int i = 0; i < n; i++) { inDeg[i] = 0; outDeg[i] = 0; }
            for (int i = 0; i < m; i++) { outDeg[eu[i]]++; inDeg[ev[i]]++; }
            int count = 0;
            for (int i = 0; i < m; i++)
            {
                int sU = outDeg[eu[i]] - inDeg[eu[i]], sV = outDeg[ev[i]] - inDeg[ev[i]];
                if (sU < sV || (sU == sV && eu[i] >= ev[i])) { remove[i] = true; count++; } else remove[i] = false;
            }
            return count;
        }
    }

    public static unsafe class MinimumPathCoverDag
    {
        public static int Run(int n, int m, int* eu, int* ev, int* mL, int* mR)
        {
            int* h = stackalloc int[n + 1], t = stackalloc int[m + 1], nx = stackalloc int[m + 1];
            for (int i = 0; i <= n; i++) h[i] = 0;
            for (int i = 0; i < m; i++) { int u = eu[i] + 1; t[i + 1] = ev[i] + 1; nx[i + 1] = h[u]; h[u] = i + 1; }
            int* dist = stackalloc int[n + 1], q = stackalloc int[n + 1];
            for (int i = 0; i <= n; i++) { mL[i] = 0; mR[i] = 0; }
            int match = 0;
            while (TryBfs(n, h, t, nx, mL, mR, dist, q))
                for (int i = 1; i <= n; i++) if (mL[i] == 0 && TryDfs(i, h, t, nx, mL, mR, dist)) match++;
            return n - match;
        }
        private static bool TryBfs(int n, int* h, int* t, int* nx, int* mL, int* mR, int* dist, int* q)
        {
            int qh = 0, qt = 0; for (int i = 1; i <= n; i++) { if (mL[i] == 0) { dist[i] = 0; q[qt++] = i; } else dist[i] = int.MaxValue; }
            dist[0] = int.MaxValue;
            while (qh < qt) { int u = q[qh++]; if (dist[u] >= dist[0]) continue; for (int e = h[u]; e != 0; e = nx[e]) if (dist[mR[t[e]]] == int.MaxValue) { dist[mR[t[e]]] = dist[u] + 1; q[qt++] = mR[t[e]]; } }
            return dist[0] != int.MaxValue;
        }
        private static bool TryDfs(int u, int* h, int* t, int* nx, int* mL, int* mR, int* dist)
        {
            if (u == 0) return true;
            for (int e = h[u]; e != 0; e = nx[e]) if (dist[mR[t[e]]] == dist[u] + 1 && TryDfs(mR[t[e]], h, t, nx, mL, mR, dist)) { mR[t[e]] = u; mL[u] = t[e]; return true; }
            dist[u] = int.MaxValue; return false;
        }
    }

    public static unsafe class DilworthDecomposition
    {
        public static int Run(int n, int m, int* eu, int* ev, int* mL, int* mR, bool* tc)
        {
            for (int i = 0; i < n * n; i++) tc[i] = false;
            for (int i = 0; i < m; i++) tc[eu[i] * n + ev[i]] = true;
            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    if (tc[i * n + k]) for (int j = 0; j < n; j++) if (tc[k * n + j]) tc[i * n + j] = true;
            int tcE = 0; for (int i = 0; i < n * n; i++) if (tc[i]) tcE++;
            int* tceu = stackalloc int[tcE], tcev = stackalloc int[tcE];
            int idx = 0; for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) if (tc[i * n + j]) { tceu[idx] = i; tcev[idx++] = j; }
            return MinimumPathCoverDag.Run(n, tcE, tceu, tcev, mL, mR);
        }
    }

    public static unsafe class MaximumAntichain
    {
        public static int Run(int n, int m, int* eu, int* ev, bool* inAnti, bool* tc)
        {
            int* mL = stackalloc int[n + 1], mR = stackalloc int[n + 1];
            int width = DilworthDecomposition.Run(n, m, eu, ev, mL, mR, tc);
            bool* vL = stackalloc bool[n + 1], vR = stackalloc bool[n + 1];
            for (int i = 0; i <= n; i++) { vL[i] = false; vR[i] = false; }
            int* q = stackalloc int[n + 1]; int qh = 0, qt = 0;
            for (int i = 1; i <= n; i++) if (mL[i] == 0) { vL[i] = true; q[qt++] = i; }
            while (qh < qt)
            {
                int u = q[qh++];
                for (int j = 0; j < n; j++) if (tc[(u - 1) * n + j] && !vR[j + 1]) { vR[j + 1] = true; int ln = mR[j + 1]; if (ln != 0 && !vL[ln]) { vL[ln] = true; q[qt++] = ln; } }
            }
            for (int i = 0; i < n; i++) inAnti[i] = vL[i + 1] && !vR[i + 1];
            return width;
        }
    }

    public static unsafe class TransitiveReductionDag
    {
        public static int Run(int n, int m, int* eu, int* ev, bool* keep, bool* tc)
        {
            for (int i = 0; i < n * n; i++) tc[i] = false;
            for (int i = 0; i < m; i++) tc[eu[i] * n + ev[i]] = true;
            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    if (tc[i * n + k]) for (int j = 0; j < n; j++) if (tc[k * n + j]) tc[i * n + j] = true;
            int count = 0;
            for (int i = 0; i < m; i++)
            {
                bool redundant = false;
                for (int k = 0; k < n; k++) if (k != eu[i] && k != ev[i] && tc[eu[i] * n + k] && tc[k * n + ev[i]]) { redundant = true; break; }
                keep[i] = !redundant; if (!redundant) count++;
            }
            return count;
        }
    }
}
