import os
import json

def main():
    batch_file = '/home/l/Github/IAFahim.CS.New/.agents/orchestrator/batches/batch_5.txt'
    src_dir = '/home/l/Github/IAFahim.CS.New/src'
    output_file = '/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_5/inputs.json'

    with open(batch_file, 'r', encoding='utf-8') as f:
        packages = [line.strip() for line in f if line.strip()]

    result = {}
    for pkg in packages:
        pkg_dir = os.path.join(src_dir, pkg)
        if not os.path.exists(pkg_dir):
            print(f"Warning: Directory {pkg_dir} does not exist.")
            continue

        pkg_data = {
            "files": {},
            "readme": ""
        }

        # Gather C# files (excluding bin, obj)
        for root, dirs, files in os.walk(pkg_dir):
            if 'bin' in dirs:
                dirs.remove('bin')
            if 'obj' in dirs:
                dirs.remove('obj')

            for file in files:
                if file.endswith('.cs'):
                    file_path = os.path.join(root, file)
                    rel_path = os.path.relpath(file_path, pkg_dir)
                    try:
                        with open(file_path, 'r', encoding='utf-8') as f_code:
                            pkg_data["files"][rel_path] = f_code.read()
                    except Exception as e:
                        print(f"Error reading {file_path}: {e}")

        # Gather README.md if it exists
        readme_path = os.path.join(pkg_dir, 'README.md')
        if os.path.exists(readme_path):
            try:
                with open(readme_path, 'r', encoding='utf-8') as f_readme:
                    pkg_data["readme"] = f_readme.read()
            except Exception as e:
                print(f"Error reading README.md in {pkg_dir}: {e}")

        result[pkg] = pkg_data

    with open(output_file, 'w', encoding='utf-8') as f_out:
        json.dump(result, f_out, indent=2, ensure_ascii=False)
    print("Aggregation complete. Inputs written to inputs.json.")

if __name__ == '__main__':
    main()
