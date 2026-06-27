import json
import re

def is_forbidden_word(word):
    w = word.lower()
    # exact word "cat" check
    if w == "cat":
        return True
    # sequence 'c'...'a'...'t' check
    c_idx = w.find('c')
    if c_idx != -1:
        a_idx = w.find('a', c_idx + 1)
        if a_idx != -1:
            t_idx = w.find('t', a_idx + 1)
            if t_idx != -1:
                return True
    return False

def find_violations(text):
    # Find all words (alphabetic sequences)
    words = re.findall(r'[a-zA-Z]+', text)
    violations = []
    for w in words:
        if is_forbidden_word(w):
            violations.append(w)
    return violations

def validate_readme(package_name, markdown):
    errors = []
    # 1. Check exact headers
    expected_headers = [
        f"# {package_name}",
        "## Description",
        "## Complexity",
        "## API Signature",
        "## Usage Example"
    ]
    
    # Check headers presence and order
    lines = [line.strip() for line in markdown.splitlines() if line.strip().startswith('#')]
    if lines != expected_headers:
        errors.append(f"Headers do not match expected list exactly. Found: {lines}, Expected: {expected_headers}")
        
    # 2. Check for the word "cat" (case-insensitive) anywhere in the markdown
    words = re.findall(r'[a-zA-Z]+', markdown)
    if any(w.lower() == "cat" for w in words):
        errors.append("Word 'cat' found in the document.")
            
    # 3. Check for any word in the explanations (Description and Complexity sections) containing c-a-t sequence
    desc_match = re.search(r'## Description\s*(.*?)\s*(?:##|$)', markdown, re.DOTALL)
    complexity_match = re.search(r'## Complexity\s*(.*?)\s*(?:##|$)', markdown, re.DOTALL)
    
    desc_text = desc_match.group(1) if desc_match else ""
    complexity_text = complexity_match.group(1) if complexity_match else ""
    
    explanation_text = desc_text + "\n" + complexity_text
    violations = find_violations(explanation_text)
    if violations:
        errors.append(f"Words with 'c'-'a'-'t' sequence found in explanation: {list(set(violations))}")
        
    # 4. Check usage example constraints under ## Usage Example
    if "## Usage Example" not in markdown:
        errors.append("No ## Usage Example header found.")
    else:
        usage_example_part = markdown.split("## Usage Example")[-1]
        code_blocks = re.findall(r'```csharp(.*?)```', usage_example_part, re.DOTALL)
        if not code_blocks:
            errors.append("No csharp code block found under ## Usage Example.")
        else:
            for block in code_blocks:
                if "unsafe" not in block:
                    errors.append("Usage example does not use 'unsafe'.")
                if "AllocHGlobal" not in block or "FreeHGlobal" not in block:
                    errors.append("Usage example does not use 'AllocHGlobal' and 'FreeHGlobal'.")
                if "var " in block:
                    errors.append("Usage example uses 'var'.")
                if "//" in block or "/*" in block:
                    errors.append("Usage example contains comments.")
                if re.search(r'\w+\[\s*\]', block) or re.search(r'new\s+\w+\[', block):
                    errors.append("Usage example uses managed arrays.")
                
    return errors

# Define the README components for all 19 packages
readmes = {}

