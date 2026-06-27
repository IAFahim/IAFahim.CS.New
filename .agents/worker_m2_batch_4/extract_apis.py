import json
import re

def extract_api_info(content):
    class_match = re.search(r'public\s+(?:unsafe\s+)?(?:static\s+)?(?:class|struct)\s+(\w+)', content)
    if not class_match:
        class_match = re.search(r'(?:class|struct)\s+(\w+)', content)
        if not class_match:
            return None
    class_name = class_match.group(1)
    
    methods = []
    for line in content.splitlines():
        line = line.strip()
        if "public" in line and "(" in line and "class" not in line and "struct" not in line and "delegate" not in line:
            methods.append(line)
            
    return {
        "class_name": class_name,
        "methods": methods
    }

def main():
    with open("inputs.json", "r", encoding="utf-8") as f:
        data = json.load(f)
        
    out_lines = []
    for pkg_name, pkg_data in data.items():
        out_lines.append(f"==================================================")
        out_lines.append(f"Package: {pkg_name}")
        for file, content in pkg_data["csharp_files"].items():
            info = extract_api_info(content)
            if info:
                out_lines.append(f"  File: {file} -> Class: {info['class_name']}")
                for m in info['methods']:
                    out_lines.append(f"    {m}")
                    
    with open("extracted_apis.txt", "w", encoding="utf-8") as f_out:
        f_out.write("\n".join(out_lines))

if __name__ == "__main__":
    main()
