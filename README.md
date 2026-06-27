# IAFahim.CS

A high-performance, unmanaged C# library providing a comprehensive suite of algorithms and data structures optimized for competitive programming, scientific computing, computer graphics, and performance-critical systems. The entire codebase is designed to run seamlessly on both pure .NET and Unity environments with zero overhead and zero garbage collection pressure.

## Architecture Guidelines

All packages in this repository strictly adhere to the following architecture rules:
- **Zero GC Allocations**: No garbage-collected heap operations are allowed in the core libraries (`src/`). Memory allocation uses unmanaged stack alloc (`stackalloc`) or unmanaged allocators.
- **Dual Target Support**: Compiles under pure .NET (using stubs from `IAFahim.Collections.NoDeps` and `UnityMathematics.NoDeps` during build) and links directly to the real `com.unity.collections` and `com.unity.mathematics` when imported in Unity projects.
- **Raw Pointer API**: Algorithms take raw pointers (`T* ptr, int len`) and operate directly on unmanaged memory. Data structures own their memory and implement `IDisposable` to allow explicit disposal.
- **No Magic Numbers**: Constants must be explicitly named. No implicit casting or automatic conversions allowed.
- **Low Cyclomatic Complexity**: All leaf functions are kept small and focused, and decorated with `[MethodImpl(MethodImplOptions.AggressiveInlining)]` to preserve zero-overhead performance.
- **Totality Patterns**: Public APIs expose unchecked fast paths (caller guarantees index safety) and checked variants prefixed with `Try*` returning a boolean status and returning results via `out` parameters.

---

## Package Index

Below is the complete index of all active packages in the repository, organized by algorithm/data structure family. Each link points to the package's folder containing detailed complexity specifications, API signatures, and unsafe usage examples.

### Algebra

- **[IAFahim.Algebra.GraphPoly](./src/IAFahim.Algebra.GraphPoly/README.md)**: This package provides functions to evaluate graph polynomials. It supports Tutte polynomials, independence polynomials, matching polynomials, reliability polynomials, rook polynomials, and chromatic polynomials. All calculations use unsafe raw pointers to achieve maximum efficiency without managed overhead.
- **[IAFahim.Algebra.Polynomial](./src/IAFahim.Algebra.Polynomial/README.md)**: This package provides algorithms for univariate polynomial operations over finite fields. It includes division, greatest common divisor, multipoint evaluation, interpolation, roots searching, and factorization. It also implements polynomial product computation using Number Theoretic Transform, Schonhage-Strassen, and Toom-Cook algorithms. All methods run on raw pointers for maximum performance.
- **[IAFahim.Algebra.Sequence](./src/IAFahim.Algebra.Sequence/README.md)**: This package provides methods to generate, rank, and transform combinatorial sequences and values. It supports Prufer sequence transformations, binomial transforms, Stirling numbers of the first and second kind, Bell numbers, Eulerian numbers, Narayana numbers, and Lah numbers. It also supports generating function operations such as exponential and ordinary generating function products.

### Collision Detection

- **[IAFahim.Collision.Gjk](./src/IAFahim.Collision.Gjk/README.md)**: This package implements the Gilbert-Johnson-Keerthi (GJK) collision detection algorithm and the Expanding Polytope Algorithm (EPA) for three-dimensional physics queries. It computes overlap and minimum distance between convex shapes defined by support functions. Shape support functions include sphere, box, capsule, and convex hull.

### Combinatorics

- **[IAFahim.Combinatorics.Generation](./src/IAFahim.Combinatorics.Generation/README.md)**: This package provides enumerators and generators for combinatorial objects. It supports set partitions, permutations, combinations, necklaces, bracelets, and random graph structures. It also includes methods to rank and unrank these objects to convert them to and from integers.

### Compression

- **[IAFahim.Compress](./src/IAFahim.Compress/README.md)**: This package provides algorithms for compressing and restoring integer arrays. It transforms regular raw values into a compressed representation and provides tools to restore the original values. It helps minimize memory footprint when storing large lists of numbers.
- **[IAFahim.Compress.Coordinate](./src/IAFahim.Compress.Coordinate/README.md)**: This package provides coordinate discretization and rank compression for coordinates. It transforms an array of numbers into their relative sorted rank offsets, reducing the range of values to [0, U-1] where U is the count of unique values. This is useful for data structures that require small coordinate ranges.

### Data Structures

