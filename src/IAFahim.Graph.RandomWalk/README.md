# IAFahim.Graph.RandomWalk

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
```