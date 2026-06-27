import os
import json

def main():
    base_dir = "/home/l/Github/IAFahim.CS.New"
    outputs_file = os.path.join(base_dir, ".agents/worker_m2_batch_4/outputs.json")
    
    with open(outputs_file, "r", encoding="utf-8") as f:
        readmes = json.load(f)
        
    for pkg_name, content in readmes.items():
        dest_dir = os.path.join(base_dir, "src", pkg_name)
        if not os.path.exists(dest_dir):
            print(f"Creating directory: {dest_dir}")
            os.makedirs(dest_dir, exist_ok=True)
            
        readme_path = os.path.join(dest_dir, "README.md")
        with open(readme_path, "w", encoding="utf-8") as f_out:
            f_out.write(content)
        print(f"Distributed README.md to {readme_path}")
        
    print("Distribution completed successfully.")

if __name__ == "__main__":
    main()