- **[IAFahim.DS.Dsu](./src/IAFahim.DS.Dsu/README.md)**: This package provides a Disjoint Set Union (DSU) implementation. It supports path compression, union by size, rollback operations, bipartite graph checks with parity, and small-to-large merging.
- **[IAFahim.DS.Fenwick](./src/IAFahim.DS.Fenwick/README.md)**: This package provides a Fenwick Tree (Binary Indexed Tree) implementation. It supports point updates, range sum queries in both one and two dimensions, range updates with point queries, and persistent variants.
- **[IAFahim.DS.FixedCollections](./src/IAFahim.DS.FixedCollections/README.md)**: This package provides fixed-size and unmanaged collection types that do not depend on garbage collection. It includes spin locks, fixed-size bitmasks, fixed-size hash maps, thread-local collections, thread-safe random number helpers, fast counters, and unmanaged object pools.
- **[IAFahim.DS.GapBuffer](./src/IAFahim.DS.GapBuffer/README.md)**: This package provides a gap buffer structure for efficient text editing operations. It keeps an empty gap at the current edit position, enabling fast insertion and deletion at that cursor offset. It avoids copying the entire buffer on consecutive edits.
- **[IAFahim.DS.Grid](./src/IAFahim.DS.Grid/README.md)**: This package provides helper functions for manipulating two-dimensional grids stored in flat arrays. It supports grid generation, rotation, reversal, cell shuffling, neighbor collection (4-way and 8-way), breadth-first search pathfinding, and fast cell filling.
- **[IAFahim.DS.Heap](./src/IAFahim.DS.Heap/README.md)**: This package provides priority queue and deque operations on raw buffers. It includes binary heap insertion, deletion, and heapify helpers, deque push and pop for double-ended queues, monotonic queue minimum queries, and monotonic stack processing.
- **[IAFahim.DS.HilbertOrder](./src/IAFahim.DS.HilbertOrder/README.md)**: This package provides algorithms to encode multi-dimensional coordinates into one-dimensional order values. It features the Hilbert space-filling curve, Gilbert curve for arbitrary grid sizes, and block-based query ordering for offline query sorting algorithms.
- **[IAFahim.DS.LinkCut](./src/IAFahim.DS.LinkCut/README.md)**: This package implements a Link-Cut Tree data structure. It represents a forest of trees and supports tree structural changes (linking and cutting paths) and path query operations. It is designed using splay trees and raw node pointers.
- **[IAFahim.DS.Mo](./src/IAFahim.DS.Mo/README.md)**: Mo algorithm for offline query processing. It sorts queries using block decomposition to minimize pointer movement.
- **[IAFahim.DS.OrderedSet](./src/IAFahim.DS.OrderedSet/README.md)**: An ordered set implementation built on a sorted pointer sequence. Supports insertions, deletions, rank checks, and index queries.
- **[IAFahim.DS.PerfectHashMap](./src/IAFahim.DS.PerfectHashMap/README.md)**: A perfect hash map structure. Resolves key queries in O(1) time.
- **[IAFahim.DS.PersistentDsu](./src/IAFahim.DS.PersistentDsu/README.md)**: A persistent disjoint set union structure implemented using a persistent segment tree. It allows querying set membership and merging sets at any historical version.
- **[IAFahim.DS.PersistentTreap](./src/IAFahim.DS.PersistentTreap/README.md)**: A persistent treap (randomized binary search tree) implementation. Supports split, merge, insert, erase, and find operations while preserving previous versions by copying nodes on updates.
- **[IAFahim.DS.PieceTable](./src/IAFahim.DS.PieceTable/README.md)**: A piece table data structure designed for text editing. It tracks changes using an original buffer, an append buffer, and a sequence of pieces pointing to segments of either buffer.
- **[IAFahim.DS.RollbackSeg](./src/IAFahim.DS.RollbackSeg/README.md)**: A segment tree implementation supporting rollback operations to restore previous states, along with dynamic Li Chao trees and divide and conquer optimization utilities.
- **[IAFahim.DS.RollbackStack](./src/IAFahim.DS.RollbackStack/README.md)**: A collection of undoable data structures. Includes rollback stacks, undoable union find (DSU), undoable bipartite DSU, and undoable binary heaps to support reverting updates.
- **[IAFahim.DS.Rope](./src/IAFahim.DS.Rope/README.md)**: A rope data structure for managing long strings. It represents a string as a binary tree of nodes, allowing insertions, deletions, and substring operations on large texts.
- **[IAFahim.DS.SegmentTree](./src/IAFahim.DS.SegmentTree/README.md)**: A library of segment tree structures. Includes standard segment trees, lazy propagation segment trees, persistent segment trees (Chairman tree), merge sort trees, mergeable segment trees, and Li Chao trees.
- **[IAFahim.DS.Sparse](./src/IAFahim.DS.Sparse/README.md)**: A library for range query structures including sparse tables, disjoint sparse tables, and square root decomposition. Primarily useful for range minimum query (RMQ) operations.
- **[IAFahim.DS.SpatialMap](./src/IAFahim.DS.SpatialMap/README.md)**: A collection of spatial hashing maps for multidimensional grid hashing. Includes 2D spatial maps, 3D spatial maps, hexagonal spatial maps, and local spatial maps to hash positions to grids.
- **[IAFahim.DS.Splay](./src/IAFahim.DS.Splay/README.md)**: A splay tree implementation. This self-balancing binary search tree structure moves recently accessed nodes closer to the root. Supports range queries and range reversals.
- **[IAFahim.DS.Treap](./src/IAFahim.DS.Treap/README.md)**: A randomized binary search tree (treap) implementation. Supports implicit index queries, range sum updates, range minimum queries, range reversals, range rotations, and affine transformations.
- **[IAFahim.DS.Trie](./src/IAFahim.DS.Trie/README.md)**: A trie structure supporting byte sequences, binary values, and persistent versions. Useful for prefix matching, word tracking, and bitwise XOR query operations.
- **[IAFahim.DS.UnsafeArray](./src/IAFahim.DS.UnsafeArray/README.md)**: An unmanaged array wrapper that provisions raw memory using a specified memory manager. Implements disposal to prevent memory leaks.
- **[IAFahim.DS.WaveletMatrix](./src/IAFahim.DS.WaveletMatrix/README.md)**: A wavelet matrix data structure for succinct representation of sequences. Supports retrieving the kth smallest element in a range, quantile queries, and rank/select operations.

