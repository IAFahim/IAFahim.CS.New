import json
import re

def contains_cat_seq(word):
    word = word.lower()
    c_idx = word.find('c')
    if c_idx == -1:
        return False
    a_idx = word.find('a', c_idx + 1)
    if a_idx == -1:
        return False
    t_idx = word.find('t', a_idx + 1)
    if t_idx == -1:
        return False
    return True

def validate_readme(pkg_name, content):
    # 1. Check for case-insensitive 'cat' anywhere
    if "cat" in content.lower():
        return False, "Contains case-insensitive substring 'cat'"
        
    # 2. Check for required headers
    required_headers = [
        f"# {pkg_name}",
        "## Description",
        "## Complexity",
        "## API Signature",
        "## Usage Example"
    ]
    for header in required_headers:
        if header not in content:
            return False, f"Missing required header: {header}"
            
    # 3. Check for words in explanation containing 'c', 'a', 't' in sequence
    # Remove C# code blocks
    explanation = re.sub(r'```csharp.*?```', '', content, flags=re.DOTALL)
    words = re.findall(r'\b\w+\b', explanation)
    for w in words:
        if contains_cat_seq(w):
            return False, f"Explanation contains word with 'c','a','t' sequence: '{w}'"
            
    # 4. Check C# block constraints
    if "## Usage Example" not in content:
        return False, "Missing '## Usage Example' section"
        
    usage_part = content.split("## Usage Example")[1]
    if "```csharp" not in usage_part:
        return False, "Missing C# code block"
        
    cs_block = usage_part.split("```csharp")[1].split("```")[0]
    
    # Check no 'var'
    if "var" in re.findall(r'\bvar\b', cs_block):
        return False, "C# block contains forbidden 'var' keyword"
        
    # Check no managed arrays
    if re.search(r'new\s+\w+\[', cs_block):
        return False, "C# block contains managed array ('new T[]')"
        
    # Check no comments
    if "//" in cs_block or "/*" in cs_block:
        return False, "C# block contains comments"
        
    # Check AllocHGlobal and FreeHGlobal (except for empty packages if they don't allocate, but we put allocations in all of them)
    if "AllocHGlobal" not in cs_block or "FreeHGlobal" not in cs_block:
        return False, "C# block missing AllocHGlobal or FreeHGlobal"
        
    if "try" not in cs_block or "finally" not in cs_block:
        return False, "C# block missing try/finally"
        
    if "unsafe" not in cs_block:
        return False, "C# block missing 'unsafe'"
        
    return True, ""

readmes = {}

# 1. IAFahim.Graph.DAG
readmes["IAFahim.Graph.DAG"] = """# IAFahim.Graph.DAG

## Description
This package provides algorithms for directed acyclic graphs. It supports topological sorting, path counts, longest antichain search, minimum path covers, and cycle checks.

## Complexity
Time complexity for topological sorting is O(V + E) where V is the node count and E is the edge count. Minimum path cover runs in O(V * E) time.

## API Signature
```csharp
public static unsafe class CountTopologicalOrders
{
    public static long Run(int* adjMask, int n, long* dp)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 4;
    int* adjMask = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    long* dp = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal((1 << n) * sizeof(long));
    try
    {
        adjMask[0] = 2;
        adjMask[1] = 4;
        adjMask[2] = 8;
        adjMask[3] = 0;
        long total = IAFahim.Graph.DAG.CountTopologicalOrders.Run(adjMask, n, dp);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)adjMask);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)dp);
    }
}
```"""

# 2. IAFahim.Graph.Decomposition
readmes["IAFahim.Graph.Decomposition"] = """# IAFahim.Graph.Decomposition

## Description
This package provides graph decomposition methods to split graphs into sub-components.

## Complexity
Time and space complexity depend on the specific decomposition routine.

## API Signature
```csharp
public static unsafe class Decomposition
{
    public static void Run(int* ptr, int len)
}
```

## Usage Example
```csharp
unsafe
{
    int len = 10;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        IAFahim.Graph.Decomposition.Run(ptr, len);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```"""

