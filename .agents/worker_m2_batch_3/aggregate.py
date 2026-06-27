import os
import json

batch_file = "/home/l/Github/IAFahim.CS.New/.agents/orchestrator/batches/batch_3.txt"
src_dir = "/home/l/Github/IAFahim.CS.New/src"
output_file = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_3/inputs.json"

if not os.path.exists(batch_file):
    print(f"Error: batch file {batch_file} not found")
    exit(1)

with open(batch_file, "r") as f:
    packages = [line.strip() for line in f if line.strip()]

aggregated = {}

for pkg in packages:
    pkg_dir = os.path.join(src_dir, pkg)
    if not os.path.exists(pkg_dir):
        print(f"Warning: directory {pkg_dir} does not exist")
        continue
    
    code_files = {}
    readme_content = None
    
    for root, dirs, files in os.walk(pkg_dir):
        # Ignore bin and obj directories
        if "bin" in dirs:
            dirs.remove("bin")
        if "obj" in dirs:
            dirs.remove("obj")
            
        for file in files:
            file_path = os.path.join(root, file)
            rel_path = os.path.relpath(file_path, pkg_dir)
            if file.endswith(".cs"):
                try:
                    with open(file_path, "r", encoding="utf-8") as f_in:
                        code_files[rel_path] = f_in.read()
                except Exception as e:
                    print(f"Error reading C# file {file_path}: {e}")
            elif file.lower() == "readme.md":
                try:
                    with open(file_path, "r", encoding="utf-8") as f_in:
                        readme_content = f_in.read()
                except Exception as e:
                    print(f"Error reading README file {file_path}: {e}")
                    
    aggregated[pkg] = {
        "code": code_files,
        "readme": readme_content
    }

with open(output_file, "w", encoding="utf-8") as f_out:
    json.dump(aggregated, f_out, indent=2, ensure_ascii=False)

print(f"Aggregated {len(aggregated)} packages to {output_file}")
