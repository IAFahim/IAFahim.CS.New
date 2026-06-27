import os
import json

def main():
    outputs_file = '/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_5/outputs.json'
    src_dir = '/home/l/Github/IAFahim.CS.New/src'

    with open(outputs_file, 'r', encoding='utf-8') as f:
        readmes = json.load(f)

    for pkg_name, content in readmes.items():
        pkg_dir = os.path.join(src_dir, pkg_name)
        if not os.path.exists(pkg_dir):
            print(f"Directory {pkg_dir} does not exist. Creating it...")
            os.makedirs(pkg_dir, exist_ok=True)
        
        readme_path = os.path.join(pkg_dir, 'README.md')
        try:
            with open(readme_path, 'w', encoding='utf-8') as f_out:
                f_out.write(content)
            print(f"Wrote README.md to {readme_path}")
        except Exception as e:
            print(f"Error writing to {readme_path}: {e}")

    print("Distribution complete.")

if __name__ == '__main__':
    main()
