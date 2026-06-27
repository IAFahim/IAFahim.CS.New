export const meta = {
  name: 'algo-judge-only',
  description: 'Re-judge already-applied fixes (two-judge gate) without re-fixing',
  phases: [{ title: 'Judge' }],
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

function judgePrompt(item, n) {
  return `You are adversarial code judge #${n}. A fixer already modified ONE file (changes are in the working tree, uncommitted). Your job is to FIND ANY MISTAKE — default to skepticism.

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
const sliceFile = (A && A.sliceFile) || 'scratch/round4_slice.json'

const SLICE_SCHEMA = {
  type: 'object', additionalProperties: false, required: ['items'],
  properties: { items: { type: 'array', items: {
    type: 'object', additionalProperties: false, required: ['file', 'findingsPath'],
    properties: { file: { type: 'string' }, findingsPath: { type: 'string' } } } } },
}
const boot = await agent(
  `Read ${sliceFile} (a JSON array of {file, findingsPath}). Return {items: [...]} containing every element verbatim, in order.`,
  { label: 'load-slice', phase: 'Judge', schema: SLICE_SCHEMA }
)
const items = boot.items
log(`Judging ${items.length} already-applied fixes`)

const verdicts = await pipeline(
  items,
  (item) => parallel([
    () => agent(judgePrompt(item, 1), { label: `judge1:${item.file.split('/').pop()}`, phase: 'Judge', schema: JUDGE_SCHEMA }),
    () => agent(judgePrompt(item, 2), { label: `judge2:${item.file.split('/').pop()}`, phase: 'Judge', schema: JUDGE_SCHEMA }),
  ]).then(([j1, j2]) => {
    const a1 = j1 && j1.approve, a2 = j2 && j2.approve
    return {
      file: item.file,
      findingsPath: item.findingsPath,
      accepted: !!(a1 && a2),
      judge1: j1 ? { approve: a1, blocking: j1.blocking || [], notes: j1.notes } : null,
      judge2: j2 ? { approve: a2, blocking: j2.blocking || [], notes: j2.notes } : null,
    }
  })
)

const clean = verdicts.filter(Boolean)
const accepted = clean.filter(v => v.accepted)
const rejected = clean.filter(v => !v.accepted)
log(`Accepted ${accepted.length}/${clean.length}; rejected ${rejected.length}`)
return { total: clean.length, acceptedCount: accepted.length, accepted: accepted.map(v => v.file), rejected }
