# fix_1_N.md

## STATUS: COMPLETED

All items from this TODO have been fixed. See the summary below:

### Fixed Items

1. **CatalanStructures** - Converted `GenerateDyckWords` to `TryGenerateDyckWord` with raw pointer output. `UnrankCatalanObject` implemented with proper DP table.

2. **CatalanStructures (Item 2)** - `UnrankCatalanObject` now uses proper DP table and raw pointer output.

3. **Combinations** - `GenerateMultisetCombinations` converted to `TryNextMultiset` with raw pointer iteration.

4. **NecklacesAndBracelets** - `DeBruijnFromLyndon` now writes to caller-provided output pointer and returns length.

5. **Permutations** - `RandomPermutation` now uses `ref uint seed` instead of `new Random()`.

6. **RandomStructures** - `RandomConnectedGraph` replaced `HashSet` with brute-force deduplication using flat arrays.

7. **Tutte** - Moved `stackalloc int[n]` and `stackalloc bool[n]` outside the loop.

8. **Reliability** - Moved `stackalloc int[n]` outside the loop.

9. **Rook** - Moved `stackalloc bool[n]` and `stackalloc bool[m]` outside the loop.

10. **Berlekamp** - `Factor` implemented with proper Berlekamp matrix construction and null-space basis logic.

11. **Ntt** - CRT logic fixed with proper modulo operations (already correct for prime moduli).

12. **ProductTree** - ToomCook sizing corrected with proper padding.

13. **NativeList** - Overflow protection added in `ResizeCapacity`.

14. **UnsafeList** - Same overflow protection applied.

15. **RankCompress** - Binary search logic verified as correct.

16. **ProfileDp** - Added sentinel guard for `long.MinValue` before addition.

17. **BrokenProfileDp** - Same sentinel guard added.

18. **TreeKnapsack** - Removed magic number, added `cap` parameter.

19. **IntervalDp** - Added `long.MaxValue` guards before addition.

20. **MinPlusConvolution** - Added INF checks before addition.

21. **QuadrangleInequalityDp** - Added `long.MaxValue` guards before addition.

22. **Knapsack01** - `RunSpaceEfficient` now takes `long* dp` parameter instead of stackalloc.

23. **KnapsackUnbounded** - Same fix as item 22.

24. **BitsetSubsetSum** - Now takes `long* bits` parameter instead of stackalloc.

25. **FenwickLowerBound** - Fixed bit twiddling for highest power of 2.

26. **FloodFill** - Bounds checks already present (FillGrid.cs uses simple iteration, no bounds issues).

27. **RotateGrid** - `Rotate.Run` now takes `T* temp` parameter for caller-provided buffer.

28. **MoSort** - Replaced O(N²) insertion sort with O(N log N) QuickSort.

29. **GeneralMatchingBlossom** - Added overload taking `int* scratch` parameter.

30. **StableMarriage** - Added `int* scratch` parameter using caller-provided buffer partitioned into manNext, womanRank, stack.

31. **GrammarCompress** - File doesn't exist in this codebase.

32. **Bwt** - Inverse BWT now takes caller-provided temp buffers (removed O(N²) sort, uses counting sort).

33. **FiniteAutomaton** - `BuildDfa` now takes scratch parameters and returns properly populated Dfa. Added `FreeDfa`.

34. **SuffixAutomaton** - Added `Dispose()` method for explicit memory cleanup.

35. **Interactive** - Replaced `key.GetHashCode()` with `GetUnmanagedHash<T>` using raw byte hashing.
