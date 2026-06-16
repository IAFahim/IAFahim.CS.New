namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class BottleneckAssignment
    {
        private const int Unmatched = -1;
        private const int NoFeasibleAssignment = -1;

        // Solves the bottleneck (min-max) assignment problem on an n x n cost matrix
        // stored row-major in cost. Finds a perfect matching of rows to distinct columns
        // that minimizes the maximum used edge cost. Writes the chosen column for each row
        // into match[i] and returns the minimal feasible bottleneck cost, or
        // NoFeasibleAssignment (-1) when no perfect matching exists (only possible if n == 0
        // returns 0; a complete n x n matrix always admits a perfect matching for n > 0).
        // Caller guarantees cost is a valid n*n buffer and match has length n.
        public static int Run(int* cost, int n, int* match)
        {
            for (int i = 0; i < n; i++) match[i] = Unmatched;
            if (n == 0) return 0;

            int total = n * n;

            // Collect and sort all distinct candidate thresholds (the cost values).
            int* sorted = stackalloc int[total];
            for (int i = 0; i < total; i++) sorted[i] = cost[i];
            SortAscending(sorted, total);

            // Compact to distinct values to bound the binary search range.
            int distinct = 0;
            for (int i = 0; i < total; i++)
            {
                if (distinct == 0 || sorted[i] != sorted[distinct - 1])
                    sorted[distinct++] = sorted[i];
            }

            int* matchRight = stackalloc int[n];
            int* seen = stackalloc int[n];

            // Binary search for the smallest threshold index that yields a perfect matching.
            int lo = 0;
            int hi = distinct - 1;
            int answerIndex = NoFeasibleAssignment;
            while (lo <= hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                if (HasPerfectMatching(cost, n, sorted[mid], matchRight, seen))
                {
                    answerIndex = mid;
                    hi = mid - 1;
                }
                else
                {
                    lo = mid + 1;
                }
            }

            if (answerIndex == NoFeasibleAssignment) return NoFeasibleAssignment;

            int threshold = sorted[answerIndex];
            // Recompute the matching at the chosen threshold and write it into match[].
            HasPerfectMatching(cost, n, threshold, matchRight, seen);
            for (int v = 0; v < n; v++)
            {
                int u = matchRight[v];
                if (u != Unmatched) match[u] = v;
            }
            return threshold;
        }

        // Tests whether a perfect matching of all n rows exists using only edges with
        // cost[u*n+v] <= threshold. On success matchRight[v] holds the row matched to
        // column v (or Unmatched). Returns true iff every row was matched.
        private static bool HasPerfectMatching(int* cost, int n, int threshold, int* matchRight, int* seen)
        {
            for (int v = 0; v < n; v++) matchRight[v] = Unmatched;
            int matched = 0;
            for (int u = 0; u < n; u++)
            {
                for (int v = 0; v < n; v++) seen[v] = 0;
                if (TryAugment(u, cost, n, threshold, matchRight, seen)) matched++;
                else return false;
            }
            return matched == n;
        }

        private static bool TryAugment(int u, int* cost, int n, int threshold, int* matchRight, int* seen)
        {
            int rowBase = u * n;
            for (int v = 0; v < n; v++)
            {
                if (cost[rowBase + v] > threshold || seen[v] != 0) continue;
                seen[v] = 1;
                if (matchRight[v] == Unmatched ||
                    TryAugment(matchRight[v], cost, n, threshold, matchRight, seen))
                {
                    matchRight[v] = u;
                    return true;
                }
            }
            return false;
        }

        private static void SortAscending(int* a, int count)
        {
            // Iterative heapsort: in-place, O(count log count), no recursion/allocations.
            for (int start = (count >> 1) - 1; start >= 0; start--)
                SiftDown(a, start, count);
            for (int end = count - 1; end > 0; end--)
            {
                int tmp = a[0];
                a[0] = a[end];
                a[end] = tmp;
                SiftDown(a, 0, end);
            }
        }

        private static void SiftDown(int* a, int root, int count)
        {
            while (true)
            {
                int child = (root << 1) + 1;
                if (child >= count) break;
                if (child + 1 < count && a[child + 1] > a[child]) child++;
                if (a[root] >= a[child]) break;
                int tmp = a[root];
                a[root] = a[child];
                a[child] = tmp;
                root = child;
            }
        }
    }
}