### Dynamic Programming

- **[IAFahim.DP](./src/IAFahim.DP/README.md)**: This package provides a collection of dynamic programming algorithms and optimizations. It includes multiple knapsack variants, subset sum solvers, divide and conquer optimization, Knuth optimization, Convex Hull Trick, Li Chao Tree, SMAWK algorithm, Alien DP, and Sum over Subsets (SOS) DP.
- **[IAFahim.DP.General](./src/IAFahim.DP.General/README.md)**: This package provides general dynamic programming routines. It implements profile DP, broken profile DP, tree knapsack, interval DP, min-plus convolution, and quadrangle inequality DP optimizations. It operates using raw pointers for speed.
- **[IAFahim.DP.Knapsack](./src/IAFahim.DP.Knapsack/README.md)**: This package provides dynamic programming algorithms specifically for knapsack optimization. It features implementations for the 0-1 knapsack, unbounded knapsack, and bounded knapsack models, along with subset sum and bitset-accelerated subset sum solvers.
- **[IAFahim.DP.Optimization](./src/IAFahim.DP.Optimization/README.md)**: This package provides dynamic programming optimizations. It features Knuth optimization for reducing complexity on interval DP, and a Li Chao tree line-insertion optimization for linear function queries.

### Game Theory

- **[IAFahim.GameTheory](./src/IAFahim.GameTheory/README.md)**: A collection of game theory algorithms. Includes Grundy value derivation on directed graphs, Nim sum solvers, minimax search with alpha-beta pruning, and game dynamic programming utilities.

### Geometry

- **[IAFahim.Geometry.Advanced](./src/IAFahim.Geometry.Advanced/README.md)**: A collection of advanced geometric algorithms. Supports convex hull diameter using rotating calipers, closest pair of points, Minkowski sum, circumcenter, minimum enclosing circle, Pick's theorem, and polygon boolean operations.
- **[IAFahim.Geometry.Arrangement](./src/IAFahim.Geometry.Arrangement/README.md)**: This package provides algorithms for subdivision arrangement analysis. It constructs partitions, builds query grids, computes vertical decomposition, builds trapezoidal maps, and solves polygon union and intersection.
- **[IAFahim.Geometry.Azimuth](./src/IAFahim.Geometry.Azimuth/README.md)**: This package provides methods for azimuth solving. It supports spherical azimuth, spherical distance on a sphere, and planar 2D azimuth.
- **[IAFahim.Geometry.Basic](./src/IAFahim.Geometry.Basic/README.md)**: This package provides basic geometry operations. It includes point arithmetic, dot products, cross products, point rotation, orientation tests, segment intersection checks, projection and reflection, distance formulas, polygon area, centroid solving, and inclusion checks.
- **[IAFahim.Geometry.Bvh](./src/IAFahim.Geometry.Bvh/README.md)**: This package provides a bounding volume hierarchy tree for 3D meshes. It enables efficient ray query operations and spatial partitioning for collision tests.
- **[IAFahim.Geometry.Curve](./src/IAFahim.Geometry.Curve/README.md)**: This package provides curve evaluation algorithms. It includes cubic Bezier curve evaluation, tangent evaluation, arc length integration, and uniform sampling along a path.
- **[IAFahim.Geometry.Frame](./src/IAFahim.Geometry.Frame/README.md)**: This package provides methods for frame generation along a curve. It utilizes parallel transport to construct consistent orthogonal frames without twist.
- **[IAFahim.Geometry.Hull](./src/IAFahim.Geometry.Hull/README.md)**: This package provides geometric hull and partition algorithms. It includes Minkowski sum solving, straight skeleton construction, convex hull trick with rollback history, half-space intersection, rotating calipers for bounding boxes, and 3D convex hull generation.
- **[IAFahim.Geometry.Intersect](./src/IAFahim.Geometry.Intersect/README.md)**: This package provides methods for geometric intersection solving. It computes polyhedron volume, line-sphere intersection, sphere-sphere intersection, point-plane distances, line-plane intersection, segment-plane intersection, and plane-plane intersections.
- **[IAFahim.Geometry.Mesh](./src/IAFahim.Geometry.Mesh/README.md)**: This package provides algorithms for mesh updates. It supports vertex deformation and normal recomputing.
- **[IAFahim.Geometry.Spatial](./src/IAFahim.Geometry.Spatial/README.md)**: This package provides spatial query data structures. It includes cover trees, kd-trees, quadtrees, range trees, segment trees, octrees, ball trees, 3D binary indexed trees, and methods for Euclidean, Manhattan, and rectilinear minimum spanning trees.
- **[IAFahim.Geometry.Triangulation](./src/IAFahim.Geometry.Triangulation/README.md)**: This package provides methods for polygon triangulation. It implements ear clipping to decompose simple polygons into triangles.
- **[IAFahim.Geometry.Voronoi](./src/IAFahim.Geometry.Voronoi/README.md)**: This package provides Voronoi diagrams and related spatial graph algorithms. It includes Delaunay triangulation, Fortune's sweep-line solver, visibility graph construction, nearest neighbor search on KD-trees, and shortest path solving.

### Graph Algorithms

