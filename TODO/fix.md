# Bug Report: Correctness Errors in the Algorithm Library

I went through the whole library. Below is every genuine correctness bug I found, grouped by severity. I'm distinguishing **wrong-answer bugs** (produces incorrect results) from **crash/UB bugs** (out-of-bounds, infinite loops, overflow) from **API contract violations** (signature lies about what it does).

I'll be direct about what's broken and why, with the minimal fix where it's short.

---

## Critical: Fenwick tree traversal is fundamentally broken

This is the most damaging one because Fenwick is used everywhere.

`IAFahim.DS.Fenwick/Fenwick.cs` and `FenwickOps.cs` use a **0-based** index convention inconsistently and the update/query loops are wrong.

`Fenwick.AddInt64`:
```cs
idx = (idx + 1) & -idx;   // WRONG
```
The standard 1-indexed update is `idx += idx & -idx`. Your `(idx + 1) & -idx` is not the Fenwick step at all. `SumInt64` has the mirror error:
```cs
idx = (idx + 1) & -idx;   // should be idx -= idx & -idx (1-indexed) or use the 0-indexed variant correctly
```
With `idx = 0`, `-idx == 0`, so `(0+1)&0 == 0` and your `if (idx == 0) break;` masks the breakage by terminating — but for any nonzero index the recurrence visits the wrong cells. Every `Fenwick`, `FenwickAdd`, `FenwickSum`, `FenwickRangeSum`, `FenwickPointQuery`, `FenwickRangeAdd` built on this is wrong. `Fenwick2DAdd`/`Fenwick2DSum` use the correct `i += i & -i` / `i -= i & -i`, so those are fine — which makes the inconsistency more dangerous because the 2D ones look like a model the 1D ones don't follow.

`FenwickRangeAdd.Run` is also conceptually confused — it does two point-updates to the *same* `idx` (a value-BIT and an index-weighted BIT) but they're stored in the **same array** `bit`, so they corrupt each other. A range-add/range-sum Fenwick needs two separate arrays.

`FenwickLowerBound`/`Fenwick.LowerBoundInt64`: the `Math.Min(idx, n - 1)` clamp hides off-by-one but the walk assumes a 1-indexed tree with power-of-two strides; combined with the broken `Add`, results are meaningless.

**Fix:** pick one convention. The cleanest is 1-indexed:
```cs
public static void AddInt64(long* bit, int n, int idx, long val) {
    for (idx += 1; idx <= n; idx += idx & -idx) bit[idx] += val;
}
public static long SumInt64(long* bit, int idx) {
    long res = 0;
    for (idx += 1; idx > 0; idx -= idx & -idx) res += bit[idx];
    return res;
}
```
(and size arrays as `n+1`). The `& -idx` from the *unincremented* idx is the bug signature throughout.

---

## Critical: `Kosaraju` reads garbage as the DFS order

`IAFahim.Graph/GraphTopo.cs`, `Kosaraju`:

```cs
public static void FirstDfs(...) {
    ...
    order[0] = u;     // overwrites slot 0 every time
    order[1]++;       // uses order[1] as a counter
}
```
`FirstDfs` is supposed to record a post-order list, but it writes every finished vertex to `order[0]` and bumps a counter in `order[1]`. Then:
```cs
for (int i = n - 1; i >= 0; i--) {
    int v = order[i];   // reads uninitialized order[2..n-1]
    ...
}
```
So the second pass iterates over uninitialized memory. SCC results are completely wrong. You need a real stack:
```cs
// pass an int* order and int* topPtr; push u after recursing children
order[(*top)++] = u;
```
and process `order` from `top-1` down.

---

## Critical: `TarjanScc` recursion can blow the C# stack but logic is OK; `CondenseGraph` allocates `n*n` on the stack

`CondenseGraph`:
```cs
bool* seen = stackalloc bool[n * n];
```
For any non-tiny `n` this is a stack overflow. Use a hashed edge set or sort, or at minimum document the `n` bound. Not a logic bug, but a guaranteed crash on real inputs.

---

## Critical: `Toposort`/`KahnToposort` use `next[e]` with the wrong sentinel

Adjacency lists built by `GraphBasic.AddEdge` use `head[u] == 0` / `next[e]` terminating at `0`, with edge ids starting at `1` (`++(*edgeId)`). That's consistent. **But** `IAFahim.Graph.Misc/GraphMisc.cs` (`TopologicalDp`, `SccDp`) iterate with `e != -1`:
```cs
for (int e = head[u]; e != -1; e = next[e])   // wrong sentinel
```
These will read edge 0 (unused/garbage) and never terminate correctly against a `0`-terminated list. The `Graph.Misc` family assumes `-1` termination while the rest of the library uses `0`. Pick one. Given `AddEdge` uses `0`, `GraphMisc` is the outlier and is broken against the library's own builder.

---

## Wrong answer: `MonotonicQueueMin`/`SlidingWindow*` window-eviction off-by-one

`IAFahim.DS.Heap/Heap.cs`, `MonotonicQueueMin`; also `Search.Window.SlidingWindowMin/Max`:
```cs
if (deque[front] <= i - windowSize) front++;
```
The front index should be evicted when `deque[front] <= i - windowSize`, i.e. `deque[front] < i - windowSize + 1`. That's actually correct... **but** it only evicts once per step (`if`, not `while`). With distinct indices pushed one per iteration only one can expire per step, so `if` is fine here. **However** `MonotonicQueuePush.MinInt32` stores *values*, not indices, and can never evict — so it's only a min-of-prefix, not a sliding window. The naming implies windowing it doesn't do.

The real bug: `SlidingWindowMin/Max` write `dst[i - windowSize + 1]`. For `windowSize > len` this indexes negative. Guard `windowSize <= len`.

---

## Wrong answer: `SccDp` / `TopologicalDp` aside, `DagReachability` indexing

`Graph.Misc.DagReachability` sets the self-bit then ORs children — correct in spirit — but uses `e != -1` (same sentinel bug). Fix sentinel.

---

## Wrong answer: `LowerBoundInt64` / segment-tree descent assume max stride `1<<20`

`Fenwick.LowerBoundInt64`, `FenwickLowerBound`:
```cs
for (int bitMask = 1 << 20; bitMask != 0; bitMask >>= 1)
```
Hard-codes a maximum size of ~10^6. For larger `n` this silently returns wrong results. Compute the top bit from `n`. Not catastrophic but it's an undocumented partial function masquerading as total.

---

## Wrong answer: `BellmanFord`-style `Spfa` queue wraps incorrectly

