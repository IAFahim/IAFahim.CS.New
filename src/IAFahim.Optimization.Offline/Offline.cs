namespace IAFahim.Optimization.Offline
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class ParallelBinarySearch
    {
        public static void Init(int* lo, int* hi, int n)
        {
            for (int i = 0; i < n; i++)
            {
                lo[i] = 0;
                hi[i] = -1;
            }
        }

        public static void InitWithRange(int* lo, int* hi, int n, int loVal, int hiVal)
        {
            for (int i = 0; i < n; i++)
            {
                lo[i] = loVal;
                hi[i] = hiVal;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mid(int lo, int hi)
        {
            return lo + ((hi - lo) >> 1);
        }

        public static int GroupByMid(int* lo, int* hi, int* queryIdx, int* bucketSize, int n, int* buckets)
        {
            int active = 0;
            for (int i = 0; i < n; i++)
            {
                int q = queryIdx[i];
                if (lo[q] >= hi[q]) continue;
                buckets[active++] = q;
            }
            for (int i = 1; i < active; i++)
            {
                int q = buckets[i];
                int mq = Mid(lo[q], hi[q]);
                int j = i - 1;
                while (j >= 0 && Mid(lo[buckets[j]], hi[buckets[j]]) > mq)
                {
                    buckets[j + 1] = buckets[j];
                    j--;
                }
                buckets[j + 1] = q;
            }
            if (bucketSize != null)
            {
                bucketSize[0] = active;
            }
            return active;
        }
    }

    public static unsafe class DivideConquerAnswer
    {
        public static void Solve(
            int* answers,
            int lo,
            int hi,
            int* queryIdx,
            int nQueries,
            void* context,
            delegate*<void*, int, void> applyFn,
            delegate*<void*, int, void> undoFn,
            delegate*<void*, int, bool> checkFn)
        {
            if (nQueries <= 0) return;
            if (lo == hi)
            {
                for (int i = 0; i < nQueries; i++)
                {
                    answers[queryIdx[i]] = lo;
                }
                return;
            }

            int mid = lo + ((hi - lo) >> 1);
            for (int v = lo; v <= mid; v++)
            {
                applyFn(context, v);
            }

            long halfBytes = (long)nQueries * sizeof(int);
            int* left = (int*)Marshal.AllocHGlobal((nint)halfBytes);
            int* right = (int*)Marshal.AllocHGlobal((nint)halfBytes);
            try
            {
                int leftCount = 0;
                int rightCount = 0;
                for (int i = 0; i < nQueries; i++)
                {
                    int q = queryIdx[i];
                    if (checkFn(context, q))
                    {
                        left[leftCount++] = q;
                    }
                    else
                    {
                        right[rightCount++] = q;
                    }
                }

                for (int v = mid; v >= lo; v--)
                {
                    undoFn(context, v);
                }

                Solve(answers, lo, mid, left, leftCount, context, applyFn, undoFn, checkFn);

                for (int v = lo; v <= mid; v++)
                {
                    applyFn(context, v);
                }

                Solve(answers, mid + 1, hi, right, rightCount, context, applyFn, undoFn, checkFn);

                for (int v = mid; v >= lo; v--)
                {
                    undoFn(context, v);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)right);
                Marshal.FreeHGlobal((nint)left);
            }
        }
    }

    public static unsafe class Cdq3DDominance
    {
        public static void SortByX(int* x, int* y, int* z, int* idx, int* tmp, int l, int r)
        {
            if (l >= r) return;
            int mid = l + ((r - l) >> 1);
            SortByX(x, y, z, idx, tmp, l, mid);
            SortByX(x, y, z, idx, tmp, mid + 1, r);
            int i = l, j = mid + 1, k = l;
            while (i <= mid && j <= r)
            {
                if (x[idx[i]] <= x[idx[j]])
                {
                    tmp[k++] = idx[i++];
                }
                else
                {
                    tmp[k++] = idx[j++];
                }
            }
            while (i <= mid) tmp[k++] = idx[i++];
            while (j <= r) tmp[k++] = idx[j++];
            for (i = l; i <= r; i++) idx[i] = tmp[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountDominance(int* y, int* z, int* idx, int* count, int l, int mid, int r,
            int* bit, delegate*<int*, int, int, void> bitAdd, delegate*<int*, int, int> bitSum)
        {
            int i = l, j = mid + 1;
            while (j <= r)
            {
                while (i <= mid && y[idx[i]] <= y[idx[j]])
                {
                    bitAdd(bit, z[idx[i]], 1);
                    i++;
                }
                count[idx[j]] += bitSum(bit, z[idx[j]]);
                j++;
            }
            for (int t = l; t < i; t++)
            {
                bitAdd(bit, z[idx[t]], -1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MergeByY(int* y, int* idx, int* tmp, int l, int mid, int r)
        {
            int i = l, j = mid + 1, k = l;
            while (i <= mid && j <= r)
            {
                if (y[idx[i]] <= y[idx[j]]) tmp[k++] = idx[i++];
                else tmp[k++] = idx[j++];
            }
            while (i <= mid) tmp[k++] = idx[i++];
            while (j <= r) tmp[k++] = idx[j++];
            for (int t = l; t <= r; t++) idx[t] = tmp[t];
        }

        public static void Process(int* x, int* y, int* z, int* idx, int* tmp, int* count, int l, int r,
            int* bit, int maxZ,
            delegate*<int*, int, int, void> bitAdd,
            delegate*<int*, int, int> bitSum)
        {
            if (l >= r) return;
            int mid = l + ((r - l) >> 1);
            Process(x, y, z, idx, tmp, count, l, mid, bit, maxZ, bitAdd, bitSum);
            Process(x, y, z, idx, tmp, count, mid + 1, r, bit, maxZ, bitAdd, bitSum);
            CountDominance(y, z, idx, count, l, mid, r, bit, bitAdd, bitSum);
            MergeByY(y, idx, tmp, l, mid, r);
        }
    }

    public static unsafe class OfflineKthNumber
    {
        public static int BuildPersistentSegTree(
            int* leftChild, int* rightChild, int* sum,
            int* version, int prevVersion,
            int l, int r, int idx, int* allocCnt)
        {
            int node = ++(*allocCnt);
            leftChild[node] = prevVersion > 0 ? leftChild[prevVersion] : 0;
            rightChild[node] = prevVersion > 0 ? rightChild[prevVersion] : 0;
            sum[node] = prevVersion > 0 ? sum[prevVersion] + 1 : 1;
            if (l == r) return node;
            int mid = l + ((r - l) >> 1);
            if (idx <= mid)
            {
                leftChild[node] = BuildPersistentSegTree(
                    leftChild, rightChild, sum, leftChild, prevVersion > 0 ? leftChild[prevVersion] : 0,
                    l, mid, idx, allocCnt);
            }
            else
            {
                rightChild[node] = BuildPersistentSegTree(
                    leftChild, rightChild, sum, rightChild, prevVersion > 0 ? rightChild[prevVersion] : 0,
                    mid + 1, r, idx, allocCnt);
            }
            return node;
        }

        public static int QueryKth(int* leftChild, int* rightChild, int* sum,
            int node, int l, int r, int k)
        {
            if (l == r) return l;
            int leftCount = leftChild[node] != 0 ? sum[leftChild[node]] : 0;
            int mid = l + ((r - l) >> 1);
            if (k <= leftCount)
            {
                return QueryKth(leftChild, rightChild, sum, leftChild[node], l, mid, k);
            }
            return QueryKth(leftChild, rightChild, sum, rightChild[node], mid + 1, r, k - leftCount);
        }
    }
}
