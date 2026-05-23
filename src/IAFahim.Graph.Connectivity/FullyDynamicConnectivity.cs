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
            public int U;
            public int V;
            public int Time;
            public int Type;
            public int Id;

            public int CompareTo(EdgeEvent other)
            {
                if (U != other.U) return U.CompareTo(other.U);
                if (V != other.V) return V.CompareTo(other.V);
                if (Type != other.Type) return Type.CompareTo(other.Type);
                return Time.CompareTo(other.Time);
            }
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
                if (i <= j)
                {
                    EdgeEvent temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                    i++;
                    j--;
                }
            }
            if (left < j) SortEdgeEvents(arr, left, j);
            if (i < right) SortEdgeEvents(arr, i, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* queriesU, int* queriesV, int* queriesType, int q,
                               int* parent, int* size, int n, int* answers,
                               EdgeEvent* scratchEvents, EdgeEvent* scratchSorted,
                               int* scratchMatch, EdgeInterval* scratchEdges,
                               RollbackOp* scratchHistory)
        {
            if (q == 0) return;

            EdgeEvent* events = scratchEvents;
            for (int i = 0; i < q; i++)
            {
                events[i].U = queriesU[i];
                events[i].V = queriesV[i];
                if (events[i].U > events[i].V)
                {
                    int t = events[i].U; events[i].U = events[i].V; events[i].V = t;
                }
                events[i].Time = i;
                events[i].Type = queriesType[i];
                events[i].Id = i;
                answers[i] = -1;
            }

            EdgeEvent* sorted = scratchSorted;
            Buffer.MemoryCopy(events, sorted, q * sizeof(EdgeEvent), q * sizeof(EdgeEvent));
            SortEdgeEvents(sorted, 0, q - 1);

            int* match = scratchMatch;
            for (int i = 0; i < q; i++) match[i] = -1;

            for (int i = 0; i < q; i++)
            {
                if (sorted[i].Type == 0) // Add
                {
                    int j = i + 1;
                    int matchedId = -1;
                    while (j < q && sorted[j].U == sorted[i].U && sorted[j].V == sorted[i].V)
                    {
                        if (sorted[j].Type == 1 && match[sorted[j].Id] == -1) // Remove
                        {
                            matchedId = sorted[j].Id;
                            match[sorted[j].Id] = sorted[i].Id;
                            break;
                        }
                        j++;
                    }
                    match[sorted[i].Id] = matchedId != -1 ? matchedId : q;
                }
            }

            int edgeCount = 0;
            for (int i = 0; i < q; i++)
            {
                if (events[i].Type == 0) edgeCount++;
            }

            EdgeInterval* edges = scratchEdges;
            int ec = 0;
            for (int i = 0; i < q; i++)
            {
                if (events[i].Type == 0)
                {
                    edges[ec].U = events[i].U;
                    edges[ec].V = events[i].V;
                    edges[ec].StartTime = i;
                    int endTime = match[i];
                    if (endTime == -1) endTime = q;
                    edges[ec].EndTime = endTime - 1;
                    ec++;
                }
            }

            RollbackOp* history = scratchHistory;
            int historyCount = 0;
            OfflineDynamicMst.Init(parent, size, n);
            OfflineDynamicConnectivity.Solve(0, q - 1, edges, edgeCount, parent, size, history, ref historyCount, answers, queriesType, queriesU, queriesV);
        }
    }
}