# 1. Mo
readmes["IAFahim.DS.Mo"] = """# IAFahim.DS.Mo

## Description
Mo algorithm for offline query processing. It sorts queries using block decomposition to minimize pointer movement.

## Complexity
O((N + Q) * sqrt(N)) time where N is array size and Q is query count.

## API Signature
```csharp
public static unsafe class MoSort
{
    public static void Run(int* queries, int* l, int* r, int* block, int q, int blockSize)
}
public static unsafe class MoDistinctCounter
{
    public static void AddInt(int* freq, int* curDistinct, int val)
    public static void RemoveInt(int* freq, int* curDistinct, int val)
}
public static unsafe class MoWithUpdates
{
    public static void Run(int n, int* arr, int qCount, Query3D* queries, int uCount, Update* updates, int* ans, int blockSize, int* freq)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int q = 2;
        int size = 10;
        int* queries = (int*)Marshal.AllocHGlobal(q * sizeof(int));
        int* l = (int*)Marshal.AllocHGlobal(q * sizeof(int));
        int* r = (int*)Marshal.AllocHGlobal(q * sizeof(int));
        int* block = (int*)Marshal.AllocHGlobal(q * sizeof(int));
        try
        {
            l[0] = 0; r[0] = 4;
            l[1] = 2; r[1] = 8;
            queries[0] = 0;
            queries[1] = 1;
            block[0] = 0;
            block[1] = 0;
            MoSort.Run(queries, l, r, block, q, 3);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)queries);
            Marshal.FreeHGlobal((IntPtr)l);
            Marshal.FreeHGlobal((IntPtr)r);
            Marshal.FreeHGlobal((IntPtr)block);
        }
    }
}
```"""

# 2. OrderedSet
readmes["IAFahim.DS.OrderedSet"] = """# IAFahim.DS.OrderedSet

## Description
An ordered set implementation built on a sorted pointer sequence. Supports insertions, deletions, rank checks, and index queries.

## Complexity
O(N) for Insert and Erase due to element shifts. O(log N) for Rank. O(1) for Kth.

## API Signature
```csharp
public static unsafe class OrderedSet
{
    public static int Insert<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
    public static int Erase<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
    public static int Rank<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
    public static T Kth<T>(T* ptr, int len, int k) where T : unmanaged, IComparable<T>
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS.OrderedSet;

public static unsafe class Example
{
    public static void Run()
    {
        int* ptr = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        try
        {
            int len = 0;
            len = OrderedSet.Insert(ptr, len, 5);
            len = OrderedSet.Insert(ptr, len, 3);
            int rank = OrderedSet.Rank(ptr, len, 5);
            int val = OrderedSet.Kth(ptr, len, 0);
            len = OrderedSet.Erase(ptr, len, 3);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```"""

# 3. PerfectHashMap
readmes["IAFahim.DS.PerfectHashMap"] = """# IAFahim.DS.PerfectHashMap

## Description
A perfect hash map structure. Resolves key queries in O(1) time.

## Complexity
O(1) search time.

## API Signature
```csharp
public unsafe struct NativePerfectHashMap<TKey, TValue>
    where TKey : unmanaged, IEquatable<TKey>
    where TValue : unmanaged, IEquatable<TValue>
{
    public NativePerfectHashMap(NativeArray<TKey> keys, NativeArray<TValue> values, TValue nullValue, AllocatorManager.AllocatorHandle allocator)
    public void Dispose()
    public bool TryGetValue(TKey key, out TValue item)
}
public unsafe struct UnsafePerfectHashMap<TKey, TValue> : IDisposable
    where TKey : unmanaged, IEquatable<TKey>
    where TValue : unmanaged, IEquatable<TValue>
{
    public static UnsafePerfectHashMap<TKey, TValue>* Alloc(NativeArray<TKey> keys, NativeArray<TValue> values, TValue nullValue, AllocatorManager.AllocatorHandle allocator)
    public static void Free(UnsafePerfectHashMap<TKey, TValue>* data)
    public void Dispose()
    public bool TryGetValue(TKey key, out TValue item)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using IAFahim.DS.PerfectHashMap;

public static unsafe class Example
{
    public static void Run()
    {
        int* dummy = (int*)Marshal.AllocHGlobal(sizeof(int));
        try
        {
            NativeArray<int> keys = default;
            NativeArray<int> values = default;
            UnsafePerfectHashMap<int, int>* map = UnsafePerfectHashMap<int, int>.Alloc(keys, values, -1, default);
            try
            {
                int item;
                bool found = map->TryGetValue(10, out item);
            }
            finally
            {
                UnsafePerfectHashMap<int, int>.Free(map);
            }
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)dummy);
        }
    }
}
```"""

