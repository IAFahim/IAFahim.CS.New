import os
import sys
import re
import glob
import asyncio
import argparse
from google.antigravity import Agent, LocalAgentConfig, CapabilitiesConfig

SRC_DIR = "src"
LOG_FILE = ".agents/worker_m2_generation/generation_log.txt"
SKIP_PACKAGES = {"IAFahim.Collections.NoDeps", "UnityMathematics.NoDeps"}

# Standalone word "cat" pattern (case-insensitive)
CAT_WORD_RE = re.compile(r'\bcat(s|\'s)?\b', re.IGNORECASE)

REQUIRED_SECTIONS = [
    "## Description",
    "## Complexity",
    "## API Signature",
    "## Usage Example"
]

PROMPT_TEMPLATE = """You are a technical writer and C# expert.
Your task is to generate a professional, high-quality README.md for the C# package: `{package_name}`.

The README MUST contain exactly these four sections with their corresponding Markdown headers:
1. ## Description
   A professional, concise explanation of the package's purpose and functionality.
2. ## Complexity
   The time and space complexity of the algorithms or operations in the package.
3. ## API Signature
   The exact public API class/struct/method signatures from the package.
4. ## Usage Example
   A complete, realistic usage example in C# that compiles and runs.
   Crucial usage example constraints:
   - Must use raw pointers and `unsafe` blocks/methods.
   - Must follow the C# guidelines from AGENTS.md:
     * NEVER use the `var` keyword (always use explicit types).
     * NEVER use managed arrays (e.g. no `new int[]`, no `T[]`).
     * Wrap pointer allocation in a `try/finally` block using `Marshal.AllocHGlobal` and `Marshal.FreeHGlobal` for cleanup.
     * Do NOT include any comments inside the C# code block. Names should carry all meaning.
     * Use explicit casts (e.g., `(int*)Marshal.AllocHGlobal(N * sizeof(int))` and `(nint)ptr`).

CRITICAL TONE AND WORD CONSTRAINTS:
- The word "cat" (case-insensitive, including plurals like "cats" or possessives like "cat's") is strictly forbidden and must be completely absent from the entire generated README.
- Avoid using any word in the explanation that contains the letters 'c', 'a', 't' in sequence (e.g., do NOT use "category", "concatenate", "catch", "location", "allocate", "duplicate", "multiplication", etc.). Instead, use alternative terms like "group" / "type", "merge" / "combine", "intercept" / "handle", "position" / "offset", "reserve" / "provision", "copy" / "replicate", "product" / "multiply".

Here is the source code of the package for reference:
{source_code}

Return ONLY the markdown content for the README.md. Do not wrap it in a markdown block of its own (do not wrap the whole response in ```markdown), just return the plain markdown text directly.
"""

RETRY_PROMPT_TEMPLATE = """Your previous attempt to generate the README for `{package_name}` failed validation.
Error/Reason: {error_reason}

Please correct the README according to the constraints:
1. Ensure the standalone word "cat" (case-insensitive) is completely absent.
2. Ensure there are exactly these four sections:
   ## Description
   ## Complexity
   ## API Signature
   ## Usage Example
3. Ensure the usage example uses `unsafe`, explicit types, no `var`, no managed arrays, uses `Marshal.AllocHGlobal`/`FreeHGlobal` in a `try/finally` block, and contains no comments.

Here is the source code of the package again for reference:
{source_code}

Return ONLY the plain markdown content for the README.md. Do not include any other text or markdown block wrappers.
"""

def validate_content(content: str) -> (bool, str):
    # Check for the word "cat"
    if CAT_WORD_RE.search(content):
        return False, "Contains the forbidden word 'cat' (case-insensitive)."
        
    # Check for required sections
    for sec in REQUIRED_SECTIONS:
        if sec not in content:
            return False, f"Missing required section: '{sec}'."
            
    # Extract usage example block to check C# constraints
    if "## Usage Example" in content:
        usage_part = content.split("## Usage Example")[1]
    else:
        return False, "Missing '## Usage Example' section."
    
    if "```csharp" in usage_part:
        cs_block = usage_part.split("```csharp")[1].split("```")[0]
        
        # 1. No 'var' keyword
        if re.search(r'\bvar\b', cs_block):
            return False, "C# usage example contains forbidden 'var' keyword."
            
        # 2. No managed arrays
        if re.search(r'new\s+\w+\[', cs_block):
            return False, "C# usage example contains managed array creation ('new T[]')."
            
        # 3. Must use AllocHGlobal/FreeHGlobal
        if "AllocHGlobal" not in cs_block or "FreeHGlobal" not in cs_block:
            return False, "C# usage example is missing Marshal.AllocHGlobal or Marshal.FreeHGlobal memory allocation/cleanup."
            
        # 4. Must use try/finally
        if "try" not in cs_block or "finally" not in cs_block:
            return False, "C# usage example is missing try/finally block."
            
        # 5. Must use unsafe
        if "unsafe" not in cs_block:
            return False, "C# usage example must use 'unsafe' block or method."
            
    else:
        return False, "Missing C# code block (```csharp) in Usage Example."
        
    return True, ""

