import json

inputs_json_path = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_6/inputs.json"
with open(inputs_json_path, "r", encoding="utf-8") as f:
    data = json.load(f)

lines = []
for pkg, info in data.items():
    lines.append(f"========================================\nPACKAGE: {pkg}")
    for file, content in info["cs_files"].items():
        lines.append(f"--- File: {file} ---")
        for line in content.splitlines():
            stripped = line.strip()
            # match lines with public class, public struct, or public methods/properties/fields
            if stripped.startswith("public ") or " public " in line:
                # filter out private, internal
                if "private " not in line and "internal " not in line:
                    lines.append(stripped)
    lines.append("")

with open("/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_6/public_apis.txt", "w", encoding="utf-8") as f_out:
    f_out.write("\n".join(lines))

print("Done")