`IAFahim.Graph/ShortestPath.cs`, `Spfa`:
```cs
int* q = stackalloc int[n];
...
q[qt++] = v;
if (qt >= n) qt = 0;   // circular wrap
...
while (qh < qt)         // but this loop condition assumes NON-circular
```
The loop guard `qh < qt` is for a linear queue, but you wrap both `qh` and `qt` modulo `n`. Once `qt` wraps past `qh`, `qh < qt` becomes false and you terminate early or, worse, the indices alias and you process stale entries. A circular queue needs a separate `count`, not `qh < qt`. As written, SPFA terminates prematurely on graphs that re-enqueue enough nodes. The negative-cycle counter may also miss because of early termination.

---

## Wrong answer: `ZeroOneBfs` deque underflow

`IAFahim.Graph/GraphTraversal.cs`, `ZeroOneBfs`:
```cs
int* dq = stackalloc int[n];
int dh = 0, dt = 0;
...
if (w == 0) dq[dh--] = v;   // dh can go negative -> OOB write
```
Push-front via `dq[dh--]` writes at `dh` then decrements, so the first front-push writes `dq[0]` (clobbering the element being read) and `dh` becomes `-1`, then the next read `dq[dh++]` reads `dq[-1]`. This is straightforwardly out of bounds and wrong. `ShortestPath.ZeroOneShortestPath` does the wrapping version (`dh--; if (dh<0) dh=n-1;`) which avoids OOB but, like SPFA, breaks the `dh < dt` linear guard. 0-1 BFS needs a real deque with capacity bookkeeping.

---

## Wrong answer: `DijkstraRestorePath` fine; but `Dijkstra` decrease-key via `SortedSet.Remove((dist[v], v))`

`ShortestPath.Dijkstra`/`DijkstraSparse`/`PotentialDijkstra`:
```cs
pq.Remove((dist[v], v));
dist[v] = nd;
pq.Add((nd, v));
```
This is correct *only* because you remove the old key before overwriting `dist[v]`. Good. No bug — flagging because it's a common place to get wrong and yours is right. (Also: these aren't `unsafe`-pure / referentially transparent — they heap-allocate a `SortedSet` and use `System.Collections.Generic`, which contradicts the no-allocation, primitive-only ethos of the rest of the library. Same for `MinCostMaxFlow`, `PotentialDijkstra` in Flow, `AStar`, `Yen`, `Prim`, `WeightedBlossom`. Not correctness bugs, but they break the library's own contract.)

---

## Wrong answer: `SegmentTreeMinLeft` mid update for left descent

`IAFahim.DS.SegmentTree/SegmentTree.cs`, `SegmentTreeMinLeft`:
```cs
node = node * 2;
hi = mid;
```
When you descend left after going right, `lo` stays but `hi=mid` — but you already advanced `lo = mid+1` in the right branch on a prior iteration; the `lo`/`hi` bookkeeping for a right-to-left "min left" walk doesn't track the segment for `node*2` correctly after a right move. More concretely: `SegmentTreeMaxRight` is the standard correct pattern; `SegmentTreeMinLeft` mirrors it but the `lo`/`hi` are never reset relative to the chosen child's actual range, so the returned `lo` is not the boundary you want. These two functions also assume a perfect-power-of-two leaf layout (`n` leaves at depth `log2 n`) that nothing enforces. Treat as broken for non-power-of-two `n`.

---

## Wrong answer: Sparse table build is incoherent

`IAFahim.DS.Sparse/Sparse.cs`, `SparseTableBuild.RunInt32`:
```cs
int base_ = i * log[n] + (j - 1) * n;
int prev  = i * log[n] + (j - 1) * n;   // base_ == prev, duplicate
int left  = i + (1 << (j - 1));          // computed but never used as an index into table
int right = i * log[n] + (j - 1) * n;    // == prev again
table[base_ + n] = Math.Min(table[prev], table[right]);  // min(x,x)=x
```
`prev`, `base_`, `right` are all the same expression, so you compute `min(x, x)`. The `left` neighbor is never read. The whole table is just copies of level 0. `RunInt64` has a different (also wrong) indexing scheme `i * log[n] + j * n` that mixes a per-element stride `log[n]` with a per-level stride `n` — these can't both be right and they alias. And both rely on `log[n]` being prefilled, which no function in the file does. `SparseTableQuery` uses the `RunInt64`-style indexing, so even if the build were fixed they'd disagree.

This whole sparse-table pair needs rewriting around a single flat layout `table[level * n + i]`. The current `IAFahim.Search.Range/RangeMin.cs` `BuildSparse` (which uses `dst[i + k * len]`) is the correct model — use that and delete `Sparse.cs`'s versions.

`DisjointSparseBuild`/`DisjointSparseQuery` similarly recompute `b`/`bsz` inconsistently (`while ((1<<b)<=n)` in build vs `while ((1<<b)<=r-l+1)` in query) and the per-element stride `b` is recomputed and mismatched. Broken.

---

## Wrong answer: Wavelet tree family is non-functional

`IAFahim.DS.Sparse/Sparse.cs` `WaveletTreeBuild`/`WaveletRank`/`WaveletSelect`/`WaveletKth`/`WaveletRangeFreq`, and `Mo/MoAlgorithm.cs WaveletTreeRangeSum`:

`WaveletTreeBuild` does a single binary search and writes `left[node*2]`, `right[node*2]` once — it never recurses, never partitions the array stably by bit, never builds child levels. A wavelet tree requires a stable partition at each level and a bitmap with rank support. None of that exists. `WaveletKth` computes `leftCount = min(qr,mid) - max(ql,l) + 1` treating *positions* as if they were *values* — it conflates the value domain with the index domain. These functions return arbitrary numbers. The entire wavelet group is a stub and should be marked as such or removed.

`WaveletSelect` additionally has an operator-precedence bug:
```cs
Run(left, right, node * 2 + 1, (l + r) >> 1 + 1, r, ...)
```
`>> 1 + 1` parses as `>> (1+1)` = `>> 2`, not `((l+r)>>1)+1`. Wrong midpoint.

---

## Wrong answer: `MultiPointEval` is a tangle and won't compile/run correctly