# 3. IAFahim.Graph.Dominator
readmes["IAFahim.Graph.Dominator"] = """# IAFahim.Graph.Dominator

## Description
This package provides dominator tree construction algorithms for directed graphs.

## Complexity
Time complexity is O(V + E) for constructing the dominator tree.

## API Signature
```csharp
public static unsafe class Dominator
{
    public static void Run(int* ptr, int len)
}
```

## Usage Example
```csharp
unsafe
{
    int len = 10;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        IAFahim.Graph.Dominator.Run(ptr, len);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```"""

# 4. IAFahim.Graph.DynamicTrees
readmes["IAFahim.Graph.DynamicTrees"] = """# IAFahim.Graph.DynamicTrees

## Description
This package provides dynamic tree structures, including Top Trees, Link-Cut Trees, and Euler Tour Trees, supporting dynamic path queries and tree updates.

## Complexity
Amortized time complexity is O(log V) per tree update or path query.

## API Signature
```csharp
public static unsafe class LinkCutTree
{
    public static void Init(LctNode* nodes, int n)
    public static void Link(LctNode* nodes, int u, int v)
    public static void Cut(LctNode* nodes, int u, int v)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 100;
    IAFahim.Graph.DynamicTrees.LctNode* nodes = (IAFahim.Graph.DynamicTrees.LctNode*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(IAFahim.Graph.DynamicTrees.LctNode));
    try
    {
        IAFahim.Graph.DynamicTrees.LinkCutTree.Init(nodes, n);
        IAFahim.Graph.DynamicTrees.LinkCutTree.Link(nodes, 1, 2);
        IAFahim.Graph.DynamicTrees.LinkCutTree.Cut(nodes, 1, 2);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
    }
}
```"""

# 5. IAFahim.Graph.Eertree
readmes["IAFahim.Graph.Eertree"] = """# IAFahim.Graph.Eertree

## Description
This package provides the Eertree structure for indexing all distinct palindromic substrings in a sequence.

## Complexity
Time complexity is O(N) for building the palindromic tree, where N is the sequence length.

## API Signature
```csharp
public static unsafe class Node
{
    public static void Build(int* s, int len, Node* nodes, Next* next, ref int nodeCount, ref int nextCount, ref int last, ref int cur)
}
```

## Usage Example
```csharp
unsafe
{
    int len = 5;
    int* s = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    int nodeCount = 0;
    int nextCount = 0;
    int last = 0;
    int cur = 0;
    IAFahim.Graph.Eertree.Node* nodes = (IAFahim.Graph.Eertree.Node*)System.Runtime.InteropServices.Marshal.AllocHGlobal(10 * sizeof(IAFahim.Graph.Eertree.Node));
    IAFahim.Graph.Eertree.Next* next = (IAFahim.Graph.Eertree.Next*)System.Runtime.InteropServices.Marshal.AllocHGlobal(10 * sizeof(IAFahim.Graph.Eertree.Next));
    try
    {
        s[0] = 1;
        s[1] = 2;
        s[2] = 1;
        s[3] = 2;
        s[4] = 1;
        IAFahim.Graph.Eertree.Node.Build(s, len, nodes, next, ref nodeCount, ref nextCount, ref last, ref cur);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
    }
}
```"""

# 6. IAFahim.Graph.Eulerian
readmes["IAFahim.Graph.Eulerian"] = """# IAFahim.Graph.Eulerian

## Description
This package provides algorithms to search for Eulerian paths and Eulerian cycles in a graph.

## Complexity
Time complexity is O(V + E) where V is the node count and E is the edge count.

## API Signature
```csharp
public static unsafe class EulerShared
{
    public static int Run(int n, int* head, int* to, int* next, int start, int* path)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(int));
    int* path = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(int));
    try
    {
        head[0] = -1;
        head[1] = -1;
        head[2] = -1;
        int count = IAFahim.Graph.Eulerian.EulerShared.Run(n, head, to, next, 0, path);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)path);
    }
}
```"""

