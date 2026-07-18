# Reference map — world-class implementations for IAFahim.CS packages

Each family maps to defining external sources used as **correctness oracles**.
Implementations stay unmanaged/Burst-safe per AGENTS.md; we port mechanics, not managed APIs.

## Canonical repositories (top tier)

| Source | URL / identity | Strength |
|--------|----------------|----------|
| **AtCoder Library (ACL)** | `atcoder/ac-library` | Production-grade DSU, fenwick, segtree, lazy, max-flow (Dinic), SCC, 2-SAT, convolution (NTT), string (SA, Z, KMP-style) |
| **KACTL** | `kth-competitive-programming/kactl` | Short, battle-tested ICPC templates: geometry, graph, number theory, strings, flow |
| **CP-Algorithms (e-maxx)** | `cp-algorithms.com` / e-maxx.ru | Reference algorithms with proofs: graph, string, DP, NT, geometry |
| **Boost.Graph** | `boostorg/graph` | Industrial graph algorithms (Dijkstra, BFS/DFS, matching, flow concepts) |
| **geogram / CGAL** | `BrunoLevy/geogram`, CGAL | Robust geometry: Delaunay, Voronoi, hull, predicates |
| **Recast Navigation** | `recastnavigation/recastnavigation` | Navmesh build + Detour query (gold standard for this domain) |
| **.NET BCL** | `dotnet/runtime` | Behavioral baseline for Sort/BinarySearch only (not API to copy into src/) |
| **GJK literature / Bullet / Box2D** | Erin Catto GDC, Bullet Physics | Collision GJK/EPA |
| **SDSL / Succinct** | succinct data structure literature | Wavelet matrix, FM-index, rank/select |
| **Koosaga / tourist-style CP libs** | olympiad libraries | Advanced graph (weighted blossom, cactus, dominators) |

## Per-family mapping

### DS (data structures)
| Package pattern | Primary reference | Defining mechanics |
|-----------------|-------------------|-------------------|
| DS.Dsu | ACL `dsu.hpp` | path compression + union by size/rank; same component ⇔ Find equal |
| DS.Fenwick | ACL `fenwicktree.hpp` / KACTL | prefix sum: `Add(i,v)` + `Sum(r)-Sum(l)` = range |
| DS.SegmentTree / RollbackSeg | ACL `segtree.hpp` / `lazysegtree.hpp` | associative combine; lazy push correctness |
| DS.Sparse | KACTL / CP-Algo RMQ | idempotent ops: `query(l,r)` = fold of range |
| DS.Heap | binary heap literature / BCL PriorityQueue | heap property parent ≤/≥ children; extract-min order |
| DS.Treap / Splay / LinkCut / Persistent* | KACTL / CP-Algo | BST order + heap priority / splay access / LCT preferred-path |
| DS.Trie | CP-Algo | insert/search by character path |
| DS.WaveletMatrix | SDSL / CP-Algo | rank/select/quantile on static sequences |
| DS.Mo / HilbertOrder | CP-Algo Mo’s algorithm | Hilbert order reduces movement; offline answers correct |
| DS.GapBuffer / Rope / PieceTable | text-editor gap buffer / rope literature | document content = left + right of gap; edit at cursor |
| DS.UnsafeArray / FixedCollections | Unity Collections semantics | length/capacity/allocator free |
| DS.OrderedSet / PerfectHashMap / SpatialMap | order-stat tree / CHD perfect hash / spatial hashing | key order / O(1) perfect lookup / cell bucket |

### Sort
| Package | Reference | Defining mechanics |
|---------|-----------|-------------------|
| Sort.Insertion / Merge / QuickSort / Partition | CLRS + .NET `Span.Sort` baseline | permutation of input; nondecreasing order under `IComparable` |
| Sort.RadixSort | LSD/MSD radix literature | stable LSD: equal keys preserve relative order; full key sort |
| Sort.Specialized | domain-specific (counting, bucket) | same as comparison sort for integer keys |

### Search
| Package pattern | Reference | Defining mechanics |
|-----------------|-----------|-------------------|
| Search.Range / Bit / Prefix / Window / TwoPointer | CP-Algo / KACTL | lower/upper bound; sliding window invariants; two-pointer sorted sum |
| Search.LIS / Subset / MeetInMiddle / ExactCover | CP-Algo | LIS length/reconstruction; DLX exact cover; MITM split |
| Search.Selection | Quickselect / introselect | k-th order statistic |
| Search.Numerical | binary/ternary search on monotone | correct endpoint when predicate flips |
| Search.Suffix / Automaton | string automata | accept language of patterns |

