# TODO.md

## Completed

### Math/Number Theory (IAFahim.Math.NT)
- [x] MillerRabin, PollardRho, Factorize
- [x] Divisors, DivisorCount, DivisorSum
- [x] Phi, PhiSieve, Mobius, MobiusSieve, Radical
- [x] PrimitiveRoot, DiscreteLog, Bsgs
- [x] TonelliShanks, JacobiSymbol, LegendreSymbol
- [x] FloorSum, EuclidSum
- [x] SternBrocot, FareyRank, ContinuedFraction, Convergents
- [x] RandomInt, RandomInt64, RandomShuffle, SplitMix64
- [x] HashInt, XorShift, RngSeed
- [x] BitCount, BitLength, HighestBit, LowestBit, NextBit, PrevBit
- [x] BitReverse, BitCompress, BitDecompress

### Math/Combinatorics (IAFahim.Math.Combinatorics)
- [x] LinearCongruence, Factorial, InvFactorial
- [x] Binom, BinomLucas, BinomLarge
- [x] PermuteCount, MultisetPermutations, Catalan
- [x] StirlingFirst, StirlingSecond, BellNumbers
- [x] PartitionNumbers, Derangements, StarsBars
- [x] SievePrimes, LinearSieve, SegmentedSieve, IsPrime

### Math/Transform (IAFahim.Math.Transform)
- [x] SubsetZeta, SubsetMobius, SupersetZeta, SupersetMobius
- [x] SubsetConvolution
- [x] WalshHadamardXor, WalshHadamardOr, WalshHadamardAnd, FwhtConvolution
- [x] XorBasisInsert, XorBasisMax, XorBasisMin, XorBasisRank, XorBasisKth

### DS/Fenwick (IAFahim.DS.Fenwick)
- [x] FenwickAdd, FenwickSum, FenwickRangeSum, FenwickLowerBound
- [x] Fenwick2DAdd, Fenwick2DSum
- [x] FenwickRangeAdd, FenwickPointQuery

### DS/DSU (IAFahim.DS.Dsu)
- [x] DsuInit, DsuFind, DsuUnion, DsuSame, DsuSize
- [x] DsuRollbackSnapshot, DsuRollback, DsuUndo
- [x] DsuBipartiteAdd, DsuParityFind, DsuParityUnion
- [x] SmallToLargeMerge

### DS/Heap (IAFahim.DS.Heap)
- [x] HeapPush, HeapPop, HeapFix, HeapRemove
- [x] DequePush, DequePop
- [x] MonotonicQueueMin, MonotonicQueuePush, MonotonicStackProcess

### DS/Trie (IAFahim.DS.Trie)
- [x] TrieInsert, TrieDelete, TrieFind, TriePrefixCount
- [x] BinaryTrieInsert, BinaryTrieErase, BinaryTrieMaxXor, BinaryTrieMinXor
- [x] PersistentTrieInsert, PersistentTrieQuery

### DS/SegmentTree (IAFahim.DS.SegmentTree)
- [x] SegmentTreeBuild, SegmentTreeSet, SegmentTreeAdd, SegmentTreeQuery
- [x] SegmentTreeMaxRight, SegmentTreeMinLeft
- [x] LazySegmentBuild, LazySegmentApply, LazySegmentPush, LazySegmentPull
- [x] LazySegmentQuery, LazySegmentUpdate
- [x] DualSegmentApply, DualSegmentGet
- [x] PersistentSegmentBuild, PersistentSegmentUpdate, PersistentSegmentQuery
- [x] DynamicSegmentUpdate, DynamicSegmentQuery

### DS/Sparse (IAFahim.DS.Sparse)
- [x] SparseTableBuild, SparseTableQuery
- [x] DisjointSparseBuild, DisjointSparseQuery
- [x] SqrtDecomposeBuild, SqrtUpdate, SqrtQuery
- [x] WaveletTreeBuild, WaveletRank, WaveletSelect, WaveletKth, WaveletRangeFreq

### DS/Mo (IAFahim.DS.Mo)
- [x] MoAdd, MoRemove, MoAnswer, MoSort, MoRollback

### String (IAFahim.String)
- [x] ManacherOdd, ManacherEven, DuvalLyndon, MinCyclicShift
- [x] RunLengthEncode, RunLengthDecode, StringPeriod, MinPeriod, Borders, CountOccurrences
- [x] KmpPrefix, KmpSearch, ZAlgorithm
- [x] HashBuild, HashRange, HashConcat, DoubleHashBuild, RollingHash
- [x] SuffixArrayBuild, SuffixLcpBuild, SuffixCompare, SuffixLowerBound
- [x] EditDistance, Levenshtein, Lcs, LcsLength, ScsLength, WildcardMatch
- [x] SuffixAutomatonExtend, SuffixAutomatonBuild
- [x] AhoBuild, AhoNext, AhoMatch, AhoCount
- [x] PalindromicTreeAdd, PalindromicTreeBuild
- [x] RegexNfaBuild, RegexMatch
- [x] ParseExpression, ParseInteger, Tokenize

### Graph (IAFahim.Graph)
- [x] AddEdge, AddDirectedEdge, AddWeightedEdge, BuildAdjacency, TransposeGraph
- [x] Bfs, ZeroOneBfs, MultiSourceBfs, Dfs, IterativeDfs
- [x] Toposort, KahnToposort, DetectCycleDirected, DetectCycleUndirected
- [x] ConnectedComponents, Kosaraju, TarjanScc, CondenseGraph
- [x] ArticulationPoints, Bridges
- [x] IsBipartite, ColorBipartite
- [x] ShortestPathUnweighted
- [x] Dijkstra, DijkstraSparse, DijkstraDense, DijkstraRestorePath
- [x] BellmanFord, Spfa, FloydWarshall, Johnson, ZeroOneShortestPath
- [x] MinimumSpanningTreeKruskal, MinimumSpanningTreePrim, SecondBestMst
- [x] TwoSatAddClause, TwoSatSolve, Hierholzer
- [x] EulerPathDirected, EulerPathUndirected, EulerTourTree
- [x] GraphInit

### Graph/Tree (IAFahim.Graph.Tree)
- [x] LcaBuild, LcaQuery, LcaDistance
- [x] BinaryLiftBuild, BinaryLiftKthAncestor
- [x] CentroidFind, CentroidDecompose
- [x] TreeDfs, TreeParent, TreeDepth, TreeSize, TreeDiameter, TreeCenter, TreeCentroids
- [x] HldBuild, HldPathQuery, HldPathUpdate, HldSubtreeQuery, HldSubtreeUpdate
- [x] VirtualTreeBuild, TreeDp, RerootDp
- [x] EulerLcaBuild, RmqLcaQuery, TreeReroot
- [x] TreeHash, RootedTreeHash, TreeIsomorphism, CartesianTreeBuild

### Graph/Flow (IAFahim.Graph.Flow)
- [x] EdmondsKarp, DinicBfs, DinicDfs, DinicMaxFlow
- [x] MinCut, FlowDecompose
- [x] MinCostFlowAddEdge, MinCostMaxFlow

### Graph (additional)
- [x] ChuLiuEdmonds, Boruvka, KruskalReconstructionTree
- [x] YenKShortestPaths, AStar
- [x] BiconnectedComponents, EdgeBiconnectedComponents, TwoEdgeConnectedComponents
- [x] DominatorTree, FindDominators

## Pending

### Math (remaining)
- [ ] TonelliShanks (polished)
- [ ] RandomInt64 (polished)
- [ ] Additional combinatorics identities
