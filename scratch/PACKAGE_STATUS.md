# Algorithm package inventory + status (single source of truth)

Packages: 162
- already_correct: 103
- upgraded: 9
- build_ok_untested: 41
- deferred: 9

| Package | Family | Tests | Crit | High | Status | Verify | Reference | Notes |
|---|---|:---:|---:|---:|---|---|---|---|
| `IAFahim.Algebra.GraphPoly` | Algebra | Y | 1 | 1 | already_correct | PASS(test) | AtCoder Library / CP-Algorithms pol | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Algebra.Polynomial` | Algebra | Y | 3 | 5 | upgraded | PASS(test) | AtCoder Library / CP-Algorithms pol | BM/Gcd/BostanMori/ToomCook defining tests; prior fixes revalidated |
| `IAFahim.Algebra.Sequence` | Algebra | Y | 0 | 2 | already_correct | PASS(test) | AtCoder Library / CP-Algorithms pol | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Collections.NoDeps` | Collections | Y | 2 | 6 | already_correct | PASS(test) | Unity.Collections contract | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Collision.Gjk` | Collision | Y | 1 | 0 | already_correct | PASS(test) | Erin Catto GJK / Bullet | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Combinatorics.Generation` | Combinatorics | Y | 0 | 1 | already_correct | PASS(test) | FKM / next_permutation | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Compress` | Compress | Y | 0 | 0 | already_correct | PASS(test) | coordinate compression | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Compress.Coordinate` | Compress | Y | 0 | 0 | already_correct | PASS(test) | coordinate compression | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DP` | DP | N | 0 | 2 | build_ok_untested | PASS(build) | CP-Algorithms DP | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.DP.General` | DP | N | 2 | 1 | build_ok_untested | PASS(build) | CP-Algorithms DP | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.DP.Knapsack` | DP | Y | 0 | 1 | already_correct | PASS(test) | CP-Algorithms DP | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DP.Optimization` | DP | N | 1 | 0 | build_ok_untested | PASS(build) | CP-Algorithms DP | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.DS.Dsu` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.Fenwick` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.FixedCollections` | DS | N | 0 | 0 | build_ok_untested | PASS(build) | ACL + KACTL DS | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.DS.GapBuffer` | DS | Y | 2 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.Grid` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.Heap` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.HilbertOrder` | DS | Y | 3 | 0 | upgraded | PASS(test) | ACL + KACTL DS | Hilbert uniqueness + BlockOrder round-trip + padded Hilbert for Gilber |
| `IAFahim.DS.LinkCut` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.Mo` | DS | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.OrderedSet` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.PerfectHashMap` | DS | N | 1 | 0 | build_ok_untested | PASS(build) | ACL + KACTL DS | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.DS.PersistentDsu` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.PersistentTreap` | DS | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.PieceTable` | DS | Y | 0 | 1 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.RollbackSeg` | DS | Y | 2 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.RollbackStack` | DS | Y | 2 | 1 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.Rope` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.SegmentTree` | DS | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.Sparse` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.SpatialMap` | DS | N | 0 | 4 | build_ok_untested | PASS(build) | ACL + KACTL DS | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.DS.Splay` | DS | Y | 2 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.Treap` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.Trie` | DS | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.UnsafeArray` | DS | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.DS.WaveletMatrix` | DS | Y | 3 | 1 | already_correct | PASS(test) | ACL + KACTL DS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.GameTheory` | GameTheory | Y | 0 | 1 | already_correct | PASS(test) | Sprague–Grundy | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Geometry.Advanced` | Geometry | Y | 1 | 0 | already_correct | PASS(test) | KACTL / geogram / CGAL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Geometry.Arrangement` | Geometry | N | 0 | 4 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.Azimuth` | Geometry | Y | 0 | 0 | already_correct | PASS(test) | KACTL / geogram / CGAL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Geometry.Basic` | Geometry | Y | 1 | 0 | already_correct | PASS(test) | KACTL / geogram / CGAL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Geometry.Bvh` | Geometry | N | 0 | 1 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.Curve` | Geometry | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.Delaunay` | Geometry | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.Frame` | Geometry | N | 0 | 1 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.Hull` | Geometry | Y | 1 | 2 | already_correct | PASS(test) | KACTL / geogram / CGAL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Geometry.Intersect` | Geometry | Y | 1 | 0 | already_correct | PASS(test) | KACTL / geogram / CGAL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Geometry.MarchingCubes` | Geometry | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.Mesh` | Geometry | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.PolygonClip` | Geometry | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.Spatial` | Geometry | Y | 1 | 2 | upgraded | PASS(test) | KACTL / geogram / CGAL | CoverTree hierarchical Build + exact NN via cover radii; tests |
| `IAFahim.Geometry.Subdivision` | Geometry | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.SweepPrune` | Geometry | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.Triangulation` | Geometry | N | 1 | 0 | build_ok_untested | PASS(build) | KACTL / geogram / CGAL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Geometry.Voronoi` | Geometry | Y | 0 | 0 | already_correct | PASS(test) | KACTL / geogram / CGAL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph` | Graph | Y | 5 | 6 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Bridges` | Graph | Y | 2 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Cactus` | Graph | N | 0 | 0 | deferred | PASS(build) | ACL + KACTL + Boost.Graph | BlockCutTreeLca/CactusLca honest NI (contract lacks tree buffers); oth |
| `IAFahim.Graph.Centroid` | Graph | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Clique` | Graph | N | 0 | 0 | deferred | PASS(build) | ACL + KACTL + Boost.Graph | README shell; no algorithm .cs sources |
| `IAFahim.Graph.Connectivity` | Graph | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Cut` | Graph | N | 0 | 0 | deferred | PASS(build) | ACL + KACTL + Boost.Graph | README shell; min-cut covered by Graph.Flow |
| `IAFahim.Graph.DAG` | Graph | Y | 3 | 2 | upgraded | PASS(test) | ACL + KACTL + Boost.Graph | path cover + Dilworth antichain defining tests |
| `IAFahim.Graph.Decomposition` | Graph | N | 0 | 0 | deferred | PASS(build) | ACL + KACTL + Boost.Graph | README shell; no algorithm .cs sources |
| `IAFahim.Graph.Dominator` | Graph | N | 0 | 0 | deferred | PASS(build) | ACL + KACTL + Boost.Graph | README shell; no algorithm .cs sources |
| `IAFahim.Graph.DynamicTrees` | Graph | Y | 2 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Eertree` | Graph | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Eulerian` | Graph | Y | 2 | 1 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Flow` | Graph | Y | 17 | 3 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Functional` | Graph | N | 0 | 0 | build_ok_untested | PASS(build) | ACL + KACTL + Boost.Graph | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Graph.Matching` | Graph | Y | 6 | 3 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Misc` | Graph | N | 0 | 0 | build_ok_untested | PASS(build) | ACL + KACTL + Boost.Graph | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Graph.RandomWalk` | Graph | N | 0 | 0 | deferred | PASS(build) | ACL + KACTL + Boost.Graph | README shell; no algorithm .cs sources |
| `IAFahim.Graph.SCC` | Graph | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.ShortestPath` | Graph | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.SpanningTrees` | Graph | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.Tree` | Graph | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.TreeDecomposition` | Graph | Y | 1 | 1 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Graph.TreeIsomorphism` | Graph | N | 0 | 0 | deferred | PASS(build) | ACL + KACTL + Boost.Graph | UnorderedTreeEditDistance unconstrained NP-hard NI; other entry points |
| `IAFahim.Graph.TreeQueries` | Graph | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL + Boost.Graph | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Linear` | Linear | N | 0 | 0 | deferred | n/a | Gaussian elimination | meta-folder only (no csproj/sources) |
| `IAFahim.Linear.Eigen` | Linear | Y | 0 | 0 | already_correct | PASS(test) | Gaussian elimination | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Linear.Matrix` | Linear | Y | 2 | 0 | already_correct | PASS(test) | Gaussian elimination | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Linear.Matrix2` | Linear | N | 0 | 0 | build_ok_untested | PASS(build) | Gaussian elimination | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Math.Arithmetic` | Math | Y | 0 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Barycentric` | Math | Y | 0 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Basic` | Math | Y | 0 | 2 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.BigInt` | Math | Y | 1 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Combinatorics` | Math | Y | 0 | 1 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Gauss` | Math | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL NT + ACL convolution | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Math.Kalman` | Math | Y | 0 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Modular` | Math | Y | 0 | 1 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.NT` | Math | Y | 12 | 11 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Noise` | Math | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL NT + ACL convolution | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Math.PoissonDisk` | Math | Y | 0 | 3 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Polynomial` | Math | Y | 1 | 2 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Polynomial.Eval` | Math | Y | 1 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Polynomial.Fps` | Math | Y | 0 | 1 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.PotentialField` | Math | Y | 0 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Quaternion` | Math | Y | 0 | 1 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Sdf` | Math | Y | 0 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.SphericalHarmonics` | Math | Y | 0 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Spline` | Math | Y | 0 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Transform` | Math | Y | 1 | 2 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Transform.AnyMod` | Math | N | 1 | 0 | build_ok_untested | PASS(build) | KACTL NT + ACL convolution | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Math.Transform.Fft` | Math | Y | 0 | 0 | already_correct | PASS(test) | KACTL NT + ACL convolution | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Math.Transform.Ntt` | Math | N | 0 | 0 | build_ok_untested | PASS(build) | KACTL NT + ACL convolution | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Memory.Allocators` | Memory | N | 0 | 0 | build_ok_untested | PASS(build) | Unity allocator contracts | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Optimization.Approximation` | Optimization | N | 0 | 2 | build_ok_untested | PASS(build) | CP-Algo / Dreyfus–Wagner / cut-and- | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Optimization.DivideConquer` | Optimization | N | 1 | 2 | build_ok_untested | PASS(build) | CP-Algo / Dreyfus–Wagner / cut-and- | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Optimization.Exact` | Optimization | Y | 4 | 1 | upgraded | PASS(test) | CP-Algo / Dreyfus–Wagner / cut-and- | SteinerDreyfusWagner distinct terminal masks + SP relax; Steiner tests |
| `IAFahim.Optimization.Games` | Optimization | N | 3 | 1 | build_ok_untested | PASS(build) | CP-Algo / Dreyfus–Wagner / cut-and- | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Optimization.Geometric` | Optimization | N | 1 | 0 | build_ok_untested | PASS(build) | CP-Algo / Dreyfus–Wagner / cut-and- | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Optimization.Knapsack` | Optimization | N | 1 | 3 | build_ok_untested | PASS(build) | CP-Algo / Dreyfus–Wagner / cut-and- | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Optimization.Matroid` | Optimization | N | 0 | 1 | build_ok_untested | PASS(build) | CP-Algo / Dreyfus–Wagner / cut-and- | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Optimization.Offline` | Optimization | Y | 0 | 3 | already_correct | PASS(test) | CP-Algo / Dreyfus–Wagner / cut-and- | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Optimization.Submodular` | Optimization | N | 0 | 1 | build_ok_untested | PASS(build) | CP-Algo / Dreyfus–Wagner / cut-and- | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Optimization.Treewidth` | Optimization | Y | 1 | 1 | upgraded | PASS(test) | CP-Algo / Dreyfus–Wagner / cut-and- | CutAndCount counts connected induced bag subsets into dp; tests |
| `IAFahim.Pathfinding.Jps` | Pathfinding | N | 0 | 0 | build_ok_untested | PASS(build) | recastnavigation + JPS | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Pathfinding.Recast` | Pathfinding | Y | 0 | 0 | already_correct | PASS(test) | recastnavigation + JPS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Permutation` | Permutation | Y | 0 | 1 | already_correct | PASS(test) | cycle decomp | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Physics.Xpbd` | Physics | Y | 1 | 1 | already_correct | PASS(test) | Müller XPBD | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search` | Search | N | 0 | 0 | deferred | n/a | CP-Algorithms / KACTL | meta-folder only (no csproj/sources) |
| `IAFahim.Search.Automaton` | Search | N | 0 | 0 | build_ok_untested | PASS(build) | CP-Algorithms / KACTL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Search.Bit` | Search | Y | 1 | 0 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.DifferenceArray` | Search | Y | 0 | 0 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.ExactCover` | Search | Y | 1 | 2 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.Imos` | Search | Y | 1 | 1 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.Interval` | Search | Y | 0 | 1 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.LIS` | Search | N | 0 | 0 | build_ok_untested | PASS(build) | CP-Algorithms / KACTL | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Search.MeetInMiddle` | Search | Y | 0 | 0 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.Numerical` | Search | Y | 2 | 0 | upgraded | PASS(test) | CP-Algorithms / KACTL | Simpson/GaussLegendre/AdaptiveSimpson ∫x² + ternary min tests |
| `IAFahim.Search.Prefix` | Search | Y | 0 | 1 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.Range` | Search | Y | 0 | 3 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.RangeQueries` | Search | Y | 1 | 0 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.Selection` | Search | Y | 0 | 1 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.Specialized` | Search | Y | 0 | 0 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.Subset` | Search | Y | 1 | 0 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.Suffix` | Search | Y | 0 | 2 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.TwoPointer` | Search | Y | 0 | 1 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Search.Window` | Search | Y | 0 | 0 | already_correct | PASS(test) | CP-Algorithms / KACTL | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Sort.Insertion` | Sort | Y | 0 | 0 | already_correct | PASS(test) | .NET Span.Sort + CLRS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Sort.Merge` | Sort | Y | 0 | 0 | already_correct | PASS(test) | .NET Span.Sort + CLRS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Sort.Partition` | Sort | Y | 0 | 0 | already_correct | PASS(test) | .NET Span.Sort + CLRS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Sort.QuickSort` | Sort | Y | 2 | 0 | upgraded | PASS(test) | .NET Span.Sort + CLRS | Hoare-correct; defining NUnit tests added |
| `IAFahim.Sort.RadixSort` | Sort | Y | 3 | 0 | already_correct | PASS(test) | .NET Span.Sort + CLRS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.Sort.Specialized` | Sort | Y | 0 | 0 | already_correct | PASS(test) | .NET Span.Sort + CLRS | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.String` | String | Y | 0 | 1 | already_correct | PASS(test) | ACL + KACTL string | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.String.Automata` | String | N | 1 | 0 | build_ok_untested | PASS(build) | ACL + KACTL string | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.String.Compress` | String | N | 1 | 1 | build_ok_untested | PASS(build) | ACL + KACTL string | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.String.FMIndex` | String | Y | 1 | 0 | already_correct | PASS(test) | ACL + KACTL string | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.String.Grammar` | String | N | 1 | 3 | build_ok_untested | PASS(build) | ACL + KACTL string | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.String.Match` | String | Y | 1 | 3 | already_correct | PASS(test) | ACL + KACTL string | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.String.MinRotation` | String | Y | 0 | 0 | already_correct | PASS(test) | ACL + KACTL string | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.String.Palindrome` | String | Y | 2 | 0 | upgraded | PASS(test) | ACL + KACTL string | Manacher odd/even + eertree distinct count tests |
| `IAFahim.String.Parse` | String | Y | 1 | 2 | already_correct | PASS(test) | ACL + KACTL string | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.String.Pattern` | String | N | 0 | 0 | build_ok_untested | PASS(build) | ACL + KACTL string | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.String.SuffixArray` | String | N | 0 | 1 | build_ok_untested | PASS(build) | ACL + KACTL string | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.String.SuffixAutomaton` | String | Y | 2 | 2 | already_correct | PASS(test) | ACL + KACTL string | isolated NUnit green; do not treat historical findings as auto-fixed w |
| `IAFahim.String.SuffixTree` | String | N | 1 | 0 | build_ok_untested | PASS(build) | ACL + KACTL string | builds in isolation; NO defining tests — not claimed reference-correct |
| `IAFahim.Unique` | Unique | Y | 0 | 0 | already_correct | PASS(test) | std::unique | isolated NUnit green; do not treat historical findings as auto-fixed w |