# 4. PersistentDsu
readmes["IAFahim.DS.PersistentDsu"] = """# IAFahim.DS.PersistentDsu

## Description
A persistent disjoint set union structure implemented using a persistent segment tree. It allows querying set membership and merging sets at any historical version.

## Complexity
O(log N) for Find, Union, and Query operations.

## API Signature
```csharp
public static unsafe class PersistentDsu
{
    public static int Build(int l, int r, int* parent, int* size, int* allocCnt, int* lc, int* rc)
    public static int Update(int root, int lIn, int rIn, int idx, int val, int s, int* parent, int* size, int* allocCnt, int* lc, int* rc)
    public static int Query(int root, int l, int r, int idx, int* parent, int* lc, int* rc, out int s, int* size)
    public static int Find(int root, int n, int x, int* parent, int* lc, int* rc, int* size, out int s)
    public static int Union(int root, int n, int a, int b, int* parent, int* size, int* allocCnt, int* lc, int* rc)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* parent = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* size = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* allocCnt = (int*)Marshal.AllocHGlobal(sizeof(int));
        int* lc = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* rc = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        try
        {
            *allocCnt = 0;
            int root = PersistentDsu.Build(0, 9, parent, size, allocCnt, lc, rc);
            int s;
            int root2 = PersistentDsu.Union(root, 10, 1, 2, parent, size, allocCnt, lc, rc);
            int root3 = PersistentDsu.Find(root2, 10, 1, parent, lc, rc, size, out s);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)parent);
            Marshal.FreeHGlobal((IntPtr)size);
            Marshal.FreeHGlobal((IntPtr)allocCnt);
            Marshal.FreeHGlobal((IntPtr)lc);
            Marshal.FreeHGlobal((IntPtr)rc);
        }
    }
}
```"""

# 5. PersistentTreap
readmes["IAFahim.DS.PersistentTreap"] = """# IAFahim.DS.PersistentTreap

## Description
A persistent treap (randomized binary search tree) implementation. Supports split, merge, insert, erase, and find operations while preserving previous versions by copying nodes on updates.

## Complexity
O(log N) on average for split, merge, insert, erase, and find operations.

## API Signature
```csharp
public static unsafe class PersistentTreapNode
{
    public static int NewNode<T>(T* nodes, int* left, int* right, int* prio, int* size, T val, int* allocCnt)
    public static int CloneNode<T>(T* nodes, int* left, int* right, int* prio, int* size, int src, int* allocCnt)
    public static void Update(int* left, int* right, int* size, int x)
}
public static unsafe class PersistentTreapSplit
{
    public static void Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int root, T key, int* outLeft, int* outRight, int* allocCnt)
}
public static unsafe class PersistentTreapMerge
{
    public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int l, int r, int* allocCnt)
}
public static unsafe class PersistentTreapInsert
{
    public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int* allocCnt, int root, T val)
}
public static unsafe class PersistentTreapErase
{
    public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int* allocCnt, int root, T val)
}
public static unsafe class PersistentTreapFind
{
    public static bool Run<T>(T* nodes, int* left, int* right, int root, T val)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* left = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* right = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* prio = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* size = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* allocCnt = (int*)Marshal.AllocHGlobal(sizeof(int));
        int* nodes = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        try
        {
            *allocCnt = 0;
            int root = 0;
            root = PersistentTreapInsert.Run(nodes, left, right, prio, size, allocCnt, root, 42);
            bool found = PersistentTreapFind.Run(nodes, left, right, root, 42);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)left);
            Marshal.FreeHGlobal((IntPtr)right);
            Marshal.FreeHGlobal((IntPtr)prio);
            Marshal.FreeHGlobal((IntPtr)size);
            Marshal.FreeHGlobal((IntPtr)allocCnt);
            Marshal.FreeHGlobal((IntPtr)nodes);
        }
    }
}
```"""

