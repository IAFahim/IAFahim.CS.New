import json
import os
import sys

# Import validation logic directly
from validate_readmes import validate_readme

readmes = {}

# 1. IAFahim.Sort.Merge
readmes["IAFahim.Sort.Merge"] = """# IAFahim.Sort.Merge

## Description
Sorts elements in an unmanaged buffer by splitting the range, sorting sub-segments recursively, and combining them using a helper buffer.

## Complexity
Time Complexity is O(N log N).
Space Complexity is O(N) auxiliary space.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class Merge
    {
        public static void Run<T>(T* ptr, int len, T* scratch) where T : unmanaged, System.IComparable<T>;
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 4;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    int* helper = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        ptr[0] = 40;
        ptr[1] = 10;
        ptr[2] = 30;
        ptr[3] = 20;
        IAFahim.Sort.Merge.Run(ptr, length, helper);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)helper);
    }
}
```
"""

# 2. IAFahim.Sort.Partition
readmes["IAFahim.Sort.Partition"] = """# IAFahim.Sort.Partition

## Description
Reorders elements in an unmanaged buffer around a pivot. Elements smaller than or equal to the pivot move to the left, while larger elements move to the right.

## Complexity
Time Complexity is O(N) linear scan.
Space Complexity is O(1) auxiliary space.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class Partition
    {
        public static int Run<T>(T* ptr, int len, T pivot) where T : unmanaged, System.IComparable<T>;
        public static void Hoare<T>(T* ptr, int len, T pivot, out int splitIndex) where T : unmanaged, System.IComparable<T>;
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 5;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        ptr[0] = 5;
        ptr[1] = 2;
        ptr[2] = 9;
        ptr[3] = 1;
        ptr[4] = 6;
        int pivot = 5;
        int index = IAFahim.Sort.Partition.Run(ptr, length, pivot);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```
"""

# 3. IAFahim.Sort.QuickSort
readmes["IAFahim.Sort.QuickSort"] = """# IAFahim.Sort.QuickSort

## Description
Sorts elements in place using partition operations. Includes single pivot and dual pivot variations.

## Complexity
Time Complexity is O(N log N) average, O(N^2) worst case.
Space Complexity is O(log N) stack depth.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class QuickSort
    {
        public static void Run<T>(T* ptr, int len) where T : unmanaged, System.IComparable<T>;
        public static void DualPivot<T>(T* ptr, int len) where T : unmanaged, System.IComparable<T>;
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 4;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        ptr[0] = 4;
        ptr[1] = 1;
        ptr[2] = 3;
        ptr[3] = 2;
        IAFahim.Sort.QuickSort.Run(ptr, length);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```
"""

# 4. IAFahim.Sort.RadixSort
readmes["IAFahim.Sort.RadixSort"] = """# IAFahim.Sort.RadixSort

## Description
Sorts integer keys using digit-by-digit sorting based on their binary representation. Requires a helper buffer.

## Complexity
Time Complexity is O(N) linear time.
Space Complexity is O(N) helper space.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class RadixSort
    {
        public static void Run(int* ptr, int len, int* scratch);
        public static void Run(uint* ptr, int len, uint* scratch);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 3;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    int* helper = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        ptr[0] = 100;
        ptr[1] = 2;
        ptr[2] = 50;
        IAFahim.Sort.RadixSort.Run(ptr, length, helper);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)helper);
    }
}
```
"""

