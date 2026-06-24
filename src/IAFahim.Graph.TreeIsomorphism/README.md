# IAFahim.Graph.TreeIsomorphism

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
```