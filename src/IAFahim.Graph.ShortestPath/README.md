# IAFahim.Graph.ShortestPath

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
```