# 5. IAFahim.Sort.Specialized
readmes["IAFahim.Sort.Specialized"] = """# IAFahim.Sort.Specialized

## Description
Offers optimized, specialized sorting operations. This includes sorting key-value pairs simultaneously and highly optimized sorting routines for primitive integers and 64-bit integers.

## Complexity
Time Complexity is O(N log N) for general sorting, O(N) for integer-optimized methods.
Space Complexity is O(N) helper space for pair sorting, O(1) in-place for single array sorting.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class SortPairs
    {
        public static void Run<TKey, TValue>(TKey* keys, TValue* values, int len, TKey* scratchKeys, TValue* scratchValues)
            where TKey : unmanaged, System.IComparable<TKey>
            where TValue : unmanaged;
    }
    public static unsafe class SortInt64s
    {
        public static void Run(long* ptr, int len);
    }
    public static unsafe class SortInts
    {
        public static void Run(int* ptr, int len);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 3;
    int* keys = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    float* values = (float*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(float));
    int* scratchKeys = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    float* scratchValues = (float*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(float));
    try
    {
        keys[0] = 3; keys[1] = 1; keys[2] = 2;
        values[0] = 3.0f; values[1] = 1.0f; values[2] = 2.0f;
        IAFahim.Sort.SortPairs.Run(keys, values, length, scratchKeys, scratchValues);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)keys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)values);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)scratchKeys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)scratchValues);
    }
}
```
"""

# 6. IAFahim.String
readmes["IAFahim.String"] = """# IAFahim.String

## Description
Contains core and advanced string processing routines. Includes Lyndon decomposition, run-length encoding and decoding, period finding, De Bruijn sequence generation, expression parsing, NFA-based regex matching, XML and JSON tree hashing, and subsequence or substring enumeration.

## Complexity
Time Complexity is O(N) for linear string scans, expression parsing and regex matching vary depending on size.
Space Complexity is O(1) auxiliary space for in-place algorithms, or O(N) for DP tables in shortest subsequence search.

## API Signature
```csharp
namespace IAFahim.String
{
    public static unsafe class ManacherOdd
    {
        public static void Run(byte* s, int len, int* radii);
    }
    public static unsafe class DuvalLyndon
    {
        public static int Run(byte* s, int len, int* starts, int* lengths);
    }
    public static unsafe class RunLengthEncode
    {
        public static int Run(byte* s, int len, byte* values, int* counts);
    }
    public static unsafe class CountOccurrences
    {
        public static int Run(byte* text, int textLen, byte* pattern, int patLen);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 5;
    byte* s = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(byte));
    int* radii = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        s[0] = (byte)'a';
        s[1] = (byte)'b';
        s[2] = (byte)'a';
        s[3] = (byte)'b';
        s[4] = (byte)'a';
        IAFahim.String.ManacherOdd.Run(s, length, radii);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)radii);
    }
}
```
"""

# 7. IAFahim.String.Automata
readmes["IAFahim.String.Automata"] = """# IAFahim.String.Automata

## Description
Implements finite automata algorithms. Includes DFA minimization, DFA operations like union and intersection, NFA to DFA conversion, and subsequence automata construction for quick subsequence queries.

## Complexity
Time Complexity is O(N * Sigma) for automaton building, O(M) for matching a pattern of size M.
Space Complexity is O(S * Sigma) to store transitions.

## API Signature
```csharp
namespace IAFahim.String.Automata
{
    public static unsafe class SubsequenceAutomaton
    {
        public static void Build(byte* text, int len, int* next, int sigma);
        public static bool Contains(int* next, byte* pattern, int patLen, int sigma);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 3;
    int sigma = 26;
    byte* text = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal((len + 1) * sigma * sizeof(int));
    try
    {
        text[0] = (byte)'a';
        text[1] = (byte)'b';
        text[2] = (byte)'c';
        IAFahim.String.Automata.SubsequenceAutomaton.Build(text, len, next, sigma);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)text);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
    }
}
```
"""

