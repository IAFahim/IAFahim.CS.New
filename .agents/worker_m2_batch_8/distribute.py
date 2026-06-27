import json
import os

WORKSPACE_ROOT = "/home/l/Github/IAFahim.CS.New"
OUTPUTS_FILE = os.path.join(WORKSPACE_ROOT, ".agents/worker_m2_batch_8/outputs.json")

def main():
    if not os.path.exists(OUTPUTS_FILE):
        print(f"Error: {OUTPUTS_FILE} does not exist.")
        return

    with open(OUTPUTS_FILE, "r", encoding="utf-8") as f:
        readmes = json.load(f)

    for pkg, content in readmes.items():
        dest_dir = os.path.join(WORKSPACE_ROOT, "src", pkg)
        if not os.path.isdir(dest_dir):
            print(f"Warning: Destination directory {dest_dir} does not exist. Creating it.")
            os.makedirs(dest_dir, exist_ok=True)
        
        dest_file = os.path.join(dest_dir, "README.md")
        with open(dest_file, "w", encoding="utf-8") as out:
            out.write(content)
        print(f"Successfully wrote README.md to {dest_file}")

if __name__ == "__main__":
    main()
