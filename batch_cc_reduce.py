import asyncio
import glob
import os
import sys
import time
from typing import List

try:
    from google.antigravity import Agent, LocalAgentConfig, CapabilitiesConfig
except ImportError:
    print("Please install google-antigravity SDK: pip install google-antigravity")
    sys.exit(1)

PROMPT_TEMPLATE = """You are a C# refactoring expert. 
Your task is to reduce the cyclomatic complexity of the following C# code. 
- Extract deeply nested logic into private static methods.
- Decorate all extracted methods with `[MethodImpl(MethodImplOptions.AggressiveInlining)]`.
- Remove ALL comments. 
- NEVER use the `var` keyword; always use explicit types.
- Ensure the code memory mutation footprint remains exactly identical.
- Return ONLY the full, completely modified C# code file in a markdown code block. Do not include any other text or explanation.

Here is the file content:
```csharp
{code}
```"""

async def process_file(file_path: str, semaphore: asyncio.Semaphore):
    async with semaphore:
        print(f"Processing: {file_path}")
        try:
            with open(file_path, "r", encoding="utf-8") as f:
                code = f.read()

            # Skip files that are already extremely simple to save quota
            if len(code) < 150 or ("if " not in code and "for " not in code and "while " not in code):
                print(f"Skipping {file_path} (too simple)")
                return

            config = LocalAgentConfig(
                system_instructions="You are a strict C# cyclomatic complexity reducer.",
                capabilities=CapabilitiesConfig()  # Read-only
            )
            
            retries = 0
            while retries < 5:
                try:
                    async with Agent(config) as agent:
                        response = await agent.chat(PROMPT_TEMPLATE.format(code=code))
                        
                        full_response = ""
                        async for token in response:
                            full_response += token
                        
                        # Extract the code from the markdown block
                        if "```csharp" in full_response:
                            new_code = full_response.split("```csharp")[1].split("```")[0].strip()
                        elif "```" in full_response:
                            new_code = full_response.split("```")[1].split("```")[0].strip()
                        else:
                            new_code = full_response.strip()

                        if len(new_code) > 50: # Sanity check
                            with open(file_path, "w", encoding="utf-8") as f:
                                f.write(new_code)
                            print(f"Success: {file_path}")
                        else:
                            print(f"Failed to extract valid code for {file_path}")
                        break
                except Exception as e:
                    err_msg = str(e).lower()
                    if "429" in err_msg or "quota" in err_msg or "exhausted" in err_msg:
                        print(f"Rate limit hit on {file_path}. Sleeping for 15 minutes...")
                        await asyncio.sleep(900)  # Wait 15 minutes on rate limit
                        retries += 1
                    else:
                        print(f"Error on {file_path}: {e}")
                        break

        except Exception as e:
            print(f"Unhandled error on {file_path}: {e}")

async def main():
    cs_files = glob.glob("src/**/*.cs", recursive=True)
    print(f"Found {len(cs_files)} C# files to process.")
    
    # Process up to 3 files concurrently to avoid instantly tripping rate limits
    semaphore = asyncio.Semaphore(3)
    
    tasks = [process_file(f, semaphore) for f in cs_files]
    await asyncio.gather(*tasks)
    print("Batch processing complete.")

if __name__ == "__main__":
    asyncio.run(main())
