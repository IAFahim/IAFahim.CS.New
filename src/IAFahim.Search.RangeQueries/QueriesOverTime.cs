namespace IAFahim.Search.RangeQueries
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class StaticRangeMode
    {
        public static int Run(int* arr, int n, int l, int r)
        {
            if (l > r) return 0;
            int len = r - l + 1;
            int* buf = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            for (int i = 0; i < len; i++) buf[i] = arr[l + i];
            HeapSortInt(buf, len);
            int mode = buf[0];
            int bestCnt = 1;
            int cur = buf[0];
            int cnt = 1;
            for (int i = 1; i < len; i++)
            {
                if (buf[i] == cur) cnt++;
                else
                {
                    if (cnt > bestCnt) { bestCnt = cnt; mode = cur; }
                    cur = buf[i];
                    cnt = 1;
                }
            }
            if (cnt > bestCnt) mode = cur;
            Marshal.FreeHGlobal((IntPtr)buf);
            return mode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HeapSortInt(int* a, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftDownInt(a, i, len);
            for (int i = len - 1; i > 0; i--)
            {
                int t = a[0]; a[0] = a[i]; a[i] = t;
                SiftDownInt(a, 0, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SiftDownInt(int* a, int i, int len)
        {
            int half = len >> 1;
            while (i < half)
            {
                int child = (i << 1) + 1;
                int right = child + 1;
                if (right < len && a[right] > a[child]) child = right;
                if (a[child] <= a[i]) break;
                int t = a[i]; a[i] = a[child]; a[child] = t;
                i = child;
            }
        }
    }

    public static unsafe class StaticRangeMex
    {
        public static int Run(int* arr, int n, int l, int r)
        {
            if (l > r) return 0;
            int len = r - l + 1;
            int cap = len + 2;
            byte* seen = (byte*)Marshal.AllocHGlobal(sizeof(byte) * cap);
            for (int i = 0; i < cap; i++) seen[i] = 0;
            for (int i = l; i <= r; i++)
            {
                int v = arr[i];
                if (v >= 0 && v < cap) seen[v] = 1;
            }
            int mex = 0;
            while (mex < cap && seen[mex] != 0) mex++;
            Marshal.FreeHGlobal((IntPtr)seen);
            return mex;
        }
    }

    public static unsafe class StaticRangeInversions
    {
        public static long Run(int* arr, int n, int l, int r)
        {
            if (l >= r) return 0;
            int len = r - l + 1;
            int* sorted = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            int* bit = (int*)Marshal.AllocHGlobal(sizeof(int) * (len + 1));
            for (int i = 0; i < len; i++) sorted[i] = arr[l + i];
            HeapSortInt(sorted, len);
            for (int i = 0; i <= len; i++) bit[i] = 0;

            long inv = 0;
            for (int i = 0; i < len; i++)
            {
                int rank = LowerBound(sorted, len, arr[l + i]) + 1;
                long lessOrEqual = BitPrefix(bit, rank);
                inv += i - lessOrEqual;
                BitUpdate(bit, len, rank);
            }
            Marshal.FreeHGlobal((IntPtr)sorted);
            Marshal.FreeHGlobal((IntPtr)bit);
            return inv;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowerBound(int* sorted, int len, int value)
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (sorted[mid] < value) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BitUpdate(int* bit, int len, int idx)
        {
            for (int j = idx; j <= len; j += j & -j) bit[j]++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long BitPrefix(int* bit, int idx)
        {
            long s = 0;
            for (int j = idx; j > 0; j -= j & -j) s += bit[j];
            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HeapSortInt(int* a, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftDownInt(a, i, len);
            for (int i = len - 1; i > 0; i--)
            {
                int t = a[0]; a[0] = a[i]; a[i] = t;
                SiftDownInt(a, 0, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SiftDownInt(int* a, int i, int len)
        {
            int half = len >> 1;
            while (i < half)
            {
                int child = (i << 1) + 1;
                int right = child + 1;
                if (right < len && a[right] > a[child]) child = right;
                if (a[child] <= a[i]) break;
                int t = a[i]; a[i] = a[child]; a[child] = t;
                i = child;
            }
        }
    }

    public static unsafe class Offline2DRangeAddRangeSum
    {
        // Offline 2D: point updates (add delta at coordinate (x,y)) interleaved with axis-aligned
        // rectangle-sum queries. Events stream: for t in [0,E), evType[t] in {0=add,1=query},
        //   add:  (x[t], y[t], delta[t]);
        //   query: sum over x in [qx1[t], qx2[t]], y in [qy1[t], qy2[t]].
        // For a query, ans[qIdx] accumulates the sum of all adds with time <= query time and x in range.
        // CDQ over x-coordinate + BIT over compressed y. Caller passes per-query y-ranges; a query is
        // decomposed into two prefix rectangles (x<=qx2 minus x<qx1) via the sign array.
        // Output: ans[] indexed by query order (0..Q-1); qIdxOf[t] maps query events to ans slots.
        public static void Run(int* evType, int* x, int* y, long* delta,
                               int* qx1, int* qx2, int* qy1, int* qy2, int* qIdxOf,
                               int E, int Q, long* ans)
        {
            for (int i = 0; i < Q; i++) ans[i] = 0;
            if (E <= 0 || Q <= 0) return;
            // Build event list: each query -> two signed prefix events (x<=x2 +1, x<x1 -1).
            // Each add -> one event. Total events <= E + 2Q.
            int cap = E + 2 * Q + 4;
            int* evX = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
            int* evY1 = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
            int* evY2 = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
            long* evContrib = (long*)Marshal.AllocHGlobal(sizeof(long) * cap);
            int* evOut = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
            int evCount = 0;
            // collect all y values for compression
            int* allY = (int*)Marshal.AllocHGlobal(sizeof(int) * (cap + E));
            int yCount = 0;
            for (int t = 0; t < E; t++)
            {
                if (evType[t] == 0)
                {
                    evX[evCount] = x[t]; evY1[evCount] = y[t]; evY2[evCount] = y[t];
                    evContrib[evCount] = delta[t]; evOut[evCount] = -1;
                    allY[yCount++] = y[t];
                    evCount++;
                }
                else
                {
                    int qi = qIdxOf[t];
                    // prefix x<=qx2 contributes +1 factor
                    evX[evCount] = qx2[t]; evY1[evCount] = qy1[t]; evY2[evCount] = qy2[t];
                    evContrib[evCount] = 1; evOut[evCount] = qi;
                    allY[yCount++] = qy1[t]; allY[yCount++] = qy2[t] + 1;
                    evCount++;
                    // prefix x<qx1 (i.e. x<=qx1-1) contributes -1
                    evX[evCount] = qx1[t] - 1; evY1[evCount] = qy1[t]; evY2[evCount] = qy2[t];
                    evContrib[evCount] = -1; evOut[evCount] = qi;
                    evCount++;
                }
            }
            SortInt(allY, yCount);
            int uniq = 0;
            for (int i = 0; i < yCount; i++)
                if (i == 0 || allY[i] != allY[i - 1]) allY[uniq++] = allY[i];

            int* order = (int*)Marshal.AllocHGlobal(sizeof(int) * evCount);
            for (int i = 0; i < evCount; i++) order[i] = i;
            // stable-sort events by x ascending; adds (evOut<0) before queries at same x.
            SortOrderStable(order, evX, evCount);

            long* bit = (long*)Marshal.AllocHGlobal(sizeof(long) * (uniq + 2));
            for (int i = 0; i < uniq + 2; i++) bit[i] = 0;
            for (int oi = 0; oi < evCount; oi++)
            {
                int idx = order[oi];
                if (evOut[idx] < 0)
                {
                    int ry = LowerBoundInt(allY, uniq, evY1[idx]) + 1;
                    BitAdd(bit, uniq + 1, ry, evContrib[idx]);
                }
                else
                {
                    int r1 = LowerBoundInt(allY, uniq, evY1[idx]);     // y >= qy1 -> rank+1
                    int r2 = LowerBoundInt(allY, uniq, evY2[idx] + 1); // y <= qy2 -> rank
                    long sum = BitPrefix(bit, r2) - BitPrefix(bit, r1);
                    ans[evOut[idx]] += (long)evContrib[idx] * sum;
                }
            }
            Marshal.FreeHGlobal((IntPtr)bit); Marshal.FreeHGlobal((IntPtr)order);
            Marshal.FreeHGlobal((IntPtr)evX); Marshal.FreeHGlobal((IntPtr)evY1);
            Marshal.FreeHGlobal((IntPtr)evY2); Marshal.FreeHGlobal((IntPtr)evContrib);
            Marshal.FreeHGlobal((IntPtr)evOut); Marshal.FreeHGlobal((IntPtr)allY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BitAdd(long* bit, int n, int idx, long v)
        {
            for (int j = idx; j <= n; j += j & -j) bit[j] += v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long BitPrefix(long* bit, int idx)
        {
            long s = 0;
            for (int j = idx; j > 0; j -= j & -j) s += bit[j];
            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowerBoundInt(int* a, int len, int key)
        {
            int lo = 0, hi = len;
            while (lo < hi) { int m = (lo + hi) >> 1; if (a[m] < key) lo = m + 1; else hi = m; }
            return lo;
        }

        private static void SortInt(int* a, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftInt(a, i, len);
            for (int i = len - 1; i > 0; i--) { int t = a[0]; a[0] = a[i]; a[i] = t; SiftInt(a, 0, i); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SiftInt(int* a, int i, int len)
        {
            int half = len >> 1;
            while (i < half)
            {
                int c = (i << 1) + 1, r = c + 1;
                if (r < len && a[r] > a[c]) c = r;
                if (a[c] <= a[i]) break;
                int t = a[i]; a[i] = a[c]; a[c] = t; i = c;
            }
        }

        // stable sort of `order` by evX ascending; ties keep index order (adds precede queries because
        // adds are emitted earlier in the stream with smaller original index).
        private static void SortOrderStable(int* order, int* evX, int n)
        {
            // merge sort for stability
            int* tmp = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            for (int w = 1; w < n; w <<= 1)
            {
                for (int lo = 0; lo < n; lo += w << 1)
                {
                    int mid = lo + w; if (mid >= n) break;
                    int hi = mid + w; if (hi > n) hi = n;
                    int i = lo, j = mid, k = lo;
                    while (i < mid && j < hi)
                    {
                        int xi = evX[order[i]], xj = evX[order[j]];
                        if (xi < xj || (xi == xj && order[i] < order[j])) tmp[k++] = order[i++];
                        else tmp[k++] = order[j++];
                    }
                    while (i < mid) tmp[k++] = order[i++];
                    while (j < hi) tmp[k++] = order[j++];
                    for (int t = lo; t < k; t++) order[t] = tmp[t];
                }
            }
            Marshal.FreeHGlobal((IntPtr)tmp);
        }
    }

    public static unsafe class Offline3DPartialOrder
    {
        // For each point i, ans[i] = #{j != i : a[j] < a[i] && b[j] < b[i] && c[j] < c[i]} (strict 3D
        // domination). Delegates to CDQ3D with data set == query set; strict comparison excludes self.
        public static void Run(long* a, long* b, long* c, int n, long* ans)
        {
            if (n <= 0) return;
            CDQ3D.Run(a, b, c, n, a, b, c, n, ans);
        }
    }

    // Generic 3D dominance counter: for each query q, counts[q] = number of data points d with
    // dX[d] < qX[q] && dY[d] < qY[q] && dZ[d] < qZ[q]. CDQ over X, merge by Y, BIT over compressed Z.
    public static unsafe class CDQ3D
    {
        public static void Run(long* dX, long* dY, long* dZ, int D,
                               long* qX, long* qY, long* qZ, int Qnum, long* counts)
        {
            for (int i = 0; i < Qnum; i++) counts[i] = 0;
            if (D == 0 || Qnum == 0) return;
            // combined event stream: each event = {X, Y, Z, kind(0=data,1=query), src}
            int E = D + Qnum;
            long* ex = (long*)Marshal.AllocHGlobal(sizeof(long) * E);
            long* ey = (long*)Marshal.AllocHGlobal(sizeof(long) * E);
            long* ez = (long*)Marshal.AllocHGlobal(sizeof(long) * E);
            int* ekind = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            int* esrc = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            long* tz = (long*)Marshal.AllocHGlobal(sizeof(long) * E);
            int* tkind = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            int* tsrc = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            long* ty = (long*)Marshal.AllocHGlobal(sizeof(long) * E);
            long* tx = (long*)Marshal.AllocHGlobal(sizeof(long) * E);
            // compress Z across all events
            long* allZ = (long*)Marshal.AllocHGlobal(sizeof(long) * E);
            int zc = 0;
            for (int i = 0; i < D; i++) allZ[zc++] = dZ[i];
            for (int i = 0; i < Qnum; i++) allZ[zc++] = qZ[i];
            SortLong(allZ, zc);
            int uniq = 0;
            for (int i = 0; i < zc; i++)
                if (i == 0 || allZ[i] != allZ[i - 1]) allZ[uniq++] = allZ[i];

            for (int i = 0; i < D; i++)
            {
                ex[i] = dX[i]; ey[i] = dY[i]; ez[i] = LB(allZ, uniq, dZ[i]) + 1;
                ekind[i] = 0; esrc[i] = i;
            }
            for (int i = 0; i < Qnum; i++)
            {
                int k = D + i;
                ex[k] = qX[i]; ey[k] = qY[i]; ez[k] = LB(allZ, uniq, qZ[i]) + 1;
                ekind[k] = 1; esrc[k] = i;
            }
            long* bit = (long*)Marshal.AllocHGlobal(sizeof(long) * (uniq + 2));
            for (int i = 0; i < uniq + 2; i++) bit[i] = 0;
            // Sort by X asc; on equal X, queries (kind=1) before data (kind=0) so equal-X
            // data never precedes an equal-X query in the divide -> strict X enforced.
            SortByX(ex, ey, ez, ekind, esrc, E, tx, ty, tz, tkind, tsrc);
            Rec(ex, ey, ez, ekind, esrc, 0, E - 1, counts, bit, uniq, tx, ty, tz, tkind, tsrc);
            Marshal.FreeHGlobal((IntPtr)bit);
            Marshal.FreeHGlobal((IntPtr)allZ);
            Marshal.FreeHGlobal((IntPtr)ex); Marshal.FreeHGlobal((IntPtr)ey);
            Marshal.FreeHGlobal((IntPtr)ez); Marshal.FreeHGlobal((IntPtr)ekind);
            Marshal.FreeHGlobal((IntPtr)esrc); Marshal.FreeHGlobal((IntPtr)tz);
            Marshal.FreeHGlobal((IntPtr)tkind); Marshal.FreeHGlobal((IntPtr)tsrc);
            Marshal.FreeHGlobal((IntPtr)ty); Marshal.FreeHGlobal((IntPtr)tx);
        }

        private static void Rec(long* ex, long* ey, long* ez, int* ekind, int* esrc,
                                int l, int r, long* counts, long* bit, int uniq,
                                long* tx, long* ty, long* tz, int* tkind, int* tsrc)
        {
            if (l >= r) return;
            int mid = (l + r) >> 1;
            Rec(ex, ey, ez, ekind, esrc, l, mid, counts, bit, uniq, tx, ty, tz, tkind, tsrc);
            Rec(ex, ey, ez, ekind, esrc, mid + 1, r, counts, bit, uniq, tx, ty, tz, tkind, tsrc);
            // left half has X <= right half (events pre-sorted by X globally before first rec).
            // merge by Y ascending; data events in left feed BIT; query events in right read BIT (Z<qZ).
            int i = l, j = mid + 1, k = l;
            while (i <= mid && j <= r)
            {
                bool takeLeft = ey[i] < ey[j] || (ey[i] == ey[j] && ekind[i] == 1 && ekind[j] == 0);
                if (takeLeft)
                {
                    if (ekind[i] == 0) Add(bit, uniq + 1, (int)ez[i], 1);
                    tx[k] = ex[i]; ty[k] = ey[i]; tz[k] = ez[i]; tkind[k] = ekind[i]; tsrc[k] = esrc[i];
                    i++;
                }
                else
                {
                    if (ekind[j] == 1) counts[esrc[j]] += Pref(bit, (int)ez[j] - 1);
                    tx[k] = ex[j]; ty[k] = ey[j]; tz[k] = ez[j]; tkind[k] = ekind[j]; tsrc[k] = esrc[j];
                    j++;
                }
                k++;
            }
            while (i <= mid)
            {
                if (ekind[i] == 0) Add(bit, uniq + 1, (int)ez[i], 1);
                tx[k] = ex[i]; ty[k] = ey[i]; tz[k] = ez[i]; tkind[k] = ekind[i]; tsrc[k] = esrc[i];
                i++; k++;
            }
            while (j <= r)
            {
                if (ekind[j] == 1) counts[esrc[j]] += Pref(bit, (int)ez[j] - 1);
                tx[k] = ex[j]; ty[k] = ey[j]; tz[k] = ez[j]; tkind[k] = ekind[j]; tsrc[k] = esrc[j];
                j++; k++;
            }
            for (int p = l; p <= mid; p++)
                if (ekind[p] == 0) Add(bit, uniq + 1, (int)ez[p], -1);
            for (int p = l; p <= r; p++)
            { ex[p] = tx[p]; ey[p] = ty[p]; ez[p] = tz[p]; ekind[p] = tkind[p]; esrc[p] = tsrc[p]; }
        }

        // stable merge sort by (X asc, kind desc) so queries precede data on equal X.
        private static void SortByX(long* ex, long* ey, long* ez, int* ekind, int* esrc, int n,
                                    long* tx, long* ty, long* tz, int* tkind, int* tsrc)
        {
            for (int w = 1; w < n; w <<= 1)
            {
                for (int lo = 0; lo < n; lo += w << 1)
                {
                    int mid = lo + w; if (mid >= n) break;
                    int hi = mid + w; if (hi > n) hi = n;
                    int i = lo, j = mid, k = lo;
                    while (i < mid && j < hi)
                    {
                        bool takeI;
                        if (ex[i] != ex[j]) takeI = ex[i] < ex[j];
                        else takeI = ekind[i] >= ekind[j]; // equal X: query(1) before data(0)
                        if (takeI) { Cp(tx, ty, tz, tkind, tsrc, k, ex, ey, ez, ekind, esrc, i); i++; }
                        else { Cp(tx, ty, tz, tkind, tsrc, k, ex, ey, ez, ekind, esrc, j); j++; }
                        k++;
                    }
                    while (i < mid) { Cp(tx, ty, tz, tkind, tsrc, k, ex, ey, ez, ekind, esrc, i); i++; k++; }
                    while (j < hi) { Cp(tx, ty, tz, tkind, tsrc, k, ex, ey, ez, ekind, esrc, j); j++; k++; }
                    for (int t = lo; t < k; t++) Cp(ex, ey, ez, ekind, esrc, t, tx, ty, tz, tkind, tsrc, t);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Cp(long* dx, long* dy, long* dz, int* dk, int* ds, int di,
                               long* sx, long* sy, long* sz, int* sk, int* ss, int si)
        { dx[di] = sx[si]; dy[di] = sy[si]; dz[di] = sz[si]; dk[di] = sk[si]; ds[di] = ss[si]; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Add(long* bit, int n, int idx, long v)
        { for (int p = idx; p <= n; p += p & -p) bit[p] += v; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Pref(long* bit, int idx)
        { long s = 0; for (int p = idx; p > 0; p -= p & -p) s += bit[p]; return s; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LB(long* a, int len, long key)
        { int lo = 0, hi = len; while (lo < hi) { int m = (lo + hi) >> 1; if (a[m] < key) lo = m + 1; else hi = m; } return lo; }

        private static void SortLong(long* a, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftLong(a, i, len);
            for (int i = len - 1; i > 0; i--) { long t = a[0]; a[0] = a[i]; a[i] = t; SiftLong(a, 0, i); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SiftLong(long* a, int i, int len)
        {
            int half = len >> 1;
            while (i < half)
            {
                int c = (i << 1) + 1, rr = c + 1;
                if (rr < len && a[rr] > a[c]) c = rr;
                if (a[c] <= a[i]) break;
                long t = a[i]; a[i] = a[c]; a[c] = t; i = c;
            }
        }
    }

    public static unsafe class CdqDynamicInversions
    {
        // perm[0..n-1]: array of DISTINCT values (a permutation or any distinct ints).
        // removeIdx[t], t in [0,k): ORIGINAL index (0-based) of the element removed at step t.
        // ans[t] = number of inversion pairs (i,j) (origIdx i<j and perm[i]>perm[j]) that involve the
        // element removed at step t and a still-present partner (partner removed later or never).
        // Never-removed elements are treated as removed at time k. CDQ3D used in both directions.
        public static void Run(int* perm, int n, int* removeIdx, int k, long* ans)
        {
            for (int t = 0; t < k; t++) ans[t] = 0;
            if (n <= 0 || k <= 0) return;
            int* rtime = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            for (int i = 0; i < n; i++) rtime[i] = k;   // never removed
            for (int t = 0; t < k; t++) rtime[removeIdx[t]] = t;

            long* dX = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            long* dY = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            long* dZ = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            long* qX = (long*)Marshal.AllocHGlobal(sizeof(long) * k);
            long* qY = (long*)Marshal.AllocHGlobal(sizeof(long) * k);
            long* qZ = (long*)Marshal.AllocHGlobal(sizeof(long) * k);
            long* cnt = (long*)Marshal.AllocHGlobal(sizeof(long) * k);
            // left-count: partners j with origIdx<j? no: j to the LEFT of e (j<pe), val>ve, still present (tj>te)
            //   dominance: data (oj, -vj, -tj) < (pe, -ve, -te)
            for (int i = 0; i < n; i++) { dX[i] = i; dY[i] = -(long)perm[i]; dZ[i] = -(long)rtime[i]; }
            for (int t = 0; t < k; t++)
            { int e = removeIdx[t]; qX[t] = e; qY[t] = -(long)perm[e]; qZ[t] = -(long)t; }
            CDQ3D.Run(dX, dY, dZ, n, qX, qY, qZ, k, cnt);
            for (int t = 0; t < k; t++) ans[t] += cnt[t];

            // right-count: partners j with origIdx>pe, val<ve, tj>te
            //   dominance: data (-oj, vj, -tj) < (-pe, ve, -te)
            for (int i = 0; i < n; i++) { dX[i] = -(long)i; dY[i] = perm[i]; dZ[i] = -(long)rtime[i]; }
            for (int t = 0; t < k; t++)
            { int e = removeIdx[t]; qX[t] = -(long)e; qY[t] = perm[e]; qZ[t] = -(long)t; }
            CDQ3D.Run(dX, dY, dZ, n, qX, qY, qZ, k, cnt);
            for (int t = 0; t < k; t++) ans[t] += cnt[t];
            Marshal.FreeHGlobal((IntPtr)dX); Marshal.FreeHGlobal((IntPtr)dY);
            Marshal.FreeHGlobal((IntPtr)dZ); Marshal.FreeHGlobal((IntPtr)qX);
            Marshal.FreeHGlobal((IntPtr)qY); Marshal.FreeHGlobal((IntPtr)qZ);
            Marshal.FreeHGlobal((IntPtr)cnt);
            Marshal.FreeHGlobal((IntPtr)rtime);
        }
    }

    public static unsafe class DivideConquerOnTime
    {
        // Offline connectivity-over-time (线段树分治). n vertices; m edges, edge i (eu[i],ev[i]) active
        // during the inclusive time interval [el[i], er[i]]; q queries: are qu[j],qv[j] connected at
        // time qt[j]? ans[j] = 1 (connected) or 0. Builds a segment tree over [0,T), inserts each edge
        // into O(log T) nodes covering its interval, then DFSes: at each node unions its edges into a
        // rollback-DSU, recurses, and rolls back. Queries are bucketed by their time leaf.
        // T = max time coordinate (one past the largest query/edge time). O((m+q) log T * alpha).
        public static void Run(int n, int* eu, int* ev, int* el, int* er, int m,
                               int* qu, int* qv, int* qt, int q, int T, int* ans)
        {
            for (int j = 0; j < q; j++) ans[j] = 0;
            if (n <= 0 || q <= 0) return;
            if (T <= 0) T = 1;

            int nodes = 4 * T;
            int* nodeCnt = (int*)Marshal.AllocHGlobal(sizeof(int) * nodes);
            for (int i = 0; i < nodes; i++) nodeCnt[i] = 0;
            int* nodeOff = (int*)Marshal.AllocHGlobal(sizeof(int) * nodes);
            int* qCnt = (int*)Marshal.AllocHGlobal(sizeof(int) * T);
            int* qOff = (int*)Marshal.AllocHGlobal(sizeof(int) * (T + 1));
            for (int t = 0; t < T; t++) qCnt[t] = 0;

            int* parent = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* sz = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            for (int i = 0; i < n; i++) { parent[i] = i; sz[i] = 1; }

            int stkCap = m * (Log2u(T) + 2) + 4;
            int* stkChild = (int*)Marshal.AllocHGlobal(sizeof(int) * stkCap);
            int* stkPar = (int*)Marshal.AllocHGlobal(sizeof(int) * stkCap);
            int* stkOld = (int*)Marshal.AllocHGlobal(sizeof(int) * stkCap);

            // count edges per segment-tree node
            for (int i = 0; i < m; i++)
                {
                    int a = el[i], b = er[i];
                    if (a < 0) a = 0;
                    if (b > T - 1) b = T - 1;
                    if (a > b) continue;
                    CountEdge(nodeCnt, 1, 0, T - 1, a, b);
                }
                int acc = 0;
                for (int i = 0; i < nodes; i++) { nodeOff[i] = acc; acc += nodeCnt[i]; nodeCnt[i] = 0; }
                int* nodeEu = (int*)Marshal.AllocHGlobal(sizeof(int) * acc);
                int* nodeEv = (int*)Marshal.AllocHGlobal(sizeof(int) * acc);
                for (int i = 0; i < m; i++)
                {
                    int a = el[i], b = er[i];
                    if (a < 0) a = 0;
                    if (b > T - 1) b = T - 1;
                    if (a > b) continue;
                    FillEdge(nodeOff, nodeCnt, nodeEu, nodeEv, eu[i], ev[i], 1, 0, T - 1, a, b);
                }

                // bucket queries by time leaf
                for (int j = 0; j < q; j++) { int t = qt[j]; if (t >= 0 && t < T) qCnt[t]++; }
                int qacc = 0;
                for (int t = 0; t < T; t++) { qOff[t] = qacc; qacc += qCnt[t]; qCnt[t] = 0; }
                qOff[T] = qacc;
                int* qBucket = (int*)Marshal.AllocHGlobal(sizeof(int) * qacc);
                for (int j = 0; j < q; j++) { int t = qt[j]; if (t >= 0 && t < T) qBucket[qOff[t] + (qCnt[t]++)] = j; }

                int stkPtr = 0;
                Dfs(nodeOff, nodeCnt, nodeEu, nodeEv, qOff, qCnt, qBucket, qu, qv, ans,
                    parent, sz, stkChild, stkPar, stkOld, ref stkPtr, 1, 0, T - 1);
                Marshal.FreeHGlobal((IntPtr)nodeEu);
                Marshal.FreeHGlobal((IntPtr)nodeEv);
                Marshal.FreeHGlobal((IntPtr)qBucket);
                Marshal.FreeHGlobal((IntPtr)nodeCnt); Marshal.FreeHGlobal((IntPtr)nodeOff);
                Marshal.FreeHGlobal((IntPtr)qCnt); Marshal.FreeHGlobal((IntPtr)qOff);
                Marshal.FreeHGlobal((IntPtr)parent); Marshal.FreeHGlobal((IntPtr)sz);
                Marshal.FreeHGlobal((IntPtr)stkChild); Marshal.FreeHGlobal((IntPtr)stkPar); Marshal.FreeHGlobal((IntPtr)stkOld);
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
                int t = tl;
                for (int k = 0; k < qCnt[t]; k++)
                {
                    int j = qBucket[qOff[t] + k];
                    ans[j] = Find(parent, qu[j]) == Find(parent, qv[j]) ? 1 : 0;
                }
            }
            else
            {
                int mid = (tl + tr) >> 1;
                Dfs(nodeOff, nodeCnt, nodeEu, nodeEv, qOff, qCnt, qBucket, qu, qv, ans,
                    parent, sz, stkChild, stkPar, stkOld, ref stkPtr, ni << 1, tl, mid);
                Dfs(nodeOff, nodeCnt, nodeEu, nodeEv, qOff, qCnt, qBucket, qu, qv, ans,
                    parent, sz, stkChild, stkPar, stkOld, ref stkPtr, (ni << 1) | 1, mid + 1, tr);
            }
            Rollback(parent, sz, stkChild, stkPar, stkOld, ref stkPtr, saved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Find(int* parent, int x)
        { while (parent[x] != x) x = parent[x]; return x; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Union(int* parent, int* sz, int* stkChild, int* stkPar, int* stkOld, ref int stkPtr, int u, int v)
        {
            int ru = Find(parent, u), rv = Find(parent, v);
            if (ru == rv) return;
            if (sz[ru] < sz[rv]) { int t = ru; ru = rv; rv = t; }
            stkChild[stkPtr] = rv; stkPar[stkPtr] = ru; stkOld[stkPtr] = sz[ru];
            stkPtr++;
            parent[rv] = ru; sz[ru] += sz[rv];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Rollback(int* parent, int* sz, int* stkChild, int* stkPar, int* stkOld, ref int stkPtr, int to)
        {
            while (stkPtr > to)
            {
                stkPtr--;
                int rv = stkChild[stkPtr], ru = stkPar[stkPtr];
                parent[rv] = rv; sz[ru] = stkOld[stkPtr];
            }
        }

        private static void CountEdge(int* nodeCnt, int ni, int tl, int tr, int ql, int qr)
        {
            if (ql > tr || qr < tl) return;
            if (ql <= tl && tr <= qr) { nodeCnt[ni]++; return; }
            int mid = (tl + tr) >> 1;
            CountEdge(nodeCnt, ni << 1, tl, mid, ql, qr);
            CountEdge(nodeCnt, (ni << 1) | 1, mid + 1, tr, ql, qr);
        }

        private static void FillEdge(int* nodeOff, int* nodeCnt, int* nodeEu, int* nodeEv, int u, int v, int ni, int tl, int tr, int ql, int qr)
        {
            if (ql > tr || qr < tl) return;
            if (ql <= tl && tr <= qr)
            {
                int pos = nodeOff[ni] + nodeCnt[ni];
                nodeEu[pos] = u; nodeEv[pos] = v; nodeCnt[ni]++; return;
            }
            int mid = (tl + tr) >> 1;
            FillEdge(nodeOff, nodeCnt, nodeEu, nodeEv, u, v, ni << 1, tl, mid, ql, qr);
            FillEdge(nodeOff, nodeCnt, nodeEu, nodeEv, u, v, (ni << 1) | 1, mid + 1, tr, ql, qr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Log2u(int x)
        { int r = 0; while ((1 << r) < x) r++; return r < 1 ? 1 : r; }
    }

    public static unsafe class SegmentTreeOverTimeAdd
    {
        // Range-add-over-time, point-query. Effect i adds delta[i] to every time t in [l[i], r[i]]
        // (inclusive). ans[t] = total delta active at time t, for t in [0,T). Difference-array: O(m+T).
        public static void Run(int* l, int* r, long* delta, int m, int T, long* ans)
        {
            for (int t = 0; t < T; t++) ans[t] = 0;
            if (m <= 0 || T <= 0) return;
            long* diff = (long*)Marshal.AllocHGlobal(sizeof(long) * (T + 1));
            for (int t = 0; t <= T; t++) diff[t] = 0;
            for (int i = 0; i < m; i++)
            {
                int a = l[i], b = r[i];
                if (a < 0) a = 0;
                if (b > T - 1) b = T - 1;
                if (a > b) continue;
                diff[a] += delta[i];
                diff[b + 1] -= delta[i];
            }
            long acc = 0;
            for (int t = 0; t < T; t++) { acc += diff[t]; ans[t] = acc; }
            Marshal.FreeHGlobal((IntPtr)diff);
        }
    }

    public static unsafe class SegmentTreeOverTimeDfs
    {
        // Connected-component count per time point. n vertices; m edges, edge i (eu[i],ev[i]) active
        // during inclusive [el[i],er[i]]. compCount[t] = number of connected components at time t,
        // for t in [0,T). Same segment-tree-on-time + rollback-DSU engine as DivideConquerOnTime;
        // the leaf records comp = n - (#successful unions on the root-to-leaf path).
        public static void Run(int n, int* eu, int* ev, int* el, int* er, int m, int T, int* compCount)
        {
            for (int t = 0; t < T; t++) compCount[t] = n;
            if (n <= 0) return;
            if (T <= 0) T = 1;

            int nodes = 4 * T;
            int* nodeCnt = (int*)Marshal.AllocHGlobal(sizeof(int) * nodes);
            for (int i = 0; i < nodes; i++) nodeCnt[i] = 0;
            int* nodeOff = (int*)Marshal.AllocHGlobal(sizeof(int) * nodes);

            int* parent = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* sz = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            for (int i = 0; i < n; i++) { parent[i] = i; sz[i] = 1; }

            int stkCap = m * (Log2u(T) + 2) + 4;
            int* stkChild = (int*)Marshal.AllocHGlobal(sizeof(int) * stkCap);
            int* stkPar = (int*)Marshal.AllocHGlobal(sizeof(int) * stkCap);
            int* stkOld = (int*)Marshal.AllocHGlobal(sizeof(int) * stkCap);

            for (int i = 0; i < m; i++)
            {
                int a = el[i], b = er[i];
                if (a < 0) a = 0; if (b > T - 1) b = T - 1; if (a > b) continue;
                CountEdge(nodeCnt, 1, 0, T - 1, a, b);
            }
            int acc = 0;
            for (int i = 0; i < nodes; i++) { nodeOff[i] = acc; acc += nodeCnt[i]; nodeCnt[i] = 0; }
            int* nodeEu = (int*)Marshal.AllocHGlobal(sizeof(int) * acc);
            int* nodeEv = (int*)Marshal.AllocHGlobal(sizeof(int) * acc);
            for (int i = 0; i < m; i++)
            {
                int a = el[i], b = er[i];
                if (a < 0) a = 0; if (b > T - 1) b = T - 1; if (a > b) continue;
                FillEdge(nodeOff, nodeCnt, nodeEu, nodeEv, eu[i], ev[i], 1, 0, T - 1, a, b);
            }

            int stkPtr = 0;
            Dfs(nodeOff, nodeCnt, nodeEu, nodeEv, compCount, parent, sz, stkChild, stkPar, stkOld,
                ref stkPtr, n, 1, 0, T - 1);
            Marshal.FreeHGlobal((IntPtr)nodeEu);
            Marshal.FreeHGlobal((IntPtr)nodeEv);
            Marshal.FreeHGlobal((IntPtr)nodeCnt); Marshal.FreeHGlobal((IntPtr)nodeOff);
            Marshal.FreeHGlobal((IntPtr)parent); Marshal.FreeHGlobal((IntPtr)sz);
            Marshal.FreeHGlobal((IntPtr)stkChild); Marshal.FreeHGlobal((IntPtr)stkPar); Marshal.FreeHGlobal((IntPtr)stkOld);
        }

        private static void Dfs(int* nodeOff, int* nodeCnt, int* nodeEu, int* nodeEv, int* compCount,
                                int* parent, int* sz, int* stkChild, int* stkPar, int* stkOld,
                                ref int stkPtr, int comp, int ni, int tl, int tr)
        {
            int saved = stkPtr;
            int cnt = nodeCnt[ni], off = nodeOff[ni];
            for (int i = 0; i < cnt; i++)
                if (Union(parent, sz, stkChild, stkPar, stkOld, ref stkPtr, nodeEu[off + i], nodeEv[off + i])) comp--;

            if (tl == tr) compCount[tl] = comp;
            else
            {
                int mid = (tl + tr) >> 1;
                Dfs(nodeOff, nodeCnt, nodeEu, nodeEv, compCount, parent, sz, stkChild, stkPar, stkOld, ref stkPtr, comp, ni << 1, tl, mid);
                Dfs(nodeOff, nodeCnt, nodeEu, nodeEv, compCount, parent, sz, stkChild, stkPar, stkOld, ref stkPtr, comp, (ni << 1) | 1, mid + 1, tr);
            }
            Rollback(parent, sz, stkChild, stkPar, stkOld, ref stkPtr, saved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Find(int* parent, int x)
        { while (parent[x] != x) x = parent[x]; return x; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Union(int* parent, int* sz, int* stkChild, int* stkPar, int* stkOld, ref int stkPtr, int u, int v)
        {
            int ru = Find(parent, u), rv = Find(parent, v);
            if (ru == rv) return false;
            if (sz[ru] < sz[rv]) { int t = ru; ru = rv; rv = t; }
            stkChild[stkPtr] = rv; stkPar[stkPtr] = ru; stkOld[stkPtr] = sz[ru];
            stkPtr++;
            parent[rv] = ru; sz[ru] += sz[rv];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Rollback(int* parent, int* sz, int* stkChild, int* stkPar, int* stkOld, ref int stkPtr, int to)
        {
            while (stkPtr > to)
            {
                stkPtr--;
                int rv = stkChild[stkPtr], ru = stkPar[stkPtr];
                parent[rv] = rv; sz[ru] = stkOld[stkPtr];
            }
        }

        private static void CountEdge(int* nodeCnt, int ni, int tl, int tr, int ql, int qr)
        {
            if (ql > tr || qr < tl) return;
            if (ql <= tl && tr <= qr) { nodeCnt[ni]++; return; }
            int mid = (tl + tr) >> 1;
            CountEdge(nodeCnt, ni << 1, tl, mid, ql, qr);
            CountEdge(nodeCnt, (ni << 1) | 1, mid + 1, tr, ql, qr);
        }

        private static void FillEdge(int* nodeOff, int* nodeCnt, int* nodeEu, int* nodeEv, int u, int v, int ni, int tl, int tr, int ql, int qr)
        {
            if (ql > tr || qr < tl) return;
            if (ql <= tl && tr <= qr)
            {
                int pos = nodeOff[ni] + nodeCnt[ni];
                nodeEu[pos] = u; nodeEv[pos] = v; nodeCnt[ni]++; return;
            }
            int mid = (tl + tr) >> 1;
            FillEdge(nodeOff, nodeCnt, nodeEu, nodeEv, u, v, ni << 1, tl, mid, ql, qr);
            FillEdge(nodeOff, nodeCnt, nodeEu, nodeEv, u, v, (ni << 1) | 1, mid + 1, tr, ql, qr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Log2u(int x)
        { int r = 0; while ((1 << r) < x) r++; return r < 1 ? 1 : r; }
    }
}
