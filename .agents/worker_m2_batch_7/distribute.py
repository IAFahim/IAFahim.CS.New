import os
import json

def main():
    workspace_dir = "/home/l/Github/IAFahim.CS.New"
    outputs_json_path = os.path.join(workspace_dir, ".agents/worker_m2_batch_7/outputs.json")
    
    with open(outputs_json_path, "r", encoding="utf-8") as f:
        readmes = json.load(f)
        
    for pkg_name, content in readmes.items():
        pkg_dir = os.path.join(workspace_dir, "src", pkg_name)
        if not os.path.isdir(pkg_dir):
            print(f"Warning: Directory {pkg_dir} does not exist. Creating it.")
            os.makedirs(pkg_dir, exist_ok=True)
            
        readme_path = os.path.join(pkg_dir, "README.md")
        with open(readme_path, "w", encoding="utf-8") as f_out:
            f_out.write(content)
            
        print(f"Wrote README.md to {readme_path}")
        
    print(f"Successfully distributed {len(readmes)} READMEs.")

if __name__ == "__main__":
    main()
