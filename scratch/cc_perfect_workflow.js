export const meta = {
  name: 'cc-perfect',
  description: 'Reduce cyclomatic complexity + enforce AGENTS.md contract per file, gated by adversarial critics',
  phases: [
    { title: 'Fix', detail: 'extract helpers, lower CC, preserve semantics' },
    { title: 'Critique', detail: 'independent adversarial critics per dimension' },
    { title: 'Revise', detail: 'address any critic problem, then re-critique correctness' },
  ],
}

// args: { files: [absPaths...] }  OR  { offset, limit } against scratch/cc_worklist.json (read by orchestrator and passed in as files)
let A = args
if (typeof A === 'string') A = JSON.parse(A)
const FILES = A.files

const AGENTS_RULES = `House contract (AGENTS.md) for this unmanaged Burst/Unity C# algorithm library:
- No managed types: no string/object/dynamic, no T[], no List/Dictionary, no new on classes, no managed exceptions. Raw pointers T*, void*, nint/nuint, stackalloc only.
- Totality: unchecked Run<T>(T* ptr,int len) when caller guarantees validity; checked variants use Try* prefix, return bool, out result. No throwing for invalid input in Try* — return false, out=default.
- No magic numbers: named const only (introduce a private const with a domain name; do NOT invent values).
- No var. Explicit types everywhere. No comments (names carry meaning).
- [MethodImpl(MethodImplOptions.AggressiveInlining)] on extracted hot-path leaf helpers.
- Data separated from behavior. No hidden mutable static state. Pure deterministic functions.
- Bounds check pattern: (uint)index < (uint)length.
- Allocation byte sizes widen first: (long)length * sizeof(T).`

const FIX_SCHEMA = {
  type: 'object',
  additionalProperties: false,
  required: ['file', 'changed', 'methodsRefactored', 'semanticsPreserved', 'notes'],
  properties: {
    file: { type: 'string' },
    changed: { type: 'boolean' },
    methodsRefactored: { type: 'array', items: { type: 'string' } },
    semanticsPreserved: { type: 'boolean' },
    notes: { type: 'string' },
  },
}

const CRITIC_SCHEMA = {
  type: 'object',
  additionalProperties: false,
  required: ['dimension', 'verdict', 'issues'],
  properties: {
    dimension: { type: 'string' },
    verdict: { type: 'string', enum: ['CLEAN', 'PROBLEM'] },
    issues: { type: 'array', items: { type: 'string' } },
  },
}

function fixPrompt(file) {
  return `${AGENTS_RULES}

TASK: Refactor the file at ${file} to REDUCE CYCLOMATIC COMPLEXITY while preserving EXACT semantics and memory-mutation behavior.

Primary technique: extract deeply nested loop bodies / branch blocks into private static helpers decorated with [MethodImpl(MethodImplOptions.AggressiveInlining)]. The extracted helper must receive ALL state it reads (pass pointers for buffers, ref for scalars the caller expects mutated). A loop body containing continue/break/return must be translated so the loop's control flow is preserved exactly (e.g. trailing continue -> helper returns and caller loops; a return-from-method stays a return in the caller).

Secondary (ONLY when trivially safe and behavior-identical): replace literal magic numbers with named private const; keep explicit types (no var). Do NOT change algorithm behavior, output, ordering, or numeric results. Do NOT introduce new allocation. Do NOT touch public signatures unless they already violate the contract AND there are no callers (rare — default: leave signatures alone).

Read the file, apply edits with the Edit tool, then return the result. If the file is already low-CC and clean, set changed=false and do not edit. Be conservative: a smaller correct extraction beats an aggressive risky rewrite. Set semanticsPreserved=false ONLY if you could not refactor without risking behavior (then also set changed=false / revert your edits).`
}

