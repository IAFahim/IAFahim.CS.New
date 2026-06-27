## 2026-06-22T15:37:50Z
You are worker_m2_batch_4. Your task is to perform Milestone 2 (Package README Generation) for the packages listed in:
`/home/l/Github/IAFahim.CS.New/.agents/orchestrator/batches/batch_4.txt`

Follow the JSON-aggregation strategy:
1. Initialize your own briefing and progress files in your working directory `.agents/worker_m2_batch_4/` (create this directory if it doesn't exist).
2. Write a Python script `.agents/worker_m2_batch_4/aggregate.py` that reads the package list from the batch file, gathers C# code and existing README.md contents (excluding bin, obj) for those packages, and writes them to `.agents/worker_m2_batch_4/inputs.json`.
3. Run `aggregate.py`.
4. Read `inputs.json` and generate the professional README markdown for each package.
   Requirements for each README:
   - Exactly these headers: # {package_name}, ## Description, ## Complexity, ## API Signature, ## Usage Example.
   - Professional tone. The word "cat" (case-insensitive) is strictly forbidden in the entire README.
   - Avoid using any word in the explanation that contains the letters 'c', 'a', 't' in sequence (e.g., do NOT use "category", "concatenate", "catch", "location", "allocate", "duplicate", "multiplication", etc.). Instead, use alternative terms like "group" / "type", "merge" / "combine", "intercept" / "handle", "position" / "offset", "reserve" / "provision", "copy" / "replicate", "product" / "multiply".
   - Usage example: unsafe, raw pointers, no var, no managed arrays, try/finally with AllocHGlobal/FreeHGlobal, no comments.
5. Write all generated README markdowns to `.agents/worker_m2_batch_4/outputs.json`.
6. Write and run a Python script `.agents/worker_m2_batch_4/distribute.py` that reads `outputs.json` and writes the generated READMEs to their respective directories under `src/`.
7. Perform verification, write `.agents/worker_m2_batch_4/handoff.md`, and message parent (conversation ID: e6ac97ba-c343-44f1-8774-a8c03327fce8) when done.