# 7. IAFahim.Graph.Flow
readmes["IAFahim.Graph.Flow"] = """# IAFahim.Graph.Flow

## Description
This package provides flow network routines, including maximum flow, minimum cut, minimum cost maximum flow, and vertex-limited flows.

## Complexity
Time complexity depends on the chosen method; push-relabel runs in O(V^2 * E) time, Dinic runs in O(V^2 * E) time.

## API Signature
```csharp
public static unsafe class PushRelabelGap
{
    public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 2;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* cap = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* flow = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    try
    {
        head[0] = -1;
        head[1] = -1;
        long total = IAFahim.Graph.Flow.PushRelabelGap.Run(n, 0, 1, head, to, next, cap, flow);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)cap);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)flow);
    }
}
```"""

# 8. IAFahim.Graph.Functional
readmes["IAFahim.Graph.Functional"] = """# IAFahim.Graph.Functional

## Description
This package provides algorithms for functional graphs, where every node has exactly one outgoing edge. It includes path queries, cycle detection, and meeting points.

## Complexity
Path successor query runs in O(log K) time using binary lifting. Cycle detection runs in O(V) time.

## API Signature
```csharp
public static unsafe class PermutationCyclePower
{
    public static void Run(int* p, int n, long k, int* res)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int* p = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* res = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        p[0] = 1;
        p[1] = 2;
        p[2] = 0;
        IAFahim.Graph.Functional.PermutationCyclePower.Run(p, n, 5, res);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)p);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)res);
    }
}
```"""

# 9. IAFahim.Graph.Matching
readmes["IAFahim.Graph.Matching"] = """# IAFahim.Graph.Matching

## Description
This package provides matching routines for graphs, supporting stable marriage, stable roommates, bipartite matching, and Hungarian methods.

## Complexity
Stable marriage runs in O(V^2) time. Hungarian method runs in O(V^3) time.

## API Signature
```csharp
public static unsafe class StableMarriage
{
    public static void Run(int n, int* proposerPref, int* receiverPref, int* proposerMatch, int* receiverMatch, int* scratch)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 2;
    int* proposerPref = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * n * sizeof(int));
    int* receiverPref = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * n * sizeof(int));
    int* proposerMatch = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* receiverMatch = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* scratch = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * n * sizeof(int));
    try
    {
        proposerPref[0] = 0; proposerPref[1] = 1;
        proposerPref[2] = 1; proposerPref[3] = 0;
        receiverPref[0] = 1; receiverPref[1] = 0;
        receiverPref[2] = 0; receiverPref[3] = 1;
        IAFahim.Graph.Matching.StableMarriage.Run(n, proposerPref, receiverPref, proposerMatch, receiverMatch, scratch);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)proposerPref);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)receiverPref);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)proposerMatch);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)receiverMatch);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)scratch);
    }
}
```"""

# 10. IAFahim.Graph.Misc
readmes["IAFahim.Graph.Misc"] = """# IAFahim.Graph.Misc

## Description
This package provides miscellaneous graph utility algorithms, including topological dynamic programming and node access closure checks.

## Complexity
Time complexity for topological dynamic programming is O(V + E) where V is the node count and E is the edge count.

## API Signature
```csharp
public static unsafe class TopologicalDp
{
    public static long Run(int n, int* order, long* dp, int* to, int* next, int* head)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 2;
    int* order = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    long* dp = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        order[0] = 0;
        order[1] = 1;
        head[0] = -1;
        head[1] = -1;
        long total = IAFahim.Graph.Misc.TopologicalDp.Run(n, order, dp, to, next, head);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)order);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)dp);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
    }
}
```"""

# 11. IAFahim.Graph.RandomWalk
readmes["IAFahim.Graph.RandomWalk"] = """# IAFahim.Graph.RandomWalk

## Description
This package provides random walk routines for graph path simulations.

## Complexity
Time complexity depends on the walk step count.

## API Signature
```csharp
public static unsafe class RandomWalk
{
    public static void Run(int* ptr, int len)
}
```

## Usage Example
```csharp
unsafe
{
    int len = 10;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        IAFahim.Graph.RandomWalk.Run(ptr, len);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```"""