# 8. IAFahim.String.Compress
readmes["IAFahim.String.Compress"] = """# IAFahim.String.Compress

## Description
Implements diverse data compression algorithms. Contains implementations of Huffman coding, Lempel-Ziv variants, arithmetic coding, and Move-To-Front transforms.

## Complexity
Time Complexity is O(N log Sigma) for Huffman encoding, O(N) for MTF, LZ77, and LZ78 algorithms.
Space Complexity is O(Sigma) for Huffman tree and MTF symbols, O(N) for Lempel-Ziv tokens.

## API Signature
```csharp
namespace IAFahim.String.Compress
{
    public static unsafe class Lz78
    {
        public struct Token
        {
            public int Phrase;
            public byte Literal;
        }
        public static int Encode(byte* input, int len, Token* output);
        public static int Decode(Token* input, int count, byte* output);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* input = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    IAFahim.String.Compress.Lz78.Token* output = (IAFahim.String.Compress.Lz78.Token*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(IAFahim.String.Compress.Lz78.Token));
    try
    {
        input[0] = (byte)'a';
        input[1] = (byte)'b';
        input[2] = (byte)'a';
        input[3] = (byte)'b';
        int tokenCount = IAFahim.String.Compress.Lz78.Encode(input, len, output);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)input);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)output);
    }
}
```
"""

# 9. IAFahim.String.FMIndex
readmes["IAFahim.String.FMIndex"] = """# IAFahim.String.FMIndex

## Description
Implements the Burrows-Wheeler Transform and the FM-Index data structure. This enables efficient substring queries and finding occurrences of a pattern within a compressed text using wavelets or occurrence tables.

## Complexity
Time Complexity is O(N) to build, and O(M) to search for a pattern of length M.
Space Complexity is O(N * Sigma) or O(N) depending on occurrence table density.

## API Signature
```csharp
namespace IAFahim.String.FMIndex
{
    public static unsafe class FMIndex
    {
        public static void Build(int* text, int len, int sigma, int* occ);
        public static int Count(int* text, int len, int* pattern, int patLen, int* sa);
        public static void Locate(int* text, int len, int* occ, int* pattern, int patLen, int* sa, int* result, int* count);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    int sigma = 256;
    int* text = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    int* occ = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal((len + 1) * sigma * sizeof(int));
    try
    {
        text[0] = 97;
        text[1] = 98;
        text[2] = 97;
        text[3] = 0;
        IAFahim.String.FMIndex.FMIndex.Build(text, len, sigma, occ);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)text);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)occ);
    }
}
```
"""

# 10. IAFahim.String.Grammar
readmes["IAFahim.String.Grammar"] = """# IAFahim.String.Grammar

## Description
Implements grammar-based string compression and Straight-Line Programs. Represents a string as a context-free grammar to shrink size and query individual symbols in logarithmic time.

## Complexity
Time Complexity is O(N log N) to construct the grammar representation, O(log N) to query a specific symbol position.
Space Complexity is O(G) where G is the grammar size.

## API Signature
```csharp
namespace IAFahim.String.Grammar
{
    public static unsafe class StraightLineProgram
    {
        public struct Rule
        {
            public int Left;
            public int Right;
            public int Len;
            public byte Char;
            public bool IsTerminal;
        }
        public static int Build(byte* s, int len, int maxRules, Rule* rules, ref int ruleCount);
        public static byte Query(Rule* rules, int ruleId, int pos);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* s = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int maxRules = 10;
    IAFahim.String.Grammar.StraightLineProgram.Rule* rules = (IAFahim.String.Grammar.StraightLineProgram.Rule*)System.Runtime.InteropServices.Marshal.AllocHGlobal(maxRules * sizeof(IAFahim.String.Grammar.StraightLineProgram.Rule));
    try
    {
        s[0] = (byte)'a';
        s[1] = (byte)'b';
        s[2] = (byte)'a';
        s[3] = (byte)'b';
        int ruleCount = 0;
        IAFahim.String.Grammar.StraightLineProgram.Build(s, len, maxRules, rules, ref ruleCount);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)rules);
    }
}
```
"""

