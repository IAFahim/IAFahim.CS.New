import json
import re

def has_cat_subsequence(word):
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

def validate_readme(name, readme_text):
    # Check standalone word "cat" case-insensitive anywhere
    if re.search(r'\bcat\b', readme_text, re.IGNORECASE):
        raise ValueError(f"Package {name} contains forbidden word 'cat'")
    
    # Split explanation (text outside of code blocks)
    # We will strip code blocks to analyze only the explanation text
    parts = readme_text.split("```")
    explanation_text = ""
    for idx, part in enumerate(parts):
        if idx % 2 == 0:
            explanation_text += " " + part
            
    # Tokenize explanation
    words = re.findall(r'\b[a-zA-Z]+\b', explanation_text)
    for w in words:
        if has_cat_subsequence(w):
            raise ValueError(f"Package {name} contains word '{w}' which has 'c', 'a', 't' in sequence.")

# We will define each README content. Let's make sure we write clean explanations and use a try/finally with Marshal.AllocHGlobal, no var, no managed arrays.
readmes = {}

# 1. Automaton
readmes["IAFahim.Search.Automaton"] = """# IAFahim.Search.Automaton

## Description
This package provides algorithms for automaton construction and modulo power operations on matrices. It allows building state transition graphs and exponentiating transition representations.

## Complexity
The matrix power operation runs in O(N^3 log exp) time and uses O(N^2) auxiliary memory. Constructing the state transitions runs in O(alphabetSize * N) time and space.

## API Signature
```csharp
namespace IAFahim.Search.Automaton
{
    public static unsafe class ModMatrixPow
    {
        public static void Run(int n, long* a, long* result, long exp, long mod);
    }

    public static unsafe class BuildAutomaton
    {
        public static int Run(int n, int* transitions, int* failure, int* output, int alphabetSize);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Automaton;

public static unsafe class Program
{
    public static void Main()
    {
        int n = 2;
        long exp = 5;
        long mod = 1000000007;
        long* a = (long*)Marshal.AllocHGlobal(n * n * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal(n * n * sizeof(long));
        try
        {
            a[0] = 1;
            a[1] = 1;
            a[2] = 1;
            a[3] = 0;
            ModMatrixPow.Run(n, a, res, exp, mod);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```"""

# 2. Bit
readmes["IAFahim.Search.Bit"] = """# IAFahim.Search.Bit

## Description
This package provides bitwise operations on arrays of bits, including logical operations, shifting, and search algorithms like longest increasing subsequence lengths.

## Complexity
Bitwise operations run in O(N) time where N is the word count. Binary search runs in O(log N) time. Longest increasing subsequence runs in O(N log N) time.

## API Signature
```csharp
namespace IAFahim.Search.Bit
{
    public static unsafe class BitsetOr
    {
        public static void Run(int n, long* a, long* b, long* res, int wordsPerRow);
    }

    public static unsafe class BitSearch
    {
        public static int Run(int n, int* arr);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Bit;

public static unsafe class Program
{
    public static void Main()
    {
        int n = 64;
        int words = 1;
        long* a = (long*)Marshal.AllocHGlobal(words * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(words * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal(words * sizeof(long));
        try
        {
            a[0] = 1;
            b[0] = 2;
            BitsetOr.Run(n, a, b, res, words);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```"""

# 3. DifferenceArray
readmes["IAFahim.Search.DifferenceArray"] = """# IAFahim.Search.DifferenceArray

## Description
This package provides a difference buffer structure to support range additions and value updates on linear memory buffers.

## Complexity
Applying a range increment runs in O(1) time. Building the original representation runs in O(N) time where N is the buffer length.

## API Signature
```csharp
namespace IAFahim.Search.DifferenceArray
{
    public static unsafe class Diff
    {
        public static void Apply(int* diff, int len, int start, int end, int val);
        public static void Build(int* output, int* diff, int len);
        public static int RangeSum(int* prefix, int idx);
        public static void PrefixFromDiff(int* prefix, int* diff, int len);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.DifferenceArray;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 10;
        int* diff = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* output = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            int i = 0;
            while (i < len)
            {
                diff[i] = 0;
                i = i + 1;
            }
            Diff.Apply(diff, len, 2, 5, 10);
            Diff.Build(output, diff, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)diff);
            Marshal.FreeHGlobal((IntPtr)output);
        }
    }
}
```"""

