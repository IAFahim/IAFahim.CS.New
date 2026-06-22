# CC-Perfection Handoff — resume here

Goal (user, 2026-06-22): reduce cyclomatic complexity AND enforce the full
AGENTS.md contract (out-param+bool `Try*`, no managed types, no magic numbers,
small pure one-job functions, no hidden state, no logic dup, AggressiveInlining
leaves) across every `src/` file. Adversarial critics gate every change. Never
benchmark. One project build / one test at a time (tests can hang the PC).
Commit only explicit verified paths — NEVER `git add -A`.

## State (as of master HEAD e492bea + rounds 4-6)

- 60 high-CC files refactored + committed (30 in this turn: rounds 4, 5, 6, each
  10 files one-per-project). Single inline agent (no Workflow/sub-agent tool in
  this harness): each file got fix + adversarial self-critique across 3
  dimensions (semantics / contract / complexity) + per-module build + test
  (where a test project exists).
- Tree is clean. All CC commits are LOCAL (unpushed, ahead of origin/master).
- `scratch/cc_done.txt` = 60 files already done (skip these).
- `scratch/cc_worklist.json` = 153 files with a method CC>=10, sorted worst-first.
- 81 non-Recast high-CC files remain + 12 Recast DEFERRED (var-laden
  recastnavigation port — explicit types required in any new helpers).

DECISION LOG (this turn):

- Tested files: full refactor + comment strip (tests catch regressions).
- Untested subtle algorithms (StableRoommates, Berlekamp): mechanical body-
  extraction only (semantics preserved by construction); comments KEPT
  (load-bearing invariant docs; correctness outranks no-comments on untested code).
- Repeatedly-found dup centralized into internal shared classes where same
  project/namespace: SpArrays, MstShared, StShared, DagShared. Cross-project
  near-twins left (e.g. MinCostFlow{Spfa,CapacityScaling}) — a later dedup pass.

NOTE: this harness has no Workflow/sub-agent tool. Run the gate INLINE: read
file -> refactor (extract AggressiveInlining helpers, preserve EXACT semantics)
-> self-critique semantics/contract/complexity -> build -> test (one project)
-> commit explicit paths -> append to cc_done.txt. One file fully through the
gate before the next.

## The machinery

- `scratch/cc_perfect_workflow.js` — per file: fix (extract AggressiveInlining
  helpers, lower CC, preserve EXACT semantics) -> 3 parallel adversarial critics
  (semantics / contract / complexity) -> revise+recheck on any PROBLEM ->
  returns {accepted, rejected, unchanged, failed}. Args: `{"files":[absPaths]}`.
- `scratch/buildsweep.sh MODSFILE RESULTFILE` — per-project build, then test if a
  `test/<Module>.Tests` exists; TIMEOUT reported separately from FAIL.

## Round loop (repeat until cc_worklist exhausted, then broaden to all files)

1. Pick next ~10 undone files ONE-PER-PROJECT (so a build/test FAIL isolates to
   one file) from cc_worklist.json minus cc_done.txt, excluding Recast:

   ```python
   import json,os
   done=set(l.strip() for l in open('scratch/cc_done.txt') if l.strip())
   wl=json.load(open('scratch/cc_worklist.json'))
   proj=lambda f:f.split('/')[1]
   undone=[w for w in wl if w['file'] not in done and proj(w['file'])!='IAFahim.Pathfinding.Recast']
   batch=[];seen=set()
   for w in undone:
     if proj(w['file']) in seen: continue
     seen.add(proj(w['file'])); batch.append(w)
     if len(batch)>=10: break
   json.dump({"files":[os.path.abspath(w['file']) for w in batch]}, open('scratch/round_args.json','w'))
   ```

2. `Workflow {scriptPath:"scratch/cc_perfect_workflow.js", args:<round_args.json contents>}` (runs in background).
3. On completion: `git checkout HEAD -- <file>` every rejected/failed file
   (rate-limited "fixfail" leaves partial edits — always revert them).
4. Write accepted modules (one per line, e.g. `IAFahim.Graph`) to a mods file;
   `bash scratch/buildsweep.sh <mods> <result>`. Revert+requeue any FAIL(build/test).
5. `git add <explicit accepted .cs paths>` (NEVER -A) and commit on master.
6. Append accepted paths to `scratch/cc_done.txt`; `sort -u` it. Goto 1.

## Next batch

Regenerate with the python snippet in step 1 (it auto-skips cc_done.txt +
Recast, picks 10 one-per-project worst-first). scratch/round_args.json holds
the most recent batch; re-run the snippet to advance.

## Watch out for

- Transient server rate-limiting kills fix agents mid-round (R3 lost 5). They
  come back as `failed`; revert any partial edit and requeue next round.
- 4 of R1's modules and several others have NO test project (PASS(build) only) —
  they rest on the critic gate. A later phase should add tests for these.
- After CC files are exhausted, broaden: the ~1000 files with no method CC>4 still
  need the contract critique (magic numbers, Try*/out, naming). Run them through
  the same workflow in batches; most will return `unchanged`.

## Background / prior context (memory files, persist across sessions)

`~/.claude/.../memory/`: `cc-perfection-phase.md` (live state, mirrors this),
`fix-verification-protocol.md` (2-judge gate), `perfection-goal.md` (perf lens,
never benchmark), `perfection-execution-state.md` (the completed correctness batch).
Repo contract: `AGENTS.md`.
