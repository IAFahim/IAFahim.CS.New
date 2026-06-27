import json
import re

with open("/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_3/inputs.json", "r") as f:
    data = json.load(f)

for pkg, info in data.items():
    print(f"==================================================")
    print(f"PACKAGE: {pkg}")
    if info["readme"]:
        print(f"Existing README:\n{info['readme']}")
    else:
        print("Existing README: None")
    
    for filepath, code in info["code"].items():
        print(f"File: {filepath}")
        for line in code.splitlines():
            line_stripped = line.strip()
            # Only match public or internal members
            if ("public" in line_stripped or "internal" in line_stripped) and not line_stripped.startswith("//") and not line_stripped.startswith("/*"):
                if any(x in line_stripped for x in ["class", "struct", "enum", "interface", "void", "int", "bool", "double", "float", "long", "byte", "T"]):
                    print(f"  {line_stripped}")
    print()