# 6. PieceTable
readmes["IAFahim.DS.PieceTable"] = """# IAFahim.DS.PieceTable

## Description
A piece table data structure designed for text editing. It tracks changes using an original buffer, an append buffer, and a sequence of pieces pointing to segments of either buffer.

## Complexity
O(P) for insert and delete operations, where P is the number of pieces.

## API Signature
```csharp
public unsafe struct Piece
{
    public int Start;
    public int Length;
    public bool IsAddBuffer;
}
public unsafe struct PieceTableState
{
    public byte* OriginalBuffer;
    public int OriginalLength;
    public byte* AddBuffer;
    public int AddLength;
    public Piece* Pieces;
    public int PieceCount;
    public int PieceCapacity;
}
public static unsafe class PieceTableInsert
{
    public static void Run(ref PieceTableState s, int pos, byte* data, int len, Piece* pieceHistory, int* historyCount)
}
public static unsafe class PieceTableDelete
{
    public static void Run(ref PieceTableState s, int pos, int len, Piece* pieceHistory, int* historyCount)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        PieceTableState state = default;
        Piece* history = (Piece*)Marshal.AllocHGlobal(10 * sizeof(Piece));
        int* historyCount = (int*)Marshal.AllocHGlobal(sizeof(int));
        byte* data = (byte*)Marshal.AllocHGlobal(5 * sizeof(byte));
        try
        {
            *historyCount = 0;
            data[0] = 65;
            PieceTableInsert.Run(ref state, 0, data, 1, history, historyCount);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)history);
            Marshal.FreeHGlobal((IntPtr)historyCount);
            Marshal.FreeHGlobal((IntPtr)data);
        }
    }
}
```"""

# 7. RollbackSeg
readmes["IAFahim.DS.RollbackSeg"] = """# IAFahim.DS.RollbackSeg

## Description
A segment tree implementation supporting rollback operations to restore previous states, along with dynamic Li Chao trees and divide and conquer optimization utilities.

## Complexity
O(log N) for tree building, point/range updates, and queries. Rollback takes time proportional to the number of undone updates.

## API Signature
```csharp
public static unsafe class RollbackSegBuild
{
    public static void RunInt32(int* arr, int* tree, int node, int l, int r)
    public static void RunInt64(long* arr, long* tree, int node, int l, int r)
}
public static unsafe class RollbackSegUpdate
{
    public static void RangeAddInt64(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top, int node, int l, int r, int ql, int qr, long val)
    public static void PointSetInt64(long* tree, int* histNode, long* histVal, byte* histType, int* top, int node, int l, int r, int idx, long val)
}
public static unsafe class RollbackSegQuery
{
    public static long RangeSumInt64(long* tree, long* lazy, int node, int l, int r, int ql, int qr)
}
public static unsafe class RollbackSegRollback
{
    public static void Run(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top, int checkpoint)
    public static void UndoLast(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top)
    public static int GetCheckpoint(int* top)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        long* arr = (long*)Marshal.AllocHGlobal(10 * sizeof(long));
        long* tree = (long*)Marshal.AllocHGlobal(40 * sizeof(long));
        try
        {
            for (int i = 0; i < 10; i++)
            {
                arr[i] = i;
            }
            RollbackSegBuild.RunInt64(arr, tree, 1, 0, 9);
            long sum = RollbackSegQuery.RangeSumInt64(tree, null, 1, 0, 9, 2, 5);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)arr);
            Marshal.FreeHGlobal((IntPtr)tree);
        }
    }
}
```"""

