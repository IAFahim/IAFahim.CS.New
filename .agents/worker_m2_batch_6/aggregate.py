import os
import json

workspace_dir = "/home/l/Github/IAFahim.CS.New"
batch_file_path = os.path.join(workspace_dir, ".agents/orchestrator/batches/batch_6.txt")
inputs_json_path = os.path.join(workspace_dir, ".agents/worker_m2_batch_6/inputs.json")

packages = []
with open(batch_file_path, "r", encoding="utf-8") as f:
    for line in f:
        line = line.strip()
        if not line:
            continue
        if line.startswith("Created At:") or line.startswith("Completed At:") or line.startswith("File Path:") or line.startswith("Total Lines:") or line.startswith("Total Bytes:") or line.startswith("Showing lines"):
            continue
        # In case the file has line number prefix from some representation, though batch_6.txt usually does not.
        # Just strip IAFahim packages.
        if line.startswith("IAFahim."):
            packages.append(line)
        elif ":" in line:
            parts = line.split(":", 1)
            pkg = parts[1].strip()
            if pkg.startswith("IAFahim."):
                packages.append(pkg)

print("Packages found in batch:", len(packages))

inputs_data = {}
for pkg in packages:
    pkg_dir = os.path.join(workspace_dir, "src", pkg)
    if not os.path.exists(pkg_dir):
        print(f"Directory {pkg_dir} does not exist!")
        continue
    
    cs_files = {}
    existing_readme = None
    
    for root, dirs, files in os.walk(pkg_dir):
        # Exclude bin and obj
        if "bin" in dirs:
            dirs.remove("bin")
        if "obj" in dirs:
            dirs.remove("obj")
            
        for file in files:
            file_path = os.path.join(root, file)
            rel_path = os.path.relpath(file_path, pkg_dir)
            if file.endswith(".cs"):
                try:
                    with open(file_path, "r", encoding="utf-8") as f_cs:
                        cs_files[rel_path] = f_cs.read()
                except Exception as e:
                    print(f"Error reading {file_path}: {e}")
            elif file.lower() == "readme.md":
                try:
                    with open(file_path, "r", encoding="utf-8") as f_rd:
                        existing_readme = f_rd.read()
                except Exception as e:
                    print(f"Error reading {file_path}: {e}")
                    
    inputs_data[pkg] = {
        "cs_files": cs_files,
        "existing_readme": existing_readme
    }

with open(inputs_json_path, "w", encoding="utf-8") as f_out:
    json.dump(inputs_data, f_out, indent=2)

print("Saved inputs.json with", len(inputs_data), "packages.")
