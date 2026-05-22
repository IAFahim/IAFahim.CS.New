namespace IAFahim.Graph.TreeDecomposition
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MoQuery
    {
        public int Id;
        public int L;
        public int R;
        public int Lca;
        public int BlockId;
    }

    public static unsafe class MoAlgorithmOnTree
    {
        public static void BuildEulerTour(
            int u, int p,
            int* head, int* to, int* next,
            int* euler, ref int timer,
            int* inTime, int* outTime)
        {
            inTime[u] = timer;
            euler[timer++] = u;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    BuildEulerTour(v, u, head, to, next, euler, ref timer, inTime, outTime);
                }
            }
            outTime[u] = timer;
            euler[timer++] = u;
        }

        private static void QSort(MoQuery* queries, int left, int right)
        {
            if (left >= right)
            {
                return;
            }
            int i = left, j = right;
            MoQuery pivot = queries[(left + right) / 2];
            while (i <= j)
            {
                while (Compare(queries[i], pivot) < 0) i++;
                while (Compare(queries[j], pivot) > 0) j--;
                if (i <= j)
                {
                    MoQuery t = queries[i];
                    queries[i] = queries[j];
                    queries[j] = t;
                    i++;
                    j--;
                }
            }
            QSort(queries, left, j);
            QSort(queries, i, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Compare(MoQuery a, MoQuery b)
        {
            if (a.BlockId != b.BlockId)
            {
                return a.BlockId.CompareTo(b.BlockId);
            }
            if ((a.BlockId & 1) != 0)
            {
                return a.R.CompareTo(b.R);
            }
            else
            {
                return b.R.CompareTo(a.R);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortQueries(MoQuery* queries, int q)
        {
            QSort(queries, 0, q - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Toggle(int node, byte* onPath, void* context, delegate*<int, void*, void> addFn, delegate*<int, void*, void> removeFn)
        {
            if (onPath[node] == 0)
            {
                onPath[node] = 1;
                addFn(node, context);
            }
            else
            {
                onPath[node] = 0;
                removeFn(node, context);
            }
        }

        public static void TreeMoQuery(
            int n, int q,
            int* euler, int* inTime, int* outTime,
            int* depth, int* parent, // standard parent and depth array from LCA
            MoQuery* queries,
            delegate*<int, void*, void> addFn,
            delegate*<int, void*, void> removeFn,
            delegate*<int, int, void*, void> queryFn, // queries[i].Id, queryIndex, context
            void* context,
            int blockSize)
        {
            byte* onPath = stackalloc byte[n];
            for (int i = 0; i < n; i++)
            {
                onPath[i] = 0;
            }

            int curl = 0;
            int curr = -1;

            for (int i = 0; i < q; i++)
            {
                int l = queries[i].L;
                int r = queries[i].R;
                int lca = queries[i].Lca;

                while (curr < r)
                {
                    curr++;
                    Toggle(euler[curr], onPath, context, addFn, removeFn);
                }
                while (curr > r)
                {
                    Toggle(euler[curr], onPath, context, addFn, removeFn);
                    curr--;
                }
                while (curl < l)
                {
                    Toggle(euler[curl], onPath, context, addFn, removeFn);
                    curl++;
                }
                while (curl > l)
                {
                    curl--;
                    Toggle(euler[curl], onPath, context, addFn, removeFn);
                }

                if (lca != -1)
                {
                    Toggle(lca, onPath, context, addFn, removeFn);
                }

                queryFn(queries[i].Id, i, context);

                if (lca != -1)
                {
                    Toggle(lca, onPath, context, addFn, removeFn);
                }
            }
        }
    }
}
