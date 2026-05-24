# IAFahim.CS Algorithm Collection

IAFahim.CS is a high-performance, unmanaged C# library providing a comprehensive suite of algorithms and data structures optimized for competitive programming, scientific computing, and performance-critical applications.

## Algebra
- **GraphPoly**: Polynomials associated with graphs (e.g., chromatic, Tutte polynomials). Use for computing graph invariants and solving complex counting problems.
- **Polynomial**: Core operations on univariate and multivariate polynomials. Use for symbolic mathematics, interpolation, and algebraic modeling.
- **Sequence**: Algorithms for sequence manipulation and formal power series. Use for generating functions, linear recurrences, and combinatorial identity verification.

## Combinatorics
- **Generation**: Efficient algorithms for generating permutations, combinations, and subsets. Use for exhaustive search, sampling, and combinatorial optimization.

## Compression
- **Coordinate**: Mapping large, sparse coordinate ranges to a dense integer space. Use for memory-efficient range queries and discrete geometric processing.
- **Value**: General-purpose value compression techniques. Use for reducing the state space of DP algorithms or optimizing data structure memory footprints.

## Dynamic Programming
- **General**: Implementations of standard DP paradigms (LCS, LIS, Edit Distance). Use for classic optimization and alignment problems.
- **Knapsack**: Highly optimized solvers for various knapsack problem variants. Use for resource allocation and constrained optimization.
- **Optimization**: Advanced DP optimizations (e.g., Convex Hull Trick, Knuth Optimization, WQS Binary Search). Use for reducing time complexity from $O(N^2)$ or $O(N^3)$ to $O(N \log N)$ or $O(N)$.

## Data Structures
- **Dsu (Disjoint Set Union)**: Efficiently manage and merge disjoint sets with path compression and union by rank. Use for connectivity queries and Kruskal's MST.
- **Fenwick (Binary Indexed Tree)**: Low-overhead structure for prefix sums and point updates. Use for dynamic frequency counting and cumulative distributions.
- **Grid**: Specialized 2D structures for grid-based operations. Use for image processing, map simulations, and matrix-like data manipulation.
- **Heap**: Diverse priority queue implementations (Binary, Fibonacci, Pairing). Use for Dijkstra's algorithm and real-time task scheduling.
- **Mo**: Implementation of Mo's algorithm for offline range queries. Use for complex range queries where updates are infrequent or nonexistent.
- **Sparse**: Sparse Table for $O(1)$ Range Minimum Queries (RMQ) after $O(N \log N)$ preprocessing. Use for static range queries in time-critical paths.
- **Splay**: Self-adjusting binary search tree. Use for dynamic sets where temporal locality of access is expected.
- **Treap**: Randomized binary search tree with easy split/merge operations. Use for persistent data structures and dynamic sequences.
- **Trie**: Prefix tree for string storage and prefix-based searching. Use for dictionary lookups, autocomplete, and IP routing.
- **LinkCut**: Link-Cut Tree for dynamic tree connectivity and path queries. Use for maintaining forests with edge additions/deletions.
- **PersistentDsu**: DSU implementation that preserves historical states. Use for connectivity queries across different versions of a graph.
- **PersistentTreap**: Treap implementation that preserves historical states. Use for functional programming patterns and undo/redo logic in data management.
- **PieceTable**: Data structure for efficient text editing. Use for large-scale text editors requiring fast insertions and deletions.
- **Rope**: Heavy-weight string representation for large text blocks. Use for efficient manipulation of very long strings where standard arrays fail.
- **UnsafeArray**: Low-level, high-performance unmanaged arrays. Use for maximum cache locality and minimizing GC pressure in performance-critical loops.
- **WaveletMatrix**: Succinct structure for range queries on large alphabets. Use for range quantiles, frequency counting, and string indexing.
- **HilbertOrder**: Mapping 2D/3D coordinates to 1D via Hilbert space-filling curves. Use for improving cache locality in spatial algorithms and Mo's algorithm.

## Game Theory
- **Game Theory**: Algorithms for impartial games, including Nim-sum and Sprague-Grundy theorem. Use for solving combinatorial games and determining winning strategies.

