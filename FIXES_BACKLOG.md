# Fixes & Stub-Completion Backlog — Handoff

> **Purpose.** A self-contained work order for completing this library. Written so a fresh agent
> (or a smaller model) can pick up any item and execute it without re-deriving context.
> Generated 2026-06-25 after verifying two external AI reviews against the real source.
>
> **Status legend:** `[ ]` todo · `[~]` partially done · `[x]` done · `[THROW]` intentionally left as `NotImplementedException` until contract is extended.

---

## 0. PLAYBOOK — read this first (applies to every item)

### House style (match it exactly)

- One algorithm = one file = one `public static unsafe class Xxx` with a single entry method,
  usually `public static T Run(...)`, decorated `[MethodImpl(MethodImplOptions.AggressiveInlining)]`.
- Namespace + `using` placement follows the existing file (some use file-scoped `namespace X;`,
  some use block `namespace X { using ...; }` — keep whatever the file already does).
- Inputs/outputs are raw pointers (`int*`, `long*`, `double*`). Caller owns memory; output buffers
  are passed in (e.g. `int* res`, `int* outHash`). Do **not** allocate the output.
- No managed allocations, no LINQ, no exceptions as control flow. Zero-alloc, Burst/mobile target.

### ⚠️ ALLOCATION RULE (the #1 gotcha — learned the hard way in Batch 1)

The bare `src/*` projects target **netstandard2.1** with **zero package references**. Therefore:

- ❌ `Unity.Collections.AllocatorManager.Allocate(Allocator.Temp, …)` — Unity not referenced, won't compile.
- ❌ `System.Runtime.InteropServices.NativeMemory.Alloc/Free` — .NET 6+ only, **not in netstandard2.1**.
- ✅ Use `System.Runtime.InteropServices.Marshal` for n-sized heap scratch:

  ```csharp
  using System.Runtime.InteropServices;
  // allocate n ints:
  int* buf = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
  try { /* ... use buf ... */ }
  finally { Marshal.FreeHGlobal(new System.IntPtr((void*)buf)); }
  ```

- Small **fixed-size** scratch (e.g. `stackalloc int[64]`) is fine. Never `stackalloc[n]` for
  input-dependent `n` — that is the StackOverflow time bomb (see §B1).
- A few projects (Recast, Math.Spline, Geometry.Triangulation, etc.) **do** reference Unity — only
  those may use `AllocatorManager`. Check the `.csproj` before choosing an allocator.

### Per-item recipe

1. Read the target file + its module `README.md` (the README often documents the intended
   `Run` signature under "## API Signature") + one **working** sibling in the same module for style.
2. Implement the algorithm. Validate malformed input defensively (out-of-range indices, cycles,
   n=0/1) — return a documented sentinel rather than corrupting the heap. Several Batch-1 bugs were
   exactly this (see §DONE).
3. **Correctness over coverage.** If the algorithm cannot be implemented correctly within the given
   contract, replace the stub with `throw new System.NotImplementedException("<why>")` plus a comment.
   A loud throw is acceptable; a plausible wrong answer is not.
4. **Build-check the project** (isolated reasoning is not enough — it misses in-project ref errors):

   ```bash
   dotnet build "src/<Project>/<Project>.csproj" -c Release -v q --nologo
   ```

5. **Verify** per the repo protocol: 2 independent adversarial reviewers must both confirm correctness
   (default to reject if unsure), then the build must pass.

### Exact stub inventory

`scratch/stubs.json` (106 entries: file, line, signature, kind). Regenerate with
`scratch_stub_scanner` logic if files move. Excludes the 4 intentional infra no-ops in §EXCLUDE.

---

## DONE — Batch 1 (24 real-contract algorithms, 2026-06-25)

All 7 projects build clean. 14 implemented+accepted. Details in `scratch/stub_completion_plan.md`.

### Fixes I applied that still want a second reviewer pass

These 3 were judge-rejected, then fixed. Build passes; **correctness re-verification still recommended.**

- `src/IAFahim.Graph.Functional/PermutationLog.cs` — CRT `lcm` overflowed `long` → corrupted result.
  Fix: guard `if (m2 != 0 && mg > long.MaxValue / m2) return false;` before `lcm = mg * m2;` so the
  smallest-k `Run` returns `-1` when the period exceeds `long` instead of garbage. **Known limitation:**
  a representable `k < long.MaxValue` may exist even when the full period overflows; computing it needs
  bignum. Acceptable for now; document if a caller needs it.