# 4. ExactCover
readmes["IAFahim.Search.ExactCover"] = """# IAFahim.Search.ExactCover

## Description
This package solves exact cover problems using dancing links and back-tracking, including grid placement games and queen puzzle counts.

## Complexity
Time complexity is exponential in the worst case but highly optimized via dancing links. Space complexity is O(Rows * Cols) for grid representations.

## API Signature
```csharp
namespace IAFahim.Search
{
    public static unsafe class ExactCover
    {
        public static bool SolveDlx(int* matrix, int rows, int cols, int* solution, int* solutionSize, int* L, int* R, int* U, int* D, int* C, int* RowIdx, int* colSize);
        public static bool SolveSudokuDlx(int* sudoku, int* L, int* R_dlx, int* U, int* D, int* C, int* RowIdx, int* colSize);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search;

public static unsafe class Program
{
    public static void Main()
    {
        int size = 81;
        int* sudoku = (int*)Marshal.AllocHGlobal(size * sizeof(int));
        int* temp = (int*)Marshal.AllocHGlobal(400 * sizeof(int));
        try
        {
            int i = 0;
            while (i < size)
            {
                sudoku[i] = 0;
                i = i + 1;
            }
            ExactCover.SolveSudokuDlx(sudoku, temp, temp, temp, temp, temp, temp, temp);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)sudoku);
            Marshal.FreeHGlobal((IntPtr)temp);
        }
    }
}
```"""

# 5. Imos
readmes["IAFahim.Search.Imos"] = """# IAFahim.Search.Imos

## Description
This package implements multi-dimensional prefix sums and range update algorithms on grids and linear buffers, and solves grid bounding rectangle problems.

## Complexity
Range updates run in O(1) time. Grid building runs in O(Width * Height) time.

## API Signature
```csharp
namespace IAFahim.Search.Imos
{
    public static unsafe class Imos1D
    {
        public static void Add(int* diff, int len, int start, int end, int val);
        public static void Build(int* dst, int* diff, int len);
    }

    public static unsafe class Imos2D
    {
        public static void Add(int* diff, int width, int height, int r1, int c1, int r2, int c2, int val);
        public static void Build(int* dst, int* diff, int width, int height);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Imos;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 10;
        int* diff = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* dst = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            int i = 0;
            while (i < len)
            {
                diff[i] = 0;
                i = i + 1;
            }
            Imos1D.Add(diff, len, 2, 7, 5);
            Imos1D.Build(dst, diff, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)diff);
            Marshal.FreeHGlobal((IntPtr)dst);
        }
    }
}
```"""

# 6. Interval
readmes["IAFahim.Search.Interval"] = """# IAFahim.Search.Interval

## Description
This package contains methods to merge, intersect, and normalize sets of intervals, and search for interval overlaps.

## Complexity
Merging and normalization run in O(N log N) time due to sorting, where N is interval count. Space complexity is O(1) auxiliary.

## API Signature
```csharp
namespace IAFahim.Search.Interval
{
    public struct Interval
    {
        public int Start;
        public int End;
    }

    public static unsafe class MergeIntervals
    {
        public static int Run(Interval* ptr, int len);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Interval;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 2;
        Interval* ptr = (Interval*)Marshal.AllocHGlobal(len * sizeof(Interval));
        try
        {
            ptr[0].Start = 1;
            ptr[0].End = 3;
            ptr[1].Start = 2;
            ptr[1].End = 4;
            int count = MergeIntervals.Run(ptr, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```"""

