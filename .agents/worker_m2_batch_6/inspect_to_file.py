import json
import re

inputs_json_path = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_6/inputs.json"
with open(inputs_json_path, "r", encoding="utf-8") as f:
    data = json.load(f)

lines = []
for pkg, info in data.items():
    lines.append(f"=== {pkg} ===")
    if info["existing_readme"]:
        lines.append("  Has existing readme")
    else:
        lines.append("  NO existing readme")
    for file, content in info["cs_files"].items():
        lines.append(f"  File: {file}")
        # Extract namespace, public class/struct, public method signatures
        ns = re.findall(r"namespace\s+([\w\.]+)", content)
        classes = re.findall(r"public\s+static\s+(?:unsafe\s+)?class\s+(\w+)", content)
        structs = re.findall(r"public\s+(?:unsafe\s+)?struct\s+(\w+)", content)
        methods = re.findall(r"public\s+static\s+[\w\<\>\[\]\*]+\s+(\w+)\s*<.*?>\s*\(.*?\)", content)
        if not methods:
            methods = re.findall(r"public\s+static\s+[\w\<\>\[\]\*]+\s+(\w+)\s*\(.*?\)", content)
        lines.append(f"    Namespace: {ns}")
        lines.append(f"    Class: {classes}")
        lines.append(f"    Structs: {structs}")
        lines.append(f"    Methods: {methods}")
    lines.append("")

with open("/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_6/summary.txt", "w", encoding="utf-8") as f_sum:
    f_sum.write("\n".join(lines))

print("Wrote summary.txt")
