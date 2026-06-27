import re
import json

def check_text(text, is_code_block=False):
    # The word "cat" (case-insensitive) is strictly forbidden in the entire README.
    # We will search for the word "cat" (case-insensitive) as a whole word.
    # Using regex boundary \bcat\b
    if re.search(r"\bcat\b", text, re.I):
        return False, "Contains the forbidden word 'cat'"
    
    if not is_code_block:
        # In explanations, avoid any word containing the letters c, a, t in sequence.
        # Find all words
        words = re.findall(r"[a-zA-Z]+", text)
        for w in words:
            wl = w.lower()
            # Check for subsequence c ... a ... t
            c_idx = wl.find('c')
            if c_idx != -1:
                a_idx = wl.find('a', c_idx + 1)
                if a_idx != -1:
                    t_idx = wl.find('t', a_idx + 1)
                    if t_idx != -1:
                        return False, f"Explanation word '{w}' contains 'c', 'a', 't' in sequence"
    return True, ""

def validate_readme(md_content):
    # Split text into code blocks and normal text
    lines = md_content.splitlines()
    in_code_block = False
    for i, line in enumerate(lines, 1):
        if line.strip().startswith("```"):
            in_code_block = not in_code_block
            # Still check for "cat" word even in code block marker
            ok, msg = check_text(line, is_code_block=True)
            if not ok:
                return False, f"Line {i}: {msg}"
            continue
        
        ok, msg = check_text(line, is_code_block=in_code_block)
        if not ok:
            return False, f"Line {i}: {msg}"
            
    # Check headers
    # Exactly these headers: # {package_name}, ## Description, ## Complexity, ## API Signature, ## Usage Example
    headers = [line.strip() for line in lines if line.strip().startswith("#")]
    # Filter out code block lines that might start with # (though rare)
    # But let us just search for exact headers in the markdown
    expected_headers_pattern = [
        r"^# IAFahim\..+$",
        r"^## Description$",
        r"^## Complexity$",
        r"^## API Signature$",
        r"^## Usage Example$"
    ]
    
    # Let us check that they exist in that exact form (we can just search them)
    for pattern in expected_headers_pattern:
        found = False
        for line in lines:
            if re.match(pattern, line.strip()):
                found = True
                break
        if not found:
            return False, f"Missing or malformed header matching pattern: {pattern}"
            
    return True, ""

if __name__ == "__main__":
    # Test validator with a sample
    sample_ok = """# IAFahim.Sort.Merge
## Description
This package sorts elements in an unmanaged buffer using a divide and conquer strategy.
It merges sorted sub-segments.

## Complexity
Time complexity is O(N log N).

## API Signature
```csharp
public static class Merge
{
    public static void Run<T>(T* ptr, int len, T* scratch);
}
```

## Usage Example
```csharp
unsafe
{
    int* ptr = null;
    int* temp = null;
    try
    {
        ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(10 * sizeof(int));
        temp = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(10 * sizeof(int));
    }
    finally
    {
        if (ptr != null) System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        if (temp != null) System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)temp);
    }
}
```
"""
    ok, msg = validate_readme(sample_ok)
    print(f"Sample validation: {ok}, {msg}")