## Geometry
- **Basic**: Foundational primitives for points, lines, and circles. Use as the base for all 2D geometric computations.
- **Advanced**: Complex geometric algorithms, including 3D operations. Use for advanced spatial modeling and simulations.
- **Arrangement**: Algorithms for line and curve arrangements. Use for partitioning the plane into cells for topological analysis.
- **Hull**: Convex hull algorithms (Monotone Chain, Quickhull). Use for bounding volumes, collision detection, and finding the minimum area enclosure.
- **Intersect**: Fast intersection detection between various primitives. Use for collision detection systems and clipping algorithms.
- **Spatial**: Spatial indexing structures (KD-Tree, Quadtree). Use for efficient nearest-neighbor searches and range queries in multi-dimensional space.
- **Voronoi**: Voronoi diagram and Delaunay triangulation construction. Use for mesh generation, proximity analysis, and natural neighbor interpolation.

## Graph
- **Bridges**: Identification of bridges and articulation points. Use for analyzing network vulnerability and biconnected components.
- **Cactus**: Specialized algorithms for cactus graphs. Use for problems involving graphs where every edge belongs to at most one cycle.
- **Clique**: Algorithms for finding maximal and maximum cliques. Use for social network analysis and motif discovery in bioinformatics.
- **Connectivity**: Analysis of strong and weak connectivity in directed and undirected graphs. Use for structural decomposition.
- **Cut**: Minimum cut and related flow-based partitioning. Use for network reliability and image segmentation.
- **DAG**: Specialized algorithms for Directed Acyclic Graphs. Use for topological sorting, shortest paths in DAGs, and dependency resolution.
- **Decomposition**: Structural decomposition (Heavy-Light, Centroid). Use for efficient path and subtree queries in static and dynamic trees.
- **Dominator**: Lengauer-Tarjan algorithm for finding dominators. Use for compiler optimization and control-flow graph analysis.
- **DynamicTrees**: Data structures for trees undergoing structural changes. Use for dynamic network topology management.
- **Flow**: Max-flow and min-cost max-flow (Dinic, Push-Relabel). Use for matching problems, transportation networks, and scheduling.
- **Functional**: Algorithms for functional graphs (each node has exactly one outgoing edge). Use for analyzing discrete mappings and cycle detection in sequences.
- **Matching**: Bipartite and general graph matching algorithms. Use for assignment problems and maximum flow applications.
- **SCC (Strongly Connected Components)**: Tarjan's and Kosaraju's algorithms. Use for condensing directed graphs and solving 2-SAT problems.
- **ShortestPath**: Dijkstra, Bellman-Ford, and Floyd-Warshall. Use for routing, navigation, and distance matrix computation.
- **SpanningTrees**: MST algorithms (Kruskal, Prim). Use for designing minimum-cost connected networks.
- **Tree**: General tree algorithms (Diameter, Center, Isomorphism). Use for hierarchical data analysis and structural comparison.
- **TreeDecomposition**: Computing treewidth and tree decompositions. Use for solving NP-hard graph problems on graphs with bounded treewidth.
- **TreeIsomorphism**: Determining if two trees are structurally identical. Use for pattern matching in chemical structures or abstract syntax trees.
- **TreeQueries**: Optimized answers for LCA, path sums, and subtree queries. Use for heavy-duty tree-based data retrieval.
- **RandomWalk**: Algorithms for simulating and analyzing random walks. Use for PageRank, Markov chains, and graph sampling.

## Linear Algebra
- **Matrix**: High-performance matrix operations (Multiplication, Inversion, SVD, Eigenvalues). Use for solving linear systems, graphics transformations, and machine learning.

