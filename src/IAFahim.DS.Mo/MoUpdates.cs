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
        public static void Run(int n, int* arr, int qCount, Query3D* queries, int uCount, Update* updates, int* ans, int blockSize)
        {
            SortQueries(queries, qCount, blockSize);

            int curL = 0, curR = -1, curT = 0;
            int* freq = stackalloc int[1000001]; // Assuming max value
            int curAns = 0;

            for (int i = 0; i < qCount; i++)
            {
                Query3D q = queries[i];
                UpdateT(ref curT, q.T, curL, curR, arr, updates, freq, ref curAns);
                UpdateRange(ref curL, ref curR, q.L, q.R, arr, freq, ref curAns);
                ans[q.Id] = curAns;
            }
        }

        private static void SortQueries(Query3D* queries, int q, int b)
        {
            // Simple insertion sort for small q, or QuickSort for large q.
            // Using a simple lambda-based comparison for brevity in refactor.
            for (int i = 0; i < q; i++)
            {
                for (int j = i + 1; j < q; j++)
                {
                    if (CompareQueries(queries[i], queries[j], b) > 0)
                    {
                        Query3D t = queries[i]; queries[i] = queries[j]; queries[j] = t;
                    }
                }
            }
        }

        private static int CompareQueries(Query3D a, Query3D b, int blk)
        {
            int al = a.L / blk, bl = b.L / blk;
            if (al != bl) return al.CompareTo(bl);
            int ar = a.R / blk, br = b.R / blk;
            if (ar != br) return ar.CompareTo(br);
            return a.T.CompareTo(b.T);
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
