# IAFahim.DS.Dsu

## Description
This package provides a Disjoint Set Union (DSU) implementation. It supports path compression, union by size, rollback operations, bipartite graph checks with parity, and small-to-large merging.

## Complexity
- Find with path compression: O(alpha(N)) amortized.
- Union: O(alpha(N)) amortized.
- Rollback Union: O(log N) per operation.

## API Signature
```csharp
public static unsafe class DsuInit
{
    public static void Run(int* parent, int* size, int n);
}

public static unsafe class DsuFind
{
    public static int Run(int* parent, int x);
    public static int RunPathCompression(int* parent, int x);
}

public static unsafe class DsuUnion
{
    public static bool Run(int* parent, int* size, int a, int b);
}

public static unsafe class DsuRollback
{
    public static void Run(int* parent, int* size, int* history, int targetHistSize, int* currentHistSize);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.Dsu;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 5;
        int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        int* size = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            DsuInit.Run(parent, size, n);
            DsuUnion.Run(parent, size, 0, 1);
            int root0 = DsuFind.Run(parent, 0);
            int root1 = DsuFind.Run(parent, 1);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)parent);
            Marshal.FreeHGlobal((nint)size);
        }
    }
}
```