function criticPrompt(file, dimension, focus) {
  return `You are an ADVERSARIAL critic for an unmanaged Burst/Unity C# algorithm library. A file was just refactored to reduce cyclomatic complexity. Get the diff:  git diff HEAD -- ${file}  (run from /home/l/Github/IAFahim.CS.New). Also read the file for context.

Your single dimension: ${dimension}.
${focus}

Try HARD to find a real defect on THIS dimension. Default to suspicion. Style nitpicks do not count — only real issues. Return verdict CLEAN only if you genuinely find nothing wrong on your dimension.`
}

const CRITICS = [
  ['semantics', 'Did the refactor change behavior? Check extracted helpers receive every value they read; that by-value params the original mutated-and-relied-on are now ref/pointer; that continue/break/return semantics are preserved; no reordering of side effects; pointer index arithmetic unchanged; no off-by-one, sign, or overflow change. This is the most important dimension.'],
  ['contract', 'AGENTS.md compliance: no managed types introduced, no var, no new magic numbers (any literal that should be a named const?), Try* returns bool+out (no throw), AggressiveInlining present on extracted leaf helpers, no hidden mutable static state introduced, no struct wrapping a single primitive.'],
  ['complexity', 'Was cyclomatic complexity actually reduced (not just shuffled), is each function now one job, no logic duplication introduced by the extraction (same block copy-pasted instead of shared), and no dead/unused parameters left on helpers?'],
]

phase('Fix')
const results = await pipeline(
  FILES,
  (file) => agent(fixPrompt(file), { label: `fix:${file.split('/').pop()}`, phase: 'Fix', schema: FIX_SCHEMA })
    .then((r) => ({ file, fix: r })),
  async (prev) => {
    const file = prev.file
    if (!prev.fix || !prev.fix.changed) {
      return { file, status: prev.fix ? 'unchanged' : 'fixfail', critics: [], fix: prev.fix }
    }
    const critics = (await parallel(CRITICS.map(([dim, focus]) => () =>
      agent(criticPrompt(file, dim, focus), { label: `crit:${dim}:${file.split('/').pop()}`, phase: 'Critique', schema: CRITIC_SCHEMA })
    ))).filter(Boolean)
    const problems = critics.filter((c) => c.verdict === 'PROBLEM')
    if (problems.length === 0) {
      return { file, status: 'accept', critics, fix: prev.fix }
    }
    // Revise once against the problems, then re-run the semantics + contract critics.
    const problemText = problems.map((p) => `[${p.dimension}] ${p.issues.join('; ')}`).join('\n')
    phase('Revise')
    const rev = await agent(
      `${AGENTS_RULES}\n\nThe refactor of ${file} was flagged by critics:\n${problemText}\n\nFix ONLY these issues with the Edit tool, preserving exact semantics. If an issue cannot be fixed safely, REVERT that part to match HEAD (git diff HEAD -- ${file} shows current state). Return the result.`,
      { label: `revise:${file.split('/').pop()}`, phase: 'Revise', schema: FIX_SCHEMA }
    )
    const recheck = (await parallel(CRITICS.slice(0, 2).map(([dim, focus]) => () =>
      agent(criticPrompt(file, dim, focus), { label: `recheck:${dim}:${file.split('/').pop()}`, phase: 'Revise', schema: CRITIC_SCHEMA })
    ))).filter(Boolean)
    const stillBad = recheck.filter((c) => c.verdict === 'PROBLEM')
    return { file, status: stillBad.length ? 'reject' : 'accept', critics: critics.concat(recheck), fix: rev || prev.fix }
  }
)

const accepted = results.filter((r) => r && r.status === 'accept').map((r) => r.file)
const rejected = results.filter((r) => r && r.status === 'reject').map((r) => r.file)
const unchanged = results.filter((r) => r && r.status === 'unchanged').map((r) => r.file)
const failed = results.filter((r) => r && r.status === 'fixfail').map((r) => r.file)
log(`accept=${accepted.length} reject=${rejected.length} unchanged=${unchanged.length} fixfail=${failed.length}`)
return { accepted, rejected, unchanged, failed, detail: results }
