import os
import json

def distribute():
    outputs_json_path = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_1/outputs.json"
    src_dir = "/home/l/Github/IAFahim.CS.New/src"
    
    with open(outputs_json_path, "r", encoding="utf-8") as f:
        readmes = json.load(f)
        
    for package_name, content in readmes.items():
        package_path = os.path.join(src_dir, package_name)
        if not os.path.exists(package_path):
            print(f"Warning: Package path does not exist, creating it: {package_path}")
            os.makedirs(package_path, exist_ok=True)
            
        readme_path = os.path.join(package_path, "README.md")
        try:
            with open(readme_path, "w", encoding="utf-8") as f_out:
                f_out.write(content)
            print(f"Wrote README.md to {readme_path}")
        except Exception as e:
            print(f"Error writing README.md to {readme_path}: {e}")

if __name__ == "__main__":
    distribute()
