# IAFahim.Graph.Matching

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
```