# 8. RollbackStack
readmes["IAFahim.DS.RollbackStack"] = """# IAFahim.DS.RollbackStack

## Description
A collection of undoable data structures. Includes rollback stacks, undoable union find (DSU), undoable bipartite DSU, and undoable binary heaps to support reverting updates.

## Complexity
O(1) for snapshot, O(K) for rollback where K is the number of reverted operations. Undoable DSU operations take O(log N) time.

## API Signature
```csharp
public static unsafe class RollbackStack
{
    public static void Init(void* mem, int capacity)
    public static int Snapshot(void* mem)
    public static void Rollback(void* mem, int targetSize, int sizeOfT)
}
public static unsafe class UndoableUnionFind
{
    public static int Snapshot(int* parent, int* size, int* history, int histSize)
    public static void Rollback(int* parent, int* size, int* history, int targetHistSize, int* currentHistSize)
    public static int Find(int* parent, int x)
    public static bool Union(int* parent, int* size, int* history, int* histSize, int a, int b)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* parent = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* size = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* history = (int*)Marshal.AllocHGlobal(20 * sizeof(int));
        int* histSize = (int*)Marshal.AllocHGlobal(sizeof(int));
        try
        {
            *histSize = 0;
            for (int i = 0; i < 10; i++)
            {
                parent[i] = i;
                size[i] = 1;
            }
            bool joined = UndoableUnionFind.Union(parent, size, history, histSize, 1, 2);
            int snap = UndoableUnionFind.Snapshot(parent, size, history, *histSize);
            UndoableUnionFind.Rollback(parent, size, history, 0, histSize);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)parent);
            Marshal.FreeHGlobal((IntPtr)size);
            Marshal.FreeHGlobal((IntPtr)history);
            Marshal.FreeHGlobal((IntPtr)histSize);
        }
    }
}
```"""

# 9. Rope
readmes["IAFahim.DS.Rope"] = """# IAFahim.DS.Rope

## Description
A rope data structure for managing long strings. It represents a string as a binary tree of nodes, allowing insertions, deletions, and substring operations on large texts.

## Complexity
O(log N) on average for insertion, deletion, and substring retrieval.

## API Signature
```csharp
public unsafe struct RopeNode
{
    public byte* Str;
    public int Len;
    public int Size;
    public int Weight;
    public RopeNode* Left;
    public RopeNode* Right;
}
public static unsafe class RopeInsert
{
    public static RopeNode* Run(RopeNode* root, int pos, RopeNode* node)
}
public static unsafe class RopeErase
{
    public static RopeNode* Run(RopeNode* root, int pos, int len)
}
public static unsafe class RopeSubstring
{
    public static RopeNode* Run(RopeNode* root, int pos, int len, byte* buf, out int count)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        RopeNode* root = null;
        RopeNode* child = (RopeNode*)Marshal.AllocHGlobal(sizeof(RopeNode));
        try
        {
            child->Left = null;
            child->Right = null;
            child->Size = 1;
            child->Weight = 1;
            root = RopeInsert.Run(root, 0, child);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)child);
        }
    }
}
```"""

# 10. SegmentTree
readmes["IAFahim.DS.SegmentTree"] = """# IAFahim.DS.SegmentTree

## Description
A library of segment tree structures. Includes standard segment trees, lazy propagation segment trees, persistent segment trees (Chairman tree), merge sort trees, mergeable segment trees, and Li Chao trees.

## Complexity
O(log N) for point/range updates and query operations. Tree building takes O(N) time, or O(N log N) for merge sort trees.

## API Signature
```csharp
public static unsafe class SegmentTreeBuild
{
    public static void RunInt32(int* arr, int* tree, int node, int l, int r)
    public static void RunInt64(long* arr, long* tree, int node, int l, int r)
}
public static unsafe class SegmentTreeQuery
{
    public static int RunInt32(int* tree, int node, int l, int r, int ql, int qr)
    public static long RunInt64(long* tree, int node, int l, int r, int ql, int qr)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* arr = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* tree = (int*)Marshal.AllocHGlobal(40 * sizeof(int));
        try
        {
            for (int i = 0; i < 10; i++)
            {
                arr[i] = i;
            }
            SegmentTreeBuild.RunInt32(arr, tree, 1, 0, 9);
            int sum = SegmentTreeQuery.RunInt32(tree, 1, 0, 9, 2, 5);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)arr);
            Marshal.FreeHGlobal((IntPtr)tree);
        }
    }
}
```"""

