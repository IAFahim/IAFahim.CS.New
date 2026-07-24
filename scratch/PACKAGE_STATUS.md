# Algorithm package checklist — terminal perfect | deferred

Each row lists package, public algorithm types, world-class reference, status.

Total: 162 | **perfect: 142** | **deferred: 20**

| # | Package | Status | Algorithms (public types) | Reference | Notes |
|---:|---|---|---|---|---|
| 1 [x] | `IAFahim.Algebra.GraphPoly` | perfect | Tutte, Independence, Chromatic, Matching, Reliability,  | AtCoder Library / CP-Algorithms | defining tests exercise shipped APIs |
| 2 [x] | `IAFahim.Algebra.Polynomial` | perfect | BerlekampMassey, CantorZassenhaus, PowMod, SquareFree,  | AtCoder Library / CP-Algorithms | defining tests exercise shipped APIs |
| 3 [x] | `IAFahim.Algebra.Sequence` | perfect | Prufer, Transform, Combinatorial, GeneratingFunction, S | AtCoder Library / CP-Algorithms | defining tests exercise shipped APIs |
| 4 [x] | `IAFahim.Collections.NoDeps` | deferred | BLGlobalLogger, JobsUtility, UnsafeParallelHashSet, Uns | Unity.Collections | infra stubs only (Unity.Collections stand-in) |
| 5 [x] | `IAFahim.Collision.Gjk` | perfect | Gjk, Epa, MinkowskiDifference | Erin Catto GJK | defining tests exercise shipped APIs |
| 6 [x] | `IAFahim.Combinatorics.Generation` | perfect | IntegerPartitionEnumerator, SetPartitions, SetPartition | FKM | defining tests exercise shipped APIs |
| 7 [x] | `IAFahim.Compress` | perfect | CompressValues, RestoreCompressed | coord compress | defining tests exercise shipped APIs |
| 8 [x] | `IAFahim.Compress.Coordinate` | perfect | RankCompress, CoordinateCompress, Discretize | coord compress | defining tests exercise shipped APIs |
| 9 [x] | `IAFahim.DP` | perfect | BranchAndBound, Knapsack01, KnapsackUnbounded, Knapsack | CP-Algorithms DP / SMAWK row min | defining tests exercise shipped APIs |
| 10 [x] | `IAFahim.DP.General` | perfect | ProfileDp, BrokenProfileDp, TreeKnapsack, IntervalDp, M | CP-Algorithms DP / SMAWK row min | defining tests exercise shipped APIs |
| 11 [x] | `IAFahim.DP.Knapsack` | perfect | Knapsack01, KnapsackUnbounded, KnapsackBounded, SubsetS | CP-Algorithms DP / SMAWK row min | defining tests exercise shipped APIs |
| 12 [x] | `IAFahim.DP.Optimization` | perfect | KnuthOptimization, LiChaoAddLine | CP-Algorithms DP / SMAWK row min | defining tests exercise shipped APIs |
| 13 [x] | `IAFahim.DS.Dsu` | perfect | DsuInit, DsuFind, DsuUnion, DsuSame, DsuSize, DsuRollba | ACL + KACTL | defining tests exercise shipped APIs |
| 14 [x] | `IAFahim.DS.Fenwick` | perfect | PersistentFenwickUpdate, PersistentFenwickQuery, Fenwic | ACL + KACTL | defining tests exercise shipped APIs |
| 15 [x] | `IAFahim.DS.FixedCollections` | perfect | SpinLock, FixedBitMask, NativeLinearCongruentialGenerat | ACL + KACTL | defining tests exercise shipped APIs |
| 16 [x] | `IAFahim.DS.GapBuffer` | perfect | GapBufferState, GapBufferInsert, GapBufferDelete | ACL + KACTL | defining tests exercise shipped APIs |
| 17 [x] | `IAFahim.DS.Grid` | perfect | MakeGrid, Shuffle, Reverse, Rotate, GridNeighbors4, Gri | ACL + KACTL | defining tests exercise shipped APIs |
| 18 [x] | `IAFahim.DS.Heap` | perfect | HeapPush, HeapPop, HeapFix, HeapRemove, DequePush, Dequ | ACL + KACTL | defining tests exercise shipped APIs |
| 19 [x] | `IAFahim.DS.HilbertOrder` | perfect | HilbertOrder, GilbertOrder, BlockOrder | ACL + KACTL | defining tests exercise shipped APIs |
| 20 [x] | `IAFahim.DS.LinkCut` | perfect | LctNode, LinkCut | ACL + KACTL | defining tests exercise shipped APIs |
| 21 [x] | `IAFahim.DS.Mo` | perfect | Query3D, Update, MoWithUpdates, MoAdd, MoRemove, MoAnsw | ACL + KACTL | defining tests exercise shipped APIs |
| 22 [x] | `IAFahim.DS.OrderedSet` | perfect | OrderedSet | ACL + KACTL | defining tests exercise shipped APIs |
| 23 [x] | `IAFahim.DS.PerfectHashMap` | deferred | NativePerfectHashMap, UnsafePerfectHashMap | ACL + KACTL | Unity.Collections NativeArray container; not pure  |
| 24 [x] | `IAFahim.DS.PersistentDsu` | perfect | PersistentDsu | ACL + KACTL | defining tests exercise shipped APIs |
| 25 [x] | `IAFahim.DS.PersistentTreap` | perfect | PersistentTreapNode, PersistentTreapSplit, PersistentTr | ACL + KACTL | defining tests exercise shipped APIs |
| 26 [x] | `IAFahim.DS.PieceTable` | perfect | Piece, PieceTableState, PieceTableShared, PieceTableIns | ACL + KACTL | defining tests exercise shipped APIs |
| 27 [x] | `IAFahim.DS.RollbackSeg` | perfect | RollbackSegBuild, RollbackSegUpdate, RollbackSegQuery,  | ACL + KACTL | defining tests exercise shipped APIs |
| 28 [x] | `IAFahim.DS.RollbackStack` | perfect | RollbackStack, UndoableUnionFind, UndoableBipartiteDsu, | ACL + KACTL | defining tests exercise shipped APIs |
| 29 [x] | `IAFahim.DS.Rope` | perfect | RopeNode, RopeInsert, RopeErase, RopeSubstring | ACL + KACTL | defining tests exercise shipped APIs |
| 30 [x] | `IAFahim.DS.SegmentTree` | perfect | LiChaoTree, Line, OnlineChtAdd, OnlineChtQuery, LiChaoI | ACL + KACTL | defining tests exercise shipped APIs |
| 31 [x] | `IAFahim.DS.Sparse` | perfect | SparseTableBuild, DisjointSparseBuild, SparseTableQuery | ACL + KACTL | defining tests exercise shipped APIs |
| 32 [x] | `IAFahim.DS.SpatialMap` | deferred | LocalSpatialMap, ResizeKeys, QuantizeJob, UpdateMap, Pa | ACL + KACTL | Unity NativeParallelMultiHashMap container; not pu |
| 33 [x] | `IAFahim.DS.Splay` | perfect | SplayRevNode, SplayRangeReverse, SplayNode, Splay, Spla | ACL + KACTL | defining tests exercise shipped APIs |
| 34 [x] | `IAFahim.DS.Treap` | perfect | TreapMinNode, TreapRangeMin, TreapRevNode, TreapRangeRe | ACL + KACTL | defining tests exercise shipped APIs |
| 35 [x] | `IAFahim.DS.Trie` | perfect | TrieInsert, TrieDelete, TrieFind, TriePrefixCount, Bina | ACL + KACTL | defining tests exercise shipped APIs |
| 36 [x] | `IAFahim.DS.UnsafeArray` | perfect | UnsafeArray | ACL + KACTL | defining tests exercise shipped APIs |
| 37 [x] | `IAFahim.DS.WaveletMatrix` | perfect | WaveletMatrixBuild, WaveletMatrixKth, WaveletMatrixQuan | ACL + KACTL | defining tests exercise shipped APIs |
| 38 [x] | `IAFahim.GameTheory` | perfect | GrundyDAG, NimSum, Minimax, GameDp | Sprague-Grundy | defining tests exercise shipped APIs |
| 39 [x] | `IAFahim.Geometry.Advanced` | deferred | Circumcenter, MinimumEnclosingCircle, IntegerPointCount | KACTL/geogram/CGAL | PolygonBoolean Union/Difference/Xor honest NI (DCE |
| 40 [x] | `IAFahim.Geometry.Arrangement` | perfect | PointLocationBuild, PointLocationQuery, VerticalDecompo | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 41 [x] | `IAFahim.Geometry.Azimuth` | perfect | SphericalAzimuth, SphericalDistance, CartesianAzimuth | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 42 [x] | `IAFahim.Geometry.Basic` | perfect | IncircleExact, GeometryPoint, PointAdd, PointSub, Point | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 43 [x] | `IAFahim.Geometry.Bvh` | perfect | BvhNode, CentroidSortItem, BvhTree | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 44 [x] | `IAFahim.Geometry.Curve` | perfect | CatmullRom, CubicBezier | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 45 [x] | `IAFahim.Geometry.Delaunay` | deferred | — | KACTL/geogram/CGAL | empty package (no sources) |
| 46 [x] | `IAFahim.Geometry.Frame` | perfect | ParallelTransport | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 47 [x] | `IAFahim.Geometry.Hull` | perfect | MaximumInscribedCircle, MinkowskiSum, StraightSkeleton, | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 48 [x] | `IAFahim.Geometry.Intersect` | perfect | Polyhedron, Sphere, Plane | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 49 [x] | `IAFahim.Geometry.MarchingCubes` | deferred | — | KACTL/geogram/CGAL | empty package (no sources) |
| 50 [x] | `IAFahim.Geometry.Mesh` | perfect | MeshProjection | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 51 [x] | `IAFahim.Geometry.PolygonClip` | deferred | — | KACTL/geogram/CGAL | empty package (no sources) |
| 52 [x] | `IAFahim.Geometry.Spatial` | perfect | CoverTree, Node, KdTree, Node, Quadtree, Node, RangeTre | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 53 [x] | `IAFahim.Geometry.Subdivision` | deferred | — | KACTL/geogram/CGAL | empty package (no sources) |
| 54 [x] | `IAFahim.Geometry.SweepPrune` | deferred | — | KACTL/geogram/CGAL | empty package (no sources) |
| 55 [x] | `IAFahim.Geometry.Triangulation` | perfect | EarClipping | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 56 [x] | `IAFahim.Geometry.Voronoi` | perfect | VisibilityGraph, Delaunay, Triangle, BowyerWatson, Near | KACTL/geogram/CGAL | defining tests exercise shipped APIs |
| 57 [x] | `IAFahim.Graph` | perfect | AddEdge, AddDirectedEdge, AddWeightedEdge, BuildAdjacen | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 58 [x] | `IAFahim.Graph.Bridges` | perfect | BridgeAndArticulation, IncrementalDynamicBridges, Node, | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 59 [x] | `IAFahim.Graph.Cactus` | deferred | BlockCutTreeLca, CactusCycleDecompose, CactusShortestPa | ACL+KACTL+Boost.Graph | BlockCutTreeLca/CactusLca honest NI (contract lack |
| 60 [x] | `IAFahim.Graph.Centroid` | perfect | CentroidDecomposition | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 61 [x] | `IAFahim.Graph.Clique` | deferred | — | ACL+KACTL+Boost.Graph | README shell; no algorithm .cs sources |
| 62 [x] | `IAFahim.Graph.Connectivity` | perfect | DecrementalConnectivity, IncrementalConnectivity, Dynam | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 63 [x] | `IAFahim.Graph.Cut` | deferred | — | ACL+KACTL+Boost.Graph | README shell; min-cut in Graph.Flow |
| 64 [x] | `IAFahim.Graph.DAG` | perfect | DagPathCoverRestore, RandomTopologicalOrder, DagKthPath | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 65 [x] | `IAFahim.Graph.Decomposition` | deferred | — | ACL+KACTL+Boost.Graph | README shell; no .cs sources |
| 66 [x] | `IAFahim.Graph.Dominator` | deferred | — | ACL+KACTL+Boost.Graph | README shell; no .cs sources |
| 67 [x] | `IAFahim.Graph.DynamicTrees` | perfect | TopTreeNode, TopTree, EttNode, EulerTourTree, LctNode,  | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 68 [x] | `IAFahim.Graph.Eertree` | perfect | Eertree, Node, Next | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 69 [x] | `IAFahim.Graph.Eulerian` | perfect | EulerianPathUndirected, EulerianPathDirected | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 70 [x] | `IAFahim.Graph.Flow` | perfect | BfsLayerGraph, MinCostFlowDijkstra, HopcroftKarpBfs, Mi | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 71 [x] | `IAFahim.Graph.Functional` | perfect | FunctionalGraphComponent, FunctionalGraphKthSuccessor,  | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 72 [x] | `IAFahim.Graph.Matching` | perfect | HospitalResidentsMatching, StableRoommates, MaximumBipa | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 73 [x] | `IAFahim.Graph.Misc` | perfect | TopologicalDp, CycleDp, SccDp, DagReachability, Transit | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 74 [x] | `IAFahim.Graph.RandomWalk` | deferred | — | ACL+KACTL+Boost.Graph | README shell; no .cs sources |
| 75 [x] | `IAFahim.Graph.SCC` | perfect | OnlineScc, TarjanScc, SccAugmentation | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 76 [x] | `IAFahim.Graph.ShortestPath` | perfect | KthShortestPathEppstein, ReplacementPaths, AllPairsMinP | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 77 [x] | `IAFahim.Graph.SpanningTrees` | perfect | MinimumFeedbackArcSetApprox, MinimumPathCoverDag, Dilwo | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 78 [x] | `IAFahim.Graph.Tree` | perfect | HldBuild, HldPathQuery, TreeCentroids, RootedTreeHash,  | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 79 [x] | `IAFahim.Graph.TreeDecomposition` | perfect | TreeDecompositionDp, PathwidthDpAlgorithm, MoQuery, MoA | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 80 [x] | `IAFahim.Graph.TreeIsomorphism` | deferred | RootedTreeAutomorphisms, RootedTreeCanonicalForm, Unroo | ACL+KACTL+Boost.Graph | UnorderedTreeEditDistance unconstrained NP-hard NI |
| 81 [x] | `IAFahim.Graph.TreeQueries` | perfect | TreeCentroid, PathColorNode, TreePathColor, TreeDp, Tre | ACL+KACTL+Boost.Graph | defining tests exercise shipped APIs |
| 82 [x] | `IAFahim.Linear` | deferred | — | Gaussian elim | meta-folder only (no csproj/sources) |
| 83 [x] | `IAFahim.Linear.Eigen` | perfect | SymmetricEigen3, Svd3 | Gaussian elim | defining tests exercise shipped APIs |
| 84 [x] | `IAFahim.Linear.Matrix` | perfect | BerlekampMassey, Kitamasa, MatrixMul, MatrixPow, Gaussi | Gaussian elim | defining tests exercise shipped APIs |
| 85 [x] | `IAFahim.Linear.Matrix2` | perfect | MatrixNew, MatrixIdentity, MatrixAdd, MatrixSub, Matrix | Gaussian elim | defining tests exercise shipped APIs |
| 86 [x] | `IAFahim.Math.Arithmetic` | perfect | TryMul, TryDiv, TrySub, TryAdd | KACTL NT + ACL | defining tests exercise shipped APIs |
| 87 [x] | `IAFahim.Math.Barycentric` | perfect | BarycentricCoords | KACTL NT + ACL | defining tests exercise shipped APIs |
| 88 [x] | `IAFahim.Math.Basic` | perfect | AbsInt64, MinInt, CeilDiv, FastPow, IntegerSqrt, NthRoo | KACTL NT + ACL | defining tests exercise shipped APIs |
| 89 [x] | `IAFahim.Math.BigInt` | perfect | BigIntAdd, BigIntSub, BigIntMul, BigIntPow, BigIntDiv,  | KACTL NT + ACL | defining tests exercise shipped APIs |
| 90 [x] | `IAFahim.Math.Combinatorics` | perfect | PermuteCount, MultisetPermutations, Catalan, StirlingFi | KACTL NT + ACL | defining tests exercise shipped APIs |
| 91 [x] | `IAFahim.Math.Gauss` | perfect | GaussEliminationDouble, GaussModP | KACTL NT + ACL | defining tests exercise shipped APIs |
| 92 [x] | `IAFahim.Math.Kalman` | perfect | ScalarKalmanFilter, VectorKalmanFilter | KACTL NT + ACL | defining tests exercise shipped APIs |
| 93 [x] | `IAFahim.Math.Modular` | perfect | ExtendedGcd, ModSub, ModMul, Crt, ModPow, Lcm, ModInv,  | KACTL NT + ACL | defining tests exercise shipped APIs |
| 94 [x] | `IAFahim.Math.NT` | perfect | PhiSieve, PrimePiMeissel, MoebiusPrefix, LcmConvolution | KACTL NT + ACL | defining tests exercise shipped APIs |
| 95 [x] | `IAFahim.Math.Noise` | perfect | SimplexNoise, PerlinNoise | KACTL NT + ACL | defining tests exercise shipped APIs |
| 96 [x] | `IAFahim.Math.PoissonDisk` | perfect | PoissonDisk3D, PoissonDisk2D | KACTL NT + ACL | defining tests exercise shipped APIs |
| 97 [x] | `IAFahim.Math.Polynomial` | perfect | PolynomialMultipointEval, OnlineNttConvolution, Polynom | KACTL NT + ACL | defining tests exercise shipped APIs |
| 98 [x] | `IAFahim.Math.Polynomial.Eval` | perfect | MultiPointEval, ChirpZTransform | KACTL NT + ACL | defining tests exercise shipped APIs |
| 99 [x] | `IAFahim.Math.Polynomial.Fps` | perfect | FormalPowerSeriesSqrt, FormalPowerSeriesInverse, Formal | KACTL NT + ACL | defining tests exercise shipped APIs |
| 100 [x] | `IAFahim.Math.PotentialField` | perfect | PotentialField3D, PotentialField2D | KACTL NT + ACL | defining tests exercise shipped APIs |
| 101 [x] | `IAFahim.Math.Quaternion` | perfect | QuaternionSlerp, QuaternionOps, SwingTwistDecomposition | KACTL NT + ACL | defining tests exercise shipped APIs |
| 102 [x] | `IAFahim.Math.Sdf` | perfect | SdfPrimitive, SdfBoolean, SdfRayMarch, SdfTransform | KACTL NT + ACL | defining tests exercise shipped APIs |
| 103 [x] | `IAFahim.Math.SphericalHarmonics` | perfect | SHEvaluation | KACTL NT + ACL | defining tests exercise shipped APIs |
| 104 [x] | `IAFahim.Math.Spline` | perfect | CubicHermite, UniformBSpline | KACTL NT + ACL | defining tests exercise shipped APIs |
| 105 [x] | `IAFahim.Math.Transform` | perfect | SubsetConvolutionRanked, WalshHadamardXor, WalshHadamar | KACTL NT + ACL | defining tests exercise shipped APIs |
| 106 [x] | `IAFahim.Math.Transform.AnyMod` | perfect | ArbitraryModConvolution | KACTL NT + ACL | defining tests exercise shipped APIs |
| 107 [x] | `IAFahim.Math.Transform.Fft` | perfect | FftTransform, FftConvolution | KACTL NT + ACL | defining tests exercise shipped APIs |
| 108 [x] | `IAFahim.Math.Transform.Ntt` | perfect | NttInit, NttTransform, NttConvolution | KACTL NT + ACL | defining tests exercise shipped APIs |
| 109 [x] | `IAFahim.Memory.Allocators` | deferred | UnsafeFixedPoolAllocator, MemoryAllocator, UnsafeParall | Unity allocators | infra allocators only |
| 110 [x] | `IAFahim.Optimization.Approximation` | perfect | Metheuristics, Freivalds, SchwartzZippel | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 111 [x] | `IAFahim.Optimization.DivideConquer` | perfect | SlopeTrick, State, LagrangianRelaxation, MatrixSearch,  | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 112 [x] | `IAFahim.Optimization.Exact` | perfect | MaxIndependentSet, MinSetCover, MaximumClique, Hamilton | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 113 [x] | `IAFahim.Optimization.Games` | perfect | AttractorSet, MinCostFlow, Grundy, Simplex, Result, Mdp | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 114 [x] | `IAFahim.Optimization.Geometric` | perfect | WelzlSphere, Sphere, MinEnclosingBall, Circle | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 115 [x] | `IAFahim.Optimization.Knapsack` | perfect | DivideConquerKnapsack, MultipleChoiceKnapsack, MeetInMi | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 116 [x] | `IAFahim.Optimization.Matroid` | perfect | MatroidGreedy, LinearMatroid | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 117 [x] | `IAFahim.Optimization.Offline` | perfect | ParallelBinarySearch, DivideConquerAnswer, Cdq3DDominan | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 118 [x] | `IAFahim.Optimization.Submodular` | perfect | MaxCut, SubmodularGreedy, Rounding | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 119 [x] | `IAFahim.Optimization.Treewidth` | perfect | CutAndCount, ConvexHull, RankDp, FastSubsetDp, RankTran | CP-Algo/opt literature | defining tests exercise shipped APIs |
| 120 [x] | `IAFahim.Pathfinding.Jps` | deferred | — | recast/JPS | empty package (no sources) |
| 121 [x] | `IAFahim.Pathfinding.Recast` | perfect | ushort3, byte4, RcContour, RcPolyMeshDetail, RcContourS | recast/JPS | defining tests exercise shipped APIs |
| 122 [x] | `IAFahim.Permutation` | perfect | ValidatePermutation, InversePermutation, ComposePermuta | cycle decomp | defining tests exercise shipped APIs |
| 123 [x] | `IAFahim.Physics.Xpbd` | perfect | ShapeMatchingConstraint, CollisionConstraint, DistanceC | XPBD | defining tests exercise shipped APIs |
| 124 [x] | `IAFahim.Search` | deferred | — | CP-Algo/KACTL | meta-folder only (no csproj/sources) |
| 125 [x] | `IAFahim.Search.Automaton` | perfect | ModMatrixPow, BuildAutomaton | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 126 [x] | `IAFahim.Search.Bit` | perfect | BitsetOr, BitsetAnd, BitsetShift, BitsetSet, BitsetGet, | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 127 [x] | `IAFahim.Search.DifferenceArray` | perfect | Diff | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 128 [x] | `IAFahim.Search.ExactCover` | perfect | ExactCover | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 129 [x] | `IAFahim.Search.Imos` | perfect | ImosRectangle, ImosShared, LargestRectangleHistogram, L | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 130 [x] | `IAFahim.Search.Interval` | perfect | Interval, MergeIntervals, IntersectIntervals, Normalize | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 131 [x] | `IAFahim.Search.LIS` | perfect | Lis | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 132 [x] | `IAFahim.Search.MeetInMiddle` | perfect | MeetInMiddle | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 133 [x] | `IAFahim.Search.Numerical` | perfect | SimulatedAnnealing, TernaryReal, AdaptiveSimpson, Simps | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 134 [x] | `IAFahim.Search.Prefix` | perfect | PrefixSums, PrefixXor, PrefixMin, PrefixMax, PrefixSear | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 135 [x] | `IAFahim.Search.Range` | perfect | RangeMax, RangeAdd, RangeMex, MexMaintain, RangeMin, Ra | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 136 [x] | `IAFahim.Search.RangeQueries` | perfect | StaticRangeMode, StaticRangeMex, StaticRangeInversions, | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 137 [x] | `IAFahim.Search.Selection` | perfect | SelectionShared, Selection, TopK, MedianMaintain | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 138 [x] | `IAFahim.Search.Specialized` | perfect | Interactive, Scheduling, UpperBound, TernarySearch, Bin | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 139 [x] | `IAFahim.Search.Subset` | perfect | EnumerateSupersets, EnumerateMasks, EnumerateSubsets | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 140 [x] | `IAFahim.Search.Suffix` | perfect | SuffixSums, SuffixMin, SuffixMax | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 141 [x] | `IAFahim.Search.TwoPointer` | perfect | TwoPointers | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 142 [x] | `IAFahim.Search.Window` | perfect | Heap, SlidingWindowMin, SlidingWindowMax | CP-Algo/KACTL | defining tests exercise shipped APIs |
| 143 [x] | `IAFahim.Sort.Insertion` | perfect | Insertion | CLRS+BCL | defining tests exercise shipped APIs |
| 144 [x] | `IAFahim.Sort.Merge` | perfect | MergeSorted | CLRS+BCL | defining tests exercise shipped APIs |
| 145 [x] | `IAFahim.Sort.Partition` | perfect | Partition | CLRS+BCL | defining tests exercise shipped APIs |
| 146 [x] | `IAFahim.Sort.QuickSort` | perfect | QuickSort | CLRS+BCL | defining tests exercise shipped APIs |
| 147 [x] | `IAFahim.Sort.RadixSort` | perfect | RadixSortLsd | CLRS+BCL | defining tests exercise shipped APIs |
| 148 [x] | `IAFahim.Sort.Specialized` | perfect | SortPairs, Pair, SortInt64s, SortInts | CLRS+BCL | defining tests exercise shipped APIs |
| 149 [x] | `IAFahim.String` | perfect | DeBruijn, SimpleRand, Probabilistic, KmpPrefix, KmpSear | ACL+KACTL string | defining tests exercise shipped APIs |
| 150 [x] | `IAFahim.String.Automata` | perfect | FiniteAutomaton, Dfa, SubsequenceAutomaton | ACL+KACTL string | defining tests exercise shipped APIs |
| 151 [x] | `IAFahim.String.Compress` | perfect | Huffman, Code, Node, Lz78, Token, Lz77, Token, LzFactor | ACL+KACTL string | defining tests exercise shipped APIs |
| 152 [x] | `IAFahim.String.FMIndex` | perfect | FmBackwardSearch, BurrowsWheeler, FMIndex | ACL+KACTL string | defining tests exercise shipped APIs |
| 153 [x] | `IAFahim.String.Grammar` | perfect | GrammarCompress, Rule, StraightLineProgram, Rule | ACL+KACTL string | defining tests exercise shipped APIs |
| 154 [x] | `IAFahim.String.Match` | perfect | DictionaryMatch, AhoCorasick, State, Crochemore, Repeti | ACL+KACTL string | defining tests exercise shipped APIs |
| 155 [x] | `IAFahim.String.MinRotation` | perfect | Booth | ACL+KACTL string | defining tests exercise shipped APIs |
| 156 [x] | `IAFahim.String.Palindrome` | perfect | PalindromicTree, Node, DynamicPalindromicTree, LyndonFa | ACL+KACTL string | defining tests exercise shipped APIs |
| 157 [x] | `IAFahim.String.Parse` | perfect | SuffixOracle, LrParse, Action, LlParse, Earley, State,  | ACL+KACTL string | defining tests exercise shipped APIs |
| 158 [x] | `IAFahim.String.Pattern` | perfect | AhoPersistentBuild, AhoPersistentQuery | ACL+KACTL string | defining tests exercise shipped APIs |
| 159 [x] | `IAFahim.String.SuffixArray` | perfect | DynamicStringNode, DynamicSuffixArray, Locate, LcpInter | ACL+KACTL string | defining tests exercise shipped APIs |
| 160 [x] | `IAFahim.String.SuffixAutomaton` | perfect | LexicographicKth, OccurrencePositions, PersistentSam, V | ACL+KACTL string | defining tests exercise shipped APIs |
| 161 [x] | `IAFahim.String.SuffixTree` | perfect | SuffixTreeUkkonen, Node, Edge | ACL+KACTL string | defining tests exercise shipped APIs |
| 162 [x] | `IAFahim.Unique` | perfect | UniqueInt64s, UniqueInts | std::unique | defining tests exercise shipped APIs |
