import json
import re

with open("/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_3/inputs.json", "r") as f:
    data = json.load(f)

for pkg, info in data.items():
    print(f"=== {pkg} ===")
    if info["readme"]:
        print(f"Existing README: Yes ({len(info['readme'])} chars)")
    else:
        print("Existing README: No")
    print("Files:")
    for filepath, code in info["code"].items():
        print(f"  - {filepath}")
        # simple regex to extract class/struct/enum definition and method declarations
        lines = code.splitlines()
        for i, line in enumerate(lines):
            line_str = line.strip()
            if any(k in line_str for k in ["class", "struct", "enum", "interface"]) and not line_str.startswith("//") and not line_str.startswith("/*"):
                print(f"    Line {i+1}: {line_str}")
            elif ("public" in line_str or "internal" in line_str or "static" in line_str) and "(" in line_str and ")" in line_str and not line_str.startswith("//") and not line_str.startswith("/*"):
                print(f"    Line {i+1}: {line_str}")
    print()