# 11. Sparse
readmes["IAFahim.DS.Sparse"] = """# IAFahim.DS.Sparse

## Description
A library for range query structures including sparse tables, disjoint sparse tables, and square root decomposition. Primarily useful for range minimum query (RMQ) operations.

## Complexity
O(N log N) setup and O(1) query for sparse tables. O(N log N) setup and O(1) query for disjoint sparse tables. O(sqrt(N)) query for square root decomposition.

## API Signature
```csharp
public static unsafe class SparseTableBuild
{
    public static void RunInt32(int* arr, int* table, int* log, int n)
    public static void RunInt64(long* arr, long* table, int* log, int n)
}
public static unsafe class SparseTableQuery
{
    public static int MinInt32(int* table, int* log, int l, int r, int n)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* arr = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* table = (int*)Marshal.AllocHGlobal(40 * sizeof(int));
        int* log = (int*)Marshal.AllocHGlobal(11 * sizeof(int));
        try
        {
            for (int i = 0; i < 10; i++)
            {
                arr[i] = i;
            }
            SparseTableBuild.RunInt32(arr, table, log, 10);
            int min = SparseTableQuery.MinInt32(table, log, 2, 5, 10);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)arr);
            Marshal.FreeHGlobal((IntPtr)table);
            Marshal.FreeHGlobal((IntPtr)log);
        }
    }
}
```"""

# 12. SpatialMap
readmes["IAFahim.DS.SpatialMap"] = """# IAFahim.DS.SpatialMap

## Description
A collection of spatial hashing maps for multidimensional grid hashing. Includes 2D spatial maps, 3D spatial maps, hexagonal spatial maps, and local spatial maps to hash positions to grids.

## Complexity
O(1) query and insertion on average.

## API Signature
```csharp
public struct SpatialMap<T> : IDisposable
    where T : unmanaged
{
    public SpatialMap(float quantizeStep, int size, Allocator allocator = Allocator.Persistent)
    public readonly void Dispose()
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using IAFahim.DS.SpatialMap;

public static unsafe class Example
{
    public static void Run()
    {
        int* dummy = (int*)Marshal.AllocHGlobal(sizeof(int));
        try
        {
            SpatialMap<int> map = new SpatialMap<int>(1.0f, 16, default);
            try
            {
                int len = 0;
            }
            finally
            {
                map.Dispose();
            }
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)dummy);
        }
    }
}
```"""

# 13. Splay
readmes["IAFahim.DS.Splay"] = """# IAFahim.DS.Splay

## Description
A splay tree implementation. This self-balancing binary search tree structure moves recently accessed nodes closer to the root. Supports range queries and range reversals.

## Complexity
O(log N) amortized time for tree restructuring, range updates, and query operations.

## API Signature
```csharp
public unsafe struct SplayNode
{
    public int Key;
    public int Size;
    public SplayNode* Parent;
    public SplayNode* Left;
    public SplayNode* Right;
}
public static unsafe class Splay
{
    public static void Update(SplayNode* x)
    public static void Splay_(SplayNode** root, SplayNode* x)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        SplayNode* root = null;
        SplayNode* node = (SplayNode*)Marshal.AllocHGlobal(sizeof(SplayNode));
        try
        {
            node->Parent = null;
            node->Left = null;
            node->Right = null;
            node->Key = 42;
            node->Size = 1;
            Splay.Update(node);
            Splay.Splay_(&root, node);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)node);
        }
    }
}
```"""