## Math
- **Arithmetic**: Specialized high-precision and modular arithmetic. Use for numerical stability in scientific computing.
- **Basic**: Core utilities (GCD, LCM, Exponentiation). Use as foundational blocks for mathematical algorithms.
- **BigInt**: Arbitrary-precision integer arithmetic. Use for cryptography, number theory, and calculations exceeding 64-bit limits.
- **Combinatorics**: Counting, factorials, and binomial coefficients. Use for probability theory and discrete mathematics.
- **Modular**: Arithmetic operations under a prime or composite modulus. Use for competitive programming and cryptographic protocols.
- **NT (Number Theory)**: Primality testing, factorization (Pollard's rho), and sieves. Use for RSA and other cryptographic algorithms.
- **Polynomial**: Mathematical analysis of polynomials (Roots, Evaluation, Interpolation). Use for signal processing and curve fitting.
- **Transform**: Fast Fourier Transform (FFT) and Number Theoretic Transform (NTT). Use for $O(N \log N)$ polynomial multiplication and convolution.

## Optimization
- **Approximation**: Algorithms for near-optimal solutions to NP-hard problems. Use when exact solutions are computationally infeasible.
- **DivideConquer**: Optimization problems using the divide and conquer paradigm. Use for efficient recursive problem solving.
- **Exact**: Exact solvers for hard optimization problems (e.g., TSP, SAT). Use when optimal solutions are strictly required.
- **Games**: Optimization in competitive and strategic environments. Use for minimax search and alpha-beta pruning.
- **Geometric**: Optimization involving geometric constraints (e.g., smallest enclosing circle). Use for logistics and packing problems.
- **Knapsack**: Advanced techniques for knapsack optimization (e.g., branch and bound). Use for complex resource management.
- **Matroid**: Optimization on matroid structures. Use for providing theoretical foundations for greedy algorithms.
- **Offline**: Techniques for processing queries where all data is known upfront. Use for optimizing batch processing tasks.
- **Submodular**: Optimization of submodular set functions. Use for sensor placement and influence maximization in networks.
- **Treewidth**: Optimization using tree decomposition properties. Use for parameterized complexity and solving problems on nearly-tree graphs.

## Permutation
- **Permutation**: Manipulation and analysis of permutations (Rank, Unrank, Cycle Decomposition). Use for group theory and combinatorial search.

## Search
- **Automaton**: Pattern searching using finite state machines. Use for complex pattern recognition and regular expression engines.
- **Bit**: Bit-parallel search techniques. Use for extremely fast set operations and low-level optimization.
- **DifferenceArray**: 1D and 2D difference arrays. Use for $O(1)$ range updates followed by prefix sum recovery.
- **ExactCover**: Knuth's Dancing Links (Algorithm X). Use for Sudoku, tiling, and general exact cover problems.
- **Imos**: The Imos method for multidimensional range updates. Use for processing event-based range additions in $O(N)$.
- **Interval**: Search algorithms for overlapping or contained intervals. Use for scheduling and time-range queries.
- **MeetInMiddle**: Searching two halves of the problem space independently. Use for reducing $2^N$ complexity to $2^{N/2}$.
- **Numerical**: Binary search, ternary search, and Newton's method. Use for finding optima and roots in continuous or discrete functions.
- **Prefix**: Prefix-based searching and matching. Use for string analysis and stream processing.
- **Range**: General multi-dimensional range searching. Use for database indexing and geographic information systems.
- **Selection**: Quickselect and Median-of-Medians. Use for finding the k-th smallest element in $O(N)$.
- **Specialized**: Domain-specific search optimizations. Use for highly targeted performance tuning.
- **Subset**: Searching through subset spaces. Use for solving subset sum and related combinatorial problems.
- **Suffix**: Suffix-based search algorithms. Use for efficient substring lookups.
- **TwoPointer**: Linear-time searching using two indices. Use for range-based constraints on sorted data.
- **Window**: Fixed and variable-sized sliding window algorithms. Use for stream analysis and local feature extraction.

## Sorting
- **Insertion**: Simple $O(N^2)$ sort. Use for small arrays or as a base case for hybrid sorts like Timsort.
- **Merge**: Stable $O(N \log N)$ divide-and-conquer sort. Use when element order stability is required.
- **Partition**: Partitioning logic used in Quicksort and Quickselect. Use for in-place data reorganization.
- **Specialized**: Radix sort, Counting sort, and Bitonic sort. Use for specific data types or parallel hardware architectures.

## String
- **Automata**: Aho-Corasick and Suffix Automaton. Use for simultaneous multi-pattern matching and substring analysis.
- **Compress**: String-specific compression (Lempel-Ziv, Burrows-Wheeler). Use for efficient text storage.
- **FMIndex**: Compressed full-text index based on the BWT. Use for memory-efficient substring searching in large genomic or text databases.
- **Grammar**: Grammar-based string analysis and compression. Use for structural text processing and pattern discovery.
- **Match**: KMP, Boyer-Moore, and Rabin-Karp. Use for fast single-pattern string searching.
- **Palindrome**: Manacher's algorithm and Palindromic Tree. Use for finding all palindromic substrings in linear time.
- **Parse**: Expression parsers and lexical analyzers. Use for building compilers, interpreters, and data format extractors.
- **Pattern**: Regular expression and wildcard matching. Use for flexible text search and validation.
- **SuffixArray**: Construction of suffix arrays and LCP arrays. Use for substring searching, longest common prefix, and string topology.
- **SuffixAutomaton**: The smallest DFA recognizing all suffixes of a string. Use for linear-time solutions to complex string problems.
