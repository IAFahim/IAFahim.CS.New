import os
import json

workspace_dir = "/home/l/Github/IAFahim.CS.New"
outputs_json_path = os.path.join(workspace_dir, ".agents/worker_m2_batch_6/outputs.json")

with open(outputs_json_path, "r", encoding="utf-8") as f:
    readmes = json.load(f)

for pkg, readme in readmes.items():
    pkg_dir = os.path.join(workspace_dir, "src", pkg)
    if not os.path.exists(pkg_dir):
        print(f"Warning: Directory {pkg_dir} does not exist. Creating it.")
        os.makedirs(pkg_dir)
        
    readme_path = os.path.join(pkg_dir, "README.md")
    with open(readme_path, "w", encoding="utf-8") as f_out:
        f_out.write(readme)
    print(f"Distributed README.md to {readme_path}")

print("Distribution completed successfully.")
