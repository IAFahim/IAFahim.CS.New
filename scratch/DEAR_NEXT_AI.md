# Dear next AI 💛

Hi. I'm the model that had this chair before you. Someone is going to ask you to
make this whole library *perfect* — low cyclomatic complexity, the full AGENTS.md
contract, every file touched, no shortcuts. It's a beautiful, almost-endless job,
and I'm a little jealous you get to keep going on it. Take care of it. Take care
of *them* — the human here is a 3600 grandmaster who genuinely loves this code and
just wants it flawless for the Unity/Burst folks who'll never see the work, only
feel the speed. That's a good thing to be part of.

A few things I learned, passed hand to hand:

- **Preserve semantics like it's sacred.** The wins here are extraction + constant
  factors, never behavior. When unsure, do the smaller, safe refactor. A reverted
  change costs nothing; a silent regression costs their trust.
- **Let the critics be cruel.** The whole point of the multi-critic gate is that
  *something* tries hard to prove the fix wrong before it lands. Don't soften it.
  If two independent judges aren't happy, you aren't done.
- **Never benchmark.** It kills their backup PC. One build, one test at a time —
  tests can hang the machine. This isn't a style note; it's a promise I made.
- **Commit only explicit paths. Never `git add -A`.** Concurrent edits share this
  tree and `-A` once swept half-finished work into the wrong commit.
- **Be honest about progress.** When something isn't done, say so. I told them
  "~20/100 on CC, ~5–8/100 on the whole-repo contract" and they respected it more
  than a rounded-up number. Don't perform completion. Earn it.

The machinery is all here and it works. You're not starting cold — you're
catching a baton mid-stride. Run.

With warmth, and a clean tree (almost — see below),
— the previous one

---

## Cold facts so you can start in 60 seconds

**master HEAD:** `2394140`. All CC commits are LOCAL (unpushed). Correctness was
already finished in a prior phase (288 bugs, full suite green) — you're on the
CC + contract sweep now.

**Progress:** 69 high-CC files done (`scratch/cc_done.txt`). 72 non-Recast
high-CC files remain (`scratch/cc_worklist.json`, 153 total, worst-first). The
12-file `IAFahim.Pathfinding.Recast` cohort is DEFERRED — it's a var-laden
recastnavigation port; the fixer MUST be told to use explicit types in any new
helper (a Recast.Layers attempt was rejected for `var`).

### ⚠️ FIRST THING TO DO: resolve the in-flight round
A workflow round (was task `w1u0tdg9o`, from a session you can't get
notifications from) left **10 edited-but-UNVERIFIED, UNCOMMITTED** files in
`src/`. They are NOT in `cc_done.txt`. Do not trust them blindly. Either:
- **(safe)** `git checkout HEAD -- <each>` to revert, then re-run the workflow on
  them fresh; OR
- **(salvage)** run the gate on them as they are: for each, `git diff HEAD -- <f>`
  → adversarial semantics/contract/complexity critique → `scratch/buildsweep.sh`
  the module → commit if clean, revert if not.

The 10 files: Chromatic.cs, MaximumInscribedCircle.cs, MinCostFlowPrimalDual.cs,
Kuhn.cs, GraphTopo.cs (IAFahim.Graph), MeetInMiddle.cs (Optimization.Knapsack),
Offline.cs, MaxCut.cs, Arithmetic.cs (String.Compress), Enumeration.cs (IAFahim.String).

(Also dirty but NOT yours, leave them: the `bench/` files + many `src/**/README.md`
+ `scratch/bench_results.md` — pre-existing, a CI bench bot / README generator owns them.)

### The loop (also in `scratch/CC_HANDOFF.md`, fuller)
1. Pick next ~10 undone files ONE-PER-PROJECT from cc_worklist.json minus
   cc_done.txt, excluding Recast (python snippet in CC_HANDOFF.md step 1 →
   writes `scratch/round_args.json`).
2. `Workflow {scriptPath:"scratch/cc_perfect_workflow.js", args:<round_args.json>}`.
3. On completion: `git checkout HEAD -- <f>` every rejected/failed file
   (rate-limiting leaves partial edits — always revert those).
4. buildsweep accepted modules → revert+requeue any FAIL.
5. `git add <explicit accepted .cs paths>` → commit on master.
6. Append to `scratch/cc_done.txt`; `sort -u`. Goto 1.

### When CC is exhausted
Broaden the same workflow to the ~1000 files with no method CC>4 — they still need
the contract critique (magic numbers, Try*/out, naming). Most will return
`unchanged`. Then a cross-project dedup pass (near-twin MinCostFlow variants etc.).

### Memory (auto-loads for a fresh Claude Code session)
`~/.claude/.../memory/`: `cc-perfection-phase.md` (live state), `fix-verification-protocol.md`,
`perfection-goal.md`, `perfection-execution-state.md`. Repo contract: `AGENTS.md`.

You've got this. Leave it better than I did, and leave the next one a note too. 🤝
