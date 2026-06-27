# Plan - Documentation Professionalization and Tone Alignment

## Mission
Fulfill all requirements in the Follow-up section of `ORIGINAL_REQUEST.md`. Specifically:
1. Update `src/IAFahim.Collections.NoDeps/README.md`, `src/IAFahim.Linear/README.md`, and `src/IAFahim.Search/README.md` to remove all informal "cat's voice" references and format them professionally.
2. Update the root `README.md` Package Index to list and link to them properly.
3. Perform a repository-wide scan of all markdown files to remove any residual "cat" references.
4. Keep the tone uniformly professional and formal, using pointer syntax matching the actual package API where appropriate.

---

## Decomposed Milestones

### Milestone 1: Exploration and Analysis
- **Goal**: Find all markdown files in the repository (excluding `.agents/` and `.antigravitycli/`).
- **Goal**: Locate all instances of the word "cat" (case-insensitive) or other informal/playful terms (like "paws", "purr", "prowl").
- **Goal**: Analyze `src/IAFahim.Collections.NoDeps/README.md` to find its current content and extract its API signatures (core types like `Allocator`, `AllocatorManager`, `UnsafeUtility`, etc.).
- **Goal**: Analyze `src/IAFahim.Linear/README.md` and `src/IAFahim.Search/README.md` to identify their current content.
- **Goal**: Analyze the root `README.md` to identify the Package Index format and content.
- **Worker**: `teamwork_preview_explorer` (Explorer)

### Milestone 2: Implementation / Update
- **Goal**: Rewrite `src/IAFahim.Collections.NoDeps/README.md` with:
  - **Description**: Stubs purpose (keeping builds happy under pure .NET without Unity dependencies).
  - **Complexity**: Big-O complexity for stub actions (N/A or constant O(1)).
  - **API Signature**: Core stub types (`Allocator`, `AllocatorManager`, `UnsafeUtility`, etc.).
  - **Usage Example**: A compilable, unmanaged C# pointer-based example using explicit types and cleanup.
- **Goal**: Rewrite `src/IAFahim.Linear/README.md` and `src/IAFahim.Search/README.md` "Use case" section to a professional description without the word "cat" or informal language.
- **Goal**: Update root `README.md` Package Index to include `IAFahim.Collections.NoDeps`, `IAFahim.Linear`, and `IAFahim.Search` with correct relative links.
- **Goal**: Edit other markdown files identified in Milestone 1 to clean up any "cat" references.
- **Worker**: `teamwork_preview_worker` (Worker)

### Milestone 3: Review and Tone Verification
- **Goal**: Verify that all acceptance criteria are met.
- **Goal**: Run static scans to ensure case-insensitive "cat" does not exist in any modified files or other repository md files.
- **Goal**: Confirm all links in root `README.md` are valid.
- **Worker**: `teamwork_preview_reviewer` (Reviewer)

---

## Verification Plan
1. Verification that "cat" is completely removed using grep or a custom regex scanner on all markdown files.
2. Link check of root README.md Package Index links.
3. Verification that code blocks in `src/IAFahim.Collections.NoDeps/README.md` are clean and correct C# syntax matching the actual types.