- **[IAFahim.Graph](./src/IAFahim.Graph/README.md)**: This package provides core graph algorithms. It includes adjacency builders, minimum cut solvers, Eulerian path detection, 2-SAT solvers, minimum spanning tree variants, bipartite matching, shortest path routines, graph traversals, tournament analysis, topological sorting, and planar graph utilities.
- **[IAFahim.Graph.Bridges](./src/IAFahim.Graph.Bridges/README.md)**: This package provides methods for identifying bridges and cut vertices in graphs. It supports static search, incremental dynamic bridge maintenance, and biconnectivity augmentation solving.
- **[IAFahim.Graph.Cactus](./src/IAFahim.Graph.Cactus/README.md)**: This package provides algorithms for graphs where any two simple cycles share at most one vertex. It includes cycle decomposition, shortest path queries, bridge tree diameter solving, and lowest common ancestor query support.
- **[IAFahim.Graph.Centroid](./src/IAFahim.Graph.Centroid/README.md)**: This package provides centroid decomposition for tree structures. It enables divide-and-conquer algorithms on trees by finding tree centroids and building centroid trees.
- **[IAFahim.Graph.Clique](./src/IAFahim.Graph.Clique/README.md)**: This package provides algorithms for finding fully connected subgraphs in a graph. It solves the clique search problem by identifying subsets of vertices that are mutually adjacent.
- **[IAFahim.Graph.Connectivity](./src/IAFahim.Graph.Connectivity/README.md)**: This package provides methods for dynamic graph connectivity. It supports incremental union-find, decremental connectivity, offline dynamic connectivity, dynamic transitive closure, and fully dynamic connectivity.
- **[IAFahim.Graph.Cut](./src/IAFahim.Graph.Cut/README.md)**: This package provides algorithms for graph cuts and flow networks. It solves the minimum cut problem, identifying subsets of edges that partition the graph.
- **[IAFahim.Graph.DAG](./src/IAFahim.Graph.DAG/README.md)**: This package provides algorithms for directed acyclic graphs. It supports topological sorting, path counts, longest antichain search, minimum path covers, and cycle checks.
- **[IAFahim.Graph.Decomposition](./src/IAFahim.Graph.Decomposition/README.md)**: This package provides graph decomposition methods to split graphs into sub-components.
- **[IAFahim.Graph.Dominator](./src/IAFahim.Graph.Dominator/README.md)**: This package provides dominator tree construction algorithms for directed graphs.
- **[IAFahim.Graph.DynamicTrees](./src/IAFahim.Graph.DynamicTrees/README.md)**: This package provides dynamic tree structures, including Top Trees, Link-Cut Trees, and Euler Tour Trees, supporting dynamic path queries and tree updates.
- **[IAFahim.Graph.Eertree](./src/IAFahim.Graph.Eertree/README.md)**: This package provides the Eertree structure for indexing all distinct palindromic substrings in a sequence.
- **[IAFahim.Graph.Eulerian](./src/IAFahim.Graph.Eulerian/README.md)**: This package provides algorithms to search for Eulerian paths and Eulerian cycles in a graph.
- **[IAFahim.Graph.Flow](./src/IAFahim.Graph.Flow/README.md)**: This package provides flow network routines, including maximum flow, minimum cut, minimum cost maximum flow, and vertex-limited flows.
- **[IAFahim.Graph.Functional](./src/IAFahim.Graph.Functional/README.md)**: This package provides algorithms for functional graphs, where every node has exactly one outgoing edge. It includes path queries, cycle detection, and meeting points.
- **[IAFahim.Graph.Matching](./src/IAFahim.Graph.Matching/README.md)**: This package provides matching routines for graphs, supporting stable marriage, stable roommates, bipartite matching, and Hungarian methods.
- **[IAFahim.Graph.Misc](./src/IAFahim.Graph.Misc/README.md)**: This package provides miscellaneous graph utility algorithms, including topological dynamic programming and node access closure checks.
- **[IAFahim.Graph.RandomWalk](./src/IAFahim.Graph.RandomWalk/README.md)**: This package provides random walk routines for graph path simulations.
- **[IAFahim.Graph.SCC](./src/IAFahim.Graph.SCC/README.md)**: This package provides algorithms for finding strongly connected components in a directed graph, including Tarjan's algorithm and online SCC maintenance.
- **[IAFahim.Graph.ShortestPath](./src/IAFahim.Graph.ShortestPath/README.md)**: This package provides shortest path algorithms, including Eppstein's K-shortest paths and dynamic edge updates.
- **[IAFahim.Graph.SpanningTrees](./src/IAFahim.Graph.SpanningTrees/README.md)**: This package provides algorithms for spanning trees and cuts, including transitive closure construction.
- **[IAFahim.Graph.Tree](./src/IAFahim.Graph.Tree/README.md)**: This package provides basic and advanced tree algorithms, including Lowest Common Ancestor queries and Heavy-Light Decomposition.
- **[IAFahim.Graph.TreeDecomposition](./src/IAFahim.Graph.TreeDecomposition/README.md)**: This package provides dynamic programming algorithms on nice tree decompositions, pathwidth decompositions, and tree Mo algorithms.
- **[IAFahim.Graph.TreeIsomorphism](./src/IAFahim.Graph.TreeIsomorphism/README.md)**: This package provides algorithms for tree isomorphism detection, including rooted and unrooted canonical tree hashes.
- **[IAFahim.Graph.TreeQueries](./src/IAFahim.Graph.TreeQueries/README.md)**: This package provides tree query algorithms, including tree centroids, path color counting, Steiner trees, and tree hashing.