# 12. IAFahim.Graph.SCC
readmes["IAFahim.Graph.SCC"] = """# IAFahim.Graph.SCC

## Description
This package provides algorithms for finding strongly connected components in a directed graph, including Tarjan's algorithm and online SCC maintenance.

## Complexity
Tarjan's algorithm runs in O(V + E) time. Online SCC maintains components dynamically.

## API Signature
```csharp
public static unsafe class TarjanScc
{
    public static void Find(int n, int* head, int* next, int* to, int* sccId, int* sccCount)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 2;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* sccId = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int sccCount = 0;
    try
    {
        head[0] = -1;
        head[1] = -1;
        IAFahim.Graph.SCC.TarjanScc.Find(n, head, next, to, sccId, &sccCount);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)sccId);
    }
}
```"""

# 13. IAFahim.Graph.ShortestPath
readmes["IAFahim.Graph.ShortestPath"] = """# IAFahim.Graph.ShortestPath

## Description
This package provides shortest path algorithms, including Eppstein's K-shortest paths and dynamic edge updates.

## Complexity
Eppstein's algorithm runs in O(E + V log V + K log K) time.

## API Signature
```csharp
public static unsafe class KthShortestPathEppstein
{
    public static void Run(int n, int m, int k, int* eu, int* ev, long* ew, int s, long* dists)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int m = 2;
    int k = 1;
    int* eu = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    int* ev = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    long* ew = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(long));
    long* dists = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(k * sizeof(long));
    try
    {
        eu[0] = 0; ev[0] = 1; ew[0] = 5;
        eu[1] = 1; ev[1] = 2; ew[1] = 3;
        IAFahim.Graph.ShortestPath.KthShortestPathEppstein.Run(n, m, k, eu, ev, ew, 0, dists);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)eu);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ev);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ew);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)dists);
    }
}
```"""

# 14. IAFahim.Graph.SpanningTrees
readmes["IAFahim.Graph.SpanningTrees"] = """# IAFahim.Graph.SpanningTrees

## Description
This package provides algorithms for spanning trees and cuts, including transitive closure construction.

## Complexity
Transitive closure construction runs in O(V * E) time.

## API Signature
```csharp
public static unsafe class StShared
{
    public static void BuildTransitiveClosure(int* eu, int* ev, int m, int n, bool* tc)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int m = 2;
    int* eu = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    int* ev = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    bool* tc = (bool*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * n * sizeof(bool));
    try
    {
        eu[0] = 0; ev[0] = 1;
        eu[1] = 1; ev[1] = 2;
        IAFahim.Graph.SpanningTrees.StShared.BuildTransitiveClosure(eu, ev, m, n, tc);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)eu);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ev);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)tc);
    }
}
```"""

# 15. IAFahim.Graph.Tree
readmes["IAFahim.Graph.Tree"] = """# IAFahim.Graph.Tree

## Description
This package provides basic and advanced tree algorithms, including Lowest Common Ancestor queries and Heavy-Light Decomposition.

## Complexity
Lowest Common Ancestor query runs in O(log V) time after O(V log V) preprocessing.

## API Signature
```csharp
public static unsafe class LcaBuild
{
    public static void Run(int n, int root, int* head, int* to, int* next, int* parent, int* depth, int* ancestors, int logN)
    public static int Run(int u, int v, int* depth, int* ancestors, int logN)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int logN = 2;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* parent = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* depth = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* ancestors = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * logN * sizeof(int));
    try
    {
        head[0] = -1;
        head[1] = -1;
        head[2] = -1;
        IAFahim.Graph.Tree.LcaBuild.Run(n, 0, head, to, next, parent, depth, ancestors, logN);
        int lca = IAFahim.Graph.Tree.LcaBuild.Run(1, 2, depth, ancestors, logN);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)parent);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)depth);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ancestors);
    }
}
```"""