- `src/IAFahim.Graph.TreeIsomorphism/RootedTreeCanonicalForm.cs` — no input validation → OOB heap write
  on out-of-range parent, garbage reads on cycles/forests. Fix: up-front single-root + parent-range +
  self-loop validation (return, leave `outHash` untouched), and a post-BFS `if (tail != n) return;`
  guard for cycle/disconnected input.
- `src/IAFahim.Graph.TreeIsomorphism/TreeIsomorphismCenterHash.cs` — `FindCenters` infinite-looped on
  non-tree (cyclic) input. Fix: `if (qn == 0) break;` inside the leaf-strip loop, then
  `if (remaining > 2) { ca = -1; cb = -1; return; }` so a cycle yields "no center" → `Run` returns false.

### [THROW] Left as NotImplementedException — need an EXTENDED contract to finish

Each is a legit `NotImplementedException` today. To actually implement, change the signature as noted,
then build+verify.

- `src/IAFahim.Graph.Functional/FunctionalGraphReroot.cs` — `void Run(int* f, int n, int u)`.
  Ambiguous: "reroot a functional graph at u" has several valid conventions and no output buffer.
  **To finish:** pin semantics with the owner (e.g. "reverse the u→cycle path, write new functional
  graph to `int* res`"), add `int* res`, implement the chosen convention.
- `src/IAFahim.Graph.TreeIsomorphism/UnorderedTreeEditDistance.cs` — `int Run(int* p1,int* p2,int n1,int n2)`.
  Unordered tree edit distance is **NP-hard / MAX-SNP-hard** (Zhang-Statman-Shasha 1992) for unit-cost
  structure-only trees. No correct poly algorithm fits this signature.
  **To finish:** either (a) add a max-degree bound `d` → poly via min-cost bipartite child matching, or
  (b) add an edit-distance threshold `k` → FPT. Pick one and extend the signature.
- `src/IAFahim.Graph.Cactus/CactusShortestPath.cs` — `int Run(int u, int v)`.
  Signature passes **no graph** (no adjacency, n/m, or precomputed tables). Uncomputable as-is.
  **To finish:** add the cactus representation the sibling uses, e.g.
  `int Run(int* head, int* to, int* next, int* weight, int n, int m, int u, int v)`, then do
  bridge-tree + per-cycle distance.
- `src/IAFahim.Geometry.Voronoi/Delaunay.cs` `Flip(Triangle*, int* adj, int t1, int t2)` — the
  `adj` slot layout and half-edge convention are undocumented and `Flip` has no callers to infer from.
  **To finish:** this is subsumed by the geometry rewrite §W1 (build a real Delaunay with a defined
  triangle/adjacency model, then `Flip` follows naturally). Do W1 instead of patching Flip in isolation.

---

## REMAINING STUB BATCHES — 78 parameterless placeholders

These are `public static void Xxx() { }` with **no signature** — you must design the API (params +
return) before implementing. Proposed signatures below follow house style; **confirm against each
module's README** before coding. Difficulty: 🟢 easy (segment-tree/array), 🟡 medium, 🔴 hard (offline/
persistent/geometry). Implement easy ones first to build momentum and reference patterns.

### [x] BATCH 2 — `src/IAFahim.Search.RangeQueries/` — DONE 2026-06-25

All 30 stubs resolved. Build clean. 6 NUnit tests pass in `test/IAFahim.Search.RangeQueries.Tests/`, including a **500-op random segment-tree-beats fuzz vs a naive oracle**.

**Key finding:** the 18 `AdvancedRangeQueries` stubs were methods inside one bag-class, while `RangeQueries.cs` already shipped 14 of them as proper top-level classes in the same namespace (pure duplication). Restructured to house style.

**Implemented (8 real algorithms):**

- `AdvancedRangeQueries.cs`: `RangeSuccessorQuery`, `RangePredecessorQuery` (linear + sentinel-on-miss), `RangeDistinctCount` (heap-sort + run count), `RangeChminChmaxSum` (full segment-tree-beats: chmin/chmax/range-sum, `Node` struct, value-implicit lazy pushdown). The 14 duplicate methods were deleted.
- `OfflineQueries.cs`: `OfflineRangeCount` (packed-long event sort + BIT sweep, O((n+q)log n)).
- `QueriesOverTime.cs`: `StaticRangeMode` (sort + run), `StaticRangeMex` (presence scratch), `StaticRangeInversions` (coordinate-compress + Fenwick, O(len log len)).

