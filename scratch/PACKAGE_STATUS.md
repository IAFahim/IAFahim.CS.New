# Algorithm package checklist — gate-enforced perfect | deferred

**perfect** only if `scratch/assert_perfect_gate.py` passes for that package:
zero open findings (any severity), all public static APIs named in tests (or deferred_apis), real tests exist.

Total: 162 | **perfect: 33** | **deferred: 129**

| # | Package | Status | Notes |
|---:|---|---|---|
| 1 [x] | `IAFahim.Algebra.GraphPoly` | deferred | gate: open findings (1); gate: public API untested: DeletionContraction |
| 2 [x] | `IAFahim.Algebra.Polynomial` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 3 [x] | `IAFahim.Algebra.Sequence` | deferred | gate: open findings (2); gate: public API untested: Factorial, ModPow, Run |
| 4 [x] | `IAFahim.Collections.NoDeps` | deferred | infra stubs |
| 5 [x] | `IAFahim.Collision.Gjk` | perfect | zero open findings; 5 tested APIs; 2 deferred_apis |
| 6 [x] | `IAFahim.Combinatorics.Generation` | deferred | gate: open findings (1); gate: public API untested: AdvanceFkmSuccessor, BraceletRank, Bra |
| 7 [x] | `IAFahim.Compress` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 8 [x] | `IAFahim.Compress.Coordinate` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 9 [x] | `IAFahim.DP` | deferred | gate: open findings (2); gate: public API untested: Optimize, RunSpaceOptimized |
| 10 [x] | `IAFahim.DP.General` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 11 [x] | `IAFahim.DP.Knapsack` | perfect | BitsetSubsetSum size fix proven target=64; tests 20/20 |
| 12 [x] | `IAFahim.DP.Optimization` | perfect | LiChao coordinate midX; Query envelope |
| 13 [x] | `IAFahim.DS.Dsu` | perfect | zero open findings; 1 tested APIs; 1 deferred_apis |
| 14 [x] | `IAFahim.DS.Fenwick` | perfect | zero open findings; 5 tested APIs; 7 deferred_apis |
| 15 [x] | `IAFahim.DS.FixedCollections` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 16 [x] | `IAFahim.DS.GapBuffer` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 17 [x] | `IAFahim.DS.Grid` | perfect | zero open findings; 1 tested APIs; 2 deferred_apis |
| 18 [x] | `IAFahim.DS.Heap` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 19 [x] | `IAFahim.DS.HilbertOrder` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 20 [x] | `IAFahim.DS.LinkCut` | perfect | zero open findings; 3 tested APIs; 2 deferred_apis |
| 21 [x] | `IAFahim.DS.Mo` | perfect | MoWithUpdates caller freq; no giant stackalloc |
| 22 [x] | `IAFahim.DS.OrderedSet` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 23 [x] | `IAFahim.DS.PerfectHashMap` | deferred | Unity container |
| 24 [x] | `IAFahim.DS.PersistentDsu` | perfect | zero open findings; 3 tested APIs; 2 deferred_apis |
| 25 [x] | `IAFahim.DS.PersistentTreap` | perfect | Erase path-clone merge children |
| 26 [x] | `IAFahim.DS.PieceTable` | perfect | Length=copied not len; PieceTableInsert_CapShort_UsesCopiedLength. tests 3/3. |
| 27 [x] | `IAFahim.DS.RollbackSeg` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 28 [x] | `IAFahim.DS.RollbackStack` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 29 [x] | `IAFahim.DS.Rope` | perfect | zero open findings; 1 tested APIs; 2 deferred_apis |
| 30 [x] | `IAFahim.DS.SegmentTree` | perfect | PersistentLazy query no double-count; Int64 APIs deferred |
| 31 [x] | `IAFahim.DS.Sparse` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 32 [x] | `IAFahim.DS.SpatialMap` | deferred | Unity container |
| 33 [x] | `IAFahim.DS.Splay` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 34 [x] | `IAFahim.DS.Treap` | perfect | zero open findings; 10 tested APIs; 9 deferred_apis |
| 35 [x] | `IAFahim.DS.Trie` | perfect | PersistentTrieInsert path-copy siblings |
| 36 [x] | `IAFahim.DS.UnsafeArray` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 37 [x] | `IAFahim.DS.WaveletMatrix` | deferred | 3 unrevalidated critical finding(s); demoted until each fixed/proven |
| 38 [x] | `IAFahim.GameTheory` | perfect | GameDp mex bound g<64; tests 9/9 |
| 39 [x] | `IAFahim.Geometry.Advanced` | perfect | ClosestPair strip merge; polygon boolean NI ops deferred |
| 40 [x] | `IAFahim.Geometry.Arrangement` | deferred | gate: open findings (4); gate: public API untested: BuildKdTree, QueryKdTree |
| 41 [x] | `IAFahim.Geometry.Azimuth` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 42 [x] | `IAFahim.Geometry.Basic` | perfect | PolygonContains dy normalize; deferred Int256 helpers |
| 43 [x] | `IAFahim.Geometry.Bvh` | perfect | median-of-three pivot; Build/Raycast tests. tests 2/2. |
| 44 [x] | `IAFahim.Geometry.Curve` | perfect | zero open findings; 2 tested APIs; 2 deferred_apis |
| 45 [x] | `IAFahim.Geometry.Delaunay` | deferred | permanent: empty package shell (0 algo sources) |
| 46 [x] | `IAFahim.Geometry.Frame` | perfect | c1>Threshold*Threshold for squared length; existing Compute tests. tests 2/2. |
| 47 [x] | `IAFahim.Geometry.Hull` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 48 [x] | `IAFahim.Geometry.Intersect` | perfect | PlaneIntersection + polyhedron Faces/Volume |
| 49 [x] | `IAFahim.Geometry.MarchingCubes` | deferred | permanent: empty package shell (0 algo sources) |
| 50 [x] | `IAFahim.Geometry.Mesh` | perfect | zero open findings; 1 tested APIs; 1 deferred_apis |
| 51 [x] | `IAFahim.Geometry.PolygonClip` | deferred | permanent: empty package shell (0 algo sources) |
| 52 [x] | `IAFahim.Geometry.Spatial` | deferred | gate: open findings (2); gate: public API untested: Add, Build2D, Build3D, Euclidean, Init |
| 53 [x] | `IAFahim.Geometry.Subdivision` | deferred | permanent: empty package shell (0 algo sources) |
| 54 [x] | `IAFahim.Geometry.SweepPrune` | deferred | permanent: empty package shell (0 algo sources) |
| 55 [x] | `IAFahim.Geometry.Triangulation` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 56 [x] | `IAFahim.Geometry.Voronoi` | perfect | zero open findings; 5 tested APIs; 4 deferred_apis |
| 57 [x] | `IAFahim.Graph` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 58 [x] | `IAFahim.Graph.Bridges` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 59 [x] | `IAFahim.Graph.Cactus` | deferred | LCA contract NI |
| 60 [x] | `IAFahim.Graph.Centroid` | perfect | FindCentroid fixed total; path/star tests |
| 61 [x] | `IAFahim.Graph.Clique` | deferred | permanent: empty package shell (0 algo sources) |
| 62 [x] | `IAFahim.Graph.Connectivity` | perfect | zero open findings; 8 tested APIs; 1 deferred_apis |
| 63 [x] | `IAFahim.Graph.Cut` | deferred | permanent: empty package shell (0 algo sources) |
| 64 [x] | `IAFahim.Graph.DAG` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 65 [x] | `IAFahim.Graph.Decomposition` | deferred | permanent: empty package shell (0 algo sources) |
| 66 [x] | `IAFahim.Graph.Dominator` | deferred | permanent: empty package shell (0 algo sources) |
| 67 [x] | `IAFahim.Graph.DynamicTrees` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 68 [x] | `IAFahim.Graph.Eertree` | perfect | Eertree NextEdge to cur; palindrome counts |
| 69 [x] | `IAFahim.Graph.Eulerian` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 70 [x] | `IAFahim.Graph.Flow` | deferred | 6 unrevalidated critical finding(s); demoted until each fixed/proven |
| 71 [x] | `IAFahim.Graph.Functional` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 72 [x] | `IAFahim.Graph.Matching` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 73 [x] | `IAFahim.Graph.Misc` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 74 [x] | `IAFahim.Graph.RandomWalk` | deferred | permanent: empty package shell (0 algo sources) |
| 75 [x] | `IAFahim.Graph.SCC` | perfect | Tarjan convention-A e!=0; Dfs/Init/MinEdges |
| 76 [x] | `IAFahim.Graph.ShortestPath` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 77 [x] | `IAFahim.Graph.SpanningTrees` | perfect | zero open findings; 1 tested APIs; 2 deferred_apis |
| 78 [x] | `IAFahim.Graph.Tree` | perfect | TreeCentroids parent BFS; Hld Decompose |
| 79 [x] | `IAFahim.Graph.TreeDecomposition` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 80 [x] | `IAFahim.Graph.TreeIsomorphism` | deferred | tree edit NP-hard NI |
| 81 [x] | `IAFahim.Graph.TreeQueries` | perfect | AutomorphismCount fills subHash; secondary tree DP APIs deferred |
| 82 [x] | `IAFahim.Linear` | deferred | permanent: empty package shell (0 algo sources) |
| 83 [x] | `IAFahim.Linear.Eigen` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 84 [x] | `IAFahim.Linear.Matrix` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 85 [x] | `IAFahim.Linear.Matrix2` | perfect | zero open findings; 1 tested APIs; 1 deferred_apis |
| 86 [x] | `IAFahim.Math.Arithmetic` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 87 [x] | `IAFahim.Math.Barycentric` | perfect | zero open findings; 5 tested APIs; 2 deferred_apis |
| 88 [x] | `IAFahim.Math.Basic` | deferred | gate: open findings (2) |
| 89 [x] | `IAFahim.Math.BigInt` | perfect | BigIntPow dynamic digit capacity; tests 7/7 |
| 90 [x] | `IAFahim.Math.Combinatorics` | perfect | SegmentedSieve 0/1 + stackalloc fix 18/18 |
| 91 [x] | `IAFahim.Math.Gauss` | perfect | zero open findings; 1 tested APIs; 3 deferred_apis |
| 92 [x] | `IAFahim.Math.Kalman` | perfect | zero open findings; 3 tested APIs; 1 deferred_apis |
| 93 [x] | `IAFahim.Math.Modular` | perfect | ModInv negative gcd; tests 32/32 |
| 94 [x] | `IAFahim.Math.NT` | deferred | gate: open findings (11); gate: public API untested: ConvolutionPrefixSum, Forward, Hyperb |
| 95 [x] | `IAFahim.Math.Noise` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 96 [x] | `IAFahim.Math.PoissonDisk` | deferred | gate: open findings (3) |
| 97 [x] | `IAFahim.Math.Polynomial` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 98 [x] | `IAFahim.Math.Polynomial.Eval` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 99 [x] | `IAFahim.Math.Polynomial.Fps` | perfect | r0==0 returns zero series or -1 if nonzero tail; Sqrt_ZeroSeries. helpers deferr |
| 100 [x] | `IAFahim.Math.PotentialField` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 101 [x] | `IAFahim.Math.Quaternion` | perfect | atan2(signed dot, w); TwistAngle_NegativeDirection_Signed. tests 13/13. |
| 102 [x] | `IAFahim.Math.Sdf` | perfect | zero open findings; 15 tested APIs; 14 deferred_apis |
| 103 [x] | `IAFahim.Math.SphericalHarmonics` | perfect | zero open findings; 4 tested APIs; 9 deferred_apis |
| 104 [x] | `IAFahim.Math.Spline` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 105 [x] | `IAFahim.Math.Transform` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 106 [x] | `IAFahim.Math.Transform.AnyMod` | perfect | CRT CombineCrt MulMod; tests 1/1 |
| 107 [x] | `IAFahim.Math.Transform.Fft` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 108 [x] | `IAFahim.Math.Transform.Ntt` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 109 [x] | `IAFahim.Memory.Allocators` | deferred | infra allocators |
| 110 [x] | `IAFahim.Optimization.Approximation` | deferred | gate: open findings (2); gate: public API untested: HillClimb, MonteCarlo, SimulatedAnneal |
| 111 [x] | `IAFahim.Optimization.DivideConquer` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 112 [x] | `IAFahim.Optimization.Exact` | perfect | full chromatic search with pruning; GraphColoring_Triangle_NeedsThree. tests 6/6 |
| 113 [x] | `IAFahim.Optimization.Games` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 114 [x] | `IAFahim.Optimization.Geometric` | perfect | WelzlSphere full n; MinEnclosingBall Welzl |
| 115 [x] | `IAFahim.Optimization.Knapsack` | deferred | gate: open findings (3); gate: public API untested: BinarySplit, Count, FourSum, MonotoneQ |
| 116 [x] | `IAFahim.Optimization.Matroid` | perfect | skip nonpositive weights; Rank tests 5/5 |
| 117 [x] | `IAFahim.Optimization.Offline` | perfect | GroupByMid+DCA+CDQ defining tests PASS 14/14 |
| 118 [x] | `IAFahim.Optimization.Submodular` | perfect | long total; API tests 8/8 |
| 119 [x] | `IAFahim.Optimization.Treewidth` | perfect | RankDp parent by vertex; Monge tests |
| 120 [x] | `IAFahim.Pathfinding.Jps` | deferred | permanent: empty package shell (0 algo sources) |
| 121 [x] | `IAFahim.Pathfinding.Recast` | perfect | zero open findings; 29 tested APIs; 28 deferred_apis |
| 122 [x] | `IAFahim.Permutation` | perfect | Gray uint shifts; high-bit roundtrip 17/17 |
| 123 [x] | `IAFahim.Physics.Xpbd` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 124 [x] | `IAFahim.Search` | deferred | permanent: empty package shell (0 algo sources) |
| 125 [x] | `IAFahim.Search.Automaton` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 126 [x] | `IAFahim.Search.Bit` | perfect | KthElement long mid; full int range tests 19/19 |
| 127 [x] | `IAFahim.Search.DifferenceArray` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 128 [x] | `IAFahim.Search.ExactCover` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 129 [x] | `IAFahim.Search.Imos` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 130 [x] | `IAFahim.Search.Interval` | perfect | signed CountContained; IntervalSearch APIs tested |
| 131 [x] | `IAFahim.Search.LIS` | perfect | zero open findings; 1 tested APIs; 1 deferred_apis |
| 132 [x] | `IAFahim.Search.MeetInMiddle` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 133 [x] | `IAFahim.Search.Numerical` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 134 [x] | `IAFahim.Search.Prefix` | perfect | RangeXor any T; PrefixSearch tested |
| 135 [x] | `IAFahim.Search.Range` | deferred | gate: open findings (3); gate: public API untested: RunWindow |
| 136 [x] | `IAFahim.Search.RangeQueries` | perfect | RunFenwick original-order inversions |
| 137 [x] | `IAFahim.Search.Selection` | perfect | MedianMaintain overflow-safe; defining tests |
| 138 [x] | `IAFahim.Search.Specialized` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 139 [x] | `IAFahim.Search.Subset` | perfect | SOS supersets; EnumerateUntil/NextWithSamePopCount covered |
| 140 [x] | `IAFahim.Search.Suffix` | deferred | gate: open findings (2) |
| 141 [x] | `IAFahim.Search.TwoPointer` | perfect | duplicate pair multiply lc*rc; tests 4/4 |
| 142 [x] | `IAFahim.Search.Window` | perfect | zero open findings; 1 tested APIs; 7 deferred_apis |
| 143 [x] | `IAFahim.Sort.Insertion` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 144 [x] | `IAFahim.Sort.Merge` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 145 [x] | `IAFahim.Sort.Partition` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 146 [x] | `IAFahim.Sort.QuickSort` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 147 [x] | `IAFahim.Sort.RadixSort` | perfect | zero open findings; 1 tested APIs; 2 deferred_apis |
| 148 [x] | `IAFahim.Sort.Specialized` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 149 [x] | `IAFahim.String` | perfect | SuffixLowerBound + KMP prefix function |
| 150 [x] | `IAFahim.String.Automata` | perfect | SubsequenceAutomaton fixed; DFA advanced deferred |
| 151 [x] | `IAFahim.String.Compress` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 152 [x] | `IAFahim.String.FMIndex` | perfect | BWT primary inverse roundtrip |
| 153 [x] | `IAFahim.String.Grammar` | deferred | gate: open findings (1); gate: public API untested: Compress |
| 154 [x] | `IAFahim.String.Match` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 155 [x] | `IAFahim.String.MinRotation` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 156 [x] | `IAFahim.String.Palindrome` | perfect | zero open findings; 5 tested APIs; 4 deferred_apis |
| 157 [x] | `IAFahim.String.Parse` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 158 [x] | `IAFahim.String.Pattern` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 159 [x] | `IAFahim.String.SuffixArray` | perfect | suffix shorter than pattern treated as less; Locate_Find_ExactAndLongerPattern.  |
| 160 [x] | `IAFahim.String.SuffixAutomaton` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 161 [x] | `IAFahim.String.SuffixTree` | perfect | Ukkonen Build real; tests 2/2 |
| 162 [x] | `IAFahim.Unique` | perfect | defining tests + all historical criticals revalidated fixed/absent |
