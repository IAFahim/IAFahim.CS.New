import re

def check_text(text, filename="README"):
    errors = []
    
    # Rule 1: The word "cat" (case-insensitive) is strictly forbidden in the entire README.
    if re.search(r"\bcat\b", text, re.IGNORECASE):
        errors.append(f"Forbidden word 'cat' found in {filename}")
        
    # Rule 2: Avoid using any word in the explanation that contains the letters 'c', 'a', 't' in sequence.
    # The explanation is considered to be the Description and Complexity sections.
    # Let's extract Description and Complexity sections from the markdown.
    explanation_text = ""
    desc_match = re.search(r"## Description\s*(.*?)\s*(?=##|$)", text, re.DOTALL)
    if desc_match:
        explanation_text += desc_match.group(1) + "\n"
    comp_match = re.search(r"## Complexity\s*(.*?)\s*(?=##|$)", text, re.DOTALL)
    if comp_match:
        explanation_text += comp_match.group(1) + "\n"
        
    words = re.findall(r"\b[a-zA-Z]+\b", explanation_text)
    for w in words:
        wl = w.lower()
        if wl == "cat":
            errors.append(f"Forbidden word 'cat' found in explanation: {w}")
        if re.search(r"c.*a.*t", wl):
            errors.append(f"Explanation word '{w}' contains 'c', 'a', 't' in sequence")
            
    # Check headers
    required_headers = [
        "Description",
        "Complexity",
        "API Signature",
        "Usage Example"
    ]
    for h in required_headers:
        if not re.search(rf"^##\s+{h}\s*$", text, re.MULTILINE):
            errors.append(f"Missing header: ## {h}")
            
    # Check that there are no other ## headers (Exactly these headers)
    h1 = re.findall(r"^#\s+(.+)$", text, re.MULTILINE)
    h2 = re.findall(r"^##\s+(.+)$", text, re.MULTILINE)
    if len(h1) != 1:
        errors.append(f"Must have exactly one H1 header, found {len(h1)}")
    if len(h2) != 4:
        errors.append(f"Must have exactly four H2 headers, found {len(h2)}: {h2}")
    else:
        for actual, expected in zip(h2, required_headers):
            if actual.strip() != expected:
                errors.append(f"Header mismatch: expected '## {expected}', found '## {actual}'")
                
    # Check Usage Example rules:
    code_blocks = re.findall(r"```csharp\s*(.*?)\s*```", text, re.DOTALL)
    if not code_blocks:
        errors.append("No csharp code block found in Usage Example")
    for block in code_blocks:
        if "var" in re.findall(r"\bvar\b", block):
            errors.append("Usage Example contains 'var'")
        if "//" in block or "/*" in block:
            errors.append("Usage Example contains comments")
        if re.search(r"new\s+\w+\[", block):
            errors.append("Usage Example contains managed array allocation")
        if "AllocHGlobal" not in block or "FreeHGlobal" not in block:
            errors.append("Usage Example must use try/finally with AllocHGlobal/FreeHGlobal")
        if "unsafe" not in block:
            errors.append("Usage Example should have 'unsafe'")
            
    return errors

if __name__ == "__main__":
    test_text = """# IAFahim.Graph.Cactus
## Description
This is a description.
## Complexity
O(V + E) complexity.
## API Signature
public static void Run()
## Usage Example
```csharp
unsafe {
    int* ptr = null;
    try {
        ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(10);
    } finally {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```"""
    errs = check_text(test_text)
    print("Errors:", errs)
