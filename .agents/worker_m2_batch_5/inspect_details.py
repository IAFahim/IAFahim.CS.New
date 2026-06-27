import json

with open('inputs.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

output_lines = []
for pkg, info in data.items():
    output_lines.append(f"==================================================")
    output_lines.append(f"PACKAGE: {pkg}")
    output_lines.append(f"==================================================")
    for file_path, content in info['files'].items():
        output_lines.append(f"File: {file_path}")
        lines = content.splitlines()
        # Grab class declaration and public signatures
        for i, line in enumerate(lines):
            stripped = line.strip()
            if not stripped:
                continue
            if 'public' in stripped or 'class' in stripped or 'struct' in stripped:
                output_lines.append(f"  {i+1:4d}: {stripped}")
    output_lines.append("")

with open('package_details.txt', 'w', encoding='utf-8') as f_out:
    f_out.write('\n'.join(output_lines))
print("package_details.txt generated.")