### Linear Algebra

- **[IAFahim.Linear](./src/IAFahim.Linear/README.md)**: This package provides high-performance, unmanaged linear algebra utilities for vector mathematics and small-dimensional linear algebra computations.
- **[IAFahim.Linear.Matrix](./src/IAFahim.Linear.Matrix/README.md)**: This package provides matrix operations, including matrix products, matrix exponentiation, and Berlekamp-Massey recurrence solvers.
- **[IAFahim.Linear.Matrix2](./src/IAFahim.Linear.Matrix2/README.md)**: Provides basic 2D matrix operations using raw long pointers, including initialization, identity matrix, addition, subtraction, matrix exponentiation, and matrix-vector product solver.

### Mathematics

- **[IAFahim.Math.Arithmetic](./src/IAFahim.Math.Arithmetic/README.md)**: Provides checked arithmetic operations for 32-bit and 64-bit signed integers. These functions return a boolean value showing if the operation succeeded without overflow, and output the result.
- **[IAFahim.Math.Barycentric](./src/IAFahim.Math.Barycentric/README.md)**: Offers utilities for barycentric weights on triangles in 2D and 3D space. Includes weight solving, interpolation of vector and scalar values, inside-triangle testing, projection of points, and signed area.
- **[IAFahim.Math.Basic](./src/IAFahim.Math.Basic/README.md)**: Offers basic integer math utilities, including absolute values, minimum or maximum queries, rounding divisions, modulo normalization, swap functions, fast exponentiation, roots, power-of-two queries, log2 queries, and pointer-based value update helper functions.
- **[IAFahim.Math.BigInt](./src/IAFahim.Math.BigInt/README.md)**: Implements arbitrary-precision integer arithmetic using raw integer arrays. Operations include addition, subtraction, finding products, exponentiation, division by a single-digit integer, and modulo operations.
- **[IAFahim.Math.Combinatorics](./src/IAFahim.Math.Combinatorics/README.md)**: Offers functions for discrete counting and prime numbers. Includes stirling numbers, bell numbers, partition numbers, derangements, stars and bars, factorial and modular inverse factorial tables, binomial coefficient solving, linear congruences, and prime sieve utilities (segmented and linear).
- **[IAFahim.Math.Gauss](./src/IAFahim.Math.Gauss/README.md)**: Provides Gaussian elimination solver for linear equation systems over real numbers (double) and modular arithmetic (mod P). Also computes the determinant of a square matrix mod P.
- **[IAFahim.Math.Kalman](./src/IAFahim.Math.Kalman/README.md)**: Implements 1D scalar and 3D vector Kalman filtering for noise reduction and state estimation. Provides prediction and update steps, as well as utility functions to filter a series of input measurements.
- **[IAFahim.Math.Modular](./src/IAFahim.Math.Modular/README.md)**: Implements common modular arithmetic operations and number theory functions. Includes greatest common divisor (GCD), least common multiple (LCM), modular addition, modular subtraction, modular product, modular division, modular exponentiation, modular inverse, modular square root, Chinese Remainder Theorem (CRT), and Extended Chinese Remainder Theorem (EXCRT).
- **[IAFahim.Math.NT](./src/IAFahim.Math.NT/README.md)**: Implements comprehensive number theory algorithms. Includes primality testing via Miller-Rabin, integer factoring via Pollard's rho, sieve helpers (Euler totient, Mobius function, divisor sum and count tables), arithmetic function prefix sums, discrete log solvers, Legendre/Jacobi symbols, continued fractions, Stern-Brocot tree, division transforms, and bitwise utilities.
- **[IAFahim.Math.Noise](./src/IAFahim.Math.Noise/README.md)**: Provides 2D Perlin and Simplex noise algorithms. These are useful for procedural content generation, terrain generation, and visual effects.
- **[IAFahim.Math.PoissonDisk](./src/IAFahim.Math.PoissonDisk/README.md)**: Implements 2D and 3D Poisson disk sampling algorithms to generate blue noise distributions. Useful for random object placement, sampling patterns, and graphics.
- **[IAFahim.Math.Polynomial](./src/IAFahim.Math.Polynomial/README.md)**: Implements comprehensive operations on polynomials. Includes addition, subtraction, finding products, quotient and remainder division, derivative, integral, inverse, logarithm, exponent, power, square root, multipoint evaluation, Lagrange interpolation, Taylor shift, composition, and shift operations.
- **[IAFahim.Math.Polynomial.Eval](./src/IAFahim.Math.Polynomial.Eval/README.md)**: Provides advanced polynomial evaluation techniques. Includes multi-point evaluation of a polynomial at multiple points, and the Chirp Z-Transform (CZT) for evaluating a polynomial at points in a geometric progression.
- **[IAFahim.Math.Polynomial.Fps](./src/IAFahim.Math.Polynomial.Fps/README.md)**: Implements formal power series (FPS) operations modulo a prime. Includes computing the formal power series inverse, square root, natural logarithm, exponential, and arbitrary integer power of a formal power series.
- **[IAFahim.Math.PotentialField](./src/IAFahim.Math.PotentialField/README.md)**: Implements 2D and 3D potential field steering forces for path planning. Includes attractive forces towards targets, repulsive forces away from obstacles, tangential forces (2D only) to bypass obstacles, gradient evaluations, and simple pathfinding using gradient descent.
- **[IAFahim.Math.Quaternion](./src/IAFahim.Math.Quaternion/README.md)**: Offers mathematical operations for quaternions. Includes spherical linear interpolation (SLERP), conversions between quaternions and Euler angles or axis-angle representations, look rotation solvers, vector rotation, negating vector parts, normalization, and swing-twist decomposition.
- **[IAFahim.Math.Sdf](./src/IAFahim.Math.Sdf/README.md)**: Implements signed distance function (SDF) utilities for 3D computer graphics. Includes primitive shape evaluations, constructive solid geometry (CSG) boolean operations, space transforms, raymarching solvers, normal estimation, and ambient occlusion.
- **[IAFahim.Math.SphericalHarmonics](./src/IAFahim.Math.SphericalHarmonics/README.md)**: Implements Spherical Harmonics projection and evaluation up to band 2 (9 coefficients). Provides functions for basis function evaluation, projection of directional samples, irradiance convolution, and reconstruction.
- **[IAFahim.Math.Spline](./src/IAFahim.Math.Spline/README.md)**: This package provides functions to evaluate Cubic Hermite and Uniform B-Spline curves. It supports evaluation of positions, tangents, and numerical integration of spline arc lengths.
- **[IAFahim.Math.Transform](./src/IAFahim.Math.Transform/README.md)**: This package implements discrete transforms on algebraic structures, subset convolutions, fast Walsh-Hadamard transforms (bitwise OR, AND, XOR), poset zeta and Mobius transforms, partition-based convolutions, XOR vector space bases operations, and tropical min-plus/max-plus convolutions.
- **[IAFahim.Math.Transform.AnyMod](./src/IAFahim.Math.Transform.AnyMod/README.md)**: This package performs convolution modulo any integer (not necessarily prime or power of two) using double-precision arithmetic.
- **[IAFahim.Math.Transform.Fft](./src/IAFahim.Math.Transform.Fft/README.md)**: This package implements the Fast Fourier Transform (FFT) and its inverse on complex numbers using double arrays. It supports fast polynomial convolution.
- **[IAFahim.Math.Transform.Ntt](./src/IAFahim.Math.Transform.Ntt/README.md)**: This package implements the Number Theoretic Transform (NTT) for integer convolution modulo a prime. It supports fast number-theoretic forward and inverse transforms.

