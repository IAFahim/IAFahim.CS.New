import os
import json

WORKSPACE_ROOT = "/home/l/Github/IAFahim.CS.New"
BATCH_FILE = os.path.join(WORKSPACE_ROOT, ".agents/orchestrator/batches/batch_8.txt")
OUTPUT_FILE = os.path.join(WORKSPACE_ROOT, ".agents/worker_m2_batch_8/inputs.json")

def main():
    with open(BATCH_FILE, "r") as f:
        packages = [line.strip() for line in f if line.strip()]

    data = {}
    for pkg in packages:
        pkg_dir = os.path.join(WORKSPACE_ROOT, "src", pkg)
        if not os.path.isdir(pkg_dir):
            print(f"Warning: directory {pkg_dir} does not exist")
            continue

        csharp_files = {}
        existing_readme = ""
        
        for root, dirs, files in os.walk(pkg_dir):
            # Exclude bin and obj
            if "bin" in dirs:
                dirs.remove("bin")
            if "obj" in dirs:
                dirs.remove("obj")
            
            for file in files:
                full_path = os.path.join(root, file)
                rel_path = os.path.relpath(full_path, pkg_dir)
                if file.endswith(".cs"):
                    try:
                        with open(full_path, "r", encoding="utf-8") as sf:
                            csharp_files[rel_path] = sf.read()
                    except Exception as e:
                        print(f"Error reading {full_path}: {e}")
                elif file.lower() == "readme.md":
                    try:
                        with open(full_path, "r", encoding="utf-8") as rf:
                            existing_readme = rf.read()
                    except Exception as e:
                        print(f"Error reading {full_path}: {e}")

        data[pkg] = {
            "csharp_files": csharp_files,
            "existing_readme": existing_readme
        }

    with open(OUTPUT_FILE, "w", encoding="utf-8") as out:
        json.dump(data, out, indent=2)
    print(f"Successfully aggregated data to {OUTPUT_FILE}")

if __name__ == "__main__":
    main()
