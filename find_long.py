import os
import re

folders = ["src/IAFahim.Compress", "src/IAFahim.Compress.Coordinate", "src/IAFahim.DS.Dsu", "src/IAFahim.DS.Fenwick", "src/IAFahim.DS.Grid", "src/IAFahim.DS.Heap", "src/IAFahim.DS.Mo", "src/IAFahim.DS.OrderedSet", "src/IAFahim.DS.PersistentDsu", "src/IAFahim.DS.PersistentTreap", "src/IAFahim.DS.PieceTable", "src/IAFahim.DS.RollbackSeg", "src/IAFahim.DS.RollbackStack", "src/IAFahim.DS.Rope", "src/IAFahim.DS.Splay", "src/IAFahim.DS.Treap", "src/IAFahim.DS.Trie", "src/IAFahim.DS.UnsafeArray"]

for f in folders:
    for root, dirs, files in os.walk(f):
        for file in files:
            if file.endswith('.cs'):
                path = os.path.join(root, file)
                with open(path, 'r', encoding='utf-8') as f:
                    lines = f.readlines()
                
                method_start = -1
                bracket_count = 0
                in_method = False
                
                for i, line in enumerate(lines):
                    if not in_method and re.search(r'\b(public|private|internal|protected|static)\s+.*\s+[a-zA-Z0-9_]+\s*\(.*\)', line):
                        if not ';' in line:
                            method_start = i
                            in_method = True
                            bracket_count = 0
                    if in_method:
                        bracket_count += line.count('{')
                        bracket_count -= line.count('}')
                        if bracket_count == 0 and '{' in ''.join(lines[method_start:i+1]):
                            length = i - method_start + 1
                            if length > 25:
                                print(f"{path}:{method_start+1} - {length} lines")
                            in_method = False
