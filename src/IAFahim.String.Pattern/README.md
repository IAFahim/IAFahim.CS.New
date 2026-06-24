# IAFahim.String.Pattern

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