# 11. IAFahim.String.Match
readmes["IAFahim.String.Match"] = """# IAFahim.String.Match

## Description
Implements string matching algorithms. Includes exact matching, rolling hash search, approximate matching, Lyndon runs search, and parameterized matching.

## Complexity
Time Complexity is O(N + M) for linear matching, O(N * K) or O(N + K^2) for approximate matching.
Space Complexity is O(M) for pattern preprocessing arrays.

## API Signature
```csharp
namespace IAFahim.String.Match
{
    public static unsafe class ZAlgorithm
    {
        public static void Run(byte* ptr, int len, int* zPtr);
        public static void Run(int* ptr, int len, int* zPtr);
    }
    public static unsafe class PrefixFunction
    {
        public static void Run(byte* ptr, int len, int* piPtr);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* ptr = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int* zPtr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        ptr[0] = (byte)'a';
        ptr[1] = (byte)'b';
        ptr[2] = (byte)'a';
        ptr[3] = (byte)'b';
        IAFahim.String.Match.ZAlgorithm.Run(ptr, len, zPtr);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)zPtr);
    }
}
```
"""

# 12. IAFahim.String.MinRotation
readmes["IAFahim.String.MinRotation"] = """# IAFahim.String.MinRotation

## Description
Finds the starting index of the lexicographically smallest cyclic shift of a string or integer sequence using Booth's algorithm.

## Complexity
Time Complexity is O(N) linear time.
Space Complexity is O(N) space for failure function.

## API Signature
```csharp
namespace IAFahim.String.MinRotation
{
    public static unsafe class Booth
    {
        public static int Run(byte* s, int len);
        public static int Run(int* s, int len);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* s = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    try
    {
        s[0] = (byte)'b';
        s[1] = (byte)'a';
        s[2] = (byte)'b';
        s[3] = (byte)'a';
        int index = IAFahim.String.MinRotation.Booth.Run(s, len);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
    }
}
```
"""

# 13. IAFahim.String.Palindrome
readmes["IAFahim.String.Palindrome"] = """# IAFahim.String.Palindrome

## Description
Palindromic string analysis package. Includes palindromic trees for tracking distinct palindromic substrings, Manacher's algorithm for finding palindromic radii, Lyndon decomposition of strings, and occurrence counting.

## Complexity
Time Complexity is O(N) linear time for building palindromic trees, Manacher's search, and Lyndon runs.
Space Complexity is O(N) space to store nodes or radii arrays.

## API Signature
```csharp
namespace IAFahim.String.Palindrome
{
    public static unsafe class Manacher
    {
        public static void Odd(byte* s, int n, int* d);
        public static void Even(byte* s, int n, int* d);
    }
    public static unsafe class OccurrenceCount
    {
        public static long Count(byte* s, int n);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 5;
    byte* s = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int* d = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        s[0] = (byte)'a';
        s[1] = (byte)'b';
        s[2] = (byte)'a';
        s[3] = (byte)'b';
        s[4] = (byte)'a';
        IAFahim.String.Palindrome.Manacher.Odd(s, len, d);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)d);
    }
}
```
"""

# 14. IAFahim.String.Parse
readmes["IAFahim.String.Parse"] = """# IAFahim.String.Parse

## Description
Implements string parsing and recognition algorithms. Includes LL parsing, LR parsing, Earley parsing, the CYK parsing algorithm for context-free grammars, and suffix oracle construction for pattern queries.

## Complexity
Time Complexity is O(N) for LL and LR parsing, O(N^3) for general Earley and CYK parsing. Suffix oracle query is O(M) for pattern length M.
Space Complexity is O(N) for parsing tables and stacks.

## API Signature
```csharp
namespace IAFahim.String.Parse
{
    public static unsafe class SuffixOracle
    {
        public static void Build(byte* text, int len, int sigma);
        public static bool Contains(byte* pattern, int patLen);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 3;
    int sigma = 256;
    byte* text = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    try
    {
        text[0] = (byte)'a';
        text[1] = (byte)'b';
        text[2] = (byte)'a';
        IAFahim.String.Parse.SuffixOracle.Build(text, len, sigma);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)text);
    }
}
```
"""

