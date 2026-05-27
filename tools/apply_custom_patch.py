import os
import re
import sys
import subprocess

def apply_patch(patch_path):
    print(f"Processing {patch_path}...")
    with open(patch_path, 'r') as f:
        content = f.read()

    if not content.startswith("*** Begin Patch***"):
        print(f"Error: {patch_path} does not start with *** Begin Patch***")
        sys.exit(1)

    # Remove Begin and End markers
    content = content.replace("*** Begin Patch***", "")
    if content.endswith("*** End Patch"):
        content = content[:-len("*** End Patch")]
    elif content.endswith("*** End Patch\n"):
        content = content[:-len("*** End Patch\n")]

    # The first file path follows immediately after *** Begin Patch***
    # Subsequent files follow *** Update File: 
    
    # Let's split by the marker
    parts = content.split("*** Update File: ")
    
    # Handle the first part which might have "Update File: " if it was after "Begin Patch***"
    parts[0] = parts[0].strip()
    if parts[0].startswith("Update File: "):
        parts[0] = parts[0][len("Update File: "):]
    parts[0] = parts[0].strip()

    for part in parts:
        part = part.strip()
        if not part:
            continue
        
        # Format is "file_path@@ hunk"
        # The @@ is the start of the unified diff hunk header
        idx = part.find("@@")
        if idx == -1:
            print(f"Warning: Could not find @@ in part starting with: {part[:50]}")
            continue
        
        file_path = part[:idx].strip()
        diff_content = part[idx:]
        
        # Create a standard patch
        temp_patch = "temp.patch"
        with open(temp_patch, 'w') as f:
            # unified diff needs --- and +++ lines
            f.write(f"--- {file_path}\n")
            f.write(f"+++ {file_path}\n")
            f.write(diff_content)
        
        print(f"Applying patch to {file_path}...")
        # Use patch -p0 --fuzz=3 to be a bit lenient if there are minor mismatches
        # but the user said "strict sequence" so maybe I should be strict.
        result = subprocess.run(["patch", "-p0", "--verbose"], input=open(temp_patch, 'rb').read(), capture_output=True)
        
        if result.returncode != 0:
            print(f"Failed to apply patch to {file_path}")
            print(result.stdout.decode())
            print(result.stderr.decode())
            os.remove(temp_patch)
            sys.exit(1)
        
        os.remove(temp_patch)
    print(f"Successfully applied {patch_path}")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python apply_custom_patch.py <patch_files...>")
        sys.exit(1)
    for p in sys.argv[1:]:
        apply_patch(p)
