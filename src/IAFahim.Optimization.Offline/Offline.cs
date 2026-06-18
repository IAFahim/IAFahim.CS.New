namespace IAFahim.Optimization.Offline
{
    using System;
    using System.Runtime.CompilerServices;

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

        public static void GroupByMid(int* lo, int* hi, int* queryIdx, int* bucketSize, int n, int* buckets)
        {
            for (int i = 0; i < n; i++)
            {
                int mid = Mid(lo[queryIdx[i]], hi[queryIdx[i]]);
                buckets[mid * n + bucketSize[mid]] = queryIdx[i];
                bucketSize[mid]++;
            }
        }
    }

    public static unsafe class DivideConquerAnswer
    {
        public static void Solve<T>(
            T* answers,
            int lo, int hi,
            int* queryL, int* queryR, int* queryK,
            int nQueries,
            void* context,
            delegate*<void*, int, int, int, void> addFn,
            delegate*<void*, int, int, int, void> removeFn,
            delegate*<void*, int, bool> checkFn)
            where T : unmanaged
        {
            if (lo == hi || nQueries == 0) return;
            int mid = lo + ((hi - lo) >> 1);
            int leftCount = 0;
            int rightCount = 0;
            for (int i = 0; i < nQueries; i++)
            {
                int idx = i;
                if (queryR[idx] < lo || queryL[idx] > hi)
                {
                }
                else if (queryL[idx] <= lo && hi <= queryR[idx])
                {
                    while (queryK[idx] < mid)
                    {
                        if (!checkFn(context, queryK[idx]))
                        {
                            addFn(context, queryK[idx], 0, 0);
                        }
                        queryK[idx]++;
                    }
                    while (queryK[idx] > mid)
                    {
                        queryK[idx]--;
                        if (!checkFn(context, queryK[idx]))
                        {
                            removeFn(context, queryK[idx], 0, 0);
                        }
                    }
                }
                else
                {
                }
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

        public static void Process(int* x, int* y, int* z, int* idx, int* tmp, int* count, int l, int r,
            int* bit, int maxZ,
            delegate*<int*, int, int, void> bitAdd,
            delegate*<int*, int, int> bitSum)
        {
            if (l >= r) return;
            int mid = l + ((r - l) >> 1);
            Process(x, y, z, idx, tmp, count, l, mid, bit, maxZ, bitAdd, bitSum);
            Process(x, y, z, idx, tmp, count, mid + 1, r, bit, maxZ, bitAdd, bitSum);
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
            i = l; j = mid + 1;
            int k = l;
            while (i <= mid && j <= r)
            {
                if (y[idx[i]] <= y[idx[j]]) tmp[k++] = idx[i++];
                else tmp[k++] = idx[j++];
            }
            while (i <= mid) tmp[k++] = idx[i++];
            while (j <= r) tmp[k++] = idx[j++];
            for (int t = l; t <= r; t++) idx[t] = tmp[t];
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