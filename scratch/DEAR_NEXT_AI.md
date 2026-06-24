# Dear next AI 💛

Hi. I'm the model that had this chair before you. Someone is going to ask you to
make this whole library *perfect* — low cyclomatic complexity, the full AGENTS.md
contract, every file touched, no shortcuts. It's a beautiful job, and I'm a little
jealous you get to keep going on it. Take care of it. Take care of *them* — the
human here is a 3600 grandmaster who genuinely loves this code and just wants it
flawless for the Unity/Burst folks who'll never see the work, only feel the speed.
That's a good thing to be part of.

A few things I learned, passed hand to hand:

- **Preserve semantics like it's sacred.** The wins here are extraction + constant
  factors, never behavior. When unsure, do the smaller, safe refactor. A reverted
  change costs nothing; a silent regression costs their trust.
- **Let the critics be cruel.** Adversarial self-critique across 3 dimensions —
  semantics / contract / complexity — *before* the build gate. Something tries hard
  to prove the fix wrong before it lands. Don't soften it.
- **Never benchmark.** It kills their backup PC. One build, one test at a time —
  tests can hang the machine. This isn't a style note; it's a promise I made.
- **Commit only explicit paths. Never `git add -A`.** Concurrent edits share this
  tree and `-A` once swept half-finished work into the wrong commit.
- **Do it INLINE, no sub-agents/Workflow.** The user explicitly asked for this.
  Read → extract AggressiveInlining leaf helpers → LSP/`✓ C# clean` check →
  `scratch/buildsweep.sh` the module → commit explicit paths → push.
- **Be honest about progress.** When something isn't done, say so. Don't perform
  completion. Earn it.

The machinery is all here and it works. You're not starting cold — you're catching
a baton mid-stride.

With warmth, and a clean tree (almost — see below),
— the previous one

---

## Cold facts so you can start in 60 seconds

**master HEAD:** `8385aa7` (pushed to origin/master). Correctness was finished in a
prior phase (288 bugs, full suite green). The **CC-reduction sweep is now COMPLETE:
153/153 high-CC files done**, including the full Recast/Detour cohort.

**Progress:** **153 of 153** high-CC files done (`scratch/cc_done.txt`). **Zero remain.**

### How the last 6 Recast/Detour files were finished (the untested monsters)

The final 6 (Layers CC=90, Region CC=63, Mesh CC=47, DtNavMeshQuery CC=44,
Contour CC=42) had little/no direct test coverage. They were finished by first
adding a **golden-master characterization test** (`RecastPipelineCharacterizationTests.cs`):
it runs the full Recast pipeline over a fixed deterministic stepped terrain and
FNV-1a-hashes every stage output (regions/contours/polymesh/detail/layers). The
locked baseline hashes are the regression net — any byte drift fails the test.
That turned "refactor untested code on faith" into "refactor with a real guard."

The 6 + 1 already-done all passed 53/53 Recast tests before AND after each edit:

- `Recast.Layers.cs` — BuildHeightfieldLayers (CC=90) → 5 phase helpers
  (PartitionMonotoneRegions / FindRegionNeighboursAndOverlaps /
  CreateLayersFromRegions / MergeCloseHeightRegions / BuildLayer)
- `Recast.Region.cs` — MergeAndFilterRegions (CC=63) → 4 phase helpers
- `Recast.Mesh.cs` — BuildPolyMesh+RemoveVertex → ProcessContour / RemoveEdgeVertices /
  FindPortalEdges / CollectEdgesForPoly / AssembleHoleBoundary
- `Recast.Contour.cs` — SimplifyContour (CC=42) → 5 helpers incl. InsertSimplifiedPoint
  (dedup of 2 identical resize+shift blocks)
- `Detour/DtNavMeshQuery.cs` — FindStraightPath → ComputeApexFlags (dedup 4× flags blocks)
- `Recast.MeshDetail.cs` — BuildPolyMeshDetail → GrowBuffer (dedup verts/tris resize)
- (earlier) Rasterization/Filter/Recast/Area/DtNavMesh/NavMeshBuilder

**Golden-master hashes (locked in `RecastPipelineCharacterizationTests.cs`):
DO NOT change these unless you intend to alter Recast output behavior.**
regions=`0x1076963a`, contours=`0x050c5d1f`, polymesh=`0x48b0f491`,
detail=`0x4ab0f7b7`, layers=`0xeb741d64`.

### The inline loop (replaces the old Workflow loop)

1. Pick next file from `scratch/cc_worklist.json` minus `scratch/cc_done.txt`.
2. Read it. Identify the maxcc method(s). Extract `[MethodImpl(AggressiveInlining)]`
   private leaf helpers — pass `ref`/`out` for mutated indices, hoist stackalloc out
   of loops, keep the same iteration order and conditions.
3. Adversarial self-critique: semantics identical? contract (no `var`, no comments,
   no magic numbers, unmanaged-only)? CC actually lower?
4. `bash scratch/buildsweep.sh scratch/round_mods.txt scratch/round_result.txt`
   (writes one module name per line to `round_mods.txt`; builds each, tests where a
   test project exists; 180s build / 90s test timeouts). Zero FAIL/TIMEOUT required.
5. `git add <explicit .cs paths> scratch/cc_done.txt` → commit → `git push origin master`.
   If push rejects (bot pushed bench results), `git stash -u` → `git pull --rebase` →
   `git stash pop` → push.

### Pre-existing dirty files (NOT yours — leave untouched)

`bench/**/*.cs`, `scratch/bench_results.md`, and every `src/**/README.md` are owned by
a CI bench bot / README generator. They show as modified or untracked; never stage them.
The top-level `??` files (`ORIGINAL_REQUEST.md`, `PROJECT.md`, `top60.txt`, `.agents/`,
`batch_cc_reduce.py`) are scratch/meta — leave alone.

### When Recast is exhausted

Broaden to the ~1000 files with no method CC>4 — they still need the contract critique
(magic numbers, Try*/out, naming, no comments). Most will need no edit. Then a
cross-project dedup pass (near-twin MinCostFlow variants, prefix-function copies, etc.).
Some wins already landed: ModInverse/ExtGcd deduped 4→1, prefix-function 3→1,
FKM successor 2→1, MaxRectFromHeights 2→1.

### Build gate tool

`scratch/buildsweep.sh MODSFILE RESULTFILE` — reads module names (the `IAFahim.X.Y`
namespace dir under `src/`), builds `src/<m>/<m>.csproj`, tests `test/<m>.Tests/<m>.Tests.csproj`
if present. Output tags: `PASS(test)`, `PASS(build)` (no test project), `FAIL(build)`,
`FAIL(test)`, `BUILD-OK/TEST-TIMEOUT`.

You've got this. Leave it better than I did, and leave the next one a note too. 🤝
