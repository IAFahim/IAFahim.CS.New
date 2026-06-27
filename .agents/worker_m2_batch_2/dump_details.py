import json
import re

def main():
    with open("inputs.json", "r", encoding="utf-8") as f:
        data = json.load(f)
        
    out_lines = []
    for pkg in sorted(data.keys()):
        out_lines.append(f"========================================")
        out_lines.append(f"PACKAGE: {pkg}")
        out_lines.append(f"========================================")
        
        info = data[pkg]
        for file, content in sorted(info["code"].items()):
            out_lines.append(f"--- File: {file} ---")
            # extract class/struct declarations and public methods
            lines = content.splitlines()
            for i, line in enumerate(lines, 1):
                sline = line.strip()
                if not sline:
                    continue
                # Match class/struct/enum or public method signatures
                if any(x in sline for x in ["class ", "struct ", "enum ", "interface "]) or (("public " in sline or "internal " in sline) and ("(" in sline or "{" in sline)):
                    out_lines.append(f"{i:4d}: {sline}")
        out_lines.append("")
        
    with open("package_details.txt", "w", encoding="utf-8") as f:
        f.write("\n".join(out_lines))
    print("Wrote details to package_details.txt")

if __name__ == "__main__":
    main()
