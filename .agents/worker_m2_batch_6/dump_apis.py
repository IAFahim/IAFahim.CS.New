import json
import re

inputs_json_path = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_6/inputs.json"
with open(inputs_json_path, "r", encoding="utf-8") as f:
    data = json.load(f)

for pkg, info in data.items():
    print(f"========================================\nPACKAGE: {pkg}")
    for file, content in info["cs_files"].items():
        print(f"--- File: {file} ---")
        lines = content.splitlines()
        for idx, line in enumerate(lines):
            stripped = line.strip()
            # If it's a namespace, class, struct, or public method declaration
            if stripped.startswith("namespace ") or "public static unsafe class " in line or "public unsafe struct " in line or "public struct " in line:
                print(f"{idx+1}: {stripped}")
            elif "public static" in line or "public " in line:
                # print public members
                if "(" in line or "struct" in line or "class" in line or "enum" in line:
                    print(f"{idx+1}: {stripped}")
