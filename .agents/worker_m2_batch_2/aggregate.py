import os
import json

def main():
    repo_root = "/home/l/Github/IAFahim.CS.New"
    batch_file_path = os.path.join(repo_root, ".agents/orchestrator/batches/batch_2.txt")
    output_json_path = os.path.join(repo_root, ".agents/worker_m2_batch_2/inputs.json")
    
    with open(batch_file_path, "r", encoding="utf-8") as f:
        lines = f.readlines()
        
    packages = []
    for line in lines:
        line = line.strip()
        # skip lines containing metadata, only process package names starting with IAFahim.
        if line.startswith("IAFahim."):
            packages.append(line)
        elif line and not line.startswith("Created At:") and not line.startswith("Completed At:") and not line.startswith("File Path:") and not line.startswith("Total Lines:") and not line.startswith("Total Bytes:") and not line.startswith("Showing lines") and not ":" in line:
            # just in case
            packages.append(line)
            
    print(f"Found {len(packages)} packages to process.")
    
    aggregated_data = {}
    for pkg in packages:
        pkg_dir = os.path.join(repo_root, "src", pkg)
        if not os.path.exists(pkg_dir):
            print(f"Warning: Package directory {pkg_dir} does not exist!")
            continue
            
        code_files = {}
        readme_content = None
        
        for root, dirs, files in os.walk(pkg_dir):
            # prune bin/obj
            if "bin" in dirs:
                dirs.remove("bin")
            if "obj" in dirs:
                dirs.remove("obj")
                
            for file in files:
                file_path = os.path.join(root, file)
                rel_path = os.path.relpath(file_path, pkg_dir)
                if file.endswith(".cs"):
                    try:
                        with open(file_path, "r", encoding="utf-8") as cs_f:
                            code_files[rel_path] = cs_f.read()
                    except Exception as e:
                        print(f"Error reading {file_path}: {e}")
                elif file.lower() == "readme.md":
                    try:
                        with open(file_path, "r", encoding="utf-8") as rm_f:
                            readme_content = rm_f.read()
                    except Exception as e:
                        print(f"Error reading {file_path}: {e}")
                        
        aggregated_data[pkg] = {
            "code": code_files,
            "readme": readme_content
        }
        
    with open(output_json_path, "w", encoding="utf-8") as out_f:
        json.dump(aggregated_data, out_f, indent=2, ensure_ascii=False)
        
    print(f"Aggregated data written to {output_json_path}")

if __name__ == "__main__":
    main()
