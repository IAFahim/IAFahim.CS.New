# IAFahim.Graph.Dominator

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
```