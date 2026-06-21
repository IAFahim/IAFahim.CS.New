import os
import re

def count_complexity(code):
    keywords = ['if', 'for', 'while', 'foreach', 'case', '&&', '||', '?']
    cc = 1
    # Remove strings and comments
    code = re.sub(r'".*?"', '', code)
    code = re.sub(r'//.*', '', code)
    code = re.sub(r'/\*.*?\*/', '', code, flags=re.DOTALL)
    for word in keywords:
        if word in ['&&', '||', '?']:
            cc += code.count(word)
        else:
            cc += len(re.findall(r'\b' + word + r'\b', code))
    return cc

def process_file(path):
    with open(path, 'r') as f:
        content = f.read()
    
    # Very naive method extraction based on "public static", "private static", "internal static"
    methods = re.finditer(r'(public|private|internal)\s+(static\s+)?(\w+\*?\s+)*\w+\s*\([^)]*\)\s*\{', content)
    results = []
    
    for match in methods:
        start_idx = match.end() - 1
        depth = 0
        end_idx = start_idx
        for i in range(start_idx, len(content)):
            if content[i] == '{':
                depth += 1
            elif content[i] == '}':
                depth -= 1
                if depth == 0:
                    end_idx = i
                    break
        if end_idx > start_idx:
            method_body = content[start_idx:end_idx+1]
            cc = count_complexity(method_body)
            method_name = match.group(0).split('(')[0].strip().split()[-1]
            if cc > 4:
                results.append((method_name, cc))
    
    return results

high_cc_files = {}
for root, _, files in os.walk('src'):
    for file in files:
        if file.endswith('.cs'):
            path = os.path.join(root, file)
            res = process_file(path)
            if res:
                high_cc_files[path] = res

for path, methods in sorted(high_cc_files.items(), key=lambda x: max([m[1] for m in x[1]]), reverse=True):
    for m, cc in methods:
        print(f"{path}: Method {m} has CC={cc}")
