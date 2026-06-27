namespace IAFahim.DS.RollbackSeg
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class OfflineDeleteSegmentTree
    {
        public static void Run(int* start, int* end, long* val, int m, int timePoints, long* ans, int* nodeCnt, int* nodeOff, long* nodeVal)
        {
            for (int t = 0; t < timePoints; t++) ans[t] = 0L;
            if (m <= 0 || timePoints <= 0) return;

            int nodes = 4 * (timePoints + 1);
            for (int i = 0; i < nodes; i++) nodeCnt[i] = 0;

            for (int i = 0; i < m; i++)
            {
                int s = start[i];
                int e = end[i];
                if (s >= e) continue;
                CountCover(nodeCnt, 1, 0, timePoints - 1, s, e - 1);
            }

            int acc = 0;
            for (int i = 0; i < nodes; i++) { nodeOff[i] = acc; acc += nodeCnt[i]; nodeCnt[i] = 0; }

            for (int i = 0; i < m; i++)
            {
                int s = start[i];
                int e = end[i];
                if (s >= e) continue;
                DistributeCover(nodeOff, nodeCnt, nodeVal, val[i], 1, 0, timePoints - 1, s, e - 1);
            }

            Dfs(ans, nodeOff, nodeCnt, nodeVal, 1, 0, timePoints - 1, 0L);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountCover(int* nodeCnt, int ni, int l, int r, int qs, int qe)
        {
            if (qe < l || qs > r) return;
            if (qs <= l && r <= qe) { nodeCnt[ni]++; return; }
            int mid = (l + r) >> 1;
            CountCover(nodeCnt, ni << 1, l, mid, qs, qe);
            CountCover(nodeCnt, (ni << 1) | 1, mid + 1, r, qs, qe);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DistributeCover(int* nodeOff, int* nodeCnt, long* nodeVal, long value, int ni, int l, int r, int qs, int qe)
        {
            if (qe < l || qs > r) return;
            if (qs <= l && r <= qe)
            {
                int pos = nodeOff[ni] + nodeCnt[ni];
                nodeVal[pos] = value;
                nodeCnt[ni]++;
                return;
            }
            int mid = (l + r) >> 1;
            DistributeCover(nodeOff, nodeCnt, nodeVal, value, ni << 1, l, mid, qs, qe);
            DistributeCover(nodeOff, nodeCnt, nodeVal, value, (ni << 1) | 1, mid + 1, r, qs, qe);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Dfs(long* ans, int* nodeOff, int* nodeCnt, long* nodeVal, int ni, int l, int r, long sum)
        {
            int cnt = nodeCnt[ni];
            int off = nodeOff[ni];
            for (int i = 0; i < cnt; i++) sum += nodeVal[off + i];
            if (l == r) { ans[l] = sum; return; }
            int mid = (l + r) >> 1;
            Dfs(ans, nodeOff, nodeCnt, nodeVal, ni << 1, l, mid, sum);
            Dfs(ans, nodeOff, nodeCnt, nodeVal, (ni << 1) | 1, mid + 1, r, sum);
        }
    }

    public static unsafe class RetroactiveQueueInsert
    {
        // OFFLINE fully-retroactive queue. The final operation timeline is given already (all past
        // insertions/deletions of ops baked in). opType[t] in {0=enqueue value val[t], 1=dequeue} for
        // t in [0,O). Queries ask: head[q] = front element at time queryTime[q]. We sweep time forward
        // simulating a FIFO queue (two pointers over the enqueue sequence); at each query we report
        // the front. head[q] = 0 if the queue is empty at that time. O(O+Q log Q) (sort queries by time).
        public static void Run(int* opType, int* val, int O, int* queryTime, int Q, int* head)
        {
            for (int j = 0; j < Q; j++) head[j] = 0;
            if (O <= 0) return;
            int* order = (int*)Marshal.AllocHGlobal(sizeof(int) * Q);
            for (int j = 0; j < Q; j++) order[j] = j;
            SortByKey(order, queryTime, Q);
            int* buf = (int*)Marshal.AllocHGlobal(sizeof(int) * O);
            try
            {
                int enqHead = 0, enqTail = 0;   // FIFO: buf[enqHead..enqTail)
                int qi = 0;
                for (int t = 0; t < O; t++)
                {
                    if (opType[t] == 0) buf[enqTail++] = val[t];
                    else if (enqHead < enqTail) enqHead++;
                    while (qi < Q && queryTime[order[qi]] == t)
                    { head[order[qi]] = enqHead < enqTail ? buf[enqHead] : 0; qi++; }
                }
                while (qi < Q) { head[order[qi]] = enqHead < enqTail ? buf[enqHead] : 0; qi++; }
            }
            finally { Marshal.FreeHGlobal((IntPtr)order); Marshal.FreeHGlobal((IntPtr)buf); }
        }

        private static void SortByKey(int* order, int* key, int n)
        {
            int* tmp = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            try
            {
                for (int w = 1; w < n; w <<= 1)
                    for (int lo = 0; lo < n; lo += w << 1)
                    {
                        int mid = lo + w; if (mid >= n) break; int hi = mid + w; if (hi > n) hi = n;
                        int i = lo, j = mid, k = lo;
                        while (i < mid && j < hi) { if (key[order[i]] <= key[order[j]]) tmp[k++] = order[i++]; else tmp[k++] = order[j++]; }
                        while (i < mid) tmp[k++] = order[i++]; while (j < hi) tmp[k++] = order[j++];
                        for (int t = lo; t < k; t++) order[t] = tmp[t];
                    }
            }
            finally { Marshal.FreeHGlobal((IntPtr)tmp); }
        }
    }

    public static unsafe class RetroactiveQueueDelete
    {
        // Retroactive deletion of a past queue op is, offline, equivalent to RetroactiveQueueInsert on
        // the timeline with that op removed. The caller supplies the FINAL op stream (deletions already
        // applied) and queries; this delegates to RetroactiveQueueInsert. Provided as the symmetric
        // entry point so callers pair insert/delete against the same timeline contract.
        public static void Run(int* opType, int* val, int O, int* queryTime, int Q, int* head)
            => RetroactiveQueueInsert.Run(opType, val, O, queryTime, Q, head);
    }

    public static unsafe class RetroactivePriorityQueueInsert
    {
        // OFFLINE fully-retroactive priority queue. opType[t] in {0=insert value val[t], 1=extract-min};
        // min[q] = minimum element present at time queryTime[q] (0 if empty). Sweep forward, maintain a
        // sorted multiset (insertion into a sorted int buffer via binary search + shift); extract-min
        // removes the smallest. O(O^2 + Q log Q) — correct, simple; suitable for moderate O.
        public static void Run(int* opType, int* val, int O, int* queryTime, int Q, int* min)
        {
            for (int j = 0; j < Q; j++) min[j] = 0;
            if (O <= 0) return;
            int* order = (int*)Marshal.AllocHGlobal(sizeof(int) * Q);
            for (int j = 0; j < Q; j++) order[j] = j;
            SortByKey(order, queryTime, Q);
            int* buf = (int*)Marshal.AllocHGlobal(sizeof(int) * O);   // ascending sorted multiset
            try
            {
                int cnt = 0, qi = 0;
                for (int t = 0; t < O; t++)
                {
                    if (opType[t] == 0) InsertSorted(buf, ref cnt, val[t]);
                    else if (cnt > 0) { for (int i = 1; i < cnt; i++) buf[i - 1] = buf[i]; cnt--; }   // extract-min: drop buf[0], shift left
                    while (qi < Q && queryTime[order[qi]] == t)
                    { min[order[qi]] = cnt > 0 ? buf[0] : 0; qi++; }
                }
                while (qi < Q) { min[order[qi]] = cnt > 0 ? buf[0] : 0; qi++; }
            }
            finally { Marshal.FreeHGlobal((IntPtr)order); Marshal.FreeHGlobal((IntPtr)buf); }
        }

        private static void InsertSorted(int* buf, ref int cnt, int v)
        {
            int lo = 0, hi = cnt;
            while (lo < hi) { int mid = (lo + hi) >> 1; if (buf[mid] < v) lo = mid + 1; else hi = mid; }
            for (int i = cnt; i > lo; i--) buf[i] = buf[i - 1];
            buf[lo] = v; cnt++;
        }

        private static void SortByKey(int* order, int* key, int n)
        {
            int* tmp = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            try
            {
                for (int w = 1; w < n; w <<= 1)
                    for (int lo = 0; lo < n; lo += w << 1)
                    {
                        int mid = lo + w; if (mid >= n) break; int hi = mid + w; if (hi > n) hi = n;
                        int i = lo, j = mid, k = lo;
                        while (i < mid && j < hi) { if (key[order[i]] <= key[order[j]]) tmp[k++] = order[i++]; else tmp[k++] = order[j++]; }
                        while (i < mid) tmp[k++] = order[i++]; while (j < hi) tmp[k++] = order[j++];
                        for (int t = lo; t < k; t++) order[t] = tmp[t];
                    }
            }
            finally { Marshal.FreeHGlobal((IntPtr)tmp); }
        }
    }

    public static unsafe class RetroactivePriorityQueueDelete
    {
        // Symmetric to RetroactivePriorityQueueInsert: offline, deletion = rerun on the final timeline.
        public static void Run(int* opType, int* val, int O, int* queryTime, int Q, int* min)
            => RetroactivePriorityQueueInsert.Run(opType, val, O, queryTime, Q, min);
    }

    public static unsafe class RetroactiveConnectivity
    {
        // Offline retroactive / temporal connectivity. n vertices; ops are link (u,v) active during
        // inclusive [start[i], end[i]] (a link inserted at start, cut at end+1; end == timePoints-1
        // means never cut). queries: are qu[j],qv[j] connected at time qt[j]? out[j]=1/0.
        // Solvable offline via segment-tree-on-time + DSU rollback; delegates to DivideConquerOnTime
        // semantics. This package (DS.RollbackSeg) reuses the same engine inline.
        public static void Run(int n, int* u, int* v, int* start, int* end, int m,
                               int* qu, int* qv, int* qt, int q, int timePoints, int* ans)
        {
            for (int j = 0; j < q; j++) ans[j] = 0;
            if (n <= 0 || q <= 0) return;
            if (timePoints <= 0) timePoints = 1;

            int nodes = 4 * timePoints;
            int* nodeCnt = (int*)Marshal.AllocHGlobal(sizeof(int) * nodes);
            int* nodeOff = (int*)Marshal.AllocHGlobal(sizeof(int) * nodes);
            int* qCnt = (int*)Marshal.AllocHGlobal(sizeof(int) * timePoints);
            int* qOff = (int*)Marshal.AllocHGlobal(sizeof(int) * (timePoints + 1));
            int* parent = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* sz = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            for (int i = 0; i < n; i++) { parent[i] = i; sz[i] = 1; }
            for (int i = 0; i < nodes; i++) nodeCnt[i] = 0;
            for (int t = 0; t < timePoints; t++) qCnt[t] = 0;
            int stkCap = m * (Log2u(timePoints) + 2) + 4;
            int* stkChild = (int*)Marshal.AllocHGlobal(sizeof(int) * stkCap);
            int* stkPar = (int*)Marshal.AllocHGlobal(sizeof(int) * stkCap);
            int* stkOld = (int*)Marshal.AllocHGlobal(sizeof(int) * stkCap);
            try
            {
                for (int i = 0; i < m; i++)
                {
                    int a = start[i], b = end[i];
                    if (a < 0) a = 0; if (b > timePoints - 1) b = timePoints - 1; if (a > b) continue;
                    CountEdge(nodeCnt, 1, 0, timePoints - 1, a, b);
                }
                int acc = 0;
                for (int i = 0; i < nodes; i++) { nodeOff[i] = acc; acc += nodeCnt[i]; nodeCnt[i] = 0; }
                int* nodeEu = (int*)Marshal.AllocHGlobal(sizeof(int) * acc);
                int* nodeEv = (int*)Marshal.AllocHGlobal(sizeof(int) * acc);
                for (int i = 0; i < m; i++)
                {
                    int a = start[i], b = end[i];
                    if (a < 0) a = 0; if (b > timePoints - 1) b = timePoints - 1; if (a > b) continue;
                    FillEdge(nodeOff, nodeCnt, nodeEu, nodeEv, u[i], v[i], 1, 0, timePoints - 1, a, b);
                }
                for (int j = 0; j < q; j++) { int t = qt[j]; if (t >= 0 && t < timePoints) qCnt[t]++; }
                int qacc = 0;
                for (int t = 0; t < timePoints; t++) { qOff[t] = qacc; qacc += qCnt[t]; qCnt[t] = 0; }
                qOff[timePoints] = qacc;
                int* qBucket = (int*)Marshal.AllocHGlobal(sizeof(int) * qacc);
                for (int j = 0; j < q; j++) { int t = qt[j]; if (t >= 0 && t < timePoints) qBucket[qOff[t] + (qCnt[t]++)] = j; }
                int stkPtr = 0;
                Dfs(nodeOff, nodeCnt, nodeEu, nodeEv, qOff, qCnt, qBucket, qu, qv, ans,
                    parent, sz, stkChild, stkPar, stkOld, ref stkPtr, 1, 0, timePoints - 1);
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)nodeCnt); Marshal.FreeHGlobal((IntPtr)nodeOff);
                Marshal.FreeHGlobal((IntPtr)qCnt); Marshal.FreeHGlobal((IntPtr)qOff);
                Marshal.FreeHGlobal((IntPtr)parent); Marshal.FreeHGlobal((IntPtr)sz);
                Marshal.FreeHGlobal((IntPtr)stkChild); Marshal.FreeHGlobal((IntPtr)stkPar); Marshal.FreeHGlobal((IntPtr)stkOld);
            }
        }

        private static void Dfs(int* nodeOff, int* nodeCnt, int* nodeEu, int* nodeEv,
                                int* qOff, int* qCnt, int* qBucket, int* qu, int* qv, int* ans,
                                int* parent, int* sz, int* stkChild, int* stkPar, int* stkOld,
                                ref int stkPtr, int ni, int tl, int tr)
        {
            int saved = stkPtr;
            int cnt = nodeCnt[ni], off = nodeOff[ni];
            for (int i = 0; i < cnt; i++)
                Union(parent, sz, stkChild, stkPar, stkOld, ref stkPtr, nodeEu[off + i], nodeEv[off + i]);
            if (tl == tr)
            {
                for (int k = 0; k < qCnt[tl]; k++)
                {
                    int j = qBucket[qOff[tl] + k];
                    ans[j] = Find(parent, qu[j]) == Find(parent, qv[j]) ? 1 : 0;
                }
            }
            else
            {
                int mid = (tl + tr) >> 1;
                Dfs(nodeOff, nodeCnt, nodeEu, nodeEv, qOff, qCnt, qBucket, qu, qv, ans, parent, sz, stkChild, stkPar, stkOld, ref stkPtr, ni << 1, tl, mid);
                Dfs(nodeOff, nodeCnt, nodeEu, nodeEv, qOff, qCnt, qBucket, qu, qv, ans, parent, sz, stkChild, stkPar, stkOld, ref stkPtr, (ni << 1) | 1, mid + 1, tr);
            }
            Rollback(parent, sz, stkChild, stkPar, stkOld, ref stkPtr, saved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Find(int* parent, int x) { while (parent[x] != x) x = parent[x]; return x; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Union(int* parent, int* sz, int* sc, int* sp, int* so, ref int p, int u, int v)
        {
            int ru = Find(parent, u), rv = Find(parent, v); if (ru == rv) return;
            if (sz[ru] < sz[rv]) { int t = ru; ru = rv; rv = t; }
            sc[p] = rv; sp[p] = ru; so[p] = sz[ru]; p++; parent[rv] = ru; sz[ru] += sz[rv];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Rollback(int* parent, int* sz, int* sc, int* sp, int* so, ref int p, int to)
        { while (p > to) { p--; parent[sc[p]] = sc[p]; sz[sp[p]] = so[p]; } }
        private static void CountEdge(int* nc, int ni, int tl, int tr, int ql, int qr)
        { if (ql > tr || qr < tl) return; if (ql <= tl && tr <= qr) { nc[ni]++; return; } int mid = (tl + tr) >> 1; CountEdge(nc, ni << 1, tl, mid, ql, qr); CountEdge(nc, (ni << 1) | 1, mid + 1, tr, ql, qr); }
        private static void FillEdge(int* no, int* nc, int* eu, int* ev, int u, int v, int ni, int tl, int tr, int ql, int qr)
        { if (ql > tr || qr < tl) return; if (ql <= tl && tr <= qr) { int p = no[ni] + nc[ni]; eu[p] = u; ev[p] = v; nc[ni]++; return; } int mid = (tl + tr) >> 1; FillEdge(no, nc, eu, ev, u, v, ni << 1, tl, mid, ql, qr); FillEdge(no, nc, eu, ev, u, v, (ni << 1) | 1, mid + 1, tr, ql, qr); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Log2u(int x) { int r = 0; while ((1 << r) < x) r++; return r < 1 ? 1 : r; }
    }
}
