import os
import json

def main():
    base_dir = "/home/l/Github/IAFahim.CS.New"
    batch_file = os.path.join(base_dir, ".agents/orchestrator/batches/batch_4.txt")
    output_file = os.path.join(base_dir, ".agents/worker_m2_batch_4/inputs.json")
    
    with open(batch_file, "r") as f:
        packages = [line.strip() for line in f if line.strip()]
        
    inputs = {}
    
    for package in packages:
        pkg_dir = os.path.join(base_dir, "src", package)
        if not os.path.exists(pkg_dir):
            print(f"Warning: Directory {pkg_dir} does not exist.")
            continue
            
        csharp_files = {}
        readme_content = ""
        
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
                            csharp_files[rel_path] = f_cs.read()
                    except Exception as e:
                        print(f"Error reading {file_path}: {e}")
                elif file.lower() == "readme.md":
                    try:
                        with open(file_path, "r", encoding="utf-8") as f_rm:
                            readme_content = f_rm.read()
                    except Exception as e:
                        print(f"Error reading {file_path}: {e}")
                        
        inputs[package] = {
            "csharp_files": csharp_files,
            "existing_readme": readme_content
        }
        
    with open(output_file, "w", encoding="utf-8") as f_out:
        json.dump(inputs, f_out, indent=2)
        
    print(f"Aggregated {len(inputs)} packages to {output_file}")

if __name__ == "__main__":
    main()
