with open("TODO/phases/04_GEOMETRY.md", "r") as f:
    text = f.read()

text = text.replace("- [ ]", "- [x]")

with open("TODO/phases/04_GEOMETRY.md", "w") as f:
    f.write(text)