### Memory Management

- **[IAFahim.Collections.NoDeps](./src/IAFahim.Collections.NoDeps/README.md)**: This package provides minimal stub definitions and compile-time mocks for Unity's collections, job system, and math types to support pure .NET builds.
- **[IAFahim.Memory.Allocators](./src/IAFahim.Memory.Allocators/README.md)**: This package offers structures to manage memory blocks, including slab pools, fixed-size pools, parallel pools, and general memory managers.

### Optimization

- **[IAFahim.Optimization.Approximation](./src/IAFahim.Optimization.Approximation/README.md)**: This package implements metaheuristic search methods (simulated annealing, hill climbing, Monte Carlo), Freivalds probabilistic checking of matrix products, and randomized polynomial identity testing.
- **[IAFahim.Optimization.DivideConquer](./src/IAFahim.Optimization.DivideConquer/README.md)**: This package provides optimization algorithms that use divide and conquer paradigms. It includes Slope Trick for tracking piecewise linear convex functions, Lagrangian relaxation for search, matrix search (including sorted column search), online dynamic programming optimization, and double-ended queue optimization.
- **[IAFahim.Optimization.Exact](./src/IAFahim.Optimization.Exact/README.md)**: This package provides exact solvers for NP-hard problems, including Maximum Independent Set, Minimum Set Cover, Maximum Clique, Hamiltonian Path, Hamiltonian Cycle, Traveling Salesperson Problem (using Held-Karp, bitonic, and meet-in-the-middle methods), Minimum Dominating Set, Graph Coloring, and the Steiner Tree problem using the Dreyfus-Wagner algorithm.
- **[IAFahim.Optimization.Games](./src/IAFahim.Optimization.Games/README.md)**: This package provides game theory and decision process solvers. It includes finding attractor sets for infinite games, minimum cost flow (flow loops, arborescence, mean cycle), Grundy values for impartial games, the Simplex algorithm for linear programming, Markov Decision Processes value and policy iterations, retrograde analysis for game solving, and mean payoff game solvers.
- **[IAFahim.Optimization.Geometric](./src/IAFahim.Optimization.Geometric/README.md)**: This package contains geometric solvers. It includes Welzl's algorithm for finding the minimum enclosing sphere and minimum enclosing ball in multiple dimensions using randomized techniques.
- **[IAFahim.Optimization.Knapsack](./src/IAFahim.Optimization.Knapsack/README.md)**: This package implements various knapsack optimization algorithms. It includes divide-and-conquer knapsack solvers, multiple choice knapsack solvers, bounded knapsack solvers using binary split or monotone queue optimization, meet-in-the-middle knapsack solvers, K-Sum solvers, and Subset Sum solvers.
- **[IAFahim.Optimization.Matroid](./src/IAFahim.Optimization.Matroid/README.md)**: This package provides matroid-based optimization algorithms. It includes greedy solvers for independent sets on matroids and rank determination for linear matroids.
- **[IAFahim.Optimization.Offline](./src/IAFahim.Optimization.Offline/README.md)**: This package implements offline optimization techniques. It includes parallel binary search, divide-and-conquer query answering, CDQ divide-and-conquer for three-dimensional dominance, and offline K-th number queries using persistent segment trees.
- **[IAFahim.Optimization.Submodular](./src/IAFahim.Optimization.Submodular/README.md)**: This package provides algorithms for submodular optimization. It includes Max-Cut solvers (using local search and the Goemans-Williamson semidefinite programming approximation), submodular greedy solvers, greedy set cover solvers, and rounding methods (random rounding, dependent rounding, pipage rounding).
- **[IAFahim.Optimization.Treewidth](./src/IAFahim.Optimization.Treewidth/README.md)**: This package provides algorithms for treewidth-based dynamic programming optimization. It includes Cut and Count for graph problems on tree decompositions, Convex Hull checks for Monge properties, treewidth rank dynamic programming, fast subset dynamic programming, and rank transformations.

