# Algorithm package checklist — terminal perfect | deferred

Cross-off list: **perfect** when defining NUnit tests green + named world-class reference; **deferred** for shell/empty/infra/honest NI only.

Total: 162 | **perfect: 143** | **deferred: 19** | needs_work: 0

| # | Package | Status | Tests | Reference | Defining mechanics | Notes |
|---:|---|---|:---:|---|---|---|
| 1 [x] | `IAFahim.Algebra.GraphPoly` | perfect | Y | AtCoder Library / CP-Algorithms | poly BM/gcd/NTT | defining tests exercise shipped APIs |
| 2 [x] | `IAFahim.Algebra.Polynomial` | perfect | Y | AtCoder Library / CP-Algorithms | poly BM/gcd/NTT | defining tests exercise shipped APIs |
| 3 [x] | `IAFahim.Algebra.Sequence` | perfect | Y | AtCoder Library / CP-Algorithms | poly BM/gcd/NTT | defining tests exercise shipped APIs |
| 4 [x] | `IAFahim.Collections.NoDeps` | deferred | N | Unity.Collections | infra stubs | infra stubs only (Unity.Collections stand-in) |
| 5 [x] | `IAFahim.Collision.Gjk` | perfect | Y | Erin Catto GJK | CSO origin | defining tests exercise shipped APIs |
| 6 [x] | `IAFahim.Combinatorics.Generation` | perfect | Y | FKM | enumerate | defining tests exercise shipped APIs |
| 7 [x] | `IAFahim.Compress` | perfect | Y | coord compress | ranks | defining tests exercise shipped APIs |
| 8 [x] | `IAFahim.Compress.Coordinate` | perfect | Y | coord compress | ranks | defining tests exercise shipped APIs |
| 9 [x] | `IAFahim.DP` | perfect | Y | CP-Algorithms DP | knapsack/interval | defining tests exercise shipped APIs |
| 10 [x] | `IAFahim.DP.General` | perfect | Y | CP-Algorithms DP | knapsack/interval | defining tests exercise shipped APIs |
| 11 [x] | `IAFahim.DP.Knapsack` | perfect | Y | CP-Algorithms DP | knapsack/interval | defining tests exercise shipped APIs |
| 12 [x] | `IAFahim.DP.Optimization` | perfect | Y | CP-Algorithms DP | knapsack/interval | defining tests exercise shipped APIs |
| 13 [x] | `IAFahim.DS.Dsu` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 14 [x] | `IAFahim.DS.Fenwick` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 15 [x] | `IAFahim.DS.FixedCollections` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 16 [x] | `IAFahim.DS.GapBuffer` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 17 [x] | `IAFahim.DS.Grid` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 18 [x] | `IAFahim.DS.Heap` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 19 [x] | `IAFahim.DS.HilbertOrder` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 20 [x] | `IAFahim.DS.LinkCut` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 21 [x] | `IAFahim.DS.Mo` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 22 [x] | `IAFahim.DS.OrderedSet` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 23 [x] | `IAFahim.DS.PerfectHashMap` | deferred | N | ACL + KACTL | DSU/fenwick/heap | Unity.Collections NativeArray container; not pure pointer algo un |
| 24 [x] | `IAFahim.DS.PersistentDsu` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 25 [x] | `IAFahim.DS.PersistentTreap` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 26 [x] | `IAFahim.DS.PieceTable` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 27 [x] | `IAFahim.DS.RollbackSeg` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 28 [x] | `IAFahim.DS.RollbackStack` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 29 [x] | `IAFahim.DS.Rope` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 30 [x] | `IAFahim.DS.SegmentTree` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 31 [x] | `IAFahim.DS.Sparse` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 32 [x] | `IAFahim.DS.SpatialMap` | deferred | N | ACL + KACTL | DSU/fenwick/heap | Unity NativeParallelMultiHashMap container; not pure pointer algo |
| 33 [x] | `IAFahim.DS.Splay` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 34 [x] | `IAFahim.DS.Treap` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 35 [x] | `IAFahim.DS.Trie` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 36 [x] | `IAFahim.DS.UnsafeArray` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 37 [x] | `IAFahim.DS.WaveletMatrix` | perfect | Y | ACL + KACTL | DSU/fenwick/heap | defining tests exercise shipped APIs |
| 38 [x] | `IAFahim.GameTheory` | perfect | Y | Sprague-Grundy | nimbers | defining tests exercise shipped APIs |
| 39 [x] | `IAFahim.Geometry.Advanced` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 40 [x] | `IAFahim.Geometry.Arrangement` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 41 [x] | `IAFahim.Geometry.Azimuth` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 42 [x] | `IAFahim.Geometry.Basic` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 43 [x] | `IAFahim.Geometry.Bvh` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 44 [x] | `IAFahim.Geometry.Curve` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 45 [x] | `IAFahim.Geometry.Delaunay` | deferred | N | KACTL/geogram/CGAL | hull/spatial | empty package (no sources) |
| 46 [x] | `IAFahim.Geometry.Frame` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 47 [x] | `IAFahim.Geometry.Hull` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 48 [x] | `IAFahim.Geometry.Intersect` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 49 [x] | `IAFahim.Geometry.MarchingCubes` | deferred | N | KACTL/geogram/CGAL | hull/spatial | empty package (no sources) |
| 50 [x] | `IAFahim.Geometry.Mesh` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 51 [x] | `IAFahim.Geometry.PolygonClip` | deferred | N | KACTL/geogram/CGAL | hull/spatial | empty package (no sources) |
| 52 [x] | `IAFahim.Geometry.Spatial` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 53 [x] | `IAFahim.Geometry.Subdivision` | deferred | N | KACTL/geogram/CGAL | hull/spatial | empty package (no sources) |
| 54 [x] | `IAFahim.Geometry.SweepPrune` | deferred | N | KACTL/geogram/CGAL | hull/spatial | empty package (no sources) |
| 55 [x] | `IAFahim.Geometry.Triangulation` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 56 [x] | `IAFahim.Geometry.Voronoi` | perfect | Y | KACTL/geogram/CGAL | hull/spatial | defining tests exercise shipped APIs |
| 57 [x] | `IAFahim.Graph` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 58 [x] | `IAFahim.Graph.Bridges` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 59 [x] | `IAFahim.Graph.Cactus` | deferred | N | ACL+KACTL+Boost.Graph | flow/SCC/paths | BlockCutTreeLca/CactusLca honest NI (contract lacks tree buffers) |
| 60 [x] | `IAFahim.Graph.Centroid` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 61 [x] | `IAFahim.Graph.Clique` | deferred | N | ACL+KACTL+Boost.Graph | flow/SCC/paths | README shell; no algorithm .cs sources |
| 62 [x] | `IAFahim.Graph.Connectivity` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 63 [x] | `IAFahim.Graph.Cut` | deferred | N | ACL+KACTL+Boost.Graph | flow/SCC/paths | README shell; min-cut in Graph.Flow |
| 64 [x] | `IAFahim.Graph.DAG` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 65 [x] | `IAFahim.Graph.Decomposition` | deferred | N | ACL+KACTL+Boost.Graph | flow/SCC/paths | README shell; no .cs sources |
| 66 [x] | `IAFahim.Graph.Dominator` | deferred | N | ACL+KACTL+Boost.Graph | flow/SCC/paths | README shell; no .cs sources |
| 67 [x] | `IAFahim.Graph.DynamicTrees` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 68 [x] | `IAFahim.Graph.Eertree` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 69 [x] | `IAFahim.Graph.Eulerian` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 70 [x] | `IAFahim.Graph.Flow` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 71 [x] | `IAFahim.Graph.Functional` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 72 [x] | `IAFahim.Graph.Matching` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 73 [x] | `IAFahim.Graph.Misc` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 74 [x] | `IAFahim.Graph.RandomWalk` | deferred | N | ACL+KACTL+Boost.Graph | flow/SCC/paths | README shell; no .cs sources |
| 75 [x] | `IAFahim.Graph.SCC` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 76 [x] | `IAFahim.Graph.ShortestPath` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 77 [x] | `IAFahim.Graph.SpanningTrees` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 78 [x] | `IAFahim.Graph.Tree` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 79 [x] | `IAFahim.Graph.TreeDecomposition` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 80 [x] | `IAFahim.Graph.TreeIsomorphism` | deferred | N | ACL+KACTL+Boost.Graph | flow/SCC/paths | UnorderedTreeEditDistance unconstrained NP-hard NI |
| 81 [x] | `IAFahim.Graph.TreeQueries` | perfect | Y | ACL+KACTL+Boost.Graph | flow/SCC/paths | defining tests exercise shipped APIs |
| 82 [x] | `IAFahim.Linear` | deferred | N | Gaussian elim | Ax=b | meta-folder only (no csproj/sources) |
| 83 [x] | `IAFahim.Linear.Eigen` | perfect | Y | Gaussian elim | Ax=b | defining tests exercise shipped APIs |
| 84 [x] | `IAFahim.Linear.Matrix` | perfect | Y | Gaussian elim | Ax=b | defining tests exercise shipped APIs |
| 85 [x] | `IAFahim.Linear.Matrix2` | perfect | Y | Gaussian elim | Ax=b | defining tests exercise shipped APIs |
| 86 [x] | `IAFahim.Math.Arithmetic` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 87 [x] | `IAFahim.Math.Barycentric` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 88 [x] | `IAFahim.Math.Basic` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 89 [x] | `IAFahim.Math.BigInt` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 90 [x] | `IAFahim.Math.Combinatorics` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 91 [x] | `IAFahim.Math.Gauss` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 92 [x] | `IAFahim.Math.Kalman` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 93 [x] | `IAFahim.Math.Modular` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 94 [x] | `IAFahim.Math.NT` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 95 [x] | `IAFahim.Math.Noise` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 96 [x] | `IAFahim.Math.PoissonDisk` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 97 [x] | `IAFahim.Math.Polynomial` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 98 [x] | `IAFahim.Math.Polynomial.Eval` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 99 [x] | `IAFahim.Math.Polynomial.Fps` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 100 [x] | `IAFahim.Math.PotentialField` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 101 [x] | `IAFahim.Math.Quaternion` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 102 [x] | `IAFahim.Math.Sdf` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 103 [x] | `IAFahim.Math.SphericalHarmonics` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 104 [x] | `IAFahim.Math.Spline` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 105 [x] | `IAFahim.Math.Transform` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 106 [x] | `IAFahim.Math.Transform.AnyMod` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 107 [x] | `IAFahim.Math.Transform.Fft` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 108 [x] | `IAFahim.Math.Transform.Ntt` | perfect | Y | KACTL NT + ACL | mod/FFT/NTT | defining tests exercise shipped APIs |
| 109 [x] | `IAFahim.Memory.Allocators` | deferred | N | Unity allocators | infra | infra allocators only |
| 110 [x] | `IAFahim.Optimization.Approximation` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 111 [x] | `IAFahim.Optimization.DivideConquer` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 112 [x] | `IAFahim.Optimization.Exact` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 113 [x] | `IAFahim.Optimization.Games` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 114 [x] | `IAFahim.Optimization.Geometric` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 115 [x] | `IAFahim.Optimization.Knapsack` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 116 [x] | `IAFahim.Optimization.Matroid` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 117 [x] | `IAFahim.Optimization.Offline` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 118 [x] | `IAFahim.Optimization.Submodular` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 119 [x] | `IAFahim.Optimization.Treewidth` | perfect | Y | CP-Algo/opt literature | Steiner/knapsack/games | defining tests exercise shipped APIs |
| 120 [x] | `IAFahim.Pathfinding.Jps` | deferred | N | recast/JPS | navmesh/grid | empty package (no sources) |
| 121 [x] | `IAFahim.Pathfinding.Recast` | perfect | Y | recast/JPS | navmesh/grid | defining tests exercise shipped APIs |
| 122 [x] | `IAFahim.Permutation` | perfect | Y | cycle decomp | cycles | defining tests exercise shipped APIs |
| 123 [x] | `IAFahim.Physics.Xpbd` | perfect | Y | XPBD | constraints | defining tests exercise shipped APIs |
| 124 [x] | `IAFahim.Search` | deferred | N | CP-Algo/KACTL | LIS/bound | meta-folder only (no csproj/sources) |
| 125 [x] | `IAFahim.Search.Automaton` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 126 [x] | `IAFahim.Search.Bit` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 127 [x] | `IAFahim.Search.DifferenceArray` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 128 [x] | `IAFahim.Search.ExactCover` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 129 [x] | `IAFahim.Search.Imos` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 130 [x] | `IAFahim.Search.Interval` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 131 [x] | `IAFahim.Search.LIS` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 132 [x] | `IAFahim.Search.MeetInMiddle` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 133 [x] | `IAFahim.Search.Numerical` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 134 [x] | `IAFahim.Search.Prefix` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 135 [x] | `IAFahim.Search.Range` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 136 [x] | `IAFahim.Search.RangeQueries` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 137 [x] | `IAFahim.Search.Selection` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 138 [x] | `IAFahim.Search.Specialized` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 139 [x] | `IAFahim.Search.Subset` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 140 [x] | `IAFahim.Search.Suffix` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 141 [x] | `IAFahim.Search.TwoPointer` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 142 [x] | `IAFahim.Search.Window` | perfect | Y | CP-Algo/KACTL | LIS/bound | defining tests exercise shipped APIs |
| 143 [x] | `IAFahim.Sort.Insertion` | perfect | Y | CLRS+BCL | order | defining tests exercise shipped APIs |
| 144 [x] | `IAFahim.Sort.Merge` | perfect | Y | CLRS+BCL | order | defining tests exercise shipped APIs |
| 145 [x] | `IAFahim.Sort.Partition` | perfect | Y | CLRS+BCL | order | defining tests exercise shipped APIs |
| 146 [x] | `IAFahim.Sort.QuickSort` | perfect | Y | CLRS+BCL | order | defining tests exercise shipped APIs |
| 147 [x] | `IAFahim.Sort.RadixSort` | perfect | Y | CLRS+BCL | order | defining tests exercise shipped APIs |
| 148 [x] | `IAFahim.Sort.Specialized` | perfect | Y | CLRS+BCL | order | defining tests exercise shipped APIs |
| 149 [x] | `IAFahim.String` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 150 [x] | `IAFahim.String.Automata` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 151 [x] | `IAFahim.String.Compress` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 152 [x] | `IAFahim.String.FMIndex` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 153 [x] | `IAFahim.String.Grammar` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 154 [x] | `IAFahim.String.Match` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 155 [x] | `IAFahim.String.MinRotation` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 156 [x] | `IAFahim.String.Palindrome` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 157 [x] | `IAFahim.String.Parse` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 158 [x] | `IAFahim.String.Pattern` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 159 [x] | `IAFahim.String.SuffixArray` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 160 [x] | `IAFahim.String.SuffixAutomaton` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 161 [x] | `IAFahim.String.SuffixTree` | perfect | Y | ACL+KACTL string | SA/Manacher/Aho | defining tests exercise shipped APIs |
| 162 [x] | `IAFahim.Unique` | perfect | Y | std::unique | adjacent | defining tests exercise shipped APIs |
