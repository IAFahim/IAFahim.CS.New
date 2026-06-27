import os
import re

def parse_cs_file(filepath):
    """
    Parses a .cs file to find public classes, structs, and their public members.
    Returns a list of dicts: [{'type': 'class'/'struct', 'name': '...', 'members': [...]}]
    """
    with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()

    # Strip single line comments and multi-line comments first to avoid false positives
    # But preserve line content structure
    content_no_comments = re.sub(r'//.*', '', content)
    # Block comments
    content_no_comments = re.sub(r'/\*.*?\*/', '', content_no_comments, flags=re.DOTALL)

    lines = content_no_comments.split('\n')
    
    results = []
    current_type = None  # 'class' or 'struct'
    current_name = None
    current_members = []
    
    # We can use regex to find:
    # 1. public class/struct declarations
    # 2. public methods/properties/fields/constructors
    
    class_struct_pattern = re.compile(
        r'\bpublic\s+(?:static\s+|unsafe\s+|readonly\s+|partial\s+)*(class|struct)\s+(\w+)'
    )
    
    # Member matching logic:
    # If a line contains 'public', and we are currently inside a class/struct.
    # It must not be the class/struct line itself.
    # Let's inspect each line.
    
    bracket_depth = 0
    type_bracket_depth = -1 # depth where current class/struct was defined
    
    for line in lines:
        stripped = line.strip()
        if not stripped:
            continue
            
        # Track bracket depth to know when we exit a class/struct
        # This is a naive bracket tracker, but works well for well-formatted code
        open_brackets = stripped.count('{')
        close_brackets = stripped.count('}')
        
        # Check if we are declaring a public class or struct
        match = class_struct_pattern.search(stripped)
        if match:
            # If we were already in one, push it
            if current_name:
                results.append({
                    'type': current_type,
                    'name': current_name,
                    'members': current_members
                })
            current_type = match.group(1)
            current_name = match.group(2)
            current_members = []
            type_bracket_depth = bracket_depth
            # If the class declaration line also has '{', increment depth
            bracket_depth += open_brackets
            bracket_depth -= close_brackets
            continue
            
        # Track depth
        bracket_depth += open_brackets
        bracket_depth -= close_brackets
        
        # If we exit the class/struct scope
        if current_name and bracket_depth <= type_bracket_depth:
            results.append({
                'type': current_type,
                'name': current_name,
                'members': current_members
            })
            current_type = None
            current_name = None
            current_members = []
            type_bracket_depth = -1
            continue
            
        # If we are inside a class/struct, look for public members
        if current_name and 'public' in stripped:
            # We want to clean up the line to present the signature cleanly
            # If the signature spans multiple lines (e.g. where clauses), we might need to handle it.
            # Usually in this codebase, method signatures are on a single line or we can capture the whole declaration.
            # Let's capture the stripped line, removing opening brace if present.
            member_sig = stripped
            if member_sig.endswith('{'):
                member_sig = member_sig[:-1].strip()
            # If it's a property/method/field, clean up extra trailing semicolons or spaces
            # Ensure it's not a class declaration (redundant check)
            if 'class' not in member_sig and 'struct' not in member_sig:
                current_members.append(member_sig)
                
    # Push last one if any
    if current_name:
        results.append({
            'type': current_type,
            'name': current_name,
            'members': current_members
        })
        
    return results

def get_readme_description(package_dir):
    readme_path = os.path.join(package_dir, 'README.md')
    if not os.path.exists(readme_path):
        # try case-insensitive check
        for name in os.listdir(package_dir):
            if name.lower() == 'readme.md':
                readme_path = os.path.join(package_dir, name)
                break
        else:
            return "No README.md"
            
    with open(readme_path, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()
        
    # We want to extract the informal "cat's voice" description
    # Usually it's under "## Use case" or similar, or just paragraphs containing "cat" or descriptions.
    # Let's clean up the markdown and extract lines of interest, or return the whole text if small.
    # Let's look for sections or sentences. Let's find "Curious cat..." or the "Use case" section.
    lines = content.split('\n')
    use_case_found = False
    use_case_lines = []
    
    for line in lines:
        stripped = line.strip()
        if 'use case' in stripped.lower():
            use_case_found = True
            continue
        if use_case_found:
            if stripped.startswith('#'):
                # next header, stop
                break
            if stripped:
                use_case_lines.append(stripped)
                
    if use_case_lines:
        return ' '.join(use_case_lines)
        
    # If no "Use case" section, let's extract the first non-header non-empty line
    for line in lines:
        stripped = line.strip()
        if stripped and not stripped.startswith('#'):
            return stripped
            
    return content.strip()

def main():
    src_dir = '/home/l/Github/IAFahim.CS.New/src'
    packages = []
    
    # List all subdirectories in src
    for item in sorted(os.listdir(src_dir)):
        item_path = os.path.join(src_dir, item)
        if not os.path.isdir(item_path):
            continue
            
        # Check if there is a .csproj in this dir
        csprojs = [f for f in os.listdir(item_path) if f.endswith('.csproj')]
        if not csprojs:
            continue
            
        # This is a package directory!
        package_name = item
        relative_path = f"src/{item}"
        
        # Scan C# files
        cs_files = []
        apis = []
        for root, dirs, files in os.walk(item_path):
            # Exclude bin, obj
            if 'bin' in dirs:
                dirs.remove('bin')
            if 'obj' in dirs:
                dirs.remove('obj')
                
            for file in sorted(files):
                if file.endswith('.cs'):
                    filepath = os.path.join(root, file)
                    rel_cs_path = os.path.relpath(filepath, src_dir)
                    cs_files.append(rel_cs_path)
                    
                    file_apis = parse_cs_file(filepath)
                    apis.extend(file_apis)
                    
        # Read README description
        desc = get_readme_description(item_path)
        
        packages.append({
            'name': package_name,
            'path': relative_path,
            'cs_files': cs_files,
            'apis': apis,
            'description': desc
        })
        
    # Now format handoff.md
    # Header
    report = []
    report.append("# Discovery Report — Setup & Discovery")
    report.append("\nThis report lists all 153 packages in the workspace, their public C# APIs, and existing descriptions.\n")
    
    report.append("## Packages List\n")
    
    for pkg in packages:
        report.append(f"### {pkg['name']}")
        report.append(f"- **Path**: `{pkg['path']}`")
        
        desc_clean = pkg['description'].replace('\n', ' ')
        report.append(f"- **Description**: {desc_clean}")
        
        if pkg['cs_files']:
            report.append("- **C# Source Files**:")
            for cs in pkg['cs_files']:
                report.append(f"  - `{cs}`")
        else:
            report.append("- **C# Source Files**: None")
            
        if pkg['apis']:
            report.append("- **Public APIs**:")
            for api in pkg['apis']:
                type_str = api['type']
                name_str = api['name']
                report.append(f"  - **{type_str}** `{name_str}`")
                for member in api['members']:
                    report.append(f"    - `{member}`")
        else:
            report.append("- **Public APIs**: None or internal/private only")
        report.append("") # empty line separator
        
    # Write report to file
    output_path = '/home/l/Github/IAFahim.CS.New/.agents/explorer_m1_discovery/handoff_raw.md'
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write('\n'.join(report))
        
    print(f"Discovery complete. Wrote {len(packages)} packages to {output_path}")

if __name__ == '__main__':
    main()