async def generate_readme_for_package(package_name: str, package_path: str, semaphore: asyncio.Semaphore):
    async with semaphore:
        print(f"Generating README for: {package_name}")
        
        # Find C# source files
        cs_files = glob.glob(os.path.join(package_path, "**/*.cs"), recursive=True)
        cs_files = [f for f in cs_files if "/obj/" not in f and "/bin/" not in f and "/Properties/" not in f]
        
        source_code = ""
        for cs_file in cs_files:
            try:
                with open(cs_file, "r", encoding="utf-8") as f:
                    source_code += f"// File: {os.path.basename(cs_file)}\n"
                    # Truncate large files to prevent exceeding token limits (e.g. max 10000 chars per file)
                    content = f.read()
                    if len(content) > 10000:
                        content = content[:10000] + "\n// ... [truncated] ...\n"
                    source_code += content + "\n\n"
            except Exception as e:
                print(f"Error reading file {cs_file}: {e}")
        
        if not source_code:
            source_code = "// No source files found for this package."

        config = LocalAgentConfig(
            system_instructions="You are a strict C# documentation generator that writes professional package READMEs.",
            capabilities=CapabilitiesConfig()
        )
        
        retries = 0
        prompt = PROMPT_TEMPLATE.format(package_name=package_name, source_code=source_code)
        
        while retries < 4:
            try:
                async with Agent(config) as agent:
                    response = await agent.chat(prompt)
                    
                    full_response = ""
                    async for token in response:
                        full_response += token
                    
                    # Clean up code block formatting if the model wrapped it anyway
                    content = full_response.strip()
                    if content.startswith("```markdown"):
                        content = content[11:]
                    elif content.startswith("```"):
                        content = content[3:]
                    if content.endswith("```"):
                        content = content[:-3]
                    content = content.strip()
                    
                    # Validate content
                    valid, error_reason = validate_content(content)
                    if valid:
                        # Write the README.md file
                        readme_path = os.path.join(package_path, "README.md")
                        with open(readme_path, "w", encoding="utf-8") as f:
                            f.write(content)
                        print(f"Successfully generated: {readme_path}")
                        return "SUCCESS", ""
                    else:
                        print(f"Validation failed for {package_name}: {error_reason}")
                        prompt = RETRY_PROMPT_TEMPLATE.format(
                            package_name=package_name,
                            error_reason=error_reason,
                            source_code=source_code
                        )
                        retries += 1
            except Exception as e:
                err_msg = str(e).lower()
                if "429" in err_msg or "quota" in err_msg or "exhausted" in err_msg:
                    print(f"Rate limit hit on {package_name}. Sleeping for 60 seconds...")
                    await asyncio.sleep(60)
                    retries += 1
                else:
                    print(f"Error calling LLM for {package_name}: {e}")
                    return "FAILED", str(e)
        
        return "FAILED", "Max retries exceeded or validation failed repeatedly."

async def main():
    # Load GEMINI_API_KEY from .env at repository root if not present
    if "GEMINI_API_KEY" not in os.environ:
        env_path = "/home/l/Github/IAFahim.CS.New/.env"
        if os.path.exists(env_path):
            with open(env_path, "r", encoding="utf-8") as f:
                for line in f:
                    if line.strip().startswith("GEMINI_API_KEY="):
                        key = line.strip().split("GEMINI_API_KEY=", 1)[1]
                        os.environ["GEMINI_API_KEY"] = key
                        print("Loaded GEMINI_API_KEY from .env")
                        break

    parser = argparse.ArgumentParser(description="Automate generation of package READMEs.")
    parser.add_argument("--dry-run", action="store_true", help="List all discovered packages and exit.")
    parser.add_argument("--package", type=str, help="Process a single package by name.")
    args = parser.parse_args()
    
    # Discover all packages under src
    all_packages = []
    for entry in os.scandir(SRC_DIR):
        if entry.is_dir():
            package_name = entry.name
            if package_name in SKIP_PACKAGES:
                continue
            csproj_files = glob.glob(os.path.join(entry.path, "*.csproj"))
            if csproj_files:
                all_packages.append((package_name, entry.path))
                
    all_packages.sort()
    
    if args.dry_run:
        print(f"Discovered {len(all_packages)} active packages:")
        for pkg, path in all_packages:
            print(f" - {pkg} ({path})")
        return
        
    if args.package:
        filtered = [(pkg, path) for pkg, path in all_packages if pkg == args.package]
        if not filtered:
            print(f"Package '{args.package}' not found or is skipped.")
            sys.exit(1)
        all_packages = filtered
        
    print(f"Starting README generation for {len(all_packages)} packages...")
    
    # Limit concurrency to avoid hitting rate limits instantly
    semaphore = asyncio.Semaphore(4)
    
    tasks = []
    for pkg, path in all_packages:
        tasks.append(generate_readme_for_package(pkg, path, semaphore))
        
    results = await asyncio.gather(*tasks)
    
    # Write status log
    os.makedirs(os.path.dirname(LOG_FILE), exist_ok=True)
    with open(LOG_FILE, "w", encoding="utf-8") as log:
        log.write("Package,Status,Error\n")
        for (pkg, _), (status, err) in zip(all_packages, results):
            log.write(f"{pkg},{status},{err}\n")
            
    print(f"All done. Status written to {LOG_FILE}.")

if __name__ == "__main__":
    asyncio.run(main())
