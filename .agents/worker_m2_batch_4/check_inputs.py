import json
import re

def contains_cat_seq(s):
    s = s.lower()
    c_idx = s.find('c')
    if c_idx == -1:
        return False
    a_idx = s.find('a', c_idx + 1)
    if a_idx == -1:
        return False
    t_idx = s.find('t', a_idx + 1)
    if t_idx == -1:
        return False
    return True

def main():
    with open("inputs.json", "r", encoding="utf-8") as f:
        data = json.load(f)
        
    for pkg_name, pkg_data in data.items():
        print(f"Package: {pkg_name}")
        for file, content in pkg_data["csharp_files"].items():
            # Check for cat or sequence
            words = re.findall(r'\b\w+\b', content)
            bad_words = [w for w in words if contains_cat_seq(w)]
            if bad_words:
                print(f"  File {file} contains potential sequence words: {set(bad_words)}")
            if "cat" in content.lower():
                # find where
                lines = content.splitlines()
                for idx, line in enumerate(lines):
                    if "cat" in line.lower():
                        print(f"  File {file}:{idx+1} contains 'cat' (case-insensitive): {line.strip()}")

if __name__ == "__main__":
    main()
