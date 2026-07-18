# Package status — reference upgrade pass

## Summary
- Total packages: **162**
- already_correct (PASS test): **103**
- upgraded this pass: **2**
- verified_build (build only): **50**
- deferred (honest limitation / shell): **7**

Artifacts: `scratch/INVENTORY.md`, `scratch/REFERENCE_MAP.md`, `scratch/PACKAGE_STATUS.json`, `{SCRATCH}/verify-*.log`, `scratch/contract-notes.txt`

## Top-tier reference sources
| Source | Use |
|---|---|
| AtCoder Library (atcoder/ac-library) | DSU, fenwick, segtree, Dinic, SCC, NTT/convolution, string |
| KACTL (kth-competitive-programming/kactl) | Geometry, graph, NT, strings |
| CP-Algorithms (cp-algorithms.com) | Reference algorithms + proofs |
| Boost.Graph | Industrial graph concepts |
| geogram / CGAL | Robust Delaunay/Voronoi/hull |
| recastnavigation | Navmesh build + Detour |
| .NET BCL Span.Sort | Sort behavioral baseline only |
| Erin Catto GJK / Bullet | Collision GJK-EPA |

## Per-package
| Package | Status | Verify | Family reference | Notes |
|---|---|---|---|---|
| `IAFahim.Algebra.GraphPoly` | already_correct | PASS(test) | AtCoder Library / CP-Algorithms poly | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Algebra.Polynomial` | verified_build | PASS(build) | AtCoder Library / CP-Algorithms poly | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Algebra.Sequence` | already_correct | PASS(test) | AtCoder Library / CP-Algorithms poly | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Collections.NoDeps` | already_correct | PASS(test) | Unity.Collections contract | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Collision.Gjk` | already_correct | PASS(test) | Erin Catto GJK / Bullet Physics | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Combinatorics.Generation` | already_correct | PASS(test) | FKM prenecklace / next_permutation | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Compress` | already_correct | PASS(test) | coordinate compression literature | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Compress.Coordinate` | already_correct | PASS(test) | coordinate compression literature | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DP` | verified_build | PASS(build) | CP-Algorithms DP | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.DP.General` | verified_build | PASS(build) | CP-Algorithms DP | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.DP.Knapsack` | already_correct | PASS(test) | CP-Algorithms DP | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DP.Optimization` | verified_build | PASS(build) | CP-Algorithms DP | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.DS.Dsu` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.Fenwick` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.FixedCollections` | verified_build | PASS(build) | ACL dsu/fenwick/segtree + KACTL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.DS.GapBuffer` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.Grid` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.Heap` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.HilbertOrder` | upgraded | PASS(test) | ACL dsu/fenwick/segtree + KACTL | aligned to reference defining mechanics; tests strengthened/added |
| `IAFahim.DS.LinkCut` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.Mo` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.OrderedSet` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.PerfectHashMap` | verified_build | PASS(build) | ACL dsu/fenwick/segtree + KACTL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.DS.PersistentDsu` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.PersistentTreap` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.PieceTable` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.RollbackSeg` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.RollbackStack` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.Rope` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.SegmentTree` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.Sparse` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.SpatialMap` | verified_build | PASS(build) | ACL dsu/fenwick/segtree + KACTL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.DS.Splay` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.Treap` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.Trie` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.UnsafeArray` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.DS.WaveletMatrix` | already_correct | PASS(test) | ACL dsu/fenwick/segtree + KACTL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.GameTheory` | already_correct | PASS(test) | Sprague–Grundy theorem | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Geometry.Advanced` | already_correct | PASS(test) | KACTL geometry / geogram / CGAL | Package tests green; PolygonBoolean Union/Diff/XOR honest NI until DCEL contract |
| `IAFahim.Geometry.Arrangement` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Azimuth` | already_correct | PASS(test) | KACTL geometry / geogram / CGAL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Geometry.Basic` | already_correct | PASS(test) | KACTL geometry / geogram / CGAL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Geometry.Bvh` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Curve` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Delaunay` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Frame` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Hull` | already_correct | PASS(test) | KACTL geometry / geogram / CGAL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Geometry.Intersect` | already_correct | PASS(test) | KACTL geometry / geogram / CGAL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Geometry.MarchingCubes` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Mesh` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.PolygonClip` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Spatial` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Subdivision` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.SweepPrune` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Triangulation` | verified_build | PASS(build) | KACTL geometry / geogram / CGAL | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Geometry.Voronoi` | already_correct | PASS(test) | KACTL geometry / geogram / CGAL | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Bridges` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Cactus` | verified_build | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Graph.Centroid` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Clique` | deferred | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | README shell; no algorithm sources (empty package) |
| `IAFahim.Graph.Connectivity` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Cut` | deferred | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | README shell; min-cut covered by Graph.Flow |
| `IAFahim.Graph.DAG` | verified_build | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Graph.Decomposition` | deferred | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | README shell; no algorithm sources |
| `IAFahim.Graph.Dominator` | deferred | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | README shell; no algorithm sources |
| `IAFahim.Graph.DynamicTrees` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Eertree` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Eulerian` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Flow` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Functional` | verified_build | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Graph.Matching` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Misc` | verified_build | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Graph.RandomWalk` | deferred | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | README shell; no algorithm sources |
| `IAFahim.Graph.SCC` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.ShortestPath` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.SpanningTrees` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.Tree` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.TreeDecomposition` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Graph.TreeIsomorphism` | verified_build | PASS(build) | ACL maxflow/SCC + KACTL + Boost.Graph | Build OK; UnorderedTreeEditDistance unconstrained NP-hard NI; RunConstrained is poly path |
| `IAFahim.Graph.TreeQueries` | already_correct | PASS(test) | ACL maxflow/SCC + KACTL + Boost.Graph | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Linear` | deferred | n/a | Gaussian elimination / eigen literature | meta-folder only (no csproj); use Linear.Matrix/Eigen/Matrix2 |
| `IAFahim.Linear.Eigen` | already_correct | PASS(test) | Gaussian elimination / eigen literature | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Linear.Matrix` | already_correct | PASS(test) | Gaussian elimination / eigen literature | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Linear.Matrix2` | verified_build | PASS(build) | Gaussian elimination / eigen literature | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Math.Arithmetic` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Barycentric` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Basic` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.BigInt` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Combinatorics` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Gauss` | verified_build | PASS(build) | KACTL NT + ACL convolution/NTT | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Math.Kalman` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Modular` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.NT` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Noise` | verified_build | PASS(build) | KACTL NT + ACL convolution/NTT | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Math.PoissonDisk` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Polynomial` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Polynomial.Eval` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Polynomial.Fps` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.PotentialField` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Quaternion` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Sdf` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.SphericalHarmonics` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Spline` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Transform` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Transform.AnyMod` | verified_build | PASS(build) | KACTL NT + ACL convolution/NTT | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Math.Transform.Fft` | already_correct | PASS(test) | KACTL NT + ACL convolution/NTT | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Math.Transform.Ntt` | verified_build | PASS(build) | KACTL NT + ACL convolution/NTT | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Memory.Allocators` | verified_build | PASS(build) | Unity allocator contracts | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Optimization.Approximation` | verified_build | PASS(build) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Optimization.DivideConquer` | verified_build | PASS(build) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Optimization.Exact` | verified_build | PASS(build) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Optimization.Games` | verified_build | PASS(build) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Optimization.Geometric` | verified_build | PASS(build) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Optimization.Knapsack` | verified_build | PASS(build) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Optimization.Matroid` | verified_build | PASS(build) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Optimization.Offline` | already_correct | PASS(test) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Optimization.Submodular` | verified_build | PASS(build) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Optimization.Treewidth` | verified_build | PASS(build) | CP-Algo CHT/Li Chao/D&C opt / approx | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Pathfinding.Jps` | verified_build | PASS(build) | recastnavigation + Harabor JPS | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Pathfinding.Recast` | already_correct | PASS(test) | recastnavigation + Harabor JPS | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Permutation` | already_correct | PASS(test) | cycle decomposition literature | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Physics.Xpbd` | already_correct | PASS(test) | Müller XPBD paper | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search` | deferred | n/a | CP-Algorithms / KACTL search | meta-folder only (no csproj); use Search.* packages |
| `IAFahim.Search.Automaton` | verified_build | PASS(build) | CP-Algorithms / KACTL search | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Search.Bit` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.DifferenceArray` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.ExactCover` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.Imos` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.Interval` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.LIS` | verified_build | PASS(build) | CP-Algorithms / KACTL search | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Search.MeetInMiddle` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.Numerical` | verified_build | PASS(build) | CP-Algorithms / KACTL search | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Search.Prefix` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.Range` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.RangeQueries` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.Selection` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.Specialized` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.Subset` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.Suffix` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.TwoPointer` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Search.Window` | already_correct | PASS(test) | CP-Algorithms / KACTL search | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Sort.Insertion` | already_correct | PASS(test) | .NET Span.Sort baseline + CLRS | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Sort.Merge` | already_correct | PASS(test) | .NET Span.Sort baseline + CLRS | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Sort.Partition` | already_correct | PASS(test) | .NET Span.Sort baseline + CLRS | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Sort.QuickSort` | upgraded | PASS(test) | .NET Span.Sort baseline + CLRS | aligned to reference defining mechanics; tests strengthened/added |
| `IAFahim.Sort.RadixSort` | already_correct | PASS(test) | .NET Span.Sort baseline + CLRS | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.Sort.Specialized` | already_correct | PASS(test) | .NET Span.Sort baseline + CLRS | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.String` | already_correct | PASS(test) | ACL string + KACTL string | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.String.Automata` | verified_build | PASS(build) | ACL string + KACTL string | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.String.Compress` | verified_build | PASS(build) | ACL string + KACTL string | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.String.FMIndex` | already_correct | PASS(test) | ACL string + KACTL string | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.String.Grammar` | verified_build | PASS(build) | ACL string + KACTL string | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.String.Match` | already_correct | PASS(test) | ACL string + KACTL string | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.String.MinRotation` | already_correct | PASS(test) | ACL string + KACTL string | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.String.Palindrome` | verified_build | PASS(build) | ACL string + KACTL string | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.String.Parse` | already_correct | PASS(test) | ACL string + KACTL string | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.String.Pattern` | verified_build | PASS(build) | ACL string + KACTL string | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.String.SuffixArray` | verified_build | PASS(build) | ACL string + KACTL string | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.String.SuffixAutomaton` | already_correct | PASS(test) | ACL string + KACTL string | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
| `IAFahim.String.SuffixTree` | verified_build | PASS(build) | ACL string + KACTL string | isolated build OK; no dedicated test project; mechanics match family reference by review |
| `IAFahim.Unique` | already_correct | PASS(test) | std::unique / adjacent unique | isolated NUnit green; defining outcomes asserted; historical crit findings revalidated fix |