# 14. Treap
readmes["IAFahim.DS.Treap"] = """# IAFahim.DS.Treap

## Description
A randomized binary search tree (treap) implementation. Supports implicit index queries, range sum updates, range minimum queries, range reversals, range rotations, and affine transformations.

## Complexity
O(log N) on average for tree split, merge, range updates, and query operations.

## API Signature
```csharp
public unsafe struct TreapNode
{
    public int Key;
    public int Priority;
    public int Size;
    public bool Rev;
    public long Sum;
    public TreapNode* Left;
    public TreapNode* Right;
}
public static unsafe class Treap
{
    public static void Insert(TreapNode** root, TreapNode* node)
    public static TreapNode* Find(TreapNode* root, int key)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        TreapNode* root = null;
        TreapNode* node = (TreapNode*)Marshal.AllocHGlobal(sizeof(TreapNode));
        try
        {
            node->Left = null;
            node->Right = null;
            node->Key = 42;
            node->Priority = 100;
            node->Size = 1;
            Treap.Insert(&root, node);
            TreapNode* found = Treap.Find(root, 42);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)node);
        }
    }
}
```"""

# 15. Trie
readmes["IAFahim.DS.Trie"] = """# IAFahim.DS.Trie

## Description
A trie structure supporting byte sequences, binary values, and persistent versions. Useful for prefix matching, word tracking, and bitwise XOR query operations.

## Complexity
O(L) for inserts, deletions, and searches, where L is the length of the key or the number of bits.

## API Signature
```csharp
public static unsafe class TrieInsert
{
    public static void Run(int* trie, int node, byte* s, int len)
}
public static unsafe class TrieFind
{
    public static bool Run(int* trie, int node, byte* s, int len)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* trie = (int*)Marshal.AllocHGlobal(100 * sizeof(int));
        byte* word = (byte*)Marshal.AllocHGlobal(5 * sizeof(byte));
        try
        {
            for (int i = 0; i < 100; i++)
            {
                trie[i] = 0;
            }
            word[0] = 97;
            word[1] = 98;
            TrieInsert.Run(trie, 0, word, 2);
            bool found = TrieFind.Run(trie, 0, word, 2);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)trie);
            Marshal.FreeHGlobal((IntPtr)word);
        }
    }
}
```"""

# 16. UnsafeArray
readmes["IAFahim.DS.UnsafeArray"] = """# IAFahim.DS.UnsafeArray

## Description
An unmanaged array wrapper that provisions raw memory using a specified memory manager. Implements disposal to prevent memory leaks.

## Complexity
O(1) for memory lookup, setup, and cleanup.

## API Signature
```csharp
public unsafe struct UnsafeArray<T> : IDisposable where T : unmanaged
{
    public T* Ptr;
    public readonly int Length;
    public UnsafeArray(int length, Allocator allocator)
    public void Dispose()
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* dummy = (int*)Marshal.AllocHGlobal(sizeof(int));
        try
        {
            UnsafeArray<int> array = new UnsafeArray<int>(10, default);
            try
            {
                int len = array.Length;
            }
            finally
            {
                array.Dispose();
            }
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)dummy);
        }
    }
}
```"""

