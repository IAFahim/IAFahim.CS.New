import os
import re

def is_forbidden_word(word):
    w = word.lower()
    if w == "cat":
        return True
    c_idx = w.find('c')
    if c_idx != -1:
        a_idx = w.find('a', c_idx + 1)
        if a_idx != -1:
            t_idx = w.find('t', a_idx + 1)
            if t_idx != -1:
                return True
    return False

def find_violations(text):
    words = re.findall(r'[a-zA-Z]+', text)
    violations = []
    for w in words:
        if is_forbidden_word(w):
            violations.append(w)
    return violations

def main():
    repo_root = "/home/l/Github/IAFahim.CS.New"
    batch_file_path = os.path.join(repo_root, ".agents/orchestrator/batches/batch_2.txt")
    
    with open(batch_file_path, "r", encoding="utf-8") as f:
        lines = f.readlines()
        
    packages = []
    for line in lines:
        line = line.strip()
        if line.startswith("IAFahim."):
            packages.append(line)
            
    print(f"Verifying {len(packages)} packages...")
    
    all_ok = True
    for pkg in packages:
        readme_path = os.path.join(repo_root, "src", pkg, "README.md")
        if not os.path.exists(readme_path):
            print(f"Error: README.md not found for {pkg} at {readme_path}")
            all_ok = False
            continue
            
        with open(readme_path, "r", encoding="utf-8") as f:
            content = f.read()
            
        # Check headers
        expected_headers = [
            f"# {pkg}",
            "## Description",
            "## Complexity",
            "## API Signature",
            "## Usage Example"
        ]
        headers = [line.strip() for line in content.splitlines() if line.strip().startswith('#')]
        if headers != expected_headers:
            print(f"Error: Headers mismatch in {pkg}. Found: {headers}")
            all_ok = False
            
        # Check forbidden word "cat"
        words = re.findall(r'[a-zA-Z]+', content)
        if any(w.lower() == "cat" for w in words):
            print(f"Error: Word 'cat' found in {pkg} README.")
            all_ok = False
            
        # Check c-a-t sequence in Description / Complexity
        desc_match = re.search(r'## Description\s*(.*?)\s*(?:##|$)', content, re.DOTALL)
        complexity_match = re.search(r'## Complexity\s*(.*?)\s*(?:##|$)', content, re.DOTALL)
        
        desc_text = desc_match.group(1) if desc_match else ""
        complexity_text = complexity_match.group(1) if complexity_match else ""
        explanation_text = desc_text + "\n" + complexity_text
        
        violations = find_violations(explanation_text)
        if violations:
            print(f"Error: 'c'-'a'-'t' sequence violations in {pkg} explanations: {list(set(violations))}")
            all_ok = False
            
        # Check Usage Example code block
        if "## Usage Example" not in content:
            print(f"Error: No '## Usage Example' header in {pkg}.")
            all_ok = False
        else:
            usage_part = content.split("## Usage Example")[-1]
            code_blocks = re.findall(r'```csharp(.*?)```', usage_part, re.DOTALL)
            if not code_blocks:
                print(f"Error: No csharp code block in {pkg} usage example.")
                all_ok = False
            else:
                for block in code_blocks:
                    if "unsafe" not in block:
                        print(f"Error: 'unsafe' missing in {pkg} code block.")
                        all_ok = False
                    if "AllocHGlobal" not in block or "FreeHGlobal" not in block:
                        print(f"Error: 'AllocHGlobal'/'FreeHGlobal' missing in {pkg} code block.")
                        all_ok = False
                    if "var " in block:
                        print(f"Error: 'var' keyword found in {pkg} code block.")
                        all_ok = False
                    if "//" in block or "/*" in block:
                        print(f"Error: comments found in {pkg} code block.")
                        all_ok = False
                    if re.search(r'\w+\[\s*\]', block) or re.search(r'new\s+\w+\[', block):
                        print(f"Error: managed arrays found in {pkg} code block.")
                        all_ok = False
                        
    if all_ok:
        print("VERIFICATION PASSED: All README.md files are fully correct and compliant.")
    else:
        print("VERIFICATION FAILED. See errors above.")

if __name__ == "__main__":
    main()
