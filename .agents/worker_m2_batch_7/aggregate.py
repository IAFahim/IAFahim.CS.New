import os
import json

def main():
    workspace_dir = "/home/l/Github/IAFahim.CS.New"
    batch_file_path = os.path.join(workspace_dir, ".agents/orchestrator/batches/batch_7.txt")
    output_json_path = os.path.join(workspace_dir, ".agents/worker_m2_batch_7/inputs.json")
    
    with open(batch_file_path, "r", encoding="utf-8") as f:
        packages = [line.strip() for line in f if line.strip()]
        
    aggregated = []
    
    for pkg in packages:
        pkg_dir = os.path.join(workspace_dir, "src", pkg)
        if not os.path.isdir(pkg_dir):
            print(f"Warning: Directory {pkg_dir} does not exist.")
            continue
            
        pkg_data = {
            "package_name": pkg,
            "files": {}
        }
        
        for root, dirs, files in os.walk(pkg_dir):
            # Exclude bin and obj
            if "bin" in dirs:
                dirs.remove("bin")
            if "obj" in dirs:
                dirs.remove("obj")
                
            for file in files:
                if file.endswith(".cs") or file.lower() == "readme.md":
                    full_path = os.path.join(root, file)
                    rel_path = os.path.relpath(full_path, workspace_dir)
                    try:
                        with open(full_path, "r", encoding="utf-8") as f_in:
                            content = f_in.read()
                        pkg_data["files"][rel_path] = content
                    except Exception as e:
                        print(f"Error reading {full_path}: {e}")
                        
        aggregated.append(pkg_data)
        
    with open(output_json_path, "w", encoding="utf-8") as f_out:
        json.dump(aggregated, f_out, indent=2, ensure_ascii=False)
        
    print(f"Aggregated {len(aggregated)} packages to {output_json_path}")

if __name__ == "__main__":
    main()
