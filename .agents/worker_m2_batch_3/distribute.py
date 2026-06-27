import os
import json

outputs_file = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_3/outputs.json"
src_dir = "/home/l/Github/IAFahim.CS.New/src"

if not os.path.exists(outputs_file):
    print(f"Error: {outputs_file} not found")
    exit(1)

with open(outputs_file, "r", encoding="utf-8") as f:
    readmes = json.load(f)

for pkg, md in readmes.items():
    pkg_dir = os.path.join(src_dir, pkg)
    if not os.path.exists(pkg_dir):
        print(f"Creating directory: {pkg_dir}")
        os.makedirs(pkg_dir, exist_ok=True)
        
    readme_path = os.path.join(pkg_dir, "README.md")
    with open(readme_path, "w", encoding="utf-8") as f_out:
        f_out.write(md)
        # Add a trailing newline if not present
        if not md.endswith("\n"):
            f_out.write("\n")
            
    print(f"Wrote README.md to {readme_path}")

print("Distribution complete!")