# 7. LIS
readmes["IAFahim.Search.LIS"] = """# IAFahim.Search.LIS

## Description
This package computes the length and elements of the longest increasing subsequence in an array of values.

## Complexity
The algorithm runs in O(N log N) time and uses O(N) space where N is the length of the input.

## API Signature
```csharp
namespace IAFahim.Search.LIS
{
    public static unsafe class Lis
    {
        public static int Run(int* ptr, int len, int* result);
        public static int RunLong(long* ptr, int len, int* result);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.LIS;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* res = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 3;
            ptr[1] = 1;
            ptr[2] = 4;
            ptr[3] = 2;
            ptr[4] = 5;
            int size = Lis.Run(ptr, len, res);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```"""

# 8. MeetInMiddle
readmes["IAFahim.Search.MeetInMiddle"] = """# IAFahim.Search.MeetInMiddle

## Description
This package implements search algorithms using the meet-in-the-middle technique, splitting search sets to solve subset sum problems.

## Complexity
Subset sum search runs in O(2^(N/2) * log(2^(N/2))) time where N is set size. Space complexity is O(2^(N/2)).

## API Signature
```csharp
namespace IAFahim.Search.MeetInMiddle
{
    public static unsafe class MeetInMiddle
    {
        public static int SubsetSumCount(int* values, int len, int target);
        public static bool HasSubsetSum(int* values, int len, int target);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.MeetInMiddle;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 4;
        int target = 9;
        int* values = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            values[0] = 2;
            values[1] = 4;
            values[2] = 5;
            values[3] = 10;
            bool found = MeetInMiddle.HasSubsetSum(values, len, target);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)values);
        }
    }
}
```"""

# 9. Numerical
readmes["IAFahim.Search.Numerical"] = """# IAFahim.Search.Numerical

## Description
This package provides numerical search, optimization, and integration methods, including simulated annealing, ternary real search, and adaptive integration.

## Complexity
Annealing runs for a fixed iteration count. Ternary search converges in O(log((hi - lo)/tol)) operations. Adaptive integration runs dynamically.

## API Signature
```csharp
namespace IAFahim.Search.Numerical
{
    public static unsafe class TernaryReal
    {
        public static double Run(double* func, int maxIter, double lo, double hi);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Numerical;

public static unsafe class Program
{
    public static void Main()
    {
        int maxIter = 100;
        double lo = 0.0;
        double hi = 10.0;
        double* func = (double*)Marshal.AllocHGlobal(sizeof(double));
        try
        {
            double res = TernaryReal.Run(func, maxIter, lo, hi);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)func);
        }
    }
}
```"""

# 10. Prefix
readmes["IAFahim.Search.Prefix"] = """# IAFahim.Search.Prefix

## Description
This package provides prefix sum, prefix min, prefix max, and prefix XOR algorithms, along with string pattern searching.

## Complexity
Prefix operations run in O(N) time and use O(1) auxiliary space. String pattern matching runs in O(N + M) time.

## API Signature
```csharp
namespace IAFahim.Search.Prefix
{
    public static unsafe class PrefixSums
    {
        public static long Run(long* ptr, int len);
        public static int Run(int* ptr, int len);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Prefix;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 1;
            ptr[1] = 2;
            ptr[2] = 3;
            ptr[3] = 4;
            ptr[4] = 5;
            PrefixSums.Run(ptr, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```"""

# 11. Range
readmes["IAFahim.Search.Range"] = """# IAFahim.Search.Range

## Description
This package provides range sum, range minimum, range maximum, and range minimum excluded value query structures like sparse tables.

## Complexity
Sparse table construction runs in O(N log N) time and space. Range queries run in O(1) time.

## API Signature
```csharp
namespace IAFahim.Search.Range
{
    public static unsafe class RangeMin
    {
        public static void BuildSparse(int* dst, int* src, int len);
        public static int Query(int* sparse, int* src, int len, int start, int end);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Range;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 4;
        int* src = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* dst = (int*)Marshal.AllocHGlobal(len * 4 * sizeof(int));
        try
        {
            src[0] = 4;
            src[1] = 1;
            src[2] = 3;
            src[3] = 2;
            RangeMin.BuildSparse(dst, src, len);
            int minVal = RangeMin.Query(dst, src, len, 1, 3);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)src);
            Marshal.FreeHGlobal((IntPtr)dst);
        }
    }
}
```"""