`IAFahim.Math.Polynomial.Eval/PolynomialEval.cs`, `MultiPointEval`:
- `BuildTree` references `mod` (the static field) before `EvalTree` sets it — `mod` is `1000000007` at build time regardless of caller's `m`.
- `GetNodeSize` returns a constant `4` (`while (n < 4) n <<= 1` → n=4) ignoring the actual subtree polynomial degree, so `PolynomialRemainder` is fed the wrong divisor length.
- `tree` is indexed both as `tree[node]` (single coeff) in `BuildTree` and as `tree + node*4` in `EvalTree` — inconsistent layout.
This routine cannot produce correct multipoint evaluation; it needs a proper product-tree where each node stores a *polynomial* (with offsets), not a scalar.

---

## Wrong answer / overflow: CRT and friends

`IAFahim.Math.Modular/Crt.cs`:
The overflow handling is half-written:
```cs
if (m1 > 0 && m2_g > 0 && lcm <= 0) {
     if (result < 0) return result;   // returns an unnormalized negative number
}
```
On overflow it returns a garbage value with a comment admitting it. Either use Int128/`ModMul` throughout or document the precondition `lcm < 2^63`. Also `ModMul.Run(x, diff, m2_g)` is used to compute `t` but then `result = r1 + t * m1` uses a *plain* `t * m1` which overflows for large moduli. Inconsistent: you reach for `ModMul` for one product and not the adjacent one.

`Excrt.cs`: `r = ModNormalize.Run(r + ModMul.Run(diff, m, lcm) * x, lcm)` — `ModMul(diff, m, lcm) * x` again multiplies by `x` outside the modular routine, overflowing, and `diff` can be negative so `ModMul` (which normalizes b≥0 but the `diff/g` can exceed range) is suspect. The classic ExCRT update is `r = r + m * ((diff * inv) mod (m2/g))`; your factoring doesn't match it.

---

## Wrong answer: `LinearRecurrence` / `Kitamasa` index folding

`IAFahim.Linear.Matrix/Recurrence.cs`, `Kitamasa`:
```cs
int ni = i + j;
if (ni >= k) ni = ni % k + k;   // nonsensical fold; can exceed array bounds
```
Multiplying two degree-`<k` polynomials gives degree `<2k-1`; the reduction modulo the characteristic polynomial must use the recurrence coefficients, not `ni % k + k`. As written, `ni` can be `>= k` (up to `k-1 + k`) indexing out of a `k`-length buffer, and the reduction is mathematically wrong. The `% mod` is also hard-coded to `1000000007` while the function takes no `mod` parameter for the multiply but the caller might want another modulus. Broken.

`LinearRecurrence.Run` (the matrix version) builds `baseMat` then immediately overwrites with `mat` initialized from the same `temp`, and the companion-matrix orientation (`mat[i]` = trans for row 0, identity sub-diagonal) doesn't match how it reads out `res[j] * init[k-1-j]`. The index pairing `res[j]` with `init[k-1-j]` only works for one specific companion convention; combined with the duplicate initialization it's not self-consistent. Verify against a known sequence (Fibonacci) — it will not reproduce it.

`CharacteristicPolynomial` uses the Faddeev–LeVerrier idea but `poly[0] = -n` and the trace/power loop don't implement the actual recurrence; it returns wrong coefficients.

---

## Wrong answer: `MatrixDeterminant` does fraction-free elimination with integer division

`IAFahim.Linear.Matrix/Matrix.cs`, `MatrixDeterminant`:
```cs
for (int j = i + 1; j < n; j++) a[j * n + i] /= a[i * n + i];   // integer division
for (...) a[j*n+k] -= a[j*n+i] * a[i*n+k];
det *= a[i * n + i];
```
This is Gaussian elimination over the *integers* using truncating `/`, which is not exact unless every pivot divides cleanly. For a general integer matrix the determinant is wrong. You need either Bareiss (fraction-free) or rational arithmetic. Same issue in `GaussianElimination`, `GaussJordan`, `MatrixInverse`, `MatrixRank`, `LinearSystemSolve`, `GraphBipartite.HungarianMin` cost handling — all use `long` with `/=` and will silently truncate. If the intent is *modular* elimination, you must multiply by modular inverses, not divide. As-is these are correct only when all intermediate divisions are exact.

---

## Wrong answer: `LisLength` uses lower_bound but `LdsLength` mirror is wrong direction

`IAFahim.Search.Bit/BitSearch.cs`:
`LisLength` (strictly increasing) uses `tail[mid] < arr[i] → lo=mid+1`. Correct for strict LIS.
`LdsLength`:
```cs
if (tail[mid] > arr[i]) lo = mid + 1; else hi = mid;
```
For a *decreasing* tail you maintain `tail` as a decreasing sequence; this binary search treats `tail` as if sorted ascending and compares with `>`. The invariant breaks — `tail` is being written in a non-monotone way relative to the comparator. The standard trick is to negate and reuse LIS. As written `LdsLength` returns wrong lengths on many inputs.

`BitonicLength` builds `inc` left-to-right and `dec` right-to-left and returns `lenI + lenD - 1`, but `lenI`/`lenD` are global LIS lengths, not LIS-ending-at-i / LDS-starting-at-i, so they don't share a peak. This computes (longest increasing) + (longest decreasing) − 1, which is not the longest bitonic subsequence.

---

## Wrong answer: `InversionCount` allocates `long* temp` but copies into `int`

`BitSearch.InversionCount`: stores into `long* temp` then `arr[i] = (int)temp[i]` — fine for values, but the merge writes `temp[k++] = arr[j++]` (int → long) and reads back as int. Works for `int`-range values. Not a bug per se, but `KthElement` does an `O(n)` count inside a binary search over the full int range `[int.MinValue, int.MaxValue]` — `~32 * n` passes, and the midpoint `(lo+hi)>>1` **overflows** for `lo=int.MinValue, hi=int.MaxValue`: `lo+hi == -1`, `>>1 == -1`, so it can loop oddly. Use `lo + ((hi-lo)>>1)`.

---

## Wrong answer: `RotateGrid` ignores `times`

`IAFahim.DS.Grid/GridBfs.cs`, `RotateGrid`:
```cs
times = ((times % 4) + 4) % 4;
if (times == 0) { copy; return; }
// then performs exactly ONE 90° rotation regardless of times being 1,2,or 3
```
For `times == 2` or `3` it still does a single rotation. Wrong for anything but `times % 4 == 1`.

---

## Wrong answer: `Rotate.Run` (generic grid) uses `stackalloc T[len]` with runtime `len`

`IAFahim.DS.Grid/Rotate.cs`: `T* temp = stackalloc T[len];` — `len = width*height` at runtime can overflow the stack for large grids (crash, not wrong answer). Logic of the rotation itself looks correct.

---

## Wrong answer: `FloodFill` can revisit and `maxStack` guard drops fills

