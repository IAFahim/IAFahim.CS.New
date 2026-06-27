import os

raw_path = '/home/l/Github/IAFahim.CS.New/.agents/explorer_m1_discovery/handoff_raw.md'
handoff_path = '/home/l/Github/IAFahim.CS.New/.agents/explorer_m1_discovery/handoff.md'

with open(raw_path, 'r', encoding='utf-8') as f:
    raw_content = f.read()

# Remove the top header of raw_content to integrate it smoothly
lines = raw_content.split('\n')
if lines[0].startswith('# Discovery Report'):
    lines = lines[3:] # Skip the title and intro sentence
conclusion_content = '\n'.join(lines)

handoff_template = f"""# Handoff Report — Milestone 1 Setup & Discovery

This report details the discovery of all packages, their C# files, public API signatures, and README descriptions in the workspace.

## 1. Observation
- Verified that the codebase contains exactly 153 package directories (each containing a `.csproj` file) using the command:
  ```bash
  find src -name "*.csproj" | wc -l
  ```
  which returned `153`.
- Directly inspected `src/IAFahim.Sort.Insertion/README.md` to confirm the presence of the informal "cat's voice" description:
  ```markdown
  # IAFahim.Sort.Insertion

  ## Use case
  Curious cat uses insertion sort for tiny arrays or nearly sorted data. It is simple and cache‑friendly.
  ```
- Directly inspected `src/IAFahim.Sort.Insertion/Insertion.cs` to confirm the public static class and method signatures:
  ```csharp
  public static unsafe class Insertion
  {{
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
      ...
  }}
  ```
- Successfully ran a custom Python parsing tool `.agents/explorer_m1_discovery/discovery.py` to recursively extract data from all 153 packages.

## 2. Logic Chain
- As defined by `AGENTS.md`, each folder in `src/` containing a `.csproj` file constitutes an independent package (e.g. Algorithm or Data Structure).
- Reading files matching `*.cs` inside each package (excluding build output dirs `bin` and `obj`) exposes all public classes, structs, and their members.
- Programmatically parsing the source files with Python regexes matches keywords like `public static class`, `public struct`, and nested public methods/fields.
- Reading the `README.md` at each package's root directory retrieves the cat's voice "Use case" section.
- Compiling these details yields a complete and accurate map of the repository's API surface area and metadata.

## 3. Caveats
- The python C# signature extractor relies on regexes optimized for standard C# formatting. If methods or classes span multiple lines with uncommon indentation or are hidden under conditional compiler directives, signatures may be slightly truncated.
- Several package folders do not have a `README.md` file (reported as "No README.md").
- Some packages have `README.md` files but lack a dedicated "## Use case" header, in which case the script falls back to the first non-header text block found.

## 4. Conclusion
The programmatically extracted mapping of the 153 packages, their files, public APIs, and descriptions is detailed below.

{conclusion_content}

## 5. Verification Method
- **Spot-check verification**:
  Choose any package (e.g., `src/IAFahim.DS.Dsu`), look up its entry in the list below, and verify that the listed classes (`DsuInit`, `DsuFind`, etc.) and public methods (`Run`, `RunPathCompression`) match the contents of `src/IAFahim.DS.Dsu/Dsu.cs`.
- **Re-run tool command**:
  Run the discovery tool to regenerate the raw list:
  ```bash
  python3 .agents/explorer_m1_discovery/discovery.py
  ```
- **Compilation verification**:
  Run the dotnet build command on the workspace solution to ensure all discovered package files compile successfully:
  ```bash
  dotnet build IAFahim.CS.sln
  ```
"""

with open(handoff_path, 'w', encoding='utf-8') as f:
    f.write(handoff_template)

print(f"Handoff report successfully written to {handoff_path}")
