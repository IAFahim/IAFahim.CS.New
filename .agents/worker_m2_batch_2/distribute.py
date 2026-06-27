import os
import json

def main():
    repo_root = "/home/l/Github/IAFahim.CS.New"
    outputs_path = os.path.join(repo_root, ".agents/worker_m2_batch_2/outputs.json")
    
    if not os.path.exists(outputs_path):
        print(f"Error: {outputs_path} does not exist!")
        return
        
    with open(outputs_path, "r", encoding="utf-8") as f:
        data = json.load(f)
        
    for pkg, markdown in data.items():
        dest_dir = os.path.join(repo_root, "src", pkg)
        if not os.path.exists(dest_dir):
            os.makedirs(dest_dir, exist_ok=True)
            print(f"Created directory {dest_dir}")
            
        dest_file = os.path.join(dest_dir, "README.md")
        with open(dest_file, "w", encoding="utf-8") as out_f:
            out_f.write(markdown)
        print(f"Wrote README.md for {pkg} to {dest_file}")
        
    print("Distribution completed successfully.")

if __name__ == "__main__":
    main()