`Grid/GridBfs.cs FloodFill`:
```cs
if (top + 4 > maxStack) continue;   // silently abandons remaining neighbors
```
When the stack is near capacity it skips pushing neighbors and never returns to them — so large regions are under-filled and `count` is wrong. It also pushes neighbors before checking `target`, relying on the pop-time check, which is fine, but the capacity guard makes it a partial function that lies about completing the fill.

---

## Wrong answer: `Prefix2D.Build` double-counts? No — but `GridBfs` level tracking is dead code

`GridBfs.Run` maintains `levelSize`/`levelIdx` but never uses them to do anything; harmless dead state. `Prefix2D` is correct. Flagging the dead code only.

---

## Wrong answer: `IntervalDp` (DP.General) returns `dp[0]` but fills `dp[i*n+j]`

`IAFahim.DP.General/General.cs`, `IntervalDp.Run`:
```cs
for (...) dp[i] = 0;        // initializes dp[0..n-1]
... dp[i * n + j] = best;   // fills 2D
return dp[0];               // returns dp[0], not dp[0*n + (n-1)]
```
The answer for the whole interval is `dp[0*n + (n-1)]`, not `dp[0]`. Returns wrong cell. Same pattern in `QuadrangleInequalityDp` (`return dp[0]`) and in `DP.Optimization/KnuthOptimization` it correctly returns `dp[0*n+(n-1)]` — so the inconsistency is the tell.

`ProbabilityDp` and `ExpectationDp` are also suspect: `ExpectationDp` does `dp[j] += dp[i] + 1; dp[j] *= p[i];` inside a double loop, accumulating then multiplying repeatedly — that's not a coherent expectation recurrence and will produce nonsense.

---

## Wrong answer: `TreeKnapsack` hard-codes `* 1000` strides

`DP.General/TreeKnapsack`: indexes `dp[u * 1000 + i]` and bounds `i + w2 < 1000`. This assumes capacity < 1000 and silently drops items beyond. It also never iterates the child's dp correctly (`dp[v2*1000 + subSize]` uses `subSize` as if it were a capacity index). Magic `1000` is an undocumented hard limit and the merge is wrong.

`DP/DivideConquerDp.DcRec` likewise uses `dp[k-1] + k*1000 + mid` — `1000` is a placeholder cost, so this computes a fabricated objective, not a real D&C optimization.

---

## Wrong answer: `KnapsackBounded` (DP.Knapsack) inner loop order