# 16. IAFahim.Graph.TreeDecomposition
readmes["IAFahim.Graph.TreeDecomposition"] = """# IAFahim.Graph.TreeDecomposition

## Description
This package provides dynamic programming algorithms on nice tree decompositions, pathwidth decompositions, and tree Mo algorithms.

## Complexity
Independent set query runs in linear time with respect to the tree decomposition size.

## API Signature
```csharp
public static unsafe class PathwidthDpAlgorithm
{
    public static long PathwidthDpIndependentSet(int n, int width, int* bagSize, int* bagVertices, int* parent, int* vertexWeight)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 2;
    int width = 1;
    int* bagSize = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* bagVertices = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * (width + 1) * sizeof(int));
    int* parent = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* vertexWeight = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        bagSize[0] = 1;
        bagSize[1] = 1;
        long total = IAFahim.Graph.TreeDecomposition.PathwidthDpAlgorithm.PathwidthDpIndependentSet(n, width, bagSize, bagVertices, parent, vertexWeight);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)bagSize);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)bagVertices);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)parent);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)vertexWeight);
    }
}
```"""

# 17. IAFahim.Graph.TreeIsomorphism
readmes["IAFahim.Graph.TreeIsomorphism"] = """# IAFahim.Graph.TreeIsomorphism

## Description
This package provides algorithms for tree isomorphism detection, including rooted and unrooted canonical tree hashes.

## Complexity
Tree isomorphism detection runs in O(V) time.

## API Signature
```csharp
public static unsafe class TreeIsomorphismAhU
{
    public static bool Run(int* p1, int* p2, int n)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int* p1 = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* p2 = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        p1[0] = -1; p1[1] = 0; p1[2] = 0;
        p2[0] = -1; p2[1] = 0; p2[2] = 0;
        bool isomorphic = IAFahim.Graph.TreeIsomorphism.TreeIsomorphismAhU.Run(p1, p2, n);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)p1);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)p2);
    }
}
```"""

# 18. IAFahim.Graph.TreeQueries
readmes["IAFahim.Graph.TreeQueries"] = """# IAFahim.Graph.TreeQueries

## Description
This package provides tree query algorithms, including tree centroids, path color counting, Steiner trees, and tree hashing.

## Complexity
Steiner tree runs in O(V * 3^T) time where T is the terminal node count. Tree hashing runs in O(V log V) time.

## API Signature
```csharp
public static unsafe class TreeCentroid
{
    public static void AllCentroids(int n, int* head, int* to, int* next, int* centroids, ref int count)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* centroids = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int count = 0;
    try
    {
        head[0] = -1;
        head[1] = -1;
        head[2] = -1;
        IAFahim.Graph.TreeQueries.TreeCentroid.AllCentroids(n, head, to, next, centroids, ref count);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)centroids);
    }
}
```"""

# 19. IAFahim.Linear.Matrix
readmes["IAFahim.Linear.Matrix"] = """# IAFahim.Linear.Matrix

## Description
This package provides matrix operations, including matrix products, matrix exponentiation, and Berlekamp-Massey recurrence solvers.

## Complexity
Matrix products run in O(N^3) time. Berlekamp-Massey runs in O(N^2) time.

## API Signature
```csharp
public static unsafe class BerlekampMassey
{
    public static int Run(long* s, int n, long* c)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 4;
    long* s = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
    long* c = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
    try
    {
        s[0] = 1;
        s[1] = 2;
        s[2] = 4;
        s[3] = 8;
        int len = IAFahim.Linear.Matrix.BerlekampMassey.Run(s, n, c);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)c);
    }
}
```"""

# Validate all
failed = False
for name, content in readmes.items():
    ok, err = validate_readme(name, content)
    if not ok:
        print(f"Validation failed for {name}: {err}")
        failed = True
    else:
        print(f"Validation OK for {name}")
        
if failed:
    raise ValueError("One or more validation failures.")
    
# Write outputs.json
with open("outputs.json", "w", encoding="utf-8") as f:
    json.dump(readmes, f, indent=2)
print("Wrote outputs.json successfully.")
