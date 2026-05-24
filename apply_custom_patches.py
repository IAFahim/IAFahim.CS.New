import os
import re

def apply_patch_content(file_path, hunks):
    if not os.path.exists(file_path):
        print(f"File not found: {file_path}")
        return
    
    with open(file_path, 'r') as f:
        lines = f.readlines()
    
    # Sort hunks by start line in reverse to avoid offset issues
    # But these are unified diff hunks, so we should apply them carefully.
    # A simpler way is to use the 'patch' command if we can reconstruct a standard patch.
    
    patch_text = f"--- {file_path}\n+++ {file_path}\n" + "".join(hunks)
    
    with open("temp.patch", "w") as f:
        f.write(patch_text)
    
    res = os.system(f"patch -p0 < temp.patch")
    if res != 0:
        print(f"Failed to apply patch to {file_path}")
    else:
        print(f"Applied patch to {file_path}")

def process_patch_file(patch_file):
    if not os.path.exists(patch_file):
        return

    with open(patch_file, 'r') as f:
        content = f.read()

    # Split by Update File:
    parts = re.split(r'\*\*\* Update File: |\*\*\* Begin Patch\*\*\* Update File: ', content)
    for part in parts:
        if not part.strip() or part.strip() == "*** End Patch":
            continue
        
        # Handle end of patch
        if "*** End Patch" in part:
            part = part.split("*** End Patch")[0]
            
        match = re.match(r'([^@]+)@@', part)
        if match:
            file_path = match.group(1).strip()
            hunk_content = part[len(match.group(0)) - 2:]
            
            # The hunk content starts with @@
            # We might have multiple hunks for the same file in one 'part'
            apply_patch_content(file_path, [hunk_content])

patch_files = [
    "nunit-group1.patch",
    "nunit-group2.patch",
    "nunit-group3.patch",
    "nunit-group4.patch",
    "nunit-group5a.patch",
    "nunit-group5b.patch",
    "nunit-group6.patch",
    "nunit-tests.patch"
]

for pf in patch_files:
    print(f"Processing {pf}...")
    process_patch_file(pf)
