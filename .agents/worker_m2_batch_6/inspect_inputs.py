import json
import re

inputs_json_path = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_6/inputs.json"
with open(inputs_json_path, "r", encoding="utf-8") as f:
    data = json.load(f)

for pkg, info in data.items():
    print(f"=== {pkg} ===")
    if info["existing_readme"]:
        print("  Has existing readme")
    else:
        print("  NO existing readme")
    for file, content in info["cs_files"].items():
        print(f"  File: {file}")
        # Extract namespace, public class/struct, public method signatures
        ns = re.findall(r"namespace\s+([\w\.]+)", content)
        classes = re.findall(r"public\s+static\s+unsafe\s+class\s+(\w+)", content)
        structs = re.findall(r"public\s+unsafe\s+struct\s+(\w+)", content)
        methods = re.findall(r"public\s+static\s+\w+\s+(\w+)\s*<.*?>\s*\(.*?\)", content)
        if not methods:
            methods = re.findall(r"public\s+static\s+\w+\s+(\w+)\s*\(.*?\)", content)
        print(f"    Namespace: {ns}")
        print(f"    Class: {classes}")
        print(f"    Structs: {structs}")
        print(f"    Methods: {methods}")
