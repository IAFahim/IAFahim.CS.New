# Algorithm package checklist — perfect only without open criticals

**perfect** = defining tests green AND every historical critical finding revalidated as fixed/deferred_ni (with proof in findings_reassessment.json).
**deferred** = shell/infra/NI OR demoted pending remaining critical re-audit.

Total: 162 | **perfect: 92** | **deferred: 70**

Critical findings: fixed=62 needs_manual=70

| # | Package | Status | Notes |
|---:|---|---|---|
| 1 [x] | `IAFahim.Algebra.GraphPoly` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 2 [x] | `IAFahim.Algebra.Polynomial` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 3 [x] | `IAFahim.Algebra.Sequence` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 4 [x] | `IAFahim.Collections.NoDeps` | deferred | infra stubs |
| 5 [x] | `IAFahim.Collision.Gjk` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 6 [x] | `IAFahim.Combinatorics.Generation` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 7 [x] | `IAFahim.Compress` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 8 [x] | `IAFahim.Compress.Coordinate` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 9 [x] | `IAFahim.DP` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 10 [x] | `IAFahim.DP.General` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 11 [x] | `IAFahim.DP.Knapsack` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 12 [x] | `IAFahim.DP.Optimization` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 13 [x] | `IAFahim.DS.Dsu` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 14 [x] | `IAFahim.DS.Fenwick` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 15 [x] | `IAFahim.DS.FixedCollections` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 16 [x] | `IAFahim.DS.GapBuffer` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 17 [x] | `IAFahim.DS.Grid` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 18 [x] | `IAFahim.DS.Heap` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 19 [x] | `IAFahim.DS.HilbertOrder` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 20 [x] | `IAFahim.DS.LinkCut` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 21 [x] | `IAFahim.DS.Mo` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 22 [x] | `IAFahim.DS.OrderedSet` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 23 [x] | `IAFahim.DS.PerfectHashMap` | deferred | Unity container |
| 24 [x] | `IAFahim.DS.PersistentDsu` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 25 [x] | `IAFahim.DS.PersistentTreap` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 26 [x] | `IAFahim.DS.PieceTable` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 27 [x] | `IAFahim.DS.RollbackSeg` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 28 [x] | `IAFahim.DS.RollbackStack` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 29 [x] | `IAFahim.DS.Rope` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 30 [x] | `IAFahim.DS.SegmentTree` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 31 [x] | `IAFahim.DS.Sparse` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 32 [x] | `IAFahim.DS.SpatialMap` | deferred | Unity container |
| 33 [x] | `IAFahim.DS.Splay` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 34 [x] | `IAFahim.DS.Treap` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 35 [x] | `IAFahim.DS.Trie` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 36 [x] | `IAFahim.DS.UnsafeArray` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 37 [x] | `IAFahim.DS.WaveletMatrix` | deferred | 3 unrevalidated critical finding(s); demoted until each fixed/proven |
| 38 [x] | `IAFahim.GameTheory` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 39 [x] | `IAFahim.Geometry.Advanced` | deferred | PolygonBoolean NI (DCEL) |
| 40 [x] | `IAFahim.Geometry.Arrangement` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 41 [x] | `IAFahim.Geometry.Azimuth` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 42 [x] | `IAFahim.Geometry.Basic` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 43 [x] | `IAFahim.Geometry.Bvh` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 44 [x] | `IAFahim.Geometry.Curve` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 45 [x] | `IAFahim.Geometry.Delaunay` | deferred | empty package |
| 46 [x] | `IAFahim.Geometry.Frame` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 47 [x] | `IAFahim.Geometry.Hull` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 48 [x] | `IAFahim.Geometry.Intersect` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 49 [x] | `IAFahim.Geometry.MarchingCubes` | deferred | empty package |
| 50 [x] | `IAFahim.Geometry.Mesh` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 51 [x] | `IAFahim.Geometry.PolygonClip` | deferred | empty package |
| 52 [x] | `IAFahim.Geometry.Spatial` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 53 [x] | `IAFahim.Geometry.Subdivision` | deferred | empty package |
| 54 [x] | `IAFahim.Geometry.SweepPrune` | deferred | empty package |
| 55 [x] | `IAFahim.Geometry.Triangulation` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 56 [x] | `IAFahim.Geometry.Voronoi` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 57 [x] | `IAFahim.Graph` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 58 [x] | `IAFahim.Graph.Bridges` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 59 [x] | `IAFahim.Graph.Cactus` | deferred | LCA contract NI |
| 60 [x] | `IAFahim.Graph.Centroid` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 61 [x] | `IAFahim.Graph.Clique` | deferred | README shell |
| 62 [x] | `IAFahim.Graph.Connectivity` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 63 [x] | `IAFahim.Graph.Cut` | deferred | README shell |
| 64 [x] | `IAFahim.Graph.DAG` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 65 [x] | `IAFahim.Graph.Decomposition` | deferred | README shell |
| 66 [x] | `IAFahim.Graph.Dominator` | deferred | README shell |
| 67 [x] | `IAFahim.Graph.DynamicTrees` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 68 [x] | `IAFahim.Graph.Eertree` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 69 [x] | `IAFahim.Graph.Eulerian` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 70 [x] | `IAFahim.Graph.Flow` | deferred | 6 unrevalidated critical finding(s); demoted until each fixed/proven |
| 71 [x] | `IAFahim.Graph.Functional` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 72 [x] | `IAFahim.Graph.Matching` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 73 [x] | `IAFahim.Graph.Misc` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 74 [x] | `IAFahim.Graph.RandomWalk` | deferred | README shell |
| 75 [x] | `IAFahim.Graph.SCC` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 76 [x] | `IAFahim.Graph.ShortestPath` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 77 [x] | `IAFahim.Graph.SpanningTrees` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 78 [x] | `IAFahim.Graph.Tree` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 79 [x] | `IAFahim.Graph.TreeDecomposition` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 80 [x] | `IAFahim.Graph.TreeIsomorphism` | deferred | tree edit NP-hard NI |
| 81 [x] | `IAFahim.Graph.TreeQueries` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 82 [x] | `IAFahim.Linear` | deferred | meta-folder only |
| 83 [x] | `IAFahim.Linear.Eigen` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 84 [x] | `IAFahim.Linear.Matrix` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 85 [x] | `IAFahim.Linear.Matrix2` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 86 [x] | `IAFahim.Math.Arithmetic` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 87 [x] | `IAFahim.Math.Barycentric` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 88 [x] | `IAFahim.Math.Basic` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 89 [x] | `IAFahim.Math.BigInt` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 90 [x] | `IAFahim.Math.Combinatorics` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 91 [x] | `IAFahim.Math.Gauss` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 92 [x] | `IAFahim.Math.Kalman` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 93 [x] | `IAFahim.Math.Modular` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 94 [x] | `IAFahim.Math.NT` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 95 [x] | `IAFahim.Math.Noise` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 96 [x] | `IAFahim.Math.PoissonDisk` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 97 [x] | `IAFahim.Math.Polynomial` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 98 [x] | `IAFahim.Math.Polynomial.Eval` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 99 [x] | `IAFahim.Math.Polynomial.Fps` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 100 [x] | `IAFahim.Math.PotentialField` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 101 [x] | `IAFahim.Math.Quaternion` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 102 [x] | `IAFahim.Math.Sdf` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 103 [x] | `IAFahim.Math.SphericalHarmonics` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 104 [x] | `IAFahim.Math.Spline` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 105 [x] | `IAFahim.Math.Transform` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 106 [x] | `IAFahim.Math.Transform.AnyMod` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 107 [x] | `IAFahim.Math.Transform.Fft` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 108 [x] | `IAFahim.Math.Transform.Ntt` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 109 [x] | `IAFahim.Memory.Allocators` | deferred | infra allocators |
| 110 [x] | `IAFahim.Optimization.Approximation` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 111 [x] | `IAFahim.Optimization.DivideConquer` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 112 [x] | `IAFahim.Optimization.Exact` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 113 [x] | `IAFahim.Optimization.Games` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 114 [x] | `IAFahim.Optimization.Geometric` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 115 [x] | `IAFahim.Optimization.Knapsack` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 116 [x] | `IAFahim.Optimization.Matroid` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 117 [x] | `IAFahim.Optimization.Offline` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 118 [x] | `IAFahim.Optimization.Submodular` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 119 [x] | `IAFahim.Optimization.Treewidth` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 120 [x] | `IAFahim.Pathfinding.Jps` | deferred | empty package |
| 121 [x] | `IAFahim.Pathfinding.Recast` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 122 [x] | `IAFahim.Permutation` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 123 [x] | `IAFahim.Physics.Xpbd` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 124 [x] | `IAFahim.Search` | deferred | meta-folder only |
| 125 [x] | `IAFahim.Search.Automaton` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 126 [x] | `IAFahim.Search.Bit` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 127 [x] | `IAFahim.Search.DifferenceArray` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 128 [x] | `IAFahim.Search.ExactCover` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 129 [x] | `IAFahim.Search.Imos` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 130 [x] | `IAFahim.Search.Interval` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 131 [x] | `IAFahim.Search.LIS` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 132 [x] | `IAFahim.Search.MeetInMiddle` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 133 [x] | `IAFahim.Search.Numerical` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 134 [x] | `IAFahim.Search.Prefix` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 135 [x] | `IAFahim.Search.Range` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 136 [x] | `IAFahim.Search.RangeQueries` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 137 [x] | `IAFahim.Search.Selection` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 138 [x] | `IAFahim.Search.Specialized` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 139 [x] | `IAFahim.Search.Subset` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 140 [x] | `IAFahim.Search.Suffix` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 141 [x] | `IAFahim.Search.TwoPointer` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 142 [x] | `IAFahim.Search.Window` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 143 [x] | `IAFahim.Sort.Insertion` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 144 [x] | `IAFahim.Sort.Merge` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 145 [x] | `IAFahim.Sort.Partition` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 146 [x] | `IAFahim.Sort.QuickSort` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 147 [x] | `IAFahim.Sort.RadixSort` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 148 [x] | `IAFahim.Sort.Specialized` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 149 [x] | `IAFahim.String` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 150 [x] | `IAFahim.String.Automata` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 151 [x] | `IAFahim.String.Compress` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 152 [x] | `IAFahim.String.FMIndex` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 153 [x] | `IAFahim.String.Grammar` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 154 [x] | `IAFahim.String.Match` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 155 [x] | `IAFahim.String.MinRotation` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 156 [x] | `IAFahim.String.Palindrome` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 157 [x] | `IAFahim.String.Parse` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 158 [x] | `IAFahim.String.Pattern` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 159 [x] | `IAFahim.String.SuffixArray` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 160 [x] | `IAFahim.String.SuffixAutomaton` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 161 [x] | `IAFahim.String.SuffixTree` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 162 [x] | `IAFahim.Unique` | perfect | defining tests + all historical criticals revalidated fixed/absent |