# 15. IAFahim.String.Pattern
readmes["IAFahim.String.Pattern"] = """# IAFahim.String.Pattern

## Description
Implements a persistent version of the Aho-Corasick multiple pattern matching algorithm. Allows building and querying string matchers incrementally across different versions.

## Complexity
Time Complexity is O(M * Sigma) for insertion, and O(N) for querying text of size N.
Space Complexity is O(V * Sigma) where V is the total number of states across all versions.

## API Signature
```csharp
namespace IAFahim.String.Pattern
{
    public static unsafe class AhoPersistentQuery
    {
        public static long Run(byte* text, int len, int* roots, int activeMask, int* nexts, int* counts, int sigma = 26, byte baseChar = (byte)'a');
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 3;
    byte* text = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int* roots = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* nexts = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(100 * sizeof(int));
    int* counts = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(100 * sizeof(int));
    try
    {
        text[0] = (byte)'a';
        text[1] = (byte)'b';
        text[2] = (byte)'a';
        roots[0] = 0;
        roots[1] = 0;
        long occurrences = IAFahim.String.Pattern.AhoPersistentQuery.Run(text, len, roots, 1, nexts, counts, 26, (byte)'a');
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)text);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)roots);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nexts);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)counts);
    }
}
```
"""

# 16. IAFahim.String.SuffixArray
readmes["IAFahim.String.SuffixArray"] = """# IAFahim.String.SuffixArray

## Description
Suffix array library for string search and query. Contains static suffix array building, LCP interval tree construction, suffix matching, and dynamic suffix arrays using balanced search trees.

## Complexity
Time Complexity is O(N log^2 N) or O(N log N) to construct the suffix array, O(M log N) to search for pattern of length M. Dynamic operations run in O(log^2 N) time.
Space Complexity is O(N) space.

## API Signature
```csharp
namespace IAFahim.String.SuffixArray
{
    public static unsafe class SuffixArray
    {
        public static void Build(byte* ptr, int len, int* sa, int* rank, int* tmpSa, int* count, int* tmpRank);
    }
    public static unsafe class Locate
    {
        public static int Find(int* sa, int saLen, byte* text, int textLen, byte* pattern, int patLen);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* ptr = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int* sa = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    int* rank = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    int* tmpSa = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    int* count = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(256 * sizeof(int));
    int* tmpRank = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        ptr[0] = (byte)'b';
        ptr[1] = (byte)'a';
        ptr[2] = (byte)'b';
        ptr[3] = (byte)'a';
        IAFahim.String.SuffixArray.SuffixArray.Build(ptr, len, sa, rank, tmpSa, count, tmpRank);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)sa);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)rank);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)tmpSa);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)count);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)tmpRank);
    }
}
```
"""

# 17. IAFahim.String.SuffixAutomaton
readmes["IAFahim.String.SuffixAutomaton"] = """# IAFahim.String.SuffixAutomaton

## Description
Suffix Automaton implementation. Supports generalized suffix automata for multiple strings, persistent versions, kth substring queries, and transition tree traversal.

## Complexity
Time Complexity is O(N * Sigma) to build the automaton, O(M) to traverse a pattern of size M.
Space Complexity is O(N * Sigma) state transition space.

## API Signature
```csharp
namespace IAFahim.String.SuffixAutomaton
{
    public static unsafe class SuffixAutomaton
    {
        public struct State
        {
            public int Link;
            public int Len;
            public int Head;
        }
        public struct Edge
        {
            public int To;
            public int Char;
            public int Next;
        }
        public static void Build(int* ptr, int len, State* st, Edge* e, ref int size, ref int last, ref int edgeCount);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 3;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    IAFahim.String.SuffixAutomaton.SuffixAutomaton.State* st = (IAFahim.String.SuffixAutomaton.SuffixAutomaton.State*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * 2 * sizeof(IAFahim.String.SuffixAutomaton.SuffixAutomaton.State));
    IAFahim.String.SuffixAutomaton.SuffixAutomaton.Edge* e = (IAFahim.String.SuffixAutomaton.SuffixAutomaton.Edge*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * 4 * sizeof(IAFahim.String.SuffixAutomaton.SuffixAutomaton.Edge));
    try
    {
        ptr[0] = 0;
        ptr[1] = 1;
        ptr[2] = 0;
        int size = 0;
        int last = 0;
        int edgeCount = 0;
        IAFahim.String.SuffixAutomaton.SuffixAutomaton.Build(ptr, len, st, e, ref size, ref last, ref edgeCount);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)st);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)e);
    }
}
```
"""