### Pathfinding

- **[IAFahim.Pathfinding.Recast](./src/IAFahim.Pathfinding.Recast/README.md)**: This package provides a navigation mesh building and path query system. It includes spatial heightfield generation, heightfield filtering, walkable area erosion, region building, polygon mesh generation, and path queries on generated navigation meshes.

### Permutation

- **[IAFahim.Permutation](./src/IAFahim.Permutation/README.md)**: This package offers utility functions for permutation operations. It includes validation, inversion, composition, power solving, cycle decomposition, ranking, unranking, next and prior permutation generation, Gray code generation, and cross product generation.

### Physics

- **[IAFahim.Physics.Xpbd](./src/IAFahim.Physics.Xpbd/README.md)**: This package implements the Extended Position-Based Dynamics (XPBD) simulation system. It provides static methods for integrating positions and velocities, applying damping, and solving distance, volume, bending, and shape matching bonds.

### Search Algorithms

- **[IAFahim.Search](./src/IAFahim.Search/README.md)**: This package provides a collection of general-purpose search algorithms and state-space exploration helpers.
- **[IAFahim.Search.Automaton](./src/IAFahim.Search.Automaton/README.md)**: This package provides algorithms for automaton construction and modulo power operations on matrices. It allows building state transition graphs and exponentiating transition representations.
- **[IAFahim.Search.Bit](./src/IAFahim.Search.Bit/README.md)**: This package provides bitwise operations on arrays of bits, including logical operations, shifting, and search algorithms like longest increasing subsequence lengths.
- **[IAFahim.Search.DifferenceArray](./src/IAFahim.Search.DifferenceArray/README.md)**: This package provides a difference buffer structure to support range additions and value updates on linear memory buffers.
- **[IAFahim.Search.ExactCover](./src/IAFahim.Search.ExactCover/README.md)**: This package solves exact cover problems using dancing links and back-tracking, including grid placement games and queen puzzle counts.
- **[IAFahim.Search.Imos](./src/IAFahim.Search.Imos/README.md)**: This package implements multi-dimensional prefix sums and range update algorithms on grids and linear buffers, and solves grid bounding rectangle problems.
- **[IAFahim.Search.Interval](./src/IAFahim.Search.Interval/README.md)**: This package contains methods to merge, intersect, and normalize sets of intervals, and search for interval overlaps.
- **[IAFahim.Search.LIS](./src/IAFahim.Search.LIS/README.md)**: This package computes the length and elements of the longest increasing subsequence in an array of values.
- **[IAFahim.Search.MeetInMiddle](./src/IAFahim.Search.MeetInMiddle/README.md)**: This package implements search algorithms using the meet-in-the-middle technique, splitting search sets to solve subset sum problems.
- **[IAFahim.Search.Numerical](./src/IAFahim.Search.Numerical/README.md)**: This package provides numerical search, optimization, and integration methods, including simulated annealing, ternary real search, and adaptive integration.
- **[IAFahim.Search.Prefix](./src/IAFahim.Search.Prefix/README.md)**: This package provides prefix sum, prefix min, prefix max, and prefix XOR algorithms, along with string pattern searching.
- **[IAFahim.Search.Range](./src/IAFahim.Search.Range/README.md)**: This package provides range sum, range minimum, range maximum, and range minimum excluded value query structures like sparse tables.
- **[IAFahim.Search.RangeQueries](./src/IAFahim.Search.RangeQueries/README.md)**: This package contains advanced range query algorithms, segment trees with lazy propagation, offline queries, and majority query mechanisms.
- **[IAFahim.Search.Selection](./src/IAFahim.Search.Selection/README.md)**: This package provides selection algorithms, including quick-select for finding the K-th smallest element and maintaining rolling medians.
- **[IAFahim.Search.Specialized](./src/IAFahim.Search.Specialized/README.md)**: This package implements specialized search algorithms, including binary search bounds, ternary search, scheduling generators, and stress testing utilities.
- **[IAFahim.Search.Subset](./src/IAFahim.Search.Subset/README.md)**: This package provides algorithms to enumerate sub-masks, super-masks, and same pop-count integer masks using bitwise search techniques.
- **[IAFahim.Search.Suffix](./src/IAFahim.Search.Suffix/README.md)**: This package provides suffix-based query algorithms, including suffix sums, suffix minimums, and suffix maximums on linear sequences.
- **[IAFahim.Search.TwoPointer](./src/IAFahim.Search.TwoPointer/README.md)**: This package provides two-pointer traversal algorithms, including pair-sum detection and merging of sorted sequences.
- **[IAFahim.Search.Window](./src/IAFahim.Search.Window/README.md)**: This package provides sliding window query algorithms, including minimum and maximum value tracking, and unsafe binary heap operations.