# 12. RangeQueries
readmes["IAFahim.Search.RangeQueries"] = """# IAFahim.Search.RangeQueries

## Description
This package contains advanced range query algorithms, segment trees with lazy propagation, offline queries, and majority query mechanisms.

## Complexity
Segment tree queries and updates run in O(log N) time. Range majority queries run in O(log N) time.

## API Signature
```csharp
namespace IAFahim.Search.RangeQueries
{
    public static unsafe class RangeMajorityQuery
    {
        public static int Run(int* arr, int n, int l, int r);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.RangeQueries;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int* arr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            arr[0] = 2;
            arr[1] = 2;
            arr[2] = 3;
            arr[3] = 2;
            arr[4] = 4;
            int maj = RangeMajorityQuery.Run(arr, len, 0, 4);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)arr);
        }
    }
}
```"""

# 13. Selection
readmes["IAFahim.Search.Selection"] = """# IAFahim.Search.Selection

## Description
This package provides selection algorithms, including quick-select for finding the K-th smallest element and maintaining rolling medians.

## Complexity
Finding the K-th element runs in O(N) average time and O(N^2) worst-case time. Rolling median operations run in O(N log N) time.

## API Signature
```csharp
namespace IAFahim.Search.Selection
{
    public static unsafe class Selection
    {
        public static void SelectTopK(int* ptr, int len, int k);
        public static bool TryGetKth(int* ptr, int len, int k, out int result);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Selection;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int k = 2;
        int result = 0;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 9;
            ptr[1] = 1;
            ptr[2] = 8;
            ptr[3] = 2;
            ptr[4] = 7;
            bool success = Selection.TryGetKth(ptr, len, k, out result);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```"""

# 14. Specialized
readmes["IAFahim.Search.Specialized"] = """# IAFahim.Search.Specialized

## Description
This package implements specialized search algorithms, including binary search bounds, ternary search, scheduling generators, and stress testing utilities.

## Complexity
Lower bound and upper bound binary searches run in O(log N) time. Ternary search runs in O(log N) time.

## API Signature
```csharp
namespace IAFahim.Search.Specialized
{
    public static unsafe class BinarySearch
    {
        public static bool TryFind(int* ptr, int len, int key, out int index);
    }

    public static unsafe class UpperBound
    {
        public static int Run(int* ptr, int len, int key);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Specialized;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int key = 3;
        int idx = 0;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 1;
            ptr[1] = 2;
            ptr[2] = 3;
            ptr[3] = 4;
            ptr[4] = 5;
            bool found = BinarySearch.TryFind(ptr, len, key, out idx);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```"""

# 15. Subset
readmes["IAFahim.Search.Subset"] = """# IAFahim.Search.Subset

## Description
This package provides algorithms to enumerate sub-masks, super-masks, and same pop-count integer masks using bitwise search techniques.

## Complexity
Enumerate operations run in O(2^K) time where K is the number of active bits. Space complexity is O(1) auxiliary.

## API Signature
```csharp
namespace IAFahim.Search.Subset
{
    public static unsafe class EnumerateSubsets
    {
        public static int Count(int superMask);
        public static void Run(int superMask, int* dst);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Subset;

public static unsafe class Program
{
    public static void Main()
    {
        int superMask = 5;
        int size = EnumerateSubsets.Count(superMask);
        int* dst = (int*)Marshal.AllocHGlobal(size * sizeof(int));
        try
        {
            EnumerateSubsets.Run(superMask, dst);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)dst);
        }
    }
}
```"""

