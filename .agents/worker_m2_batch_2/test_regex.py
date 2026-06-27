import re

markdown = """# IAFahim.DS.Mo

## Description
Mo algorithm for offline query processing. It sorts queries using block decomposition to minimize pointer movement.

## Complexity
O((N + Q) * sqrt(N)) time where N is array size and Q is query count.

## API Signature
```csharp
public static unsafe class MoSort
{
    public static void Run(int* queries, int* l, int* r, int* block, int q, int blockSize)
}
public static unsafe class MoDistinctCounter
{
    public static void AddInt(int* freq, int* curDistinct, int val)
    public static void RemoveInt(int* freq, int* curDistinct, int val)
}
public static unsafe class MoWithUpdates
{
    public static void Run(int n, int* arr, int qCount, Query3D* queries, int uCount, Update* updates, int* ans, int blockSize, int* freq)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int q = 2;
        int size = 10;
        int* queries = (int*)Marshal.AllocHGlobal(q * sizeof(int));
        int* l = (int*)Marshal.AllocHGlobal(q * sizeof(int));
        int* r = (int*)Marshal.AllocHGlobal(q * sizeof(int));
        int* block = (int*)Marshal.AllocHGlobal(q * sizeof(int));
        try
        {
            l[0] = 0; r[0] = 4;
            l[1] = 2; r[1] = 8;
            queries[0] = 0;
            queries[1] = 1;
            block[0] = 0;
            block[1] = 0;
            MoSort.Run(queries, l, r, block, q, 3);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)queries);
            Marshal.FreeHGlobal((IntPtr)l);
            Marshal.FreeHGlobal((IntPtr)r);
            Marshal.FreeHGlobal((IntPtr)block);
        }
    }
}
```"""

code_blocks = re.findall(r'```csharp(.*?)```', markdown, re.DOTALL)
print(f"Num blocks: {len(code_blocks)}")
for i, b in enumerate(code_blocks):
    print(f"Block {i}:")
    print(repr(b))
    print(f"AllocHGlobal in block: {'AllocHGlobal' in b}")
    print(f"FreeHGlobal in block: {'FreeHGlobal' in b}")
