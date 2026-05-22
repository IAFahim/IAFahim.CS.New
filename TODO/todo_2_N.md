# todo_2_N.md

## 36. IAFahim.Math.NT.HighlyCompositeNumbers
**Problem:** `HighlyCompositeCandidate* candidates = stackalloc HighlyCompositeCandidate[20000];`
**Reason:** Allocates `20000 * 16` = 320 KB on the stack. High risk of Stack Overflow.
**Fix:** Require the caller to provide the buffer.
```csharp
public static int Run(long limit, long* result, HighlyCompositeCandidate* scratch)
{
    if (limit <= 0) return 0;
    int count = 0;
    Generate(0, 1, 1, 60, limit, scratch, ref count);
    QuickSort(scratch, 0, count - 1);
    int outCount = 0;
    long maxDivisors = 0;
    for (int i = 0; i < count; i++)
    {
        if (scratch[i].Divisors > maxDivisors)
        {
            maxDivisors = scratch[i].Divisors;
            result[outCount++] = scratch[i].Value;
        }
    }
    return outCount;
}

37. IAFahim.Math.NT.Min25Sieve

Problem: Magic threshold v > 1000 to decide between stackalloc and
Marshal.AllocHGlobal. Reason: Violates "no magic numbers" constraint. Dynamic
allocation behavior causes unpredictable performance. Fix: Remove magical
allocation completely. Force caller to pass a pre-allocated scratch space of
size O(sqrt(N)).

public static long PrimePi(long n, int* primes, bool* isPrime, long* w, long* g, int* map1, int* map2)
{
    if (n <= 1) return 0;
    long v = (long)Math.Sqrt((double)n);
    // Remove all Marshal.AllocHGlobal and stackalloc logic.
    // Proceed directly with logic using passed pointers.
    // ...

38. IAFahim.Math.NT.SmoothNumbers

Problem: int* primes = stackalloc int[10000]; Reason: 40 KB stack allocation
with magic number 10000. Limits base b artificially and risks Stack Overflow.
Fix: Require caller to provide primes array.

public static int Generate(int b, long limit, long* result, int* primes)
{
    if (limit <= 0 || b < 2) return limit >= 1 ? (result[0] = 1, 1) : 0;
    int primeCount = GetPrimes(b, primes);
    int count = 0;
    Gen(0, 1, primeCount, primes, limit, result, ref count);
    QuickSort(result, 0, count - 1);
    return count;
}

39. IAFahim.Math.NT.SquareFree

Problem: int* mu = stackalloc int[limit + 1]; where limit = (int)Math.Sqrt(n).
Reason: If n = 10^{12}, limit = 10^6. stackalloc tries to allocate 4 MB, causing
an immediate Stack Overflow. Fix: Require caller to provide the Mobius array mu.

public static long Count(long n, int* mu)
{
    if (n <= 0) return 0;
    int limit = (int)Math.Sqrt((double)n);
    SieveMobius(limit, mu);
    long ans = 0;
    for (int d = 1; d <= limit; d++)
        if (mu[d] != 0) ans += (long)mu[d] * (n / ((long)d * d));
    return ans;
}

40. IAFahim.Math.Transform.MinMaxConvolution

Problem: long val = a[i] + b[j]; inside SolveMinPlus. Reason: If a[i] or b[j] is
long.MaxValue (infinity), addition overflows to negative, corrupting the minimum
search. Fix: Safeguard addition.

long candA = a[i];
long candB = b[j];
if (candA != long.MaxValue && candB != long.MaxValue)
{
    long val = candA + candB;
    if (val < bestVal) { bestVal = val; bestJ = j; }
}

41. IAFahim.Optimization.DivideConquer.MatrixSearch

Problem: int mid = (lo + hi) >> 1; in Run(int m, int n, int* a, int target).
Reason: lo + hi can exceed int.MaxValue for large matrices, causing negative
mid. Fix: Use safe midpoint calculation.

int mid = lo + ((hi - lo) >> 1);

42. IAFahim.Optimization.DivideConquer.SlopeTrick

Problem: AddAbs modifies Lc and Rc linearly. Slope trick requires maintaining
piecewise linear slopes via a Priority Queue, not fixed fields. Reason: The
current implementation only works for a single |x - a| operation and breaks upon
multiple additions. Fix: Convert to a true multiset/priority-queue-based slope
trick utilizing caller-provided unmanaged max-heap and min-heap pointers.

public static void AddAbs(State* s, long a, long* leftHeap, ref int leftSize, long* rightHeap, ref int rightSize)
{
    // Proper Slope Trick inserts 'a' into both left and right bounds, 
    // balancing heaps and accumulating offset.
    // ...
}

43. IAFahim.Optimization.Exact.HamiltonianCycle

Problem: long cand = dp[mask * n + last] + wlu; Reason: Adding edge weights to
long.MaxValue (infinity) causes overflow to negatives. Fix: Safeguard addition
against infinity.

long current = dp[mask * n + last];
if (current != inf && wlu != inf)
{
    long cand = current + wlu;
    if (cand < dp[newMask * n + u]) dp[newMask * n + u] = cand;
}

44. IAFahim.Optimization.Exact.HamiltonianPath

Problem: long cand = dp[mask * n + v] + wvu; Reason: Overflow possible when
adding to inf. Fix: Safeguard addition.

long current = dp[mask * n + v];
if (current != inf && wvu != inf)
{
    long cand = current + wvu;
    int newMask = mask | (1 << u);
    if (cand < dp[newMask * n + u]) dp[newMask * n + u] = cand;
}

45. IAFahim.Optimization.Exact.MaximumClique

Problem: int* tmp is shared across all recursive levels. for (int i = 0; i <
candSize; i++) tmp[i] = cand[i]; overwrites previous levels. Reason: Recursive
algorithms cannot share a single flat scratch buffer without depth offsets. Fix:
Offset tmp by depth * n for each recursive call.

int* currentTmp = tmp + depth * n;
for (int i = 0; i < candSize; i++) currentTmp[i] = cand[i];
int count = 0;
while (candSize > 0)
{
    if (solSize + candSize <= *best) return 0;
    int v = currentTmp[0];
    int sz = 0;
    for (int i = 0; i < candSize; i++)
        if (adj[v * n + currentTmp[i]]) cand[sz++] = currentTmp[i];
    // ...
}

46. IAFahim.Optimization.Exact.MaxIndependentSet

Problem: int* tmp is shared across recursion without depth offset. Reason:
Modifying tmp in deep recursive calls corrupts state for shallow calls. Fix:
Offset tmp by depth * n.

private static void Search(int n, bool* adj, int* used, int v, int* best, int cur, int depth, int* tmp)
{
    if (v == n) { if (cur > *best) *best = cur; return; }
    int* currentTmp = tmp + depth * n;
    // ... use currentTmp instead of tmp
}

47. IAFahim.Optimization.Exact.MinDominatingSet

Problem: Modifying dom[u] = 1 and dom[u] = 0 naively during backtracking.
Reason: If multiple selected vertices cover u, backtracking dom[u] = 0 uncovers
it entirely, corrupting the coverage state. Fix: Use a coverage counter instead
of a boolean.

dom[vi2]++;
for (int u = 0; u < n; u++) if (adj[vi2 * n + u]) dom[u]++;
Search(n, adj, dom, idx + 1, used + 1, best, order);
dom[vi2]--;
for (int u = 0; u < n; u++) if (adj[vi2 * n + u]) dom[u]--;

48. IAFahim.Optimization.Exact.MinSetCover

Problem: if (!hasOther) covered[elem] = 0; recalculates coverage in
O(M \times |Set|) per backtrack step. Reason: Overly slow and error-prone. Fix:
Use a frequency counter for coverage (covered[elem]++ / --).

for (int j = 0; j < setSizes[i]; j++) {
    int elem = sets[i][j];
    if (covered[elem] == 0) added++;
    covered[elem]++;
}
// Search ...
for (int j = 0; j < setSizes[i]; j++) covered[sets[i][j]]--;

49. IAFahim.Optimization.Exact.SteinerDreyfusWagner

Problem: long cand = dp[mask * n + i] + w[i * n + j] + w[j * n + k]; Reason:
Adding multiple elements that could be inf leads to overflow. Fix: Check for inf
before addition.

long d1 = dp[mask * n + i];
long w1 = w[i * n + j];
long w2 = w[j * n + k];
if (d1 != inf && w1 != inf && w2 != inf)
{
    long cand = d1 + w1 + w2;
    if (cand < dp[mask * n + k]) dp[mask * n + k] = cand;
}

50. IAFahim.Optimization.Exact.TspBitonic

Problem: Initial DP table contains -1 but long cand = dp[k * n + i - 1] +
Dist(...) assumes valid distance. Reason: Corrupts minimal distance calculations
by adding distances to -1. Fix: Properly initialize DP array and validate state.

for (int i = 0; i < n * n; i++) dp[i] = long.MaxValue;
// ...
long d = dp[k * n + i - 1];
if (d != long.MaxValue) {
    long cand = d + Dist(xs[k], ys[k], xs[i], ys[i]);
    if (cand < best) best = cand;
}

51. IAFahim.Optimization.Games.AttractorSet

Problem: bool* inAttr = stackalloc bool[n]; Reason: Stack overflow risk for
large n. Fix: Pass inAttr and queue buffers from the caller.

public static bool Solve(int n, bool* player, bool* adj, bool* even, int start, bool* inAttr, int* queue)
{
    for (int i = 0; i < n; i++) inAttr[i] = false;
    int head = 0, tail = 0;
    // ...

52. IAFahim.Optimization.Games.Grundy

Problem: int* vals = stackalloc int[counts[i]]; inside loop over n. Reason:
Accumulates stack allocations, causing Stack Overflow. Fix: Move allocation
outside the loop or pass scratch array.

public static void SpragueGrundy(int* moves, int* counts, int n, int* g, int* scratch)
{
    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < counts[i]; j++) scratch[j] = g[moves[i * 10 + j]];
        g[i] = Mex(scratch, counts[i]);
    }
}

53. IAFahim.Optimization.Games.Mdp

Problem: double* newV = stackalloc double[n]; Reason: Stack Overflow for large
state spaces. Fix: Ask caller to provide the buffer.

public static void ValueIteration(int n, int m, double* trans, double* reward, double gamma, double* v, double* newV, int iters)
{
    // ...

54. IAFahim.Optimization.Geometric.MinEnclosingBall

Problem: Allocates int* p = (int*)Marshal.AllocHGlobal(n * sizeof(int));
internally. Reason: Expensive allocation and cleanup inside a tight geometric
routine. Fix: Require scratch parameter.

public static Circle Welzl(double* xs, double* ys, int n, int* p)
{
    for (int i = 0; i < n; i++) p[i] = i;
    // ...

55. IAFahim.Optimization.Geometric.WelzlSphere

Problem: Processes points sequentially without random shuffle. Reason: Welzl's
algorithm degrades to O(N^4) without randomized insertion order. Fix: Add random
shuffle logic before processing.

ulong seed = 123456789;
for (int i = n - 1; i > 0; i--)
{
    seed = seed * 6364136223846793005UL + 1442695040888963407UL;
    int j = (int)(seed % (ulong)(i + 1));
    double tx = xs[i]; xs[i] = xs[j]; xs[j] = tx;
    double ty = ys[i]; ys[i] = ys[j]; ys[j] = ty;
    double tz = zs[i]; zs[i] = zs[j]; zs[j] = tz;
}

56. IAFahim.Optimization.Knapsack.BoundedKnapsack

Problem: long* dp = stackalloc long[cap + 1]; in BinarySplit and MonotoneQueue.
Reason: Stack overflow for large capacities. Fix: Pass dp from caller.

public static long BinarySplit(long* w, long* v, int* cnt, int n, int cap, long* dp)
{
    // ...

57. IAFahim.Optimization.Knapsack.DivideConquerKnapsack

Problem: long* dp = stackalloc long[cap + 1]; Reason: Stack overflow. Fix: Pass
dp array.

public static long Run(long* w, long* v, int* cnt, int n, int cap, long* dp)
{
    // ...

58. IAFahim.Optimization.Knapsack.MeetInMiddle

Problem: long* left = stackalloc long[leftCount * 2]; where leftCount = 1 << 20.
Reason: 16 MB allocation on the stack! Stack overflow. Fix: Require caller to
pass left array.

public static long Run(long* w, long* v, int n, long cap, long* left)
{
    // ...

59. IAFahim.Optimization.Knapsack.MultipleChoiceKnapsack

Problem: long* dp = stackalloc long[cap + 1]; Reason: Stack overflow. Fix: Pass
dp array.

public static long Run(int* groupStart, int* itemW, long* itemV, int n, int cap, long* dp)
{
    // ...

60. IAFahim.Optimization.Offline.ParallelBinarySearch

Problem: public static int* buckets; Reason: Global state in a static class
violates thread-safety and causes data corruption on concurrent use. Fix: Pass
buckets as an argument.

public static void GroupByMid(int* lo, int* hi, int* queryIdx, int* bucketSize, int n, int* buckets)
{
    for (int i = 0; i < n; i++)
    {
        int mid = Mid(lo[queryIdx[i]], hi[queryIdx[i]]);
        buckets[mid + bucketSize[0]++] = queryIdx[i];
    }
}

61. IAFahim.Optimization.Offline.Cdq3DDominance

Problem: bitAdd(bit, z[idx[i]], 1); passes maxY instead of maxZ. Reason: z
coordinates represent the BIT indexing axis, so maxZ must define the tree size.
Fix: Change parameter to maxZ.

public static void Process(int* x, int* y, int* z, int* idx, int l, int r,
    int* bit, int maxZ,
    delegate*<int*, int, int, void> bitAdd,
    delegate*<int*, int, int> bitSum)

62. IAFahim.Graph.DAG.DagCountingPaths

Problem: pathCount[u] += pathCount[v]; without bounds. Reason: Path counts in a
DAG grow exponentially (O(2^N)), leading to fast silent overflow of long. Fix:
Apply modulo or saturate to long.MaxValue. Let's use saturating addition.

long add = pathCount[v];
if (long.MaxValue - pathCount[u] < add) pathCount[u] = long.MaxValue;
else pathCount[u] += add;

63. IAFahim.Graph.DAG.DagLexicographicKthPath

Problem: Checks if (k <= pathCount[v]) safely, but expects valid path counts.
Reason: If path count saturated to long.MaxValue, the comparison is safe. No fix
needed if #62 is applied correctly.

64. IAFahim.Graph.DAG.IncrementalCycleDetection

Problem: for(int i=0; i<n; i++) visited[i] = 0; inside AddEdge. Reason: Makes
the check strictly O(N + E), degrading performance for many edge additions. Fix:
Pass a runId parameter and set visited[curr] = runId instead of resetting the
array.

public static bool AddEdge(int u, int v, int* head, int* next, int* to, int* edgeCount, int* visited, int runId)
{
    // ...
    return !Dfs(v, u, head, next, to, visited, runId);
}

65. IAFahim.Graph.Flow.Flow

Problem: MinHeap allocates using Marshal.AllocHGlobal but SuccessiveShortestPath
creates it repeatedly without proper pooling. Reason: Constant heap allocation
per flow path degrades performance. Fix: Pull MinHeap creation outside the SSP
while (true) loop.

var pq = new MinHeap(n);
try {
    while (PotentialDijkstra.Run(n, s, t, head, to, next, cap, cost, pot, dist, parent, parentEdge, ref pq)) {
        // ...
    }
} finally { pq.Dispose(); }

66. IAFahim.Graph.Flow.FlowDecompose

Problem: int* parent = stackalloc int[n]; inside while (!done) loop. Reason:
stackalloc inside loops accumulates stack usage until Stack Overflow. Fix: Move
outside loop.

int* parent = stackalloc int[n];
int* edgeId = stackalloc int[n];
int* q = stackalloc int[n];
while (!done)
{
    for (int i = 0; i < n; i++) { parent[i] = -1; edgeId[i] = -1; }
    // ...

67. IAFahim.Graph.Flow.MinCostMaxFlow

Problem: var pq = new MinHeap(n); inside while (true) loop. Reason: Heavy
unmanaged allocations in a tight loop. Memory fragmentation and slow
performance. Fix: Move pq instantiation outside the loop. Add Clear() to
MinHeap.

var pq = new MinHeap(n);
try {
    while (true) {
        pq.Size = 0;
        // ...

68. IAFahim.Graph.Flow.MinCostFlowDijkstra

Problem: O(N^2) Dijkstra using nested loops for (int j = 0; j < n; j++). Reason:
Extremely slow for sparse graphs. Fix: Utilize the existing MinHeap for
O(E \log V) resolution.

public static void Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* dist, int* parent, int* parentEdge, int* pot, ref MinHeap pq)
{
    for (int i = 0; i < n; i++) dist[i] = int.MaxValue;
    dist[s] = 0;
    pq.Size = 0;
    pq.PushOrUpdate(s, 0);
    while (pq.Size > 0)
    {
        int u = pq.Pop(out long d);
        if (d != dist[u]) continue;
        for (int e = head[u]; e != -1; e = next[e])
        {
            if (cap[e] - flow[e] > 0)
            {
                int v = to[e];
                int w = cost[e] + pot[u] - pot[v];
                if (dist[u] + w < dist[v])
                {
                    dist[v] = dist[u] + w;
                    parent[v] = u;
                    parentEdge[v] = e;
                    pq.PushOrUpdate(v, dist[v]);
                }
            }
        }
    }
}

69. IAFahim.Graph.Flow.MaxFlowLowerBounds

Problem: int* newHead = stackalloc int[nn]; and int* newTo = stackalloc int[2 *
n + 100]; Reason: 100 is a magic number for extra edges. Doesn't account for M
original edges. Fix: Compute required size exactly based on m + n. Require
arrays to be passed by caller.

public static long Run(int n, int m, int s, int t, int* head, int* to, int* next, int* lower, int* upper, int* flow, int* newHead, int* newTo, int* newNext, int* newCap, int* newCost)

70. IAFahim.Graph.Tree.LcaBuild

Problem: Binary lifting table ancestors array uses int** structure. Reason:
int** means an array of pointers, requiring disjoint memory allocations for each
row, violating simple flat array structures typical of high-performance code.
Fix: Use flat array int* ancestors indexed by i * logN + j.

public static void Run(int n, int root, int* head, int* to, int* next, int* parent, int* depth, int* ancestors, int logN)
{
    // ...
    for (int i = 0; i < n; i++) ancestors[i * logN + 0] = parent[i] < 0 ? i : parent[i];
    for (int j = 1; j < logN; j++)
        for (int i = 0; i < n; i++)
            ancestors[i * logN + j] = ancestors[ancestors[i * logN + (j - 1)] * logN + (j - 1)];
}

