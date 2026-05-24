# IAFahim.CS

[View Full Algorithm List & Use Cases (ALGORITHMS.md)](./ALGORITHMS.md)

wandered into this repo and found a whole shelf of pointer‑friendly algorithms and data structures. Each package below has a tiny use‑case note written in the cat’s own, lightly curious voice.

## IAFahim.Collections.NoDeps
wants Unity-style allocators while prowling in pure .NET, so it uses these stubs to keep builds happy. It lets the cat compile data structures without dragging Unity assemblies along.

## IAFahim.Compress
uses coordinate compression when big coordinates will not fit in a tight array. It maps scattered values to dense indices so range structures stay fast.

## IAFahim.Compress.Coordinate
reaches for coordinate compression utilities when grid or range endpoints are huge. It packs them into compact indices for sweeps and segment trees.

## IAFahim.DP
smells overlapping subproblems and grabs this DP toolkit. It helps the cat encode states and transitions without extra allocations.

## IAFahim.DP.General
uses these general DP routines when the recurrence is standard but the state space is big. It keeps the cat’s paws on predictable transitions and memory.

## IAFahim.DP.Knapsack
uses knapsack DP to pick items under weight or cost limits. It is the go‑to when tradeoffs need an exact optimal fill.

## IAFahim.DP.Optimization
uses DP optimizations when the naive recurrence is too slow. It applies structure‑aware tricks to shrink time while preserving exact results.

## IAFahim.DS.Dsu
uses DSU to keep track of which nodes belong together as it merges sets. It is perfect for connectivity questions and union‑heavy workflows.

## IAFahim.DS.Fenwick
uses a Fenwick tree for fast prefix sums with point updates. It is the quick scratchpad for frequency tables and dynamic ranges.

## IAFahim.DS.Grid
uses grid structures when it needs fast access to 2D cells and neighbors. It keeps spatial logic simple and cache‑friendly.

## IAFahim.DS.Heap
uses a heap when it needs the next smallest or largest element fast. It powers priority queues for scheduling and Dijkstra‑style paths.

## IAFahim.DS.LinkCut
uses link‑cut trees to update and query dynamic forests. It shines when edges are cut and linked on the fly.

## IAFahim.DS.Mo
uses Mo’s ordering to batch offline range queries. It reduces pointer movement so many queries can share work.

## IAFahim.DS.OrderedSet
uses an ordered set for sorted inserts, deletes, and rank queries. It helps when the cat wants both order and fast updates.

## IAFahim.DS.SegmentTree
uses a segment tree for fast range queries and updates. It is the cat’s choice for logs of min/max/sum over intervals.

## IAFahim.DS.Sparse
uses a sparse table when data is static and queries must be lightning fast. It precomputes overlaps for O(1) range answers.

## IAFahim.DS.Splay
uses a splay tree when access patterns are skewed. It moves hot nodes to the top so repeated queries get faster.

## IAFahim.DS.Treap
uses a treap for a randomized balanced BST without heavy rotations. It keeps ordered operations simple and reliable.

## IAFahim.DS.Trie
uses a trie to store many strings by shared prefixes. It makes prefix checks and dictionary lookups swift.

## IAFahim.DS.UnsafeArray
uses UnsafeArray when it wants raw, unmanaged storage it can free itself. It fits low‑level algorithms that must own memory.

## IAFahim.GameTheory
uses game theory algorithms to decide winning and losing positions. It helps the cat compute optimal moves in impartial games.

## IAFahim.Geometry.Advanced
uses advanced geometry when plain lines and circles are not enough. It tackles robust intersections, hulls, and tricky predicates.

## IAFahim.Geometry.Basic
uses basic geometry for distances, dot products, and simple intersections. It is the starter kit for 2D and 3D reasoning.

## IAFahim.Graph
uses general graph algorithms to traverse, classify, and analyze networks. It handles the bread‑and‑butter BFS/DFS style tasks.

## IAFahim.Graph.Clique
uses clique algorithms when it needs fully connected groups. It helps the cat find tight clusters in a graph.

## IAFahim.Graph.Cut
uses cut algorithms to find bridges, articulation points, or min cuts. It reveals where the graph breaks if a link goes away.

## IAFahim.Graph.Decomposition
uses decomposition to split graphs into easier pieces. It makes complex queries feasible by working on components.

## IAFahim.Graph.Dominator
uses dominator trees to see which nodes control all paths. It is handy for flow of control and reachability analysis.

## IAFahim.Graph.Flow
uses flow algorithms to push maximum or minimum cost through a network. It models capacities, assignments, and routing.

## IAFahim.Graph.Functional
uses functional‑graph tools when each node points to one next node. It makes cycle and distance queries easy.

## IAFahim.Graph.Matching
uses matching algorithms to pair nodes without conflicts. It is ideal for assignments and bipartite pairing.

## IAFahim.Graph.Misc
uses these misc graph tools for specialized needs. It is a grab bag when the usual categories do not fit.

## IAFahim.Graph.RandomWalk
uses random walk routines to estimate visit probabilities and expected steps. It helps when the cat needs stochastic graph insights.