# 16. Suffix
readmes["IAFahim.Search.Suffix"] = """# IAFahim.Search.Suffix

## Description
This package provides suffix-based query algorithms, including suffix sums, suffix minimums, and suffix maximums on linear sequences.

## Complexity
Suffix array operations run in O(N) time and use O(1) auxiliary space where N is sequence length.

## API Signature
```csharp
namespace IAFahim.Search.Suffix
{
    public static unsafe class SuffixSums
    {
        public static long Run(long* ptr, int len);
        public static int Run(int* ptr, int len);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Suffix;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 1;
            ptr[1] = 2;
            ptr[2] = 3;
            ptr[3] = 4;
            ptr[4] = 5;
            SuffixSums.Run(ptr, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```"""

# 17. TwoPointer
readmes["IAFahim.Search.TwoPointer"] = """# IAFahim.Search.TwoPointer

## Description
This package provides two-pointer traversal algorithms, including pair-sum detection and merging of sorted sequences.

## Complexity
Merging and pair-sum checks run in O(N + M) time where N and M are the sizes of the input sequences.

## API Signature
```csharp
namespace IAFahim.Search.TwoPointer
{
    public static unsafe class TwoPointers
    {
        public static int CountPairsWithSum(int* a, int aLen, int* b, int bLen, int target);
        public static bool HasPairWithSum(int* a, int aLen, int* b, int bLen, int target);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.TwoPointer;

public static unsafe class Program
{
    public static void Main()
    {
        int aLen = 3;
        int bLen = 3;
        int target = 5;
        int* a = (int*)Marshal.AllocHGlobal(aLen * sizeof(int));
        int* b = (int*)Marshal.AllocHGlobal(bLen * sizeof(int));
        try
        {
            a[0] = 1; a[1] = 2; a[2] = 3;
            b[0] = 1; b[1] = 2; b[2] = 3;
            bool success = TwoPointers.HasPairWithSum(a, aLen, b, bLen, target);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
        }
    }
}
```"""

# 18. Window
readmes["IAFahim.Search.Window"] = """# IAFahim.Search.Window

## Description
This package provides sliding window query algorithms, including minimum and maximum value tracking, and unsafe binary heap operations.

## Complexity
Sliding window queries run in O(N) total time for an array of size N. Binary heap push and pop operations run in O(log K) time.

## API Signature
```csharp
namespace IAFahim.Search.Window
{
    public static unsafe class SlidingWindowMin
    {
        public static void Run(int* src, int* dst, int len, int windowSize);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Window;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int win = 3;
        int* src = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* dst = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            src[0] = 4;
            src[1] = 1;
            src[2] = 3;
            src[3] = 2;
            src[4] = 5;
            SlidingWindowMin.Run(src, dst, len, win);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)src);
            Marshal.FreeHGlobal((IntPtr)dst);
        }
    }
}
```"""

# 19. Insertion
readmes["IAFahim.Sort.Insertion"] = """# IAFahim.Sort.Insertion

## Description
This package provides insertion sorting algorithms for arrays of values using raw memory pointer blocks.

## Complexity
The algorithm sorts values in O(N^2) time in the worst case and O(N) in the best case, and uses O(1) auxiliary memory space.

## API Signature
```csharp
namespace IAFahim.Sort.Insertion
{
    public static unsafe class Insertion
    {
        public static void Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>;
        public static void RunDescending<T>(T* ptr, int len) where T : unmanaged, IComparable<T>;
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Sort.Insertion;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 5;
            ptr[1] = 2;
            ptr[2] = 4;
            ptr[3] = 1;
            ptr[4] = 3;
            Insertion.Run(ptr, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```"""

# Validate all generated READMEs
for name, text in readmes.items():
    validate_readme(name, text)

# Write to outputs.json
output_json_path = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_7/outputs.json"
with open(output_json_path, "w", encoding="utf-8") as f:
    json.dump(readmes, f, indent=2, ensure_ascii=False)

print(f"Validated and generated {len(readmes)} READMEs to {output_json_path}")