`IAFahim.DP.Knapsack/Knapsack.cs`, `KnapsackBounded.Run`, the `cnt[i] > 1` branch:
```cs
for (long c = cap; c >= 0; c--)
    for (long k = 1; k <= maxUse && c >= k * w[i]; k++)
        dp[c] = max(dp[c], dp[c - k*w[i]] + k*v[i]);
```
Because `c` decreases and you read `dp[c - k*w[i]]` (smaller indices, not yet updated this item) this *happens* to avoid reusing the same item more than the bound within one `c`... actually no: by trying all `k` against the *original* `dp[c-k*w]`, multiple item `i` copies are allowed up to `maxUse`, which is the intent. This one is OK. (Flagging because it looks wrong at a glance; verify but I believe it's correct.)

---

## Wrong answer: `BitsetSubsetSum` (DP/Dp.cs) shift direction

`IAFahim.DP/Dp.cs`, `BitsetSubsetSum`:
```cs
for (int b = bitLen; b >= 0; b--)
    if ((bitset[b] & mask) != 0 && b + idx < bitLen)
        bitset[b + idx] |= bitset[b] << (int)(arr[i] & 63);
```
This is not the standard `bits |= bits << a[i]` whole-bitset shift; it does per-word conditional ORs keyed on a single `mask` bit and never propagates the cross-word carry of the shift. It will miss most reachable sums. The `DP.Knapsack/BitsetSubsetSum` version (the one with `(ulong)bits[j-offset-1] >> (64-shift)`) is the correct shifting one. The `DP/Dp.cs` version is broken.

---

## Wrong answer: `AlienDp` uses `dist` as both predicate and accumulator inconsistently

`DP/Dp.cs AlienDp`:
```cs
if (cur + dist(cur, arr[i]) > mid) { groups++; cur = arr[i]; }
else cur += dist(cur, arr[i]);
```
`cur` is a running cost but reset to `arr[i]` (a value, not a cost) on a new group. Mixing value-space and cost-space. The Lagrangian/Alien trick needs a DP that returns (cost, groupCount) at a penalty `mid`; this greedy substitute is not that and won't bisect to the right answer.

---

## Wrong answer: `LiChao` add-line recursion picks the wrong child interval

`DP.Optimization/LiChaoAddLine.Run`:
```cs
if (yL < yR) Run(seg, m, b, node*2+1, l, mid, x1, (x1+x2)>>1);
else         Run(seg, m, b, node*2+2, mid, r, (x1+x2)>>1, x2);
```
The decision to recurse left vs right in a Li Chao tree must be based on comparison at the *midpoint after* the swap (which segment can still beat the stored line on a half), and you should recurse into exactly one child but the `x1/x2` endpoints passed must match the child's `[l,mid]`/`[mid,r]` *coordinate* range, not `(x1+x2)>>1`. Mixing node-index range `(l,mid)` with coordinate range `(x1, (x1+x2)>>1)` double-tracks the domain inconsistently. The `DP/LiChaoAddLine` version (node-index based, integer domain `l..r`) is the correct, self-consistent one. The `DP.Optimization` variant is broken.

`LiChaoQuery` (Optimization) has the same `x < x1 + (x2 - x1 >> 1)` precedence trap: `x2 - x1 >> 1` parses as `(x2 - x1) >> 1`? No — `>>` has lower precedence than `-`, so it's `(x2-x1)>>1`, which is actually intended here. OK, but fragile.

`ChtDp.AddLine` (DP.General) computes intersection `x = (b1-b2)/(m2-m1)` with integer division and a hand-rolled floor correction that only triggers when signs differ — but it can divide by zero when `m2 == m1` (parallel lines, common with duplicate slopes). Guard needed.

---

## Wrong answer: `Hungarian` (Matching) `matchL`/`matchR` typing and final fill

`IAFahim.Graph.Matching/Hungarian.cs`, `HungarianMin`:
- signature is `(int n, long* a, long* matchL, long* matchR)` — uses `long*` for match arrays (indices), then `matchR[i-1] = p[i]-1` and reads `matchR[i-1]` as a column index into `a`. Storing indices in `long*` is just wasteful, not wrong.
- `matchL[matchR[j]] = j;` at the end: `matchR[j]` is a `long` used as an index — fine, but `HungarianMax` does the exact same `-a[...]` trick which only negates costs; for a max-cost assignment with the Jonker–Volgenant potentials initialized to 0 and `u/v` updated by `delta`, negating the cost matrix is valid, **but** the result accumulation `result += a[(i-1)*n + matchR[i-1]]` sums the *original* `a`, which for `Max` is correct, while for `Min` it's also `a` — both return `sum of original a over the matching`, so `HungarianMin` and `HungarianMax` return the *same* number for the same matching if they pick the same matching. Since `Max` only flips the sign inside the solver, it should pick a different matching — verify it actually does; the `GraphBipartite.HungarianMax` (other file) instead does `n*maxVal - HungarianMin(maxVal - cost)`, which is the correct reduction. The `Matching/HungarianMax` (negate-in-place) is the dubious one.

`AssignmentSolve` (Matching) calls `HungarianMin(n, cost, matchL, matchR)` with `long* matchL` then `assign[i] = (int)matchL[i]` — but `HungarianMin` fills `matchL[matchR[j]] = j` only for `j in [0,n)` over `matchR` values; if the matching is a permutation it's fine, but `matchL` is never zero-initialized so any unset slot is garbage.

---

## Wrong answer: `BlossomGeneral` is not a correct blossom algorithm

`IAFahim.Graph.Matching/Blossom.cs`: `FindPath` does a BFS but never contracts blossoms (the defining feature). The augmentation walk:
```cs
int nextMatched = prev == -1 ? -1 : (match[prev] == cur ? prev : -1);
match[cur] = prev; match[prev] = cur;
cur = nextMatched;
```
sets `match` along the parent chain without alternating correctly and without odd-cycle handling, so on any graph with an odd alternating cycle it produces an invalid (non-)matching. `EdmondsMatching` just calls it. `WeightedBlossom` doesn't even attempt matching — it runs a Dijkstra-like potential update and at the end reads `match[i]` which is still all `-1`, so it returns 0 always. These are stubs; flag as non-functional.

`GraphBipartite.GeneralMatchingBlossom` similarly lacks contraction; the augmentation `while (v != -1) { ...swap... }` walk is ad hoc and incorrect for general graphs.

---

## Wrong answer: `StableMarriage` stability check and queue bound

`Graph/GraphBipartite.StableMarriage`:
```cs
while (qh < n)   // only processes first n dequeues; rejected men re-enqueued at qt may be skipped
```
Free men get re-added via `queue[qt++]` but the loop stops at `qh < n`, so men who are rejected and re-enqueued beyond the first `n` slots are never re-processed → not all men get matched → not a stable/complete matching. The `Matching/StableMarriage` version uses `while (qh < qt)` (correct). The `GraphBipartite` copy is broken.

Also `Matching/StableMarriage.IsStable` reads `manPref[m*n + w]` as a *rank* (`mPrefW`) but `manPref` stores the *woman id at preference position*, not the rank of a given woman. To get a rank you need the inverse table. So `IsStable` compares ids as if they were ranks — wrong.

---

## Wrong answer: Flow `MinCut`/`FlowDecompose`/lower-bounds are stubs or wrong

`Graph.Flow/Flow.cs`:
- `MinCut.Run` computes reachability then `return 0;` — never reports the cut value or the partition usefully. Caller gets nothing.
- `CycleCanceling.Run` just sums negative-cost saturated edges — not min-cost flow.
- `MaxFlowLowerBounds`/`CirculationWithDemands` are clearly unfinished (`return totalDemand == 0;`, `DinicMaxFlow.Run(...); return 0;`). They don't actually route the feasibility flow back.
- `EdmondsKarp` parent-tracking uses `parent[v] = e` (edge id) and reconstructs via `to[parent[u] ^ 1]` — that gives the tail of edge `e` only if edge ids are paired `e, e^1`. But `MinCostFlowAddEdge` and the Dinic builder pair as `id, id+1` with `++(*edgeId)` starting at 1, so `e ^ 1` pairs `(2,3),(4,5)...` but `id=1` pairs with `0` (unused). The first real edge has id 1, and `1 ^ 1 == 0` points at the dummy edge. **Off-by-one in the XOR pairing** because ids start at 1, not 0. Residual edges won't be found correctly. This affects `EdmondsKarp`, `DinicDfs` (`flow[i ^ 1]`), and every flow routine relying on `e ^ 1`. Fix: start edge ids at 0 (or 2) so paired edges are `2k, 2k+1`.

This `e ^ 1` pairing bug is library-wide and serious for all flow/min-cost code.

---

## Wrong answer: `Treap` struct holds raw pointers but `TreapNode` not declared `unsafe`-safely

`IAFahim.DS.Treap/Treap.cs`: `public struct TreapNode { ... public TreapNode* Left; }` — a struct containing pointers to itself is fine in `unsafe`, but `RangeQuery` does:
```cs
Split(root, l, &left, &mid);
Split(mid, r+1, &mid, &right);
long result = SumOf(mid);
root = Merge(left, Merge(mid, right));
return result;   // 'root' reassigned locally, caller's tree pointer NOT updated
```
The caller's root is passed by value, so after `RangeQuery` the caller's tree is left split/dangling (its `root` still points at the old root whose children were rewired). This silently corrupts the tree across calls. `RangeQuery` must take `TreapNode** root`.

`Split` uses `&root->Right` as an out-param while also reassigning `*left = root` — taking the address of a field of a node that's simultaneously being relinked is correct C but easy to get wrong; here the recursion `Split(root->Right, key, &root->Right, right)` then `*left = root` is the standard pattern and looks OK.

---

## Wrong answer: `Splay.Rotate` reads `g` before null-checking `p`

`IAFahim.DS.Splay/Splay.cs`, `Rotate`:
```cs
SplayNode* p = x->Parent;
SplayNode* g = p->Parent;   // dereferences p
if (p == null) return;      // too late — already dereferenced p above
```
If `x->Parent` is null, `p->Parent` is a null dereference → crash. The guard is after the use. Same ordering issue exists in `LinkCut.Rotate`:
```cs
LctNode* g = p->Parent;
if (p == null) return;   // p already dereferenced
```

---

## Wrong answer: `LinkCut.Cut`/`Splay` may not push correctly; `IsRoot` after `Splay`

`LinkCut`: `Splay` calls `PushTo(x)` (recursive push from root to x) then rotates — standard. But `Access` does `Splay(x); x->Right = last; Update(x); last = x; x = x->Parent;` without re-splaying `last` into `x`'s preferred path; the canonical Access re-splays. The path-aggregate (`PathSum`) queries can be stale. Functionally subtle; flag for verification with a known LCT test.

---

## Wrong answer: `OrderedSet.Kth`/`Rank` assume sorted array but no balancing

`IAFahim.DS.OrderedSet/OrderedSet.cs`: `Insert`/`Erase` are `O(n)` array shifts — correct but the type name "OrderedSet" implies log-time; not a correctness bug, just `O(n)` per op and you must keep the array sorted externally. `Kth` returns `ptr[k]` with no bounds check (UB if `k>=len`). Minor.

---

## Wrong answer: `Trie` delete leaves dangling and `prefixCount` semantics

`IAFahim.DS.Trie/Trie.cs`:
- `TrieInsert` sets `trie[cur*27 + 26] = -1` as an end-of-word flag but `TriePrefixCount` returns `trie[cur*27 + 26]` expecting a *count*. Inserting sets it to `-1`, never increments a count. `TriePrefixCount` returns `-1` for any inserted word and `0` otherwise — it does not count prefixes. Mismatch between insert (flag) and prefixCount (count).
- `BinaryTrieErase` recomputes the bit as `(val >> (pathLen-1-i)) & 1` while traversal used `(val >> i)` (i from `bits-1` down). The index `pathLen-1-i` doesn't recover the same bit order, so it decrements/prunes the wrong child. Erase corrupts the structure.

---

## Wrong answer: `PersistentSegmentBuild`/`Update` use `tree[0]` as node counter and as a value

`IAFahim.DS.SegmentTree/PersistentTree.cs`: `int node = ++tree[0];` uses `tree[0]` as the allocation counter, but `tree[node]` also stores subtree sums — so node 0's "sum" slot is the allocator and the root could be node 1 whose `tree[1]` is a real sum. This works only if you never query node 0; but `PersistentSegmentQuery` treats `node == 0` as null. So far consistent. **However** `RunInt32` build: `if (prev != 0) { tree[node] = tree[prev]; }` then overwrites — and uses `prev == 0 ? 0 : left[prev]` for children, where `0` means "no node", but a legitimately allocated node could be 0 only if `tree[0]` started at -1; it starts at 0 and `++tree[0]` makes the first node 1, so 0 is safely "null". OK. The build is fine. (Flagging to confirm `tree[0]` is reserved and never used as data — it is, so this is acceptable, if subtle.)

---

## Wrong answer: `DivisorCount`/`Phi`/etc. fine; `EuclidSum` recursion base

`IAFahim.Math.NT/EuclidSum.cs`: `SumCoPrime(n, m)` recurses `SumCoPrime(m, n % m)` but the formula `n*(m-1) - m*(n-1) + SumCoPrime(m, n%m)` is not the standard sum-of-floors recurrence and the base `if (m==0) return 0` with the first call `SumCoPrime(n, m/g)` doesn't match any known closed form I can verify — this looks like it computes the wrong quantity. The well-tested one is `FloorSum`. Cross-check `EuclidSum` against brute force; I believe it's wrong.

`FloorSum.Run`: the standard AtCoder `floor_sum` requires `a, b >= 0` and handles `a >= m`, `b >= m` reductions — yours does, but the recursive call `Run(yMax, a, m, (a - (xMax % a)) % a)` swaps args in the AtCoder manner; verify `a != 0` (division by `a` and modby `a`). If `a == 0` after reduction, `(xMax + a - 1)/a` divides by zero. Guard `a==0 → return ans`.

`SternBrocot.Run`: `long steps = (n*b - a*d)/(c*d - n*dd)` can divide by zero and the loop's medd/medn convergence isn't guaranteed to terminate for non-representable targets. Risky.

`FareyRank.Run`: the Stern–Brocot descent computing `rank` by counting mediant steps does not equal the Farey rank (number of fractions ≤ a/b in F_n); the formula is unrelated to the standard `rank = sum mu-based count` or the SB-path interpretation. Wrong.

---

## Wrong answer: `HighestBit` for `int` uses `x >>> 1` on `int`

`IAFahim.Math.NT/BitOps.cs`, `HighestBit.Run(int x)`:
```cs
return x - (x >>> 1);
```
After the fill, `x` has all bits set up to the top bit. `x - (x>>>1)` isolates the top bit — correct for unsigned semantics, and `>>>` is the C# unsigned right shift, so OK. But `NextBit`/`PrevBit` build on `BitLength`; `PrevBit.Run(int)` does `HighestBit(x-1) << 1` which for `x` already a power of two gives `2*x`, not `x` — but `PrevBit` is "largest power of two ≤ ... " ambiguous; check intent. Likely off.

`BitCompress`:
```cs
buffer = (int)((uint)buffer >> 32);   // shift int by 32 is undefined/no-op in C#
```
Shifting a 32-bit `int`/`uint` by 32 in C# masks the count to `32 & 31 == 0`, i.e. **no shift**. So after emitting a word, `buffer` is not cleared and `bitCount -= 32` desyncs from `buffer`. The whole bit-packing is wrong for `bits` that fill words. Needs a 64-bit buffer.

---

## Wrong answer: `Bsgs` open-addressing probe can read past `m`

`IAFahim.Math.NT/Bsgs.cs`: linear probe `while (pos < m && keys[pos] != -1 && keys[pos] != key) pos++;` — if the table is full or the key hashes near `m`, `pos` reaches `m` and the insert is silently dropped (`if (pos < m && keys[pos]==-1)`), so some baby steps are lost → BSGS misses solutions. Table should be sized `> m` or use proper hashing. Also `m = ceil(sqrt(mod))` baby steps but the giant-step factor uses `ModPow(am, mod-2, mod)` assuming `mod` prime — for composite `mod` (the `Gcd(a,mod)` branch suggests you intend composite support) this inverse is wrong.

---

## Wrong answer: `IsPrime` (Combinatorics/Sieve.cs) `ModMul` doubling uses signed `*2`

`IAFahim.Math.Combinatorics/Sieve.cs`, `IsPrime.ModMul`:
```cs
a = (a * 2) % mod;   // a*2 can overflow before %mod for large a near 2^63
```
For `mod` near `2^62`, `a` near `mod`, `a*2` overflows signed `long`. The `Math.NT/ModMul` (binary, no `*2`) is the safe one. The Combinatorics copy is unsafe for large moduli.

---

## Wrong answer: `Catalan`/`Bell`/`Stirling` modular but use plain `*`

These (`Math.Combinatorics/Counting.cs`) do `result = (result * (n+k)) % mod` with `long` — fine for `mod < ~3e9`. `BellNumbers` uses `comb` updated by `comb * (n-k) * ModInverse(k+1)` which is a falling-factorial/binomial running product — but `StirlingSecond` is recomputed from scratch inside the `k` loop, making Bell `O(n^2 * stirling)` and, more importantly, `StirlingSecond.Run` rebuilds `s[0..k]` each call; the recurrence `s[j] = s[j-1] + j*s[j]` over `i=1..n` is the correct S2 recurrence — OK. Bell's inclusion via `sum_k S2(n,k)` is correct; the `comb` term is spurious (Bell = Σ S2(n,k), no binomial weight). The `comb` multiplication makes `BellNumbers` **wrong** — it weights each Stirling by a binomial it shouldn't.

---

## Wrong answer: `LinearCongruence` returns x mod (m/g) but solution set spacing

`Math.Combinatorics/LinearCongruence.cs`: returns one solution `x = inv * b' mod m'` — correct as *a* solution, but callers expecting the smallest non-negative get it; fine. No bug, but `g` is returned as `-1` on no-solution via `out g`, overloading `g` as both gcd and error flag — API smell.

---

## Wrong answer: `ModSqrt`/`TonelliShanks` loop bound

`Math.Modular/ModSqrt.cs`: the inner `while (tmp != 1) { tmp = ModMul(tmp,tmp,mod); i++; }` has no bound; if `a` is a non-residue that slipped past the Euler check (e.g., `mod` not prime), it loops forever. Guard with `i < m`. `TonelliShanks` has `if (i == m) return -1;` — better. `ModSqrt` lacks it.

---

## Wrong answer: `MinkowskiSum` / `ClosestPair` / circle ops use `long` sqrt and integer division

`IAFahim.Geometry.Advanced/GeometryAdvanced.cs`:
- `ClosestPair`: sorts only by x, the recursive `ClosestPairRecursive` compares `(y[i]-midX)^2 < d` using `y[]` against `midX` (an x value) and never builds the strip by y — it's not the closest-pair algorithm; returns wrong distances. Also `sortedY`/`yTemp` are never used meaningfully.
- `CircleLineIntersection`/`CircleCircleIntersection`/`CircleTangents`/`PointCircleTangents`: all use `(long)Math.Sqrt(...)` and integer division `/ d`, truncating to integers. Intersection points are wrong unless coordinates happen to be perfect. `CircleTangents` computes `cos = (long)((double)r1/d)` which truncates to 0 whenever `r1 < d` (the usual case) → all tangent math collapses. These need `double` or exact rational geometry.
- `MinkowskiSum`: the edge-merge writes `resX[m]/resY[m]` using `(i>0 ? p1[i-1] : 0)` cumulative offsets that don't actually accumulate (it re-reads a single previous edge, not a running sum), so the summed polygon vertices are wrong. The classic algorithm accumulates prefix sums of edge vectors; this doesn't.

`GeometryAdvanced2.MinimumEnclosingCircle`: stores `*r` as squared radius in some places (`*r = (*cx - x[i])^2 + ...`) and compares `dx*dx+dy*dy > *r * *r` (treating `*r` as linear) — mixing radius and radius² inconsistently. Welzl needs consistent units; this is wrong.

`IntegerPointCount`/`PickTheorem`: Pick's theorem is `Area = I + B/2 - 1`, so `I = Area - B/2 + 1`. You compute `(area - boundary + 2) / 2` where `area` is **twice** the polygon area (shoelace without /2). So `I = (2A - B + 2)/2 = A - B/2 + 1`. That's correct *if* `boundary` is the lattice points on the boundary. It is (`Gcd` sum). So this one is actually right. Good.

---

## Wrong answer: `Geometry.Basic.PolygonContains` integer division in the ray test

`IAFahim.Geometry.Basic/GeometryBasic.cs`, `PolygonContains`:
```cs
px < (x[j]-x[i])*(py-y[i])/(y[j]-y[i]) + x[i]
```
Integer division truncates the crossing x, so points near edges are misclassified. Use cross-product orientation instead of division. `LineIntersect`/`LineProjection`/`LineReflection`/`DistancePointLine` all do `t * (...) / d` with integer division — projections/reflections land on wrong integer points. These are only exact for special inputs.

`Circumcenter`/`Incenter` likewise integer-divide; `Incenter` uses `(long)Math.Sqrt` for side lengths, losing precision and breaking the weighted centroid.

---

## Wrong answer: `RotatingCalipers`/`ConvexDiameter` antipodal walk

`ConvexDiameter`: the two-pointer walk advances `ni` or `nj` by cross-product sign but the termination `while (ni != j || nj != i)` and the initial `i,j` (max/min by x) is not the standard rotating-calipers loop; it can miss the diameter pair. `RotatingCalipers` advances `j` with `while(true){ cross...; if (cross<=0) break; j=nj; }` but never resets `j` per `i` and the area-sign convention assumes CCW input that's unchecked. Both are fragile and likely wrong on general hulls.

---

## Wrong answer: `String` hashing single-hash collisions aside, `HashConcat` recomputes pow in O(len2)

`IAFahim.String/StringAdvanced.cs HashConcat`: loops to compute `p = B^len2` each call — correct but O(len2); minor. `SuffixCompare`/`SuffixLowerBound`: the comparison logic mixes `<`/`>` with a `len < patLen` special-case that doesn't correctly handle the "pattern is a prefix of suffix" boundary — binary search bounds can be off by one. `WildcardMatch` with `*` and `?` is the standard greedy and looks correct.

`PalindromicTreeAdd`: uses `len_[0]` as a node counter while nodes `0,1,2` are the two roots — `len_[0]=1, len_[1]=0, len_[2]=-1` in build, then `int now = ++len_[0]` makes the first real node `2`, colliding with the imaginary root at index 2. Node allocation overlaps the sentinel. Broken.

`SuffixAutomatonExtend`: `int cur = size;` then caller does `size++` after — but `Extend` itself may allocate a clone via `int clone = size; size++;` using a **local** `size` that isn't returned, so the caller's `size` and the automaton's internal count diverge. The clone's index is lost. Broken for any string needing a clone (most non-trivial strings).

`RegexNfaBuild`/`RegexMatch`: `*` handling sets `transitions[state*alpha + prevChar] = state` (self-loop on the literal char) which only matches repeats of that exact char, not Kleene star over the preceding token in general, and `RegexMatch` never includes epsilon/skip transitions for `*` (zero occurrences). It's a toy that mishandles `a*b` matching `b`.

---

## Wrong answer: `Search.Prefix`/`Suffix` generic `Run<T>` use `dynamic`

`IAFahim.Search.Prefix/Prefix.cs` and `Suffix/Suffix.cs`: the generic `Run<T>` and `RangeXor<T>`/`RangeSum<T>` use `dynamic` and cast `(int)(object)a` — for `T == long` this casts a boxed `long` to `int` via `(int)(object)`, which **throws InvalidCastException** at runtime (you can't unbox a `long` as `int`). And `RangeSum<T>` mutates the array (`ptr[i] = ...`) inside what looks like a read query, corrupting it. These generic overloads are broken (the typed `int`/`long` overloads are fine).

`SuffixSums.RangeSum<T>` additionally rewrites suffix sums in place then subtracts — destructive and wrong as a query.

---

## Wrong answer: `MeetInMiddle` ignores the sorted-merge optimization and miscounts? No — but `target - leftSums[i]` is computed and unused

`IAFahim.Search.MeetInMiddle/MeetInMiddle.cs`: `long rem = target - leftSums[i];` is dead; the inner loop is O(2^(n/2) * 2^(n/2)) = O(2^n) brute force, defeating the purpose, and `target` is `int` so `target - leftSums[i]` (long) is fine but unused. Correct result, wrong complexity (no real meet-in-middle). For `len` near 40 this won't finish.

---

## Wrong answer: `GameTheory.Grundy` exponential recompute and `seen` bit cap

`IAFahim.GameTheory/GameTheory.cs Grundy.Run`: recomputes Grundy from scratch recursively with no memo → exponential, and `if (g >= 0 && g < 64)` silently ignores Grundy values ≥ 64, giving wrong mex when the game has many moves. `GameDp.Run` is the memoized version but uses `long seen` bitmask capped at 64 similarly. `GrundyDAG` caps mex at 32 via `mex < 32`. These caps produce wrong Grundy numbers for large games.

`NimSum.NextMove`: returns first pile where `target < piles[i]` — correct nim move exists, fine.

`Minimax`/`AlphaBeta`: `MakeMove`/`UndoMove` are stubs (`return state[move] > 0;` / no-op) so search explores a fake tree; placeholders, not real.

---

## Wrong answer: `Permutation.PermPower` corrupts `p` while squaring

`IAFahim.Permutation/Permutation.cs PermPower`:
```cs
for (...) temp[i] = p[i];
for (...) p[i] = temp[temp[i]];   // squares p in place
```
But the result accumulation just before uses the *current* (already partially squared) `p`, and you square `p` every iteration mutating the caller's input permutation. After the call `p` is destroyed and the accumulation order (square before/after multiply) doesn't match binary exponentiation cleanly. Result is wrong and `p` is clobbered.

`PermutationRank`/`KthPermutation` use `int fact` which overflows for `n > 12` (`13! > 2^31`). `KthPermutation` declares `int* fact` — `12!` is the limit; silent overflow beyond. `PermutationRank` uses `long* fact` (better) but `KthPermutation` uses `int*` — inconsistent and overflowing.

`GrayUnrank` is correct; `GrayCode.FromGray(int)` correct.

---

## Wrong answer: `Compress`/`Coordinate` — `RankCompress` fine, `CompressValues.RunUnique` assumes sorted

`IAFahim.Compress/CompressValues.cs RunUnique`: dedupes adjacent equal — only correct if input is pre-sorted; otherwise it under-dedupes. The name implies general uniqueness. Document precondition. `CoordinateCompress.Run` sorts `tmp` then dedupes into `dstMap` — correct. `Discretize.Run` sorts `src` in place then dedupes — correct.

---

## Crash/UB: pervasive `stackalloc` with runtime-sized `n`

Many routines do `stackalloc T[n]` (or `n*n`, `(target+63)>>6`, `(la+1)*(lb+1)`, etc.) where `n` comes from the caller. For large `n` these overflow the stack (CLR throws or corrupts). Notable offenders: `EditDistance`/`Lcs` (`(la+1)*(lb+1)`), `CondenseGraph` (`n*n`), `WarshallBitset` callers, `BellNumbers`/`PartitionNumbers` (`n+1` ok-ish but unbounded), `SubsetSum`/`BitsetSubsetSum` (`target`-sized), every graph routine that `stackalloc int[n]` for queues, `Hungarian` (`n+1` arrays, fine), `Johnson` (`(n+1)` plus `m+n`), `SuffixArrayBuild` (`stackalloc int[n]` ×4 plus a nested `stackalloc int[n]` *inside the k-loop* — that one allocates per iteration of a `log n` loop, leaking stack). The library's no-allocation philosophy is fine, but these need caller-provided buffers, not `stackalloc`, for any real input size.

Specifically dangerous: `SuffixArrayBuild` does `int* sa2 = stackalloc int[n];` **inside** the `for (k...)` loop — stack allocation in a loop is not freed until the method returns, so it accumulates `log n` copies. Move it out.

---

## Summary of the worst offenders (fix these first)

1. **Fenwick 1D** (`(idx+1) & -idx` everywhere) — every BIT-based result is wrong.
2. **Flow `e ^ 1` pairing** with ids starting at 1 — all max-flow/min-cost residuals broken.
3. **Kosaraju** order array — SCC results garbage.
4. **`Graph.Misc` `-1` sentinel** vs library's `0` — infinite/garbage traversal.
5. **Sparse table + Wavelet tree** families — non-functional.
6. **Generic `Run<T>` with `dynamic`** in Prefix/Suffix — throws on `long`, mutates on query.
7. **Matrix elimination integer `/=`** — wrong determinants/inverses/solves.
8. **`PermPower`, `BitsetSubsetSum` (DP/Dp.cs), `LinearRecurrence`/`Kitamasa`, `MultiPointEval`, `BellNumbers`** — wrong math.
9. **Blossom/WeightedBlossom, Flow MinCut/lower-bounds, Minimax** — stubs that silently return wrong/zero.
10. **Treap `RangeQuery` by-value root** — corrupts tree across calls.
11. **`Rotate`/`Splay`/`LinkCut` null-deref before guard**, **`ZeroOneBfs` deque underflow**, **`SpfaCircular` guard** — crashes / early termination.

Want me to write corrected drop-in implementations for any specific group — say, the whole Fenwick file and the flow edge-pairing fix? Those two alone unblock the largest fraction of dependent code.