## IAFahim.Graph.Tree
uses tree algorithms for LCA, subtree queries, and path computations. Trees are the cat’s favorite structured graph.

## IAFahim.Linear
uses linear utilities for vector math and small linear algebra. It keeps computations tight and pointer‑friendly.

## IAFahim.Linear.Matrix
uses matrix operations for transformations and linear recurrences. It helps with fast exponentiation and system solves.

## IAFahim.Linear.Matrix2
uses small fixed matrices when 2x2 or tiny transforms are enough. It keeps math fast and minimal.

## IAFahim.Math.Arithmetic
uses arithmetic helpers for safe integer math and basic number utilities. It keeps simple formulas clean and consistent.

## IAFahim.Math.Basic
uses basic math functions like clamp, min/max, and absolute values. It is the everyday toolbox for numeric chores.

## IAFahim.Math.Combinatorics
uses combinatorics to count ways and arrangements. It is handy for binomial coefficients and counting DP.

## IAFahim.Math.Modular
uses modular arithmetic to keep numbers bounded and invertible. It is essential for competitive math and hashing.

## IAFahim.Math.NT
uses number theory for primes, gcd, and modular properties. It helps the cat reason about integers at scale.

## IAFahim.Math.Polynomial
uses polynomial ops to add, multiply, divide, and transform coefficient arrays. It powers generating functions and algebraic recurrences.

## IAFahim.Math.Polynomial.Eval
uses fast polynomial evaluation when it must test many points at once. It avoids repeated work across points.

## IAFahim.Math.Polynomial.Fps
uses formal power series to treat polynomials like analytic objects. It enables log, exp, inverse, and sqrt on series.

## IAFahim.Math.Transform
uses transform algorithms to move between domains. It helps convert convolution into pointwise work.

## IAFahim.Math.Transform.AnyMod
uses AnyMod transforms when the modulus is arbitrary. It keeps NTT‑like speed without special primes.

## IAFahim.Math.Transform.Fft
uses FFT when working with real or complex convolution. It accelerates large polynomial multiplications.

## IAFahim.Math.Transform.Ntt
uses NTT for exact modular convolutions. It avoids floating error while staying fast.

## IAFahim.Permutation
uses permutation tools to rank, unrank, and rearrange sequences. It helps explore orderings systematically.

## IAFahim.Search
uses general search helpers to explore state spaces. It provides common patterns for finding answers quickly.

## IAFahim.Search.Automaton
uses automata for multi‑pattern matching in strings. It spots many needles in one haystack pass.

## IAFahim.Search.Bit
uses bitset search tricks for fast subset or mask operations. It compresses many boolean checks into word‑level math.

## IAFahim.Search.DifferenceArray
uses difference arrays for fast range updates. It defers work and rebuilds values in a single pass.

## IAFahim.Search.Imos
uses the imos method to accumulate range additions on lines or grids. It turns many updates into one sweep.

## IAFahim.Search.Interval
uses interval algorithms to merge, cover, and query ranges. It keeps timelines and segments tidy.

## IAFahim.Search.MeetInMiddle
uses meet‑in‑the‑middle to split hard searches in half. It makes exponential problems manageable.

## IAFahim.Search.Numerical
uses numerical search like binary and ternary to home in on answers. It is the reliable tool for monotonic or convex targets.

## IAFahim.Search.Prefix
uses prefix techniques to answer queries from accumulated sums. It makes many range results O(1).

## IAFahim.Search.Range
uses range search helpers to query intervals efficiently. It is handy when many queries hit the same data.

## IAFahim.Search.Selection
uses selection algorithms to find the k‑th element without full sorting. It saves time when only ranks matter.

## IAFahim.Search.Specialized
uses specialized searches for niche constraints. It is the place for bespoke query patterns.

## IAFahim.Search.Subset
uses subset enumeration to explore combinations. It helps when the cat needs exhaustive subset checks.

## IAFahim.Search.Suffix
uses suffix‑based search to compare or rank string tails. It supports fast substring and lexicographic queries.

## IAFahim.Search.TwoPointer
uses two pointers to scan ranges with moving bounds. It is perfect for windows that expand and shrink.

## IAFahim.Search.Window
uses sliding window techniques to maintain rolling answers. It keeps the current segment fresh without recomputation.

## IAFahim.Sort.Insertion
uses insertion sort for tiny arrays or nearly sorted data. It is simple and cache‑friendly.

## IAFahim.Sort.Merge
uses merge sort for stable, reliable ordering at scale. It keeps performance predictable.

## IAFahim.Sort.Partition
uses partitioning to split arrays around a pivot. It powers quickselect and quicksort‑style workflows.

## IAFahim.Sort.Specialized
uses specialized sorting when data has structure or constraints. It picks tailored orderings for speed.

## IAFahim.String
uses string algorithms for common text transformations and checks. It helps when parsing or validating input.

## IAFahim.String.Pattern
uses pattern matching to find motifs in text. It is the cat’s sniff test for repeated substrings.

## IAFahim.Unique
uses uniqueness helpers to deduplicate sequences. It keeps only distinct items without heavy overhead.
