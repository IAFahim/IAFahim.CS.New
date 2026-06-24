# IAFahim.Graph.Bridges

## Description
This package provides methods for identifying bridges and cut vertices in graphs. It supports static search, incremental dynamic bridge maintenance, and biconnectivity augmentation solving.

## Complexity
Static bridge search runs in O(V + E) time complexity. Dynamic bridge updates run in O(log V) amortized time.

## API Signature
public static class BridgeAndArticulation
{
    public static void Find(int n, int* head, int* next, int* to, bool* isBridge, bool* isCutVertex);
}

## Usage Example
```csharp
unsafe
{
    int n = 4;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    bool* isBridge = (bool*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(bool));
    bool* isCutVertex = (bool*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(bool));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            isBridge[i] = false;
            isCutVertex[i] = false;
        }
        IAFahim.Graph.Bridges.BridgeAndArticulation.Find(n, head, next, to, isBridge, isCutVertex);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)isBridge);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)isCutVertex);
    }
}
```
