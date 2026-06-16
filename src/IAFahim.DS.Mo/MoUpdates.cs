namespace IAFahim.DS.Mo
{
    using System;
    using System.Runtime.CompilerServices;

    public struct Query3D
    {
        public int L, R, T, Id;
    }

    public struct Update
    {
        public int Pos, OldVal, NewVal;
    }

    public static unsafe class MoWithUpdates
    {
        // Unchecked: the CALLER guarantees valid input (non-null buffers, valid lengths) BY DESIGN.
        // freq is a caller-owned, pre-zeroed frequency table whose length must exceed the maximum
        // value present in arr and in updates' OldVal/NewVal (i.e. freqLen > max value). The caller
        // owns and reuses this buffer, matching the rest of the library's convention (see MoAlgorithm).
        // arr is used as scratch by the time-travel loop; on return arr is restored to its original
        // state (curT rewound to 0), so Run is idempotent across repeated calls on the same buffers.
        public static void Run(int n, int* arr, int qCount, Query3D* queries, int uCount, Update* updates, int* ans, int blockSize, int* freq)
        {
            SortQueries(queries, qCount, blockSize);

            int curL = 0, curR = -1, curT = 0;
            int curAns = 0;

            for (int i = 0; i < qCount; i++)
            {
                Query3D q = queries[i];
                UpdateT(ref curT, q.T, curL, curR, arr, updates, freq, ref curAns);
                UpdateRange(ref curL, ref curR, q.L, q.R, arr, freq, ref curAns);
                ans[q.Id] = curAns;
            }

            // Restore arr to its original state by rewinding the update timeline back to 0.
            // curL > curR after this is not required; reverts touch arr unconditionally outside [curL,curR].
            while (curT > 0) { curT--; RevertUpdate(updates[curT], curL, curR, arr, freq, ref curAns); }
        }

        private static void SortQueries(Query3D* queries, int q, int b)
        {
            if (q <= 1) return;
            QuickSort(queries, 0, q - 1, b);
        }

        // Iterative quicksort with tail-recursion elimination on the larger partition to bound
        // recursion depth to O(log q). Mirrors MoAlgorithm.MoSort.QuickSort.
        private static void QuickSort(Query3D* queries, int left, int right, int blk)
        {
            while (left < right)
            {
                int pivotIdx = left + ((right - left) >> 1);
                Query3D pivot = queries[pivotIdx];
                int i = left, j = right;
                while (i <= j)
                {
                    while (Less(queries[i], pivot, blk)) i++;
                    while (Less(pivot, queries[j], blk)) j--;
                    if (i <= j)
                    {
                        Query3D t = queries[i]; queries[i] = queries[j]; queries[j] = t;
                        i++; j--;
                    }
                }
                // Recurse into the smaller side, loop on the larger side (bounded stack depth).
                if (j - left < right - i)
                {
                    QuickSort(queries, left, j, blk);
                    left = i;
                }
                else
                {
                    QuickSort(queries, i, right, blk);
                    right = j;
                }
            }
        }

        // Total order: L-block, then R-block, then T. Returns true iff a sorts strictly before b.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Less(Query3D a, Query3D b, int blk)
        {
            int al = a.L / blk, bl = b.L / blk;
            if (al != bl) return al < bl;
            int ar = a.R / blk, br = b.R / blk;
            if (ar != br) return ar < br;
            return a.T < b.T;
        }

        private static void UpdateT(ref int curT, int targetT, int curL, int curR, int* arr, Update* updates, int* freq, ref int curAns)
        {
            while (curT < targetT) { ApplyUpdate(updates[curT], curL, curR, arr, freq, ref curAns); curT++; }
            while (curT > targetT) { curT--; RevertUpdate(updates[curT], curL, curR, arr, freq, ref curAns); }
        }

        private static void ApplyUpdate(Update u, int l, int r, int* arr, int* freq, ref int curAns)
        {
            if (u.Pos >= l && u.Pos <= r) { Remove(arr[u.Pos], freq, ref curAns); Add(u.NewVal, freq, ref curAns); }
            arr[u.Pos] = u.NewVal;
        }

        private static void RevertUpdate(Update u, int l, int r, int* arr, int* freq, ref int curAns)
        {
            if (u.Pos >= l && u.Pos <= r) { Remove(arr[u.Pos], freq, ref curAns); Add(u.OldVal, freq, ref curAns); }
            arr[u.Pos] = u.OldVal;
        }

        private static void UpdateRange(ref int curL, ref int curR, int targetL, int targetR, int* arr, int* freq, ref int curAns)
        {
            while (curL > targetL) { curL--; Add(arr[curL], freq, ref curAns); }
            while (curR < targetR) { curR++; Add(arr[curR], freq, ref curAns); }
            while (curL < targetL) { Remove(arr[curL], freq, ref curAns); curL++; }
            while (curR > targetR) { Remove(arr[curR], freq, ref curAns); curR--; }
        }

        private static void Add(int val, int* freq, ref int curAns) { if (freq[val] == 0) curAns++; freq[val]++; }
        private static void Remove(int val, int* freq, ref int curAns) { freq[val]--; if (freq[val] == 0) curAns--; }
    }
}
