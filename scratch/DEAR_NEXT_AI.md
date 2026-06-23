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

**master HEAD:** `6a1b0da` (pushed to origin/master). Correctness was finished in a
prior phase (288 bugs, full suite green) — CC + contract sweep was the active job.

**Progress:** **141 of 153** high-CC files done (`scratch/cc_done.txt`).
**ALL non-Recast files are DONE and pushed.** Only the **12-file
`IAFahim.Pathfinding.Recast` cohort remains** — see below.

### What's left: the Recast cohort (12 files)

`IAFahim.Pathfinding.Recast` is a recastnavigation port. `scratch/cc_worklist.json`
flags it worst-first (e.g. `Recast.Layers.cs` maxcc=90). It was DEFERRED because:

- It's var-laden. **AGENTS.md forbids `var`** — any new helper MUST use explicit types.
- It's a port of external C++ with subtle mesh/heightfield geometry — high regression
  risk, and (verify) it may have no test project, leaving build-only verification.
- The human's instruction was "keep going till all"; the deferral was a prior-model
  judgment call, not a hard constraint. If you take it on: go slow, one method at a
  time, preserve exact control flow, and treat the `var`→explicit conversion as the
  primary contract fix. A prior `Recast.Layers` attempt was rejected for `var`.

Run this to list the 12 and their flagged methods:

```
python3 -c "import json;[print(e['file'],e['maxcc'],e['methods']) for e in json.load(open('scratch/cc_worklist.json')) if 'Recast' in e['file']]"
```

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
