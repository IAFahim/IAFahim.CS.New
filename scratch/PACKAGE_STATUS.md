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
| 5 [x] | `IAFahim.Collision.Gjk` | deferred | gate: public API untested: CapsuleSupport, ConvexHullSupport |
| 6 [x] | `IAFahim.Combinatorics.Generation` | deferred | gate: open findings (1); gate: public API untested: AdvanceFkmSuccessor, BraceletRank, Bra |
| 7 [x] | `IAFahim.Compress` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 8 [x] | `IAFahim.Compress.Coordinate` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 9 [x] | `IAFahim.DP` | deferred | gate: open findings (2); gate: public API untested: Optimize, RunSpaceOptimized |
| 10 [x] | `IAFahim.DP.General` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 11 [x] | `IAFahim.DP.Knapsack` | perfect | BitsetSubsetSum size fix proven target=64; tests 20/20 |
| 12 [x] | `IAFahim.DP.Optimization` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 13 [x] | `IAFahim.DS.Dsu` | deferred | gate: public API untested: RunPathCompression |
| 14 [x] | `IAFahim.DS.Fenwick` | deferred | gate: public API untested: LowerBoundInt64, PrefixQuery, RangeAdd, RangeQuery, RangeSumInt |
| 15 [x] | `IAFahim.DS.FixedCollections` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 16 [x] | `IAFahim.DS.GapBuffer` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 17 [x] | `IAFahim.DS.Grid` | deferred | gate: public API untested: Collect, CollectFlat |
| 18 [x] | `IAFahim.DS.Heap` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 19 [x] | `IAFahim.DS.HilbertOrder` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 20 [x] | `IAFahim.DS.LinkCut` | deferred | gate: public API untested: Access, MakeRoot |
| 21 [x] | `IAFahim.DS.Mo` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 22 [x] | `IAFahim.DS.OrderedSet` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 23 [x] | `IAFahim.DS.PerfectHashMap` | deferred | Unity container |
| 24 [x] | `IAFahim.DS.PersistentDsu` | deferred | gate: public API untested: Query, Update |
| 25 [x] | `IAFahim.DS.PersistentTreap` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 26 [x] | `IAFahim.DS.PieceTable` | deferred | gate: open findings (1) |
| 27 [x] | `IAFahim.DS.RollbackSeg` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 28 [x] | `IAFahim.DS.RollbackStack` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 29 [x] | `IAFahim.DS.Rope` | deferred | gate: public API untested: SplitAt, Update |
| 30 [x] | `IAFahim.DS.SegmentTree` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 31 [x] | `IAFahim.DS.Sparse` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 32 [x] | `IAFahim.DS.SpatialMap` | deferred | Unity container |
| 33 [x] | `IAFahim.DS.Splay` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 34 [x] | `IAFahim.DS.Treap` | deferred | gate: public API untested: AssignRange, Erase, Insert, Push, RangeQuery, Rank, Split, Spli |
| 35 [x] | `IAFahim.DS.Trie` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 36 [x] | `IAFahim.DS.UnsafeArray` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 37 [x] | `IAFahim.DS.WaveletMatrix` | deferred | 3 unrevalidated critical finding(s); demoted until each fixed/proven |
| 38 [x] | `IAFahim.GameTheory` | perfect | GameDp mex bound g<64; tests 9/9 |
| 39 [x] | `IAFahim.Geometry.Advanced` | deferred | PolygonBoolean NI (DCEL) |
| 40 [x] | `IAFahim.Geometry.Arrangement` | deferred | gate: open findings (4); gate: public API untested: BuildKdTree, QueryKdTree |
| 41 [x] | `IAFahim.Geometry.Azimuth` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 42 [x] | `IAFahim.Geometry.Basic` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 43 [x] | `IAFahim.Geometry.Bvh` | deferred | gate: open findings (1) |
| 44 [x] | `IAFahim.Geometry.Curve` | deferred | gate: public API untested: EvaluateTangent, UniformSample |
| 45 [x] | `IAFahim.Geometry.Delaunay` | deferred | empty package |
| 46 [x] | `IAFahim.Geometry.Frame` | deferred | gate: open findings (1) |
| 47 [x] | `IAFahim.Geometry.Hull` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 48 [x] | `IAFahim.Geometry.Intersect` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 49 [x] | `IAFahim.Geometry.MarchingCubes` | deferred | empty package |
| 50 [x] | `IAFahim.Geometry.Mesh` | deferred | gate: public API untested: DeformVertices |
| 51 [x] | `IAFahim.Geometry.PolygonClip` | deferred | empty package |
| 52 [x] | `IAFahim.Geometry.Spatial` | deferred | gate: open findings (2); gate: public API untested: Add, Build2D, Build3D, Euclidean, Init |
| 53 [x] | `IAFahim.Geometry.Subdivision` | deferred | empty package |
| 54 [x] | `IAFahim.Geometry.SweepPrune` | deferred | empty package |
| 55 [x] | `IAFahim.Geometry.Triangulation` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 56 [x] | `IAFahim.Geometry.Voronoi` | deferred | gate: public API untested: BuildKD, BuildLong, SearchNearest, SearchRange |
| 57 [x] | `IAFahim.Graph` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 58 [x] | `IAFahim.Graph.Bridges` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 59 [x] | `IAFahim.Graph.Cactus` | deferred | LCA contract NI |
| 60 [x] | `IAFahim.Graph.Centroid` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 61 [x] | `IAFahim.Graph.Clique` | deferred | README shell |
| 62 [x] | `IAFahim.Graph.Connectivity` | deferred | gate: public API untested: Solve |
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
| 77 [x] | `IAFahim.Graph.SpanningTrees` | deferred | gate: public API untested: BuildTransitiveClosure, CountTrue |
| 78 [x] | `IAFahim.Graph.Tree` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 79 [x] | `IAFahim.Graph.TreeDecomposition` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 80 [x] | `IAFahim.Graph.TreeIsomorphism` | deferred | tree edit NP-hard NI |
| 81 [x] | `IAFahim.Graph.TreeQueries` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 82 [x] | `IAFahim.Linear` | deferred | meta-folder only |
| 83 [x] | `IAFahim.Linear.Eigen` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 84 [x] | `IAFahim.Linear.Matrix` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 85 [x] | `IAFahim.Linear.Matrix2` | deferred | gate: public API untested: RunSquare |
| 86 [x] | `IAFahim.Math.Arithmetic` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 87 [x] | `IAFahim.Math.Barycentric` | deferred | gate: public API untested: InterpolateScalar, SignedArea |
| 88 [x] | `IAFahim.Math.Basic` | deferred | gate: open findings (2) |
| 89 [x] | `IAFahim.Math.BigInt` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 90 [x] | `IAFahim.Math.Combinatorics` | perfect | SegmentedSieve 0/1 + stackalloc fix 18/18 |
| 91 [x] | `IAFahim.Math.Gauss` | deferred | gate: public API untested: Determinant, ModInv, ModPow |
| 92 [x] | `IAFahim.Math.Kalman` | deferred | gate: public API untested: PredictCovariance |
| 93 [x] | `IAFahim.Math.Modular` | perfect | ModInv negative gcd; tests 32/32 |
| 94 [x] | `IAFahim.Math.NT` | deferred | gate: open findings (11); gate: public API untested: ConvolutionPrefixSum, Forward, Hyperb |
| 95 [x] | `IAFahim.Math.Noise` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 96 [x] | `IAFahim.Math.PoissonDisk` | deferred | gate: open findings (3) |
| 97 [x] | `IAFahim.Math.Polynomial` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 98 [x] | `IAFahim.Math.Polynomial.Eval` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 99 [x] | `IAFahim.Math.Polynomial.Fps` | deferred | gate: open findings (1); gate: public API untested: FastPow, ModInverse |
| 100 [x] | `IAFahim.Math.PotentialField` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 101 [x] | `IAFahim.Math.Quaternion` | deferred | gate: open findings (1); gate: public API untested: FromEuler, Length, LookRotation, ToAxi |
| 102 [x] | `IAFahim.Math.Sdf` | deferred | gate: public API untested: AmbientOcclusion, Cone, Ellipsoid, MirrorY, MirrorZ, Octahedron |
| 103 [x] | `IAFahim.Math.SphericalHarmonics` | deferred | gate: public API untested: BasisL1M0, BasisL1M1, BasisL1P1, BasisL2M0, BasisL2M1, BasisL2M |
| 104 [x] | `IAFahim.Math.Spline` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 105 [x] | `IAFahim.Math.Transform` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 106 [x] | `IAFahim.Math.Transform.AnyMod` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 107 [x] | `IAFahim.Math.Transform.Fft` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 108 [x] | `IAFahim.Math.Transform.Ntt` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 109 [x] | `IAFahim.Memory.Allocators` | deferred | infra allocators |
| 110 [x] | `IAFahim.Optimization.Approximation` | deferred | gate: open findings (2); gate: public API untested: HillClimb, MonteCarlo, SimulatedAnneal |
| 111 [x] | `IAFahim.Optimization.DivideConquer` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 112 [x] | `IAFahim.Optimization.Exact` | deferred | gate: open findings (1) |
| 113 [x] | `IAFahim.Optimization.Games` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 114 [x] | `IAFahim.Optimization.Geometric` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 115 [x] | `IAFahim.Optimization.Knapsack` | deferred | gate: open findings (3); gate: public API untested: BinarySplit, Count, FourSum, MonotoneQ |
| 116 [x] | `IAFahim.Optimization.Matroid` | perfect | skip nonpositive weights; Rank tests 5/5 |
| 117 [x] | `IAFahim.Optimization.Offline` | perfect | GroupByMid+DCA+CDQ defining tests PASS 14/14 |
| 118 [x] | `IAFahim.Optimization.Submodular` | perfect | long total; API tests 8/8 |
| 119 [x] | `IAFahim.Optimization.Treewidth` | deferred | gate: open findings (1); gate: public API untested: CheckMonge, CheckQuadrangle, ComputeOr |
| 120 [x] | `IAFahim.Pathfinding.Jps` | deferred | empty package |
| 121 [x] | `IAFahim.Pathfinding.Recast` | deferred | gate: public API untested: BuildDistanceField, BuildLayerRegions, BuildRegionsMonotone, Dt |
| 122 [x] | `IAFahim.Permutation` | perfect | Gray uint shifts; high-bit roundtrip 17/17 |
| 123 [x] | `IAFahim.Physics.Xpbd` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 124 [x] | `IAFahim.Search` | deferred | meta-folder only |
| 125 [x] | `IAFahim.Search.Automaton` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 126 [x] | `IAFahim.Search.Bit` | perfect | KthElement long mid; full int range tests 19/19 |
| 127 [x] | `IAFahim.Search.DifferenceArray` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 128 [x] | `IAFahim.Search.ExactCover` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 129 [x] | `IAFahim.Search.Imos` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 130 [x] | `IAFahim.Search.Interval` | deferred | gate: open findings (1); gate: public API untested: CountContained, CountOverlapping, Find |
| 131 [x] | `IAFahim.Search.LIS` | deferred | gate: public API untested: RunLong |
| 132 [x] | `IAFahim.Search.MeetInMiddle` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 133 [x] | `IAFahim.Search.Numerical` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 134 [x] | `IAFahim.Search.Prefix` | deferred | gate: open findings (1); gate: public API untested: CountOccurrences, FindFirst, LongestCo |
| 135 [x] | `IAFahim.Search.Range` | deferred | gate: open findings (3); gate: public API untested: RunWindow |
| 136 [x] | `IAFahim.Search.RangeQueries` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 137 [x] | `IAFahim.Search.Selection` | deferred | gate: open findings (1); gate: public API untested: InsertionSort, Partition, PartitionLon |
| 138 [x] | `IAFahim.Search.Specialized` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 139 [x] | `IAFahim.Search.Subset` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 140 [x] | `IAFahim.Search.Suffix` | deferred | gate: open findings (2) |
| 141 [x] | `IAFahim.Search.TwoPointer` | perfect | duplicate pair multiply lc*rc; tests 4/4 |
| 142 [x] | `IAFahim.Search.Window` | deferred | gate: public API untested: FixInt32, HeapifyInt32, Left, Parent, PopInt32, PushInt32, Righ |
| 143 [x] | `IAFahim.Sort.Insertion` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 144 [x] | `IAFahim.Sort.Merge` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 145 [x] | `IAFahim.Sort.Partition` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 146 [x] | `IAFahim.Sort.QuickSort` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 147 [x] | `IAFahim.Sort.RadixSort` | deferred | gate: public API untested: RunLong, RunWithResult |
| 148 [x] | `IAFahim.Sort.Specialized` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 149 [x] | `IAFahim.String` | deferred | gate: open findings (1); gate: public API untested: BuildPrefixFunction |
| 150 [x] | `IAFahim.String.Automata` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 151 [x] | `IAFahim.String.Compress` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 152 [x] | `IAFahim.String.FMIndex` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 153 [x] | `IAFahim.String.Grammar` | deferred | gate: open findings (1); gate: public API untested: Compress |
| 154 [x] | `IAFahim.String.Match` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 155 [x] | `IAFahim.String.MinRotation` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 156 [x] | `IAFahim.String.Palindrome` | deferred | gate: public API untested: Add, Factorize, Init, SeriesLink |
| 157 [x] | `IAFahim.String.Parse` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 158 [x] | `IAFahim.String.Pattern` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 159 [x] | `IAFahim.String.SuffixArray` | deferred | gate: open findings (1); gate: public API untested: CompareSuffix, Erase, Find, GetHash, G |
| 160 [x] | `IAFahim.String.SuffixAutomaton` | deferred | 2 unrevalidated critical finding(s); demoted until each fixed/proven |
| 161 [x] | `IAFahim.String.SuffixTree` | deferred | 1 unrevalidated critical finding(s); demoted until each fixed/proven |
| 162 [x] | `IAFahim.Unique` | perfect | defining tests + all historical criticals revalidated fixed/absent |
