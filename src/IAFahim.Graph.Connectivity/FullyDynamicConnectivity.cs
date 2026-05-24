namespace IAFahim.Graph.Connectivity
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class FullyDynamicConnectivity
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct EdgeEvent : IComparable<EdgeEvent>
        {
            public int U, V, Time, Type, Id;
            public int CompareTo(EdgeEvent o)
            {
                if (U != o.U) return U.CompareTo(o.U);
                if (V != o.V) return V.CompareTo(o.V);
                if (Type != o.Type) return Type.CompareTo(o.Type);
                return Time.CompareTo(o.Time);
            }
        }

        public static void Run(int* qU, int* qV, int* qT, int q, int* parent, int* size, int n, int* ans,
                               EdgeEvent* scratchE, EdgeEvent* scratchS, int* scratchM, EdgeInterval* scratchEd, RollbackOp* scratchH)
        {
            if (q == 0) return;
            InitializeEvents(qU, qV, qT, q, scratchE, ans);
            
            Buffer.MemoryCopy(scratchE, scratchS, q * sizeof(EdgeEvent), q * sizeof(EdgeEvent));
            SortEdgeEvents(scratchS, 0, q - 1);

            MatchEvents(q, scratchS, scratchM);
            int edgeCount = BuildIntervals(q, scratchE, scratchM, scratchEd);

            int historyCount = 0;
            OfflineDynamicMst.Init(parent, size, n);
            OfflineDynamicConnectivity.Solve(0, q - 1, scratchEd, edgeCount, parent, size, scratchH, ref historyCount, ans, qT, qU, qV);
        }

        private static void InitializeEvents(int* qU, int* qV, int* qT, int q, EdgeEvent* events, int* ans)
        {
            for (int i = 0; i < q; i++)
            {
                int u = qU[i], v = qV[i];
                if (u > v) { int t = u; u = v; v = t; }
                events[i] = new EdgeEvent { U = u, V = v, Time = i, Type = qT[i], Id = i };
                ans[i] = -1;
            }
        }

        private static void MatchEvents(int q, EdgeEvent* sorted, int* match)
        {
            for (int i = 0; i < q; i++) match[i] = -1;
            for (int i = 0; i < q; i++)
            {
                if (sorted[i].Type == 0) // Add
                {
                    int matchedId = -1;
                    for (int j = i + 1; j < q && sorted[j].U == sorted[i].U && sorted[j].V == sorted[i].V; j++)
                        if (sorted[j].Type == 1 && match[sorted[j].Id] == -1) { matchedId = sorted[j].Id; match[sorted[j].Id] = sorted[i].Id; break; }
                    match[sorted[i].Id] = matchedId != -1 ? matchedId : q;
                }
            }
        }

        private static int BuildIntervals(int q, EdgeEvent* events, int* match, EdgeInterval* edges)
        {
            int ec = 0;
            for (int i = 0; i < q; i++)
                if (events[i].Type == 0)
                {
                    int endTime = match[i];
                    edges[ec++] = new EdgeInterval { U = events[i].U, V = events[i].V, StartTime = i, EndTime = (endTime == -1 ? q : endTime) - 1 };
                }
            return ec;
        }

        private static void SortEdgeEvents(EdgeEvent* arr, int left, int right)
        {
            if (left >= right) return;
            int i = left, j = right;
            EdgeEvent pivot = arr[left + (right - left) / 2];
            while (i <= j)
            {
                while (arr[i].CompareTo(pivot) < 0) i++;
                while (arr[j].CompareTo(pivot) > 0) j--;
                if (i <= j) { EdgeEvent t = arr[i]; arr[i] = arr[j]; arr[j] = t; i++; j--; }
            }
            if (left < j) SortEdgeEvents(arr, left, j);
            if (i < right) SortEdgeEvents(arr, i, right);
        }
    }
}
