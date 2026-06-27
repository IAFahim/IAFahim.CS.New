import json

def main():
    with open("inputs.json", "r", encoding="utf-8") as f:
        data = json.load(f)
        
    for pkg, info in data.items():
        print(f"Package: {pkg}")
        print("Files:")
        for file, content in info["code"].items():
            print(f"  - {file}")
            # print first few lines of the file to see namespace/class/struct and methods
            lines = content.splitlines()
            for line in lines[:25]:
                if any(k in line for k in ["struct", "class", "public", "void", "static"]):
                    print(f"    {line.strip()}")
        print("-" * 40)

if __name__ == "__main__":
    main()
