# IAFahim.Graph.Decomposition

## Description
This package provides graph decomposition methods to split graphs into sub-components.

## Complexity
Time and space complexity depend on the specific decomposition routine.

## API Signature
```csharp
public static unsafe class Decomposition
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
        IAFahim.Graph.Decomposition.Run(ptr, len);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```