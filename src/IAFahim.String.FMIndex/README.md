# IAFahim.String.FMIndex

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