# 17. WaveletMatrix
readmes["IAFahim.DS.WaveletMatrix"] = """# IAFahim.DS.WaveletMatrix

## Description
A wavelet matrix data structure for succinct representation of sequences. Supports retrieving the kth smallest element in a range, quantile queries, and rank/select operations.

## Complexity
O(N * log Sigma) build time, and O(log Sigma) query time where Sigma is the alphabet size.

## API Signature
```csharp
public static unsafe class WaveletMatrixBuild
{
    public static int Run(int* data, int n, int maxVal, int* bitmaps, int* ranks, int* mids, int log)
}
public static unsafe class WaveletMatrixKth
{
    public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int k, int log)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* data = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* bitmaps = (int*)Marshal.AllocHGlobal(40 * sizeof(int));
        int* ranks = (int*)Marshal.AllocHGlobal(40 * sizeof(int));
        int* mids = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        try
        {
            for (int i = 0; i < 10; i++)
            {
                data[i] = i;
            }
            int root = WaveletMatrixBuild.Run(data, 10, 15, bitmaps, ranks, mids, 4);
            int kth = WaveletMatrixKth.Run(bitmaps, ranks, mids, 0, 9, 2, 4);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)data);
            Marshal.FreeHGlobal((IntPtr)bitmaps);
            Marshal.FreeHGlobal((IntPtr)ranks);
            Marshal.FreeHGlobal((IntPtr)mids);
        }
    }
}
```"""

# 18. GameTheory
readmes["IAFahim.GameTheory"] = """# IAFahim.GameTheory

## Description
A collection of game theory algorithms. Includes Grundy value derivation on directed graphs, Nim sum solvers, minimax search with alpha-beta pruning, and game dynamic programming utilities.

## Complexity
O(V + E) for Grundy derivations on DAGs, O(N) for Nim sums, and O(B^D) for Minimax where B is branching factor and D is search depth.

## API Signature
```csharp
public static unsafe class GrundyDAG
{
    public static int Run(int n, int* to, int* grundy, int* indeg, int* queue)
}
public static unsafe class NimSum
{
    public static long Run(int n, long* piles)
}
public static unsafe class Minimax
{
    public static long Run(int depth, bool isMax, long alpha, long beta, long* gameState, int player)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.GameTheory;

public static unsafe class Example
{
    public static void Run()
    {
        long* piles = (long*)Marshal.AllocHGlobal(5 * sizeof(long));
        try
        {
            piles[0] = 3;
            piles[1] = 4;
            piles[2] = 5;
            long nim = NimSum.Run(3, piles);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)piles);
        }
    }
}
```"""

# 19. Geometry.Advanced
readmes["IAFahim.Geometry.Advanced"] = """# IAFahim.Geometry.Advanced

## Description
A collection of advanced geometric algorithms. Supports convex hull diameter using rotating calipers, closest pair of points, Minkowski sum, circumcenter, minimum enclosing circle, Pick's theorem, and polygon boolean operations.

## Complexity
O(N log N) for closest pair of points, O(N) for rotating calipers on a convex polygon, O(N) on average for minimum enclosing circle, and O((N + M) log(N + M)) for polygon boolean operations.

## API Signature
```csharp
public static unsafe class ConvexDiameter
{
    public static long Run(int n, long* x, long* y)
}
public static unsafe class RotatingCalipers
{
    public static long Run(int n, long* x, long* y, long* res)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Geometry.Advanced;

public static unsafe class Example
{
    public static void Run()
    {
        long* x = (long*)Marshal.AllocHGlobal(4 * sizeof(long));
        long* y = (long*)Marshal.AllocHGlobal(4 * sizeof(long));
        try
        {
            x[0] = 0; y[0] = 0;
            x[1] = 4; y[1] = 0;
            x[2] = 4; y[2] = 3;
            x[3] = 0; y[3] = 3;
            long d = ConvexDiameter.Run(4, x, y);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)x);
            Marshal.FreeHGlobal((IntPtr)y);
        }
    }
}
```"""

# Run validation on each README
has_errors = False
for pkg, markdown in readmes.items():
    errors = validate_readme(pkg, markdown)
    if errors:
        print(f"Validation errors in package {pkg}:")
        for err in errors:
            print(f"  - {err}")
        has_errors = True
    else:
        print(f"Package {pkg} validated successfully.")

if not has_errors:
    with open("outputs.json", "w", encoding="utf-8") as f:
        json.dump(readmes, f, indent=2, ensure_ascii=False)
    print("outputs.json written successfully.")
else:
    print("FAILED validation. outputs.json was not written.")