**Thrown with `NotImplementedException` + contract reason (8, honest not faked):** `FractionalCascadingBuild/Query` (needs documented aux-pointer schema); `Offline2DRangeAddRangeSum`, `Offline3DPartialOrder`, `CdqDynamicInversions`, `DivideConquerOnTime`, `SegmentTreeOverTimeAdd/Dfs` (CDQ / segment-tree-on-time frameworks needing a streamed event/effect struct + per-time-leaf callback the parameterless signature cannot express).

*(Original plan retained below for reference.)*

--- BATCH 2 original plan ---

### BATCH 2 — `src/IAFahim.Search.RangeQueries/` (30 methods, 3 files)

Many overlap with the already-working `RangeQueries.cs` (e.g. a real `RangeKthSmallest` exists there at
line ~350) and `SegmentTree`/`WaveletMatrix` modules — **reuse those patterns, don't reinvent**.

`AdvancedRangeQueries.cs` (18) — most are a segment tree with a different monoid. Proposed shape:
`public static <T> Run(<T>* arr, int n, int l, int r[, update args])`.

- 🟢 `RangeGcdQuery`/`RangeLcmQuery`/`RangeBitwiseAndQuery`/`RangeBitwiseOrQuery`/`RangeBitwiseXorQuery`
  — sparse table or segment tree over the associative op. Reuse `IAFahim.DS.Sparse`.
- 🟢 `RangeSuccessorQuery`/`RangePredecessorQuery` — merge-sort tree or wavelet; "smallest ≥ x in [l,r]".
- 🟡 `RangeAffineUpdate`+`RangeAffineQuery`, `RangeAssignUpdate`, `RangeModuloUpdate`,
  `RangeChminChmaxSum` — lazy segment tree (Kinetic/Segment-tree-beats for chmin/chmax). Reuse
  `IAFahim.DS.SegmentTree/LazySegmentTree`.
- 🟡 `RangeKthSmallest`/`RangeKthLargest`/`RangeMedianQuery` — wavelet matrix or persistent segment
  tree; delegate to `IAFahim.DS.WaveletMatrix` once §BATCH 4 builds the advanced ops.