### Graph (core + subpackages)
| Package pattern | Reference | Defining mechanics |
|-----------------|-----------|-------------------|
| Graph / Graph.SCC / Bridges | ACL SCC / Tarjan / CP-Algo | condensation DAG; bridge = edge whose removal increases components |
| Graph.ShortestPath | Dijkstra (non-neg), Bellman-Ford, Floyd | distance ≤ any path cost; BF detects neg cycle |
| Graph.Flow | ACL maxflow / Dinic / KACTL | max-flow = min-cut; residual reverse edges `e^1`; conservation |
| Graph.Matching | Hopcroft–Karp, Hungarian, Blossom (KACTL/Kolmogorov) | matching size/weight optimality; blossom shrink |
| Graph.Tree / TreeQueries / Centroid / HLD | CP-Algo | LCA, reroot DP, centroid decomposition partition |
| Graph.SpanningTrees | Kruskal/Prim | MST weight = min connecting forest |
| Graph.Eulerian | Hierholzer | trail uses each edge once when degrees allow |
| Graph.DAG | topo order / DP on DAG | order respects edges; DP recurrence |
| Graph.Functional / TreeIsomorphism | functional graph / AHU tree iso | cycles+trees into; canonical form equal ⇔ iso |
| Graph.Connectivity / Cut / Clique / Dominator | CP-Algo / Lengauer-Tarjan | component labels; dominator tree on CFG |
| Graph.DynamicTrees / Cactus | LCT / cactus literature | path aggregates under link/cut |

### Math / Algebra / Linear / Transform
| Package pattern | Reference | Defining mechanics |
|-----------------|-----------|-------------------|
| Math.NT / Modular / Combinatorics | KACTL NT / ACL | modular mul inverse; sieve; phi/mu; CRT |
| Math.Transform.Fft / Ntt / AnyMod | ACL convolution / KACTL FFT | convolution identity; inverse recovers input |
| Algebra.Polynomial | Berlekamp–Massey, poly gcd (CP-Algo) | LFSR min poly; gcd monic correctness |
| Algebra.GraphPoly | Tutte/reliability literature | reliability: present edge ×p, absent ×(1−p) |
| Algebra.Sequence | generating functions / Lagrange inversion | coefficient extraction matches closed forms |
| Linear.* | Gaussian elimination / eigen | Ax=b; rank; eigenvalues for small dense |

### Geometry / Collision / Physics
| Package pattern | Reference | Defining mechanics |
|-----------------|-----------|-------------------|
| Geometry.Basic / Hull / Intersect | KACTL geometry / CGAL predicates | CCW orientation; convex hull extreme; segment intersect |
| Geometry.Delaunay / Voronoi / Triangulation | geogram / Shewchuk | empty circumcircle; dual of Delaunay |
| Geometry.Bvh / Spatial / SweepPrune | BVH literature | prune correctness; no false negative overlaps |
| Collision.Gjk | Erin Catto / Bullet GJK-EPA | distance 0 ⇔ intersection; simplex support |
| Physics.Xpbd | Müller XPBD paper | constraint projection stability |

### String
| Package pattern | Reference | Defining mechanics |
|-----------------|-----------|-------------------|
| String / Match / Pattern / Automata | KACTL / ACL string | KMP π; Z-array; Aho-Corasick outputs |
| String.SuffixArray / SuffixAutomaton / SuffixTree | SA-IS / SAM / Ukkonen | LCP; endpos equivalence; substring occurrence |
| String.Palindrome | Manacher | odd/even radii match expansions |
| String.FMIndex / Compress / MinRotation | BWT/FM / Booth | LF-mapping; least rotation index |
| String.Grammar / Parse | recursive descent / CYK | accept if in language |

### DP / Optimization
| Package pattern | Reference | Defining mechanics |
|-----------------|-----------|-------------------|
| DP / DP.Knapsack / DP.General | CP-Algo DP | knapsack capacity/value; interval DP opt structure |
| DP.Optimization | CHT / Li Chao / Knuth / D&C opt | envelope queries; quadrangle inequality |
| Optimization.* | approx algorithms / matroid / submodular | greedy choice; approximation guarantees where claimed |

### Pathfinding
| Package | Reference | Defining mechanics |
|---------|-----------|-------------------|
| Pathfinding.Recast | `recastnavigation` | same pipeline stages; golden-hash regression in tests |
| Pathfinding.Jps | JPS / Harabor | path optimal on uniform grid like A* |

### Other
| Package | Reference | Defining mechanics |
|---------|-----------|-------------------|
| Compress / Compress.Coordinate | coordinate compress | order-preserving rank map |
| Combinatorics.Generation | FKM / next_permutation | enumerate without dups; rank/unrank |
| GameTheory | Sprague–Grundy | xor of nimbers = 0 ⇔ second-player win |
| Permutation / Unique | cycle decomp / unique adjacent | cycles cover; unique keeps first of runs |
| Memory.Allocators / Collections.NoDeps | Unity.Collections contract | free matches alloc; alignment; no UAF |
| IO / UnityMathematics | n/a stubs | infra only |

## How to use when upgrading a package

1. Identify family row → open primary reference source for that algorithm.
2. Extract **defining properties** (not micro-optimizations).
3. Port control flow / formulas into unmanaged `static` methods.
4. Prove with pointer tests: empty, trivial, adversarial, known closed form.
5. Isolated `buildsweep` only.
