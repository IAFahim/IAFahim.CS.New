import json

with open('inputs.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

for pkg, info in data.items():
    print(f"Package: {pkg}")
    for file_path, content in info['files'].items():
        print(f"  File: {file_path}")
        # Print namespaces, classes, methods to get a feel
        lines = content.splitlines()
        for line in lines:
            if 'public class' in line or 'public struct' in line or 'public static class' in line or 'public unsafe class' in line or 'public unsafe struct' in line or 'public static unsafe class' in line:
                print(f"    Class/Struct: {line.strip()}")
            if 'public static' in line and '(' in line:
                print(f"    Method: {line.strip()}")
    print("-" * 50)