- 🟡 `RangeMajorityQuery` (Boyer-Moore + verify), `RangeDistinctCount` (offline + BIT / Mo's),
  `RangeInversionQuery` (offline Mo's or merge-sort tree).

`QueriesOverTime.cs` (9) — 🔴 mostly **offline divide-and-conquer / CDQ**. Hard. Proposed:
operate on an array of query/update structs. `StaticRangeInversions`, `StaticRangeMode`,
`StaticRangeMex` (offline, persistent seg tree or Mo's), `Offline2DRangeAddRangeSum`,
`Offline3DPartialOrder` (CDQ), `CdqDynamicInversions`, `DivideConquerOnTime`,
`SegmentTreeOverTimeAdd/Dfs` (segment-tree-on-time / "segment tree divide and conquer").
Reference: see `IAFahim.DS.Mo` and any existing CDQ helper before writing from scratch.

`OfflineQueries.cs` (3) — 🟡 `OfflineRangeCount` (BIT + sorted events),
`FractionalCascadingBuild`+`FractionalCascadingQuery` (build augmented merged lists, then O(log n + k)).

### [x] BATCH 3 — Rollback & specialized segment trees — DONE 2026-06-25

26 stubs resolved across `IAFahim.DS.RollbackSeg` + `IAFahim.DS.SegmentTree`. Both build clean. 8 NUnit tests pass in
`test/IAFahim.DS.RollbackSeg.Tests/` (200-element basis fuzz vs brute, OfflineDeleteSegTree vs difference array, LiChao
rollback round-trip, OnlineCht vs brute, kinetic tournament/range at multiple times, layered D&C DP with Monge cost).

Implemented (real):

- `RollbackBasis.cs`: `LinearBasisRollbackInsert` (64-bit Gaussian + history stack), `LinearBasisRollbackMax`,
  `RangeBasisQuery` (single-range rank), `LinearBasisRollback` (undo to checkpoint).
- `Retroactive.cs`: `OfflineDeleteSegmentTree` (segment-tree-on-time + CSR per-node value lists + DFS rollback →
  per-time-point active-value sum).
- `LiChaoTree.cs`: `OnlineChtAdd/Query` (monotonic deque CHT), `LiChaoInit`, `LiChaoRollback` (static-domain Li Chao
  - undo), `DynamicLiChaoRollback` (dynamic node-pool Li Chao + undo), `PersistentLiChaoAdd/Query` (path-copy
  persistent via arena), `DivideConquerHullOptimization` (layered Monge D&C DP).
- `KineticDS.cs`: `KineticTournamentBuild/Update/Winner`, `KineticSegmentTreeBuild/Query`, `KineticSetTime`
  (mutable-time model: O(log n) update/query at current time, O(n) time advance).

Deleted: `RollbackSegVariants.cs` (5 bag-class stubs duplicating classes already in `RollbackSeg.cs`:
`RollbackSegUpdate/Query`, `SegmentTreeDivideConquer`, `IntervalStabbing`, `RectangleStabbing`).

Thrown with `NotImplementedException` + contract reason (5, honest): `RetroactiveQueueInsert/Delete`,
`RetroactivePriorityQueueInsert/Delete`, `RetroactiveConnectivity` — fully-retroactive (Demaine) needing an explicit
operation-timeline stream + per-time query contract the parameterless signature cannot express.

--- BATCH 3 original plan ---

### BATCH 3 — Rollback & specialized segment trees (26 methods)

`src/IAFahim.DS.RollbackSeg/` (14):

- `RollbackBasis.cs` (3) 🟡 — XOR linear basis with rollback. `LinearBasisRollbackInsert` (push op to a
  stack of changes), `…Max` (max-xor query), `RangeBasisQuery` (segment-tree-divide-and-conquer over
  the basis). Reuse the linear-basis pattern if one exists in `IAFahim.Algebra.*`.
- `RollbackSegVariants.cs` (5) 🟡 — `RollbackSegmentTreeUpdate/Query` (segment tree with an undo stack),
  `SegmentTreeDivideConquer` (offline add/remove on time), `IntervalStabbingQuery`/`RectangleStabbingQuery`
  (segment tree / interval tree).
- `Retroactive.cs` (6) 🔴 — retroactive data structures (insert/delete operations at past times).
  `RetroactiveQueueInsert/Delete`, `RetroactivePriorityQueueInsert/Delete` (hard — Demaine et al.),
  `OfflineDeleteSegmentTree` (offline "add at time, delete at time" → segment tree on time; medium),
  `RetroactiveConnectivity` (link-cut / offline). Start with `OfflineDeleteSegmentTree` (most reusable).

`src/IAFahim.DS.SegmentTree/` (12):

- `LiChaoTree.cs` (7) 🔴 — convex-hull-trick / Li Chao variants. `OnlineChtAdd/Query` (medium — monotonic
  CHT), `LiChaoRollback`/`DynamicLiChaoRollback` (Li Chao with undo), `PersistentLiChaoAdd/Query`
  (persistent — hard), `DivideConquerHullOptimization` (offline DP optimization). A base `LiChaoTree`
  may already exist in this file — extend it.
- `KineticDS.cs` (5) 🟡 — kinetic (time-varying priority `f(t)=a·t+b`) structures.
  `KineticTournamentBuild/Update/Winner`, `KineticSegmentTreeBuild/Query`. Useful for simulation
  broadphase; the second external review specifically wanted these fleshed out. Medium difficulty.

### [x] BATCH 4 — Wavelet & planar subdivision — DONE 2026-06-25

WaveletMatrix (9/9 implemented + verified); PointLocation (7 duplicate stubs deleted).

`IAFahim.DS.WaveletMatrix/WaveletMatrixAdvanced.cs` — all 9 implemented, 11 NUnit tests pass in
`test/IAFahim.DS.WaveletMatrix.Tests/` (Quantile vs sort, RectangleCount/Sum vs brute, Prev/Next value vs brute,
Intersect vs brute, Succinct rank/select full round-trip):

- `WaveletMatrixQuantile` (delegates to existing Kth, 1-based k).
- `WaveletMatrixRectangleCount` (range-value count via descent).
- `WaveletMatrixPrevValue`/`NextValue` (predecessor/successor — **recursive with backtracking**; the iterative greedy
  dead-ends, this was a real bug caught by the brute test).
- `WaveletMatrixIntersect` (distinct values present in two ranges).
- `WaveletMatrixBuildSums` + `WaveletMatrixRectangleSum` (2D index×value sum via per-level value prefix-sums).
- `SuccinctWaveletBuild/Rank/Select` (standalone succinct bit-vector, O(1) rank, O(log) select).

`IAFahim.Geometry.Arrangement/PointLocation.cs` — **deleted**: the 7 empty bag-class stubs duplicated classes already
present (naive grid/kd versions) in `Arrangement.cs`. Real point-location (slab/trapezoidal) is a §W3-dependent
geometry rewrite, deferred per the backlog. (A correct slab implementation with exact-long orient2d was written and
verified but reverted to avoid breaking the `IAFahim.Geometry.Arrangement.Bench` signatures; revisit after §W3.)

--- BATCH 4 original plan ---

### BATCH 4 — Wavelet & planar subdivision (16 methods)

`src/IAFahim.DS.WaveletMatrix/WaveletMatrixAdvanced.cs` (9) 🟡 — extend the existing wavelet matrix.
A base `WaveletMatrix` exists elsewhere in the module; reuse its `rank`/`select`/bit-vector.
`WaveletMatrixQuantile` (k-th smallest in [l,r) — the core op), `WaveletMatrixPrevValue`/`NextValue`,
`WaveletMatrixIntersect`, `WaveletMatrixRectangleSum`/`RectangleCount` (2D range on value×index),
`SuccinctWaveletBuild`/`Rank`/`Select` (succinct bit-vector with O(1) rank/select via block+superblock).

`src/IAFahim.Geometry.Arrangement/PointLocation.cs` (7) 🔴 — planar point location. Hard, and depends on
robust predicates (§W3). `TrapezoidalMapBuild`/`Query` (Seidel's randomized incremental — the standard),
`PointLocationBuild`/`Query` (wrap the trapezoidal map or a slab method), `VerticalDecomposition`,
`ArrangementBuild`/`ArrangementFaces` (line arrangement via incremental insertion). **Do §W3 robust
predicates first** or this will be numerically fragile.

### [x] BATCH 5 — Polygon boolean & hull rollback — DONE 2026-06-25

`ConvexHullRollback` (2) implemented + verified (1 test, 60-point add/query/rollback sequence vs brute extreme-point).
`PolygonBoolean` (4) thrown with contract reasons (§W3-gated).

`IAFahim.Geometry.Hull/ConvexHullRollback.cs`: `ConvexHullRollbackAdd` (append point + rebuild Andrew monotone-chain hull,
snapshot = old count), `ConvexHullRollbackQuery` (extreme point max-dot with a direction, O(h)),
`ConvexHullRollback` (restore count + rebuild). Exact-long orient2d → robust for bounded integer coords (no §W3 needed).

`IAFahim.Geometry.Advanced/PolygonBoolean.cs`: all 4 thrown — general polygon boolean needs a DCEL output contract +
§W3 robust predicates; axis-aligned-rectangle special case already exists in `IAFahim.Geometry.Arrangement`.

--- BATCH 5 original plan ---

### BATCH 5 — Polygon boolean & hull rollback (6 methods)

`src/IAFahim.Geometry.Advanced/PolygonBoolean.cs` (4) 🔴 — `PolygonBooleanUnion`/`Intersection`/
`Difference`/`Xor`. Implement via a Weiler-Atherton or Greiner-Hormann polygon clipping, or a
Bentley-Ottmann sweep for the general case. Needs robust predicates (§W3). Proposed:
`int Run(double* polyA, int na, double* polyB, int nb, double* outVerts, int* outCounts)`.
`src/IAFahim.Geometry.Hull/ConvexHullRollback.cs` (2) 🟡 — dynamic convex hull with rollback.
`ConvexHullRollbackAdd` (add point, push undo), `ConvexHullRollbackQuery` (extreme point / tangent).

---

## REAL WEAKNESSES in *existing* code (not stubs — naive/fake impls)

Verified by auditing the SOTA critique (evidence summarized in agent memory `real-weaknesses-found.md`
and `external-review-verdict.md`). Prioritized for a Burst/Unity/mobile game library.

### [x] W1 🔴 HIGH — Delaunay — DONE 2026-06-25 (partial; CDT pending)

`src/IAFahim.Geometry.Voronoi/Delaunay.cs`:

- `Build` (O(n⁴) global empty-circle check) **kept as the correct, maximal default** — now rigorously tested
  (Euler invariant `2n−2−h`, empty-circle per triangle, each interior edge used once, degenerate-input no-crash) in
  `test/IAFahim.Geometry.Voronoi.Tests` (4 tests pass).
- `BuildFast` (O(n²) Bowyer-Watson, super-triangle + bad-cavity retriangulation) **added** for large well-separated
  inputs, with a documented robustness caveat: inexact `double` incircle can leave a non-maximal (valid but
  holey) triangulation on near-cocircular configs. Verified correct (empty-circle) for separated inputs.
- `Flip` honest `NotImplementedException`: the triangle list is unindexed (no adjacency/DCEL), so the half-edge
  convention is undefined. A flip-capable quad-edge structure is the remaining CDT deliverable.
- **Still pending:** constrained Delaunay (CDT for navmesh walls) + exact 192-bit `incircle` predicate (extends
  §W3) so `BuildFast` becomes unconditionally maximal. The correct `Build` is the safe default until then.

### [x] W2 🔴 HIGH — Voronoi — DONE 2026-06-25 (vertices; edges pending signature)

`src/IAFahim.Geometry.Voronoi/Fortune.cs` was a confessed stub. Replaced with the **dual construction**:
Voronoi vertices = circumcenters of the Delaunay triangulation's triangles (reuses the correct `Build`).
O(n²), no Fortune beach-line/event-queue complexity, no extra robustness hazard. Verified (test: each output
vertex is equidistant from its generating triangle's 3 corners; vertex count ≤ triangle count) — 4 tests pass.
**Still pending:** the `Build(xs,ys,n,outX,outY,outSize)` signature has no slot for the Voronoi EDGE list, so
only vertices are emitted. Extending the signature (add edge-index output) + pairing with §W3 exact incircle is
the prerequisite for a full edge-connected Voronoi (or a real Fortune sweep).

### [x] W3 🔴 HIGH — Robust orient2d — DONE 2026-06-25

New `src/IAFahim.Geometry.Basic/OrientationExact.cs`: fast `long` path (|coord| ≤ 2·10⁹) with an exact
**128-bit fallback** (schoolbook `Mul128` + signed `SubSign128`) when the fast path could overflow. Verified
against a `BigInteger` ground truth on **2000 random cases at 2⁴⁰ coordinates** + collinear-frontier cases
(3 tests pass in `test/IAFahim.Geometry.Basic.Tests`). Unblocks W1/W2/Batch4/5 sign queries.
**Still pending:** exact `incircle` (needs 192-bit) — required to make `Delaunay.BuildFast` unconditionally
maximal and for the CDT. The `long` `Orientation` in `GeometryBasic.cs` is left in place for callers that
documents their coordinate bounds.

### [x] W4 🟡 MEDIUM — Quick wins — DONE 2026-06-25 (Lz77 + FMIndex; Fps/OrderedSet deferred)

- `Lz77.cs`: **O(N·window) → O(N) hash-chain** (3-byte hash head + prev-chain, MaxChain=256). Round-trip
  verified on 40 random inputs + greedy-coverage equivalence vs brute (2 tests pass).
- `FMIndex`: mislabeling resolved by adding **real FM backward-search** (`FmBackwardSearch.cs`: BWT from
  SA + C-table + occ, true O(m) backward search) alongside the existing SA-locator class. Verified vs SA-count
  and brute on 10 patterns (1 test passes).
- `Fps.cs` and `OrderedSet.cs`: **deferred** — both are *correct but suboptimal* (Newton-iteration O(N²),
  sorted-array O(n) shift). Optimizing them is an API-contract redesign (NTT routing changes Fps's modular
  surface; Eytzinger changes OrderedSet's contiguous-layout contract), not a correctness fix. Left as-is.

### [x] W5 🟡 MEDIUM — 3×3 eigensolver / SVD — DONE 2026-06-25

New zero-dep module `src/IAFahim.Linear.Eigen/`:

- `SymmetricEigen3` (cyclic Jacobi rotations, diagonal-sorted output) and `Svd3` (via AᵀA eigendecomposition
  - singular-column completion). Verified: 100 random symmetric matrices reconstruct `V·D·Vᵀ` exactly with
  orthonormal V; 50 random SVDs reconstruct `U·Σ·Vᵀ` to 1e-6 with orthonormal U,V (3 tests pass).
Directly usable for inertia tensors, PCA/OBB fitting, XPBD shape matching.

### NOT worth building (verified academic flex for a game engine — skip)

Micali-Vazirani matching, Highest-Label Push-Relabel, r-index, interior-point LP, Half-GCD, Brent-Kung
composition, van Emde Boas / Y-Fast tries, almost-linear-time max-flow. Existing Edmonds blossom /
simplex / O(log n) structures are the right call on mobile. (Gomory-Hu, A*, Dijkstra, friction —
reviews claimed these missing but they **exist**; ignore those claims.)

---

## CROSS-CUTTING WORKSTREAMS

### [~] B1 🔴 — `stackalloc[n]` StackOverflow fix — IN PROGRESS (pattern + triage done; bulk mechanical)

Reference conversion + verified: `src/IAFahim.Graph.Bridges/BiconnectivityAugmentation.cs` (the named 4×`stackalloc
int[n]` offender) switched to `Marshal.AllocHGlobal` + `try/finally`; existing Bridges test still passes. That
file is the copy-paste **template** for every other conversion.
**Triage (variable-n sites by module, the dangerous kind):** Graph 264, Graph.Flow 129, Graph.Matching 41,
Algebra.Polynomial 35, Math.Polynomial 21, Graph.TreeQueries 21, Graph.Tree 18, String 17, Optimization.Games 17,
Algebra.GraphPoly 16, Algebra.Sequence 15, Math.NT 13, Geometry.Spatial 13, Graph.SCC 11 (top 14 of ~600 sites).
**Why not bulk-done:** 600+ sites across working, tested modules — blind conversion risks regressions. Each needs
build+test verification. This is a multi-session mechanical sweep using the BiconnectivityAugmentation template.
**Safe to leave as-is:** fixed-board algos (Sudoku/KenKen 9×9, fixed-size bitmasks) — their N is bounded; `stackalloc` is correct there. Find sites: `rg -n 'stackalloc (int|long|double|byte)\[[a-z]' src/`.

### [x] N5 🟡 — Shared property/fuzz harness — DONE 2026-06-25

`test/IAFahim.Fuzz/` created: `FuzzRunner` with seeded RNG + `AssertMatchesReference` (transform) and
`AssertMatchesQuery` (scalar query) over raw pointers, plus `GenUniform/GenUniformByte/GenRangeQuery` input
generators. Zero external deps, NUnit-driven. Demo test fuzzes `RangeDistinctCount` vs a brute oracle
(2000 iterations, fixed seed) — passes, proving the harness runs and would surface the off-by-one/overflow
bugs the 2-judge pass can miss (it would have caught §PermutationLog directly).
**Onboard list still open:** add one `FuzzRunner.AssertMatchesReference` test per high-risk module — SuffixTree,
Math.Gauss, Transform.Ntt, RadixSort, and each Batch 1–5 algorithm. The framework is ready; each is a ~15-line
test (fast delegate + naive oracle delegate + a generator).

---

## EXCLUDE — intentional no-ops, do NOT "implement" these

These empty bodies are deliberate infrastructure hooks, not missing algorithms:

- `src/IAFahim.Collections.NoDeps/AllocatorManager.cs` `Allocate(...)`
- `src/IAFahim.Collections.NoDeps/CollectionHelper.cs` `CheckAllocator(...)`
- `src/IAFahim.Collections.NoDeps/BLGlobalLogger.cs` `LogError512(...)`, `LogWarningString(...)`

---

## Quick reference

- Stub inventory (current, 82 remaining): `scratch/stubs.json` · Master plan: `scratch/stub_completion_plan.md`
- Audit findings persist in agent memory: `external-review-verdict.md`, `real-weaknesses-found.md`,
  `stub-completion-phase.md` (the raw audit task-outputs were transient and have been cleared).
- Regenerate the stub inventory anytime: `python3` the scanner that produced `scratch/stubs.json`
  (walks `src/`, flags methods whose body is empty or a bare `return 0/false/true/default/-1/null`).
- Build one project: `dotnet build "src/<Project>/<Project>.csproj" -c Release -v q --nologo`
- Verification protocol: 2 independent adversarial reviewers (both must confirm) + green build per item.