# 18. IAFahim.String.SuffixTree
readmes["IAFahim.String.SuffixTree"] = """# IAFahim.String.SuffixTree

## Description
Constructs suffix trees using Ukkonen's linear time algorithm. Allows efficient substring indexing and pattern search in text.

## Complexity
Time Complexity is O(N * Sigma) or O(N) to build, and O(M) to search for a pattern of length M.
Space Complexity is O(N * Sigma) to store transitions and tree nodes.

## API Signature
```csharp
namespace IAFahim.String.SuffixTree
{
    public static unsafe class SuffixTreeUkkonen
    {
        public struct Node { public int Link; public int Start; public int Len; public int FirstEdge; }
        public struct Edge { public int To; public int Char; public int Next; public int Min; public int Max; }
        public static void Build(int* s, int len, Node* nodes, Edge* edges, ref int nodeCount, ref int edgeCount, ref int last);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 3;
    int* s = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    IAFahim.String.SuffixTree.SuffixTreeUkkonen.Node* nodes = (IAFahim.String.SuffixTree.SuffixTreeUkkonen.Node*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * 2 * sizeof(IAFahim.String.SuffixTree.SuffixTreeUkkonen.Node));
    IAFahim.String.SuffixTree.SuffixTreeUkkonen.Edge* edges = (IAFahim.String.SuffixTree.SuffixTreeUkkonen.Edge*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * 4 * sizeof(IAFahim.String.SuffixTree.SuffixTreeUkkonen.Edge));
    try
    {
        s[0] = 97;
        s[1] = 98;
        s[2] = 0;
        int nodeCount = 0;
        int edgeCount = 0;
        int last = 0;
        IAFahim.String.SuffixTree.SuffixTreeUkkonen.Build(s, len, nodes, edges, ref nodeCount, ref edgeCount, ref last);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)edges);
    }
}
```
"""

# 19. IAFahim.Unique
readmes["IAFahim.Unique"] = """# IAFahim.Unique

## Description
Filters out redundant values from a buffer of 64-bit or 32-bit integers in place. Returns the size of the filtered prefix.

## Complexity
Time Complexity is O(N log N) to sort and filter, or O(N) if already sorted.
Space Complexity is O(1) in-place auxiliary space.

## API Signature
```csharp
namespace IAFahim.Unique
{
    public static unsafe class UniqueInts
    {
        public static int Run(int* ptr, int len);
    }
    public static unsafe class UniqueInt64s
    {
        public static int Run(long* ptr, int len);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 5;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        ptr[0] = 10;
        ptr[1] = 20;
        ptr[2] = 10;
        ptr[3] = 30;
        ptr[4] = 20;
        int uniqueCount = IAFahim.Unique.UniqueInts.Run(ptr, len);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```
"""

def main():
    failed = False
    for pkg, content in readmes.items():
        ok, msg = validate_readme(content)
        if not ok:
            print(f"Validation failed for {pkg}: {msg}")
            failed = True
        else:
            print(f"Validation passed for {pkg}")
            
    if failed:
        sys.exit(1)
        
    # Write to outputs.json
    output_path = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_8/outputs.json"
    with open(output_path, "w", encoding="utf-8") as f:
        json.dump(readmes, f, indent=2)
    print(f"All validation passed. Wrote to {output_path}")

if __name__ == "__main__":
    main()
