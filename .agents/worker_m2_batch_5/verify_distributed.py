import os
import re

# Validation function to ensure no words contain c...a...t in sequence (except parts of the package name)
def contains_cat_in_sequence(word):
    w = word.lower()
    w = "".join([c for c in w if c.isalnum()])
    c_idx = w.find('c')
    if c_idx != -1:
        a_idx = w.find('a', c_idx + 1)
        if a_idx != -1:
            t_idx = w.find('t', a_idx + 1)
            if t_idx != -1:
                return True
    return False

def verify_readme_file(pkg_name, readme_path):
    with open(readme_path, 'r', encoding='utf-8') as f:
        readme_text = f.read()

    # 1. Check case-insensitive "cat" substring in the entire README
    if "cat" in readme_text.lower():
        print(f"Error: Substring 'cat' found in {pkg_name}!")
        return False

    # 2. Check headers
    required_headers = [
        f"# {pkg_name}",
        "## Description",
        "## Complexity",
        "## API Signature",
        "## Usage Example"
    ]
    for header in required_headers:
        if header not in readme_text:
            print(f"Error: Header '{header}' is missing in {pkg_name}.")
            return False

    # 3. Check explanation sections for c...a...t in sequence
    lines = readme_text.splitlines()
    in_explanation = False
    in_code_block = False
    code_block_content = []
    
    for line_num, line in enumerate(lines):
        stripped = line.strip()
        if stripped.startswith("```"):
            in_code_block = not in_code_block
            continue
        
        if in_code_block:
            code_block_content.append(line)
            continue
            
        if stripped.startswith("## Description") or stripped.startswith("## Complexity"):
            in_explanation = True
            continue
        elif stripped.startswith("##") or stripped.startswith("#"):
            in_explanation = False
        
        if in_explanation:
            words = re.findall(r'[a-zA-Z]+', line)
            for word in words:
                skip = False
                for part in pkg_name.split('.'):
                    if word.lower() == part.lower():
                        skip = True
                if skip:
                    continue

                if contains_cat_in_sequence(word):
                    print(f"Error in {pkg_name} on line {line_num+1}: Word '{word}' contains 'c', 'a', 't' in sequence in explanation.")
                    return False

    # 4. Check C# code block constraints
    code_text = "\n".join(code_block_content)
    if "//" in code_text or "/*" in code_text:
        print(f"Error in {pkg_name}: Code block contains comments!")
        return False
    if "var" in re.findall(r'\bvar\b', code_text):
        print(f"Error in {pkg_name}: Code block contains 'var'!")
        return False
    if "[]" in code_text:
        print(f"Error in {pkg_name}: Code block might contain managed arrays (contains '[]')!")
        return False

    return True

def main():
    batch_file = '/home/l/Github/IAFahim.CS.New/.agents/orchestrator/batches/batch_5.txt'
    src_dir = '/home/l/Github/IAFahim.CS.New/src'

    with open(batch_file, 'r', encoding='utf-8') as f:
        packages = [line.strip() for line in f if line.strip()]

    all_ok = True
    for pkg in packages:
        readme_path = os.path.join(src_dir, pkg, 'README.md')
        if not os.path.exists(readme_path):
            print(f"Error: {pkg} is missing README.md!")
            all_ok = False
            continue
        
        if not verify_readme_file(pkg, readme_path):
            all_ok = False

    if all_ok:
        print("All distributed README.md files successfully verified!")
    else:
        print("Verification failed.")

if __name__ == '__main__':
    main()
