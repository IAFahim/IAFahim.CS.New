import json

with open("inputs.json", "r") as f:
    data = json.load(f)

for pkg, val in data.items():
    cs_files = list(val["csharp_files"].keys())
    print(f"{pkg}: {len(cs_files)} files: {cs_files}")