### Sorting

- **[IAFahim.Sort.Insertion](./src/IAFahim.Sort.Insertion/README.md)**: This package provides insertion sorting algorithms for arrays of values using raw memory pointer blocks.
- **[IAFahim.Sort.Merge](./src/IAFahim.Sort.Merge/README.md)**: Sorts elements in an unmanaged buffer by splitting the range, sorting sub-segments recursively, and combining them using a helper buffer.
- **[IAFahim.Sort.Partition](./src/IAFahim.Sort.Partition/README.md)**: Reorders elements in an unmanaged buffer around a pivot. Elements smaller than or equal to the pivot move to the left, while larger elements move to the right.
- **[IAFahim.Sort.QuickSort](./src/IAFahim.Sort.QuickSort/README.md)**: Sorts elements in place using partition operations. Includes single pivot and dual pivot variations.
- **[IAFahim.Sort.RadixSort](./src/IAFahim.Sort.RadixSort/README.md)**: Sorts integer keys using digit-by-digit sorting based on their binary representation. Requires a helper buffer.
- **[IAFahim.Sort.Specialized](./src/IAFahim.Sort.Specialized/README.md)**: Offers optimized, specialized sorting operations. This includes sorting key-value pairs simultaneously and highly optimized sorting routines for primitive integers and 64-bit integers.

### String Algorithms

- **[IAFahim.String](./src/IAFahim.String/README.md)**: Contains core and advanced string processing routines. Includes Lyndon decomposition, run-length encoding and decoding, period finding, De Bruijn sequence generation, expression parsing, NFA-based regex matching, XML and JSON tree hashing, and subsequence or substring enumeration.
- **[IAFahim.String.Automata](./src/IAFahim.String.Automata/README.md)**: Implements finite automata algorithms. Includes DFA minimization, DFA operations like union and intersection, NFA to DFA conversion, and subsequence automata construction for quick subsequence queries.
- **[IAFahim.String.Compress](./src/IAFahim.String.Compress/README.md)**: Implements diverse data compression algorithms. Contains implementations of Huffman coding, Lempel-Ziv variants, arithmetic coding, and Move-To-Front transforms.
- **[IAFahim.String.FMIndex](./src/IAFahim.String.FMIndex/README.md)**: Implements the Burrows-Wheeler Transform and the FM-Index data structure. This enables efficient substring queries and finding occurrences of a pattern within a compressed text using wavelets or occurrence tables.
- **[IAFahim.String.Grammar](./src/IAFahim.String.Grammar/README.md)**: Implements grammar-based string compression and Straight-Line Programs. Represents a string as a context-free grammar to shrink size and query individual symbols in logarithmic time.
- **[IAFahim.String.Match](./src/IAFahim.String.Match/README.md)**: Implements string matching algorithms. Includes exact matching, rolling hash search, approximate matching, Lyndon runs search, and parameterized matching.
- **[IAFahim.String.MinRotation](./src/IAFahim.String.MinRotation/README.md)**: Finds the starting index of the lexicographically smallest cyclic shift of a string or integer sequence using Booth's algorithm.
- **[IAFahim.String.Palindrome](./src/IAFahim.String.Palindrome/README.md)**: Palindromic string analysis package. Includes palindromic trees for tracking distinct palindromic substrings, Manacher's algorithm for finding palindromic radii, Lyndon decomposition of strings, and occurrence counting.
- **[IAFahim.String.Parse](./src/IAFahim.String.Parse/README.md)**: Implements string parsing and recognition algorithms. Includes LL parsing, LR parsing, Earley parsing, the CYK parsing algorithm for context-free grammars, and suffix oracle construction for pattern queries.
- **[IAFahim.String.Pattern](./src/IAFahim.String.Pattern/README.md)**: Implements a persistent version of the Aho-Corasick multiple pattern matching algorithm. Allows building and querying string matchers incrementally across different versions.
- **[IAFahim.String.SuffixArray](./src/IAFahim.String.SuffixArray/README.md)**: Suffix array library for string search and query. Contains static suffix array building, LCP interval tree construction, suffix matching, and dynamic suffix arrays using balanced search trees.
- **[IAFahim.String.SuffixAutomaton](./src/IAFahim.String.SuffixAutomaton/README.md)**: Suffix Automaton implementation. Supports generalized suffix automata for multiple strings, persistent versions, kth substring queries, and transition tree traversal.
- **[IAFahim.String.SuffixTree](./src/IAFahim.String.SuffixTree/README.md)**: Constructs suffix trees using Ukkonen's linear time algorithm. Allows efficient substring indexing and pattern search in text.

### Utilities

- **[IAFahim.Unique](./src/IAFahim.Unique/README.md)**: Filters out redundant values from a buffer of 64-bit or 32-bit integers in place. Returns the size of the filtered prefix.
