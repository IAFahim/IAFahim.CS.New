import os
import json

def aggregate():
    batch_file_path = "/home/l/Github/IAFahim.CS.New/.agents/orchestrator/batches/batch_1.txt"
    src_dir = "/home/l/Github/IAFahim.CS.New/src"
    output_json_path = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_1/inputs.json"
    
    with open(batch_file_path, "r", encoding="utf-8") as f:
        packages = [line.strip() for line in f if line.strip()]
        
    aggregated_data = []
    
    for package in packages:
        package_path = os.path.join(src_dir, package)
        if not os.path.exists(package_path):
            print(f"Warning: Package path not found: {package_path}")
            continue
            
        existing_readme = None
        source_files = []
        
        for root, dirs, files in os.walk(package_path):
            # Exclude bin and obj directories
            if "bin" in dirs:
                dirs.remove("bin")
            if "obj" in dirs:
                dirs.remove("obj")
                
            for file in files:
                file_path = os.path.join(root, file)
                rel_path = os.path.relpath(file_path, package_path)
                
                if file.lower() == "readme.md":
                    try:
                        with open(file_path, "r", encoding="utf-8") as f_readme:
                            existing_readme = f_readme.read()
                    except Exception as e:
                        print(f"Error reading README for {package}: {e}")
                elif file.endswith(".cs"):
                    try:
                        with open(file_path, "r", encoding="utf-8") as f_cs:
                            content = f_cs.read()
                        source_files.append({
                            "path": rel_path,
                            "content": content
                        })
                    except Exception as e:
                        print(f"Error reading C# file {file_path}: {e}")
                        
        aggregated_data.append({
            "package_name": package,
            "existing_readme": existing_readme,
            "source_files": source_files
        })
        
    with open(output_json_path, "w", encoding="utf-8") as f_out:
        json.dump(aggregated_data, f_out, indent=2, ensure_ascii=False)
        
    print(f"Successfully aggregated data for {len(aggregated_data)} packages to {output_json_path}")

if __name__ == "__main__":
    aggregate()
