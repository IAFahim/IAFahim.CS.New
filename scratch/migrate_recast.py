import os
import shutil

src_dir = "/home/l/Github/IAFahim.CS.New/TODO/com.bovinelabs.recast/BovineLabs.Recast"
test_dir = "/home/l/Github/IAFahim.CS.New/TODO/com.bovinelabs.recast/BovineLabs.Recast.Tests"

dest_src = "/home/l/Github/IAFahim.CS.New/src/IAFahim.Pathfinding.Recast"
dest_test = "/home/l/Github/IAFahim.CS.New/test/IAFahim.Pathfinding.Recast.Tests"

os.makedirs(dest_src, exist_ok=True)
os.makedirs(dest_test, exist_ok=True)

def migrate_files(src, dest):
    for root, dirs, files in os.walk(src):
        rel_path = os.path.relpath(root, src)
        dest_root = dest if rel_path == "." else os.path.join(dest, rel_path)
        os.makedirs(dest_root, exist_ok=True)
        for file in files:
            if file.endswith(".meta") or file.endswith(".asmdef"):
                continue
            src_file = os.path.join(root, file)
            dest_file = os.path.join(dest_root, file)
            
            with open(src_file, 'r', encoding='utf-8') as f:
                content = f.read()
            
            content = content.replace("namespace BovineLabs.Recast", "namespace IAFahim.Pathfinding.Recast")
            content = content.replace("using BovineLabs.Recast", "using IAFahim.Pathfinding.Recast")
            content = content.replace("BovineLabs.Recast.Tests", "IAFahim.Pathfinding.Recast.Tests")
            
            with open(dest_file, 'w', encoding='utf-8') as f:
                f.write(content)

migrate_files(src_dir, dest_src)
migrate_files(test_dir, dest_test)
print("Migration completed successfully!")
