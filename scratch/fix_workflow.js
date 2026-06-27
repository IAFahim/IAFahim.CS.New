export const meta = {
  name: 'algo-fix-double-judged',
  description: 'Fix correctness+perf per file with a two-judge gate',
  phases: [{ title: 'Fix' }, { title: 'Judge' }],
}

const FIX_SCHEMA = {
  type: 'object', additionalProperties: false,
  required: ['file', 'correctnessFixed', 'perfApplied', 'perfSkipped', 'summary'],
  properties: {
    file: { type: 'string' },
    correctnessFixed: { type: 'number' },
    correctnessRejected: { type: 'array', items: { type: 'object', additionalProperties: true } },
    perfApplied: { type: 'number' },
    perfSkipped: { type: 'number' },
    riskNotes: { type: 'string' },
    summary: { type: 'string' },
    changed: { type: 'boolean' },
  },
}
const JUDGE_SCHEMA = {
  type: 'object', additionalProperties: false, required: ['approve', 'notes'],
  properties: {
    approve: { type: 'boolean' },
    blocking: { type: 'array', items: { type: 'object', additionalProperties: true } },
    notes: { type: 'string' },
  },
}

const HOUSE = `HOUSE CONSTRAINTS: static classes / unmanaged structs only; raw pointers (T*), stackalloc; NO managed arrays/List/Dictionary/string/interfaces/managed exceptions/var in src/; explicit types; named constants; [MethodImpl(AggressiveInlining)] on hot leaf methods. "Unchecked" Run methods assume the CALLER guarantees valid input (non-null, valid len) BY DESIGN — missing such guards is NOT a bug. Try* methods must validate. Code must stay Burst-compatible (blittable, no boxing).`

function fixPrompt(item) {
  return `You are an elite systems engineer AND Codeforces-grandmaster fixing ONE file to PERFECTION in an unmanaged C# Burst/Unity algorithm library. You cannot benchmark; reason about cost.

FILE: ${item.file}
FINDINGS: ${item.findingsPath}  (JSON with .correctness[] and .perf[] arrays)

STEPS:
1. Read the findings JSON and the full source file.
2. For EACH correctness finding: independently confirm it is a REAL defect (think through the triggering input). If real, fix it correctly and COMPLETELY (no half-fix, no new edge bug). If it is a false positive or by-design per the contract, do NOT change code for it.
3. For EACH perf opportunity: apply ONLY if genuinely SEMANTICS-PRESERVING (identical outputs for all valid inputs) and within house constraints. If it could change behavior, or you are unsure, SKIP it — correctness and safety outrank speed. Prefer high/medium impact; apply safe low-impact ones too ("every cycle matters") but never at the cost of risk.
4. ${HOUSE}
5. Preserve the public API/signature unless an ergonomics finding clearly improves it while staying source-compatible with existing tests/callers. When in doubt, keep the signature.
6. Edit ONLY ${item.file} (never other files, never csproj). Use Edit/Write. Then run \`git diff -- ${item.file}\` and re-read the result to self-verify: balanced braces, declared types, no undefined identifiers, compiles in your head, and is correct.

Be thorough but conservative. A correct, slightly-less-optimal file beats a fast broken one. Return the structured result.`
}

function judgePrompt(item, n) {
  return `You are adversarial code judge #${n}. A fixer just modified ONE file. Your job is to FIND ANY MISTAKE — default to skepticism.

FILE: ${item.file}
FINDINGS: ${item.findingsPath}

STEPS:
1. Run \`git diff -- ${item.file}\` to see exactly what changed (committed HEAD vs working tree). Read the full current file as needed.
2. Verify ALL of:
   (a) Every REAL correctness bug in the findings is actually fixed, and fixed CORRECTLY (no partial fix, no newly introduced edge bug).
   (b) The change does NOT break any previously-correct behavior or alter observable semantics for valid inputs (beyond the intended fixes).
   (c) NO new defect introduced: integer overflow, off-by-one, out-of-bounds read/write, uninitialized stackalloc, wrong/narrowed types, sign errors, modular-negative, Burst/unmanaged violations (managed types, boxing, exceptions), OR compile errors (undeclared identifiers, type mismatches, unbalanced braces, wrong signatures, missing usings).
   (d) Any applied perf change is truly semantics-preserving.
3. ${HOUSE}
4. Be concrete: cite the exact line/symbol for any problem. Any real problem is BLOCKING.

Set approve=true ONLY if you found ZERO blocking issues. Otherwise approve=false and list them.`
}

let A = args
if (typeof A === 'string') { try { A = JSON.parse(A) } catch (e) { A = {} } }
const offset = (A && A.offset) || 0
const limit = (A && A.limit) || 30

const SLICE_SCHEMA = {
  type: 'object', additionalProperties: false, required: ['items'],
  properties: { items: { type: 'array', items: {
    type: 'object', additionalProperties: false, required: ['file', 'findingsPath'],
    properties: { file: { type: 'string' }, findingsPath: { type: 'string' } } } } },
}
const planFile = (A && A.planFile) || 'scratch/fix_order.json'
const boot = await agent(
  `Read ${planFile} (a JSON array of {file, findingsPath}). Return {items: [...]} containing ONLY elements at indices ${offset} through ${offset + limit - 1} inclusive (slice offset=${offset}, limit=${limit}). Return them verbatim, in order. If fewer remain, return what exists.`,
  { label: 'load-slice', phase: 'Fix', schema: SLICE_SCHEMA }
)
const items = boot.items
log(`Processing ${items.length} files (offset ${offset}, limit ${limit})`)

const verdicts = await pipeline(
  items,
  (item) => agent(fixPrompt(item), { label: `fix:${item.file.split('/').pop()}`, phase: 'Fix', schema: FIX_SCHEMA })
    .then(fix => ({ item, fix })),
  (prev, item) => {
    if (!prev || !prev.fix) return { file: item.file, accepted: false, reason: 'fixer-failed' }
    return parallel([
      () => agent(judgePrompt(item, 1), { label: `judge1:${item.file.split('/').pop()}`, phase: 'Judge', schema: JUDGE_SCHEMA }),
      () => agent(judgePrompt(item, 2), { label: `judge2:${item.file.split('/').pop()}`, phase: 'Judge', schema: JUDGE_SCHEMA }),
    ]).then(([j1, j2]) => {
      const a1 = j1 && j1.approve, a2 = j2 && j2.approve
      return {
        file: item.file,
        accepted: !!(a1 && a2),
        fix: prev.fix,
        judge1: j1 ? { approve: a1, blocking: j1.blocking || [], notes: j1.notes } : null,
        judge2: j2 ? { approve: a2, blocking: j2.blocking || [], notes: j2.notes } : null,
      }
    })
  }
)

const clean = verdicts.filter(Boolean)
const accepted = clean.filter(v => v.accepted)
const rejected = clean.filter(v => !v.accepted)
log(`Accepted ${accepted.length}/${clean.length}; rejected ${rejected.length}`)
return { offset, limit, total: clean.length, acceptedCount: accepted.length, accepted: accepted.map(v => v.file), rejected }
