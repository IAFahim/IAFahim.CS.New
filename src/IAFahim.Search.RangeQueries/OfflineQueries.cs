namespace IAFahim.Search.RangeQueries
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class OfflineRangeCount
    {
        public static void Run(int* arr, int n, int* ql, int* qr, int* qx, int q, int* ans)
        {
            for (int j = 0; j < q; j++) ans[j] = 0;
            if (n <= 0 || q <= 0) return;

            long* elemKeys = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            long* queryKeys = (long*)Marshal.AllocHGlobal(sizeof(long) * q);
            int* bit = (int*)Marshal.AllocHGlobal(sizeof(int) * (n + 1));
            try
            {
                for (int i = 0; i < n; i++)
                    elemKeys[i] = ((long)arr[i] << 32) | (uint)i;
                for (int j = 0; j < q; j++)
                    queryKeys[j] = ((long)qx[j] << 32) | (uint)j;
                for (int i = 0; i <= n; i++) bit[i] = 0;

                HeapSortLong(elemKeys, n);
                HeapSortLong(queryKeys, q);

                int ei = 0;
                for (int sj = 0; sj < q; sj++)
                {
                    long qk = queryKeys[sj];
                    int threshold = (int)(qk >> 32);
                    int origIdx = (int)(qk & 0xFFFFFFFFL);
                    while (ei < n)
                    {
                        long ek = elemKeys[ei];
                        int eval = (int)(ek >> 32);
                        if (eval > threshold) break;
                        int pos = (int)(ek & 0xFFFFFFFFL);
                        BitUpdate(bit, n, pos);
                        ei++;
                    }
                    int a = ql[origIdx];
                    int b = qr[origIdx];
                    if (a < 0) a = 0;
                    if (b > n - 1) b = n - 1;
                    int cnt = a <= b ? BitPrefix(bit, b + 1) - BitPrefix(bit, a) : 0;
                    ans[origIdx] = cnt;
                }
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)elemKeys);
                Marshal.FreeHGlobal((IntPtr)queryKeys);
                Marshal.FreeHGlobal((IntPtr)bit);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BitUpdate(int* bit, int n, int idx)
        {
            for (int j = idx + 1; j <= n; j += j & -j) bit[j]++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BitPrefix(int* bit, int idx)
        {
            int s = 0;
            for (int j = idx; j > 0; j -= j & -j) s += bit[j];
            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HeapSortLong(long* a, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftDownLong(a, i, len);
            for (int i = len - 1; i > 0; i--)
            {
                long t = a[0]; a[0] = a[i]; a[i] = t;
                SiftDownLong(a, 0, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SiftDownLong(long* a, int i, int len)
        {
            int half = len >> 1;
            while (i < half)
            {
                int child = (i << 1) + 1;
                int right = child + 1;
                if (right < len && a[right] > a[child]) child = right;
                if (a[child] <= a[i]) break;
                long t = a[i]; a[i] = a[child]; a[child] = t;
                i = child;
            }
        }
    }

    public static unsafe class FractionalCascadingBuild
    {
        // Schema: k sorted ascending lists concatenated in `data` with per-list lengths in `sizes`.
        // Produces cascaded merged lists M_i (i = k-1 down to 0), where M_{k-1} = L_{k-1} and
        //   M_i = merge(L_i, every-second element of M_{i+1}).
        //   merged[offsets[i] + j]    = j-th element of M_i (ascending).
        //   aux[offsets[i] + j]       = index p in M_{i+1} (0-based) of first element >= M_i[j]; 0 for i == k-1.
        //   origPrefix[offsets[i]+j]  = number of ORIGINAL L_i elements among M_i[0..j).
        //   offsets[0..k]             = start of each M_i in merged (offsets[k] = total merged size).
        public static void Run(int* data, int* sizes, int k,
                               int* merged, int* aux, int* origPrefix, int* offsets)
        {
            if (k <= 0) { offsets[0] = 0; return; }
            int* mSize = stackalloc int[k];
            mSize[k - 1] = sizes[k - 1];
            for (int i = k - 2; i >= 0; i--)
                mSize[i] = sizes[i] + mSize[i + 1] / 2;
            // Each level reserves mSize[i]+1 slots so its sentinel at index mSize[i]
            // does not collide with the next level's slot 0.
            offsets[0] = 0;
            for (int i = 1; i <= k; i++) offsets[i] = offsets[i - 1] + mSize[i - 1] + 1;

            int* listStart = stackalloc int[k];
            listStart[0] = 0;
            for (int i = 1; i < k; i++) listStart[i] = listStart[i - 1] + sizes[i - 1];

            int last = k - 1;
            int lastOff = offsets[last];
            int* Llast = data + listStart[last];
            for (int j = 0; j < sizes[last]; j++)
            {
                merged[lastOff + j] = Llast[j];
                aux[lastOff + j] = 0;
                origPrefix[lastOff + j] = j;
            }
            origPrefix[lastOff + sizes[last]] = sizes[last];
            aux[lastOff + sizes[last]] = sizes[last];

            for (int i = k - 2; i >= 0; i--)
            {
                int off = offsets[i];
                int nextOff = offsets[i + 1];
                int* Li = data + listStart[i];
                int ni = sizes[i];
                int ai = 0;
                int sampleIdx = 1;
                int written = 0;
                int origCount = 0;
                while (ai < ni || sampleIdx < mSize[i + 1])
                {
                    int candidate;
                    bool fromOrig;
                    if (ai < ni && (sampleIdx >= mSize[i + 1] || Li[ai] <= merged[nextOff + sampleIdx]))
                    { candidate = Li[ai]; fromOrig = true; }
                    else
                    { candidate = merged[nextOff + sampleIdx]; fromOrig = false; }

                    if (fromOrig) ai++; else sampleIdx += 2;
                    merged[off + written] = candidate;
                    origPrefix[off + written] = origCount;
                    if (fromOrig) origCount++;
                    int p = LowerBound(merged + nextOff, mSize[i + 1], candidate);
                    aux[off + written] = p;
                    written++;
                }
                origPrefix[off + mSize[i]] = sizes[i];
                aux[off + mSize[i]] = mSize[i + 1];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowerBound(int* a, int len, int key)
        {
            int lo = 0, hi = len;
            while (lo < hi) { int mid = (lo + hi) >> 1; if (a[mid] < key) lo = mid + 1; else hi = mid; }
            return lo;
        }
    }

    public static unsafe class FractionalCascadingQuery
    {
        // outPos[i] = number of elements strictly less than `key` in the i-th ORIGINAL list L_i,
        // for all i in [0, k). O(log |M_0| + k) via aux-pointer descent. Caller guarantees outPos has k ints.
        public static void Run(int* merged, int* aux, int* origPrefix, int* offsets, int k, int key, int* outPos)
        {
            if (k <= 0) return;
            int len0 = offsets[1] - offsets[0] - 1;
            int p = LowerBound(merged + offsets[0], len0, key);
            outPos[0] = origPrefix[offsets[0] + p];
            for (int i = 1; i < k; i++)
            {
                int prevOff = offsets[i - 1];
                int off = offsets[i];
                int len = offsets[i + 1] - off - 1;
                int q = aux[prevOff + p];
                while (q > 0 && merged[off + q - 1] >= key) q--;
                while (q < len && merged[off + q] < key) q++;
                outPos[i] = origPrefix[off + q];
                p = q;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowerBound(int* a, int len, int key)
        {
            int lo = 0, hi = len;
            while (lo < hi) { int mid = (lo + hi) >> 1; if (a[mid] < key) lo = mid + 1; else hi = mid; }
            return lo;
        }
    }
}
