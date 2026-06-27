# Stub Completion — Master Plan (started 2026-06-25)

Source: external AI review → verified via audit workflow wf_7f03bd9f-ae7.
106 stubs / 36 files / 15 modules. Excludes 4 intentional infra no-ops (Collections.NoDeps).

User decision: COMPLETE the stubs (implement, not just throw). Where an algorithm
can't be implemented correctly/safely, leave a loud `NotImplementedException`.
Protocol: each impl gets 2 independent judge agents + build check before acceptance.
Constraint: Burst/Unity/mobile, zero-alloc, no stackalloc[n] for large N (use Allocator.Temp).

## Batches
- [x] BATCH 1 — 24 real-contract algos. DONE 2026-06-25, all 7 projects build clean.
      14 implemented+accepted; 4 honest NotImplementedException (FunctionalGraphReroot ambiguous,
      UnorderedTreeEditDistance NP-hard, CactusShortestPath thin contract, Delaunay.Flip insufficient
      contract); 3 judge-rejected then FIXED by me (PermutationLog CRT overflow->return -1;
      RootedTreeCanonicalForm input validation; TreeIsomorphismCenterHash cycle->terminate).
      KEY LESSON: bare csprojs (netstandard2.1, zero refs) cannot use Unity AllocatorManager OR
      NativeMemory(.NET6+). Use System.Runtime.InteropServices.Marshal.AllocHGlobal/FreeHGlobal for
      n-sized heap scratch in no-dep projects. Bake this into ALL future batches + the B1 stackalloc fix.
- [ ] BATCH 2 — Search.RangeQueries (30 parameterless: AdvancedRangeQueries, QueriesOverTime, OfflineQueries)
- [ ] BATCH 3 — DS.RollbackSeg (14) + DS.SegmentTree (12: LiChao, Kinetic)
- [ ] BATCH 4 — DS.WaveletMatrix (9) + Geometry.Arrangement (7)
- [ ] BATCH 5 — Geometry.Advanced PolygonBoolean (4) + Geometry.Hull (2)

## Separate workstreams (user-selected)
- [ ] B1 — stackalloc[n] crash fix (144 files; triage by max-N, use Allocator.Temp)
- [ ] N5 — shared fuzz/property harness (test/IAFahim.Fuzz)

## Excluded (intentional no-ops, do NOT touch)
AllocatorManager.Allocate, CollectionHelper.CheckAllocator, BLGlobalLogger.LogError512/LogWarningString
