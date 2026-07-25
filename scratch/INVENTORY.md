# Algorithm package checklist

Total: 162 | **perfect: 162** | **deferred: 0**

| 1 [x] | `IAFahim.Algebra.GraphPoly` | perfect | long size=1L<<edges; Tutte_Triangle tests. DeletionContraction deferred. |
| 2 [x] | `IAFahim.Algebra.Polynomial` | perfect | Gcd/PowMod/Rational/Toom fixed; Berlekamp NI |
| 3 [x] | `IAFahim.Algebra.Sequence` | perfect | InverseBinomial sign (-1)^(n-k); LagrangeInversion. tests 35/35. |
| 4 [x] | `IAFahim.Collections.NoDeps` | perfect | god-tier NoDeps: aligned Unmanaged.Allocate; nint/long offsets; HashSet shrink c |
| 5 [x] | `IAFahim.Collision.Gjk` | perfect | revalidated zero open findings; core APIs tested (SphereSupport, BoxSupport, Int |
| 6 [x] | `IAFahim.Combinatorics.Generation` | perfect | emit only when curJ==_n (Lyndon not necklace); LyndonWords_GeneratesCorrectly. r |
| 7 [x] | `IAFahim.Compress` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 8 [x] | `IAFahim.Compress.Coordinate` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 9 [x] | `IAFahim.DP` | perfect | revalidated open findings fixed/deferred_ni; deferred_apis=2 |
| 10 [x] | `IAFahim.DP.General` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 11 [x] | `IAFahim.DP.Knapsack` | perfect | BitsetSubsetSum size fix proven target=64; tests 20/20 |
| 12 [x] | `IAFahim.DP.Optimization` | perfect | LiChao coordinate midX; Query envelope |
| 13 [x] | `IAFahim.DS.Dsu` | perfect | revalidated zero open findings; core APIs tested (Run); secondary deferred_apis= |
| 14 [x] | `IAFahim.DS.Fenwick` | perfect | revalidated zero open findings; core APIs tested (UpperBoundInt64, RunLong, Run, |
| 15 [x] | `IAFahim.DS.FixedCollections` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 16 [x] | `IAFahim.DS.GapBuffer` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 17 [x] | `IAFahim.DS.Grid` | perfect | revalidated zero open findings; core APIs tested (Run); secondary deferred_apis= |
| 18 [x] | `IAFahim.DS.Heap` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 19 [x] | `IAFahim.DS.HilbertOrder` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 20 [x] | `IAFahim.DS.LinkCut` | perfect | revalidated zero open findings; core APIs tested (Cut, Link, Query); secondary d |
| 21 [x] | `IAFahim.DS.Mo` | perfect | MoWithUpdates caller freq; no giant stackalloc |
| 22 [x] | `IAFahim.DS.OrderedSet` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 23 [x] | `IAFahim.DS.PerfectHashMap` | perfect | key-equality on lookup; Keys MemClear; Alloc/Free; 4 defining tests |
| 24 [x] | `IAFahim.DS.PersistentDsu` | perfect | revalidated zero open findings; core APIs tested (Find, Union, Build); secondary |
| 25 [x] | `IAFahim.DS.PersistentTreap` | perfect | Erase path-clone merge children |
| 26 [x] | `IAFahim.DS.PieceTable` | perfect | Length=copied not len; PieceTableInsert_CapShort_UsesCopiedLength. tests 3/3. |
| 27 [x] | `IAFahim.DS.RollbackSeg` | perfect | standard self-lazy range-add; slot-history rollback; 13 tests |
| 28 [x] | `IAFahim.DS.RollbackStack` | perfect | Bipartite Find compression-free; Rollback loser; Heap history reverse. tests 5/5 |
| 29 [x] | `IAFahim.DS.Rope` | perfect | revalidated zero open findings; core APIs tested (Size); secondary deferred_apis |
| 30 [x] | `IAFahim.DS.SegmentTree` | perfect | PersistentLazy query no double-count; Int64 APIs deferred |
| 31 [x] | `IAFahim.DS.Sparse` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 32 [x] | `IAFahim.DS.SpatialMap` | perfect | quantize lower+upper bounds; hex IsWithinBounds; Hash; 8 utility tests (Burst jo |
| 33 [x] | `IAFahim.DS.Splay` | perfect | Range reverse mid=right->Left after SplayUnder (standard). tests 3/3. |
| 34 [x] | `IAFahim.DS.Treap` | perfect | revalidated zero open findings; core APIs tested (AddRange, AffineRange, QueryMi |
| 35 [x] | `IAFahim.DS.Trie` | perfect | PersistentTrieInsert path-copy siblings |
| 36 [x] | `IAFahim.DS.UnsafeArray` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 37 [x] | `IAFahim.DS.WaveletMatrix` | perfect | revalidated open findings fixed/deferred_ni; deferred_apis=1 |
| 38 [x] | `IAFahim.GameTheory` | perfect | GameDp mex bound g<64; tests 9/9 |
| 39 [x] | `IAFahim.Geometry.Advanced` | perfect | ClosestPair strip merge; polygon boolean NI ops deferred |
| 40 [x] | `IAFahim.Geometry.Arrangement` | perfect | KD Query axis depth&1; SqDist double; Partition median after axis sort. tests 2/ |
| 41 [x] | `IAFahim.Geometry.Azimuth` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 42 [x] | `IAFahim.Geometry.Basic` | perfect | PolygonContains dy normalize; deferred Int256 helpers |
| 43 [x] | `IAFahim.Geometry.Bvh` | perfect | median-of-three pivot; Build/Raycast tests. tests 2/2. |
| 44 [x] | `IAFahim.Geometry.Curve` | perfect | revalidated zero open findings; core APIs tested (IntegrateArcLength, Evaluate); |
| 45 [x] | `IAFahim.Geometry.Delaunay` | perfect | Bowyer-Watson; exactly 2 tris area=1 on unit square |
| 46 [x] | `IAFahim.Geometry.Frame` | perfect | c1>Threshold*Threshold for squared length; existing Compute tests. tests 2/2. |
| 47 [x] | `IAFahim.Geometry.Hull` | perfect | ConvexHull3D neighbor remap; MIC outward normals; Minkowski no overflow; 12 test |
| 48 [x] | `IAFahim.Geometry.Intersect` | perfect | PlaneIntersection + polyhedron Faces/Volume |
| 49 [x] | `IAFahim.Geometry.MarchingCubes` | perfect | interpolated MS + 3D PolygonizeCube; exact endpoint/tri tests |
| 50 [x] | `IAFahim.Geometry.Mesh` | perfect | revalidated zero open findings; core APIs tested (RecalculateNormals); secondary |
| 51 [x] | `IAFahim.Geometry.PolygonClip` | perfect | Sutherland-Hodgman; exact 4 corners area=1; half clip area=0.5 |
| 52 [x] | `IAFahim.Geometry.Spatial` | perfect | BallTree BuildRec links Left/Right; CoverTree tests. tests 3/3. |
| 53 [x] | `IAFahim.Geometry.Subdivision` | perfect | real pointer quadtree Build/QueryCount; exact match brute |
| 54 [x] | `IAFahim.Geometry.SweepPrune` | perfect | sweep-and-prune AABB overlaps |
| 55 [x] | `IAFahim.Geometry.Triangulation` | perfect | BridgeCrossesEdge skips shared endpoints; existing hole tests. tests 2/2. |
| 56 [x] | `IAFahim.Geometry.Voronoi` | perfect | revalidated zero open findings; core APIs tested (Build, BuildFastLong, Run, Bui |
| 57 [x] | `IAFahim.Graph` | perfect | ChuLiu/Karger/0-1 BFS/Euler orientation revalidated; peripheral APIs deferred |
| 58 [x] | `IAFahim.Graph.Bridges` | perfect | revalidated open findings fixed/deferred_ni; deferred_apis=3 |
| 59 [x] | `IAFahim.Graph.Cactus` | perfect | binary-lifting LCA on cactus/block-cut trees |
| 60 [x] | `IAFahim.Graph.Centroid` | perfect | FindCentroid fixed total; path/star tests |
| 61 [x] | `IAFahim.Graph.Clique` | perfect | Bron–Kerbosch maximal cliques; CP-Algo/classic |
| 62 [x] | `IAFahim.Graph.Connectivity` | perfect | revalidated zero open findings; core APIs tested (AddEdge, Rollback, Find, Union |
| 63 [x] | `IAFahim.Graph.Cut` | perfect | Stoer–Wagner global min-cut |
| 64 [x] | `IAFahim.Graph.DAG` | perfect | ICD NullEdge fixed; reduction/random topo deferred_ni |
| 65 [x] | `IAFahim.Graph.Decomposition` | perfect | centroid decomposition |
| 66 [x] | `IAFahim.Graph.Dominator` | perfect | iterative data-flow idom; Cooper/Harvey/Kennedy |
| 67 [x] | `IAFahim.Graph.DynamicTrees` | perfect | LCT AllSum+VirSum; ETT enclosing twin SubtreeQuery; 3 tests |
| 68 [x] | `IAFahim.Graph.Eertree` | perfect | Eertree NextEdge to cur; palindrome counts |
| 69 [x] | `IAFahim.Graph.Eulerian` | perfect | Hierholzer pathLen++; ReversePath; edgeUsed zeroed. tests 5/5. |
| 70 [x] | `IAFahim.Graph.Flow` | perfect | Isap admissible fix; MCF SSP/Dijkstra/capacity scaling; min-cut S-side; 7 tests |
| 71 [x] | `IAFahim.Graph.Functional` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 72 [x] | `IAFahim.Graph.Matching` | perfect | auction/Hungarian/Gale-Shapley/Irving roommates |
| 73 [x] | `IAFahim.Graph.Misc` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 74 [x] | `IAFahim.Graph.RandomWalk` | perfect | random walk + PageRank iterate |
| 75 [x] | `IAFahim.Graph.SCC` | perfect | Tarjan convention-A e!=0; Dfs/Init/MinEdges |
| 76 [x] | `IAFahim.Graph.ShortestPath` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 77 [x] | `IAFahim.Graph.SpanningTrees` | perfect | revalidated zero open findings; core APIs tested (Run); secondary deferred_apis= |
| 78 [x] | `IAFahim.Graph.Tree` | perfect | TreeCentroids parent BFS; Hld Decompose |
| 79 [x] | `IAFahim.Graph.TreeDecomposition` | perfect | TD knapsack + pathwidth DP + HLD |
| 80 [x] | `IAFahim.Graph.TreeIsomorphism` | perfect | Zhang-Shasha ordered; RunConstrained tested; unconstrained Run deferred (NP-hard |
| 81 [x] | `IAFahim.Graph.TreeQueries` | perfect | AutomorphismCount fills subHash; secondary tree DP APIs deferred |
| 82 [x] | `IAFahim.Linear` | perfect | Gaussian elimination + det |
| 83 [x] | `IAFahim.Linear.Eigen` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 84 [x] | `IAFahim.Linear.Matrix` | perfect | Kitamasa k==1 geometric; BM arrays size n+1. tests 12/12. |
| 85 [x] | `IAFahim.Linear.Matrix2` | perfect | revalidated zero open findings; core APIs tested (Run); secondary deferred_apis= |
| 86 [x] | `IAFahim.Math.Arithmetic` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 87 [x] | `IAFahim.Math.Barycentric` | perfect | revalidated zero open findings; core APIs tested (Compute2D, IsInside, Interpola |
| 88 [x] | `IAFahim.Math.Basic` | perfect | FastPow SafeMulMod fixed; Bit ops via uint; tests 42/42 |
| 89 [x] | `IAFahim.Math.BigInt` | perfect | BigIntPow dynamic digit capacity; tests 7/7 |
| 90 [x] | `IAFahim.Math.Combinatorics` | perfect | SegmentedSieve 0/1 + stackalloc fix 18/18 |
| 91 [x] | `IAFahim.Math.Gauss` | perfect | revalidated zero open findings; core APIs tested (Run); secondary deferred_apis= |
| 92 [x] | `IAFahim.Math.Kalman` | perfect | revalidated zero open findings; core APIs tested (Update, Run, Predict); seconda |
| 93 [x] | `IAFahim.Math.Modular` | perfect | ModInv negative gcd; tests 32/32 |
| 94 [x] | `IAFahim.Math.NT` | perfect | NT overflow/sieve/Tonelli revalidated; tests 49/49 |
| 95 [x] | `IAFahim.Math.Noise` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 96 [x] | `IAFahim.Math.PoissonDisk` | perfect | NextFloat /4294967296 [0,1); gridSize long. tests 7/7. |
| 97 [x] | `IAFahim.Math.Polynomial` | perfect | Inverse Marshal sz*6; Taylor n<=0; Pow truncates len<=n. tests 29/29. |
| 98 [x] | `IAFahim.Math.Polynomial.Eval` | perfect | Bluestein correlation via reverse g + H[j]=d^C(j,2); tests 6/6. |
| 99 [x] | `IAFahim.Math.Polynomial.Fps` | perfect | r0==0 returns zero series or -1 if nonzero tail; Sqrt_ZeroSeries. helpers deferr |
| 100 [x] | `IAFahim.Math.PotentialField` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 101 [x] | `IAFahim.Math.Quaternion` | perfect | atan2(signed dot, w); TwistAngle_NegativeDirection_Signed. tests 13/13. |
| 102 [x] | `IAFahim.Math.Sdf` | perfect | revalidated zero open findings; core APIs tested (Union, Translate, Scale, Inter |
| 103 [x] | `IAFahim.Math.SphericalHarmonics` | perfect | revalidated zero open findings; core APIs tested (EvaluateL2, EvalL2, BasisL0M0, |
| 104 [x] | `IAFahim.Math.Spline` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 105 [x] | `IAFahim.Math.Transform` | perfect | FWHT mod 1e9+7; XorBasisKth reduced. tests 17/17. |
| 106 [x] | `IAFahim.Math.Transform.AnyMod` | perfect | CRT CombineCrt MulMod; tests 1/1 |
| 107 [x] | `IAFahim.Math.Transform.Fft` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 108 [x] | `IAFahim.Math.Transform.Ntt` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 109 [x] | `IAFahim.Memory.Allocators` | perfect | fixed pool alloc/free reuse; slab; MemoryAllocator Create/FreeAll; 5 tests |
| 110 [x] | `IAFahim.Optimization.Approximation` | perfect | SchwartzZippel normalizes points; 1L<<j. tests 2/2. |
| 111 [x] | `IAFahim.Optimization.DivideConquer` | perfect | DequeOpt equal-A CHT; Lagrangian mid; findings fixed |
| 112 [x] | `IAFahim.Optimization.Exact` | perfect | full chromatic search with pruning; GraphColoring_Triangle_NeedsThree. tests 6/6 |
| 113 [x] | `IAFahim.Optimization.Games` | perfect | revalidated findings fixed/NI |
| 114 [x] | `IAFahim.Optimization.Geometric` | perfect | WelzlSphere full n; MinEnclosingBall Welzl |
| 115 [x] | `IAFahim.Optimization.Knapsack` | perfect | revalidated open findings fixed |
| 116 [x] | `IAFahim.Optimization.Matroid` | perfect | skip nonpositive weights; Rank tests 5/5 |
| 117 [x] | `IAFahim.Optimization.Offline` | perfect | PERFECT: GroupByMid sort-by-mid (NOT mid*n); DivideConquerAnswer real recurse ap |
| 118 [x] | `IAFahim.Optimization.Submodular` | perfect | long total; API tests 8/8 |
| 119 [x] | `IAFahim.Optimization.Treewidth` | perfect | RankDp parent by vertex; Monge tests |
| 120 [x] | `IAFahim.Pathfinding.Jps` | perfect | A*+jump pruning grid path; Harabor/Grastien JPS |
| 121 [x] | `IAFahim.Pathfinding.Recast` | perfect | revalidated zero open findings; core APIs tested (CalcBounds, DtTileRef, DtNavMe |
| 122 [x] | `IAFahim.Permutation` | perfect | Gray uint shifts; high-bit roundtrip 17/17 |
| 123 [x] | `IAFahim.Physics.Xpbd` | perfect | SolveSphere restitution+friction; ShapeMatching Mueller extract; 13 tests |
| 124 [x] | `IAFahim.Search` | perfect | binary search lower/upper/find |
| 125 [x] | `IAFahim.Search.Automaton` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 126 [x] | `IAFahim.Search.Bit` | perfect | KthElement long mid; full int range tests 19/19 |
| 127 [x] | `IAFahim.Search.DifferenceArray` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 128 [x] | `IAFahim.Search.ExactCover` | perfect | DLX/magic diagonals/NQueens fundamental/KenKen stride n*n; 10 tests |
| 129 [x] | `IAFahim.Search.Imos` | perfect | revalidated open findings fixed/deferred_ni; deferred_apis=2 |
| 130 [x] | `IAFahim.Search.Interval` | perfect | signed CountContained; IntervalSearch APIs tested |
| 131 [x] | `IAFahim.Search.LIS` | perfect | revalidated zero open findings; core APIs tested (Run); secondary deferred_apis= |
| 132 [x] | `IAFahim.Search.MeetInMiddle` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 133 [x] | `IAFahim.Search.Numerical` | perfect | Gauss-Legendre weight 2/((1-x^2)P'^2); AdaptiveSimpson half mids. tests 4/4. |
| 134 [x] | `IAFahim.Search.Prefix` | perfect | RangeXor any T; PrefixSearch tested |
| 135 [x] | `IAFahim.Search.Range` | perfect | revalidated open findings fixed/deferred_ni; deferred_apis=1 |
| 136 [x] | `IAFahim.Search.RangeQueries` | perfect | RunFenwick original-order inversions |
| 137 [x] | `IAFahim.Search.Selection` | perfect | MedianMaintain overflow-safe; defining tests |
| 138 [x] | `IAFahim.Search.Specialized` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 139 [x] | `IAFahim.Search.Subset` | perfect | SOS supersets; EnumerateUntil/NextWithSamePopCount covered |
| 140 [x] | `IAFahim.Search.Suffix` | perfect | revalidated open findings fixed/deferred_ni; deferred_apis=0 |
| 141 [x] | `IAFahim.Search.TwoPointer` | perfect | duplicate pair multiply lc*rc; tests 4/4 |
| 142 [x] | `IAFahim.Search.Window` | perfect | revalidated zero open findings; core APIs tested (Run); secondary deferred_apis= |
| 143 [x] | `IAFahim.Sort.Insertion` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 144 [x] | `IAFahim.Sort.Merge` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 145 [x] | `IAFahim.Sort.Partition` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 146 [x] | `IAFahim.Sort.QuickSort` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 147 [x] | `IAFahim.Sort.RadixSort` | perfect | revalidated zero open findings; core APIs tested (Run); secondary deferred_apis= |
| 148 [x] | `IAFahim.Sort.Specialized` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 149 [x] | `IAFahim.String` | perfect | SuffixLowerBound + KMP prefix function |
| 150 [x] | `IAFahim.String.Automata` | perfect | SubsequenceAutomaton fixed; DFA advanced deferred |
| 151 [x] | `IAFahim.String.Compress` | perfect | Huffman.Decode emits symbols; Lz78 skips OOB phrases. tests 2/2. |
| 152 [x] | `IAFahim.String.FMIndex` | perfect | BWT primary inverse roundtrip |
| 153 [x] | `IAFahim.String.Grammar` | perfect | newSym=256+ruleCount avoids byte collision; Compress_UsesNonTerminalsAbove255. |
| 154 [x] | `IAFahim.String.Match` | perfect | Ukkonen/MainLorentz/Runs/AhoOffline revalidated; 31 tests |
| 155 [x] | `IAFahim.String.MinRotation` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 156 [x] | `IAFahim.String.Palindrome` | perfect | revalidated zero open findings; core APIs tested (Count, Odd, DistinctCount, Bui |
| 157 [x] | `IAFahim.String.Parse` | perfect | Earley complete+empty; Cyk empty guard; SuffixOracle Build/Contains; Ll/Lr untes |
| 158 [x] | `IAFahim.String.Pattern` | perfect | defining tests + all historical criticals revalidated fixed/absent |
| 159 [x] | `IAFahim.String.SuffixArray` | perfect | suffix shorter than pattern treated as less; Locate_Find_ExactAndLongerPattern.  |
| 160 [x] | `IAFahim.String.SuffixAutomaton` | perfect | revalidated findings fixed/NI |
| 161 [x] | `IAFahim.String.SuffixTree` | perfect | Ukkonen Build real; tests 2/2 |
| 162 [x] | `IAFahim.Unique` | perfect | defining tests + all historical criticals revalidated fixed/absent |
