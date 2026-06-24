# IAFahim.Math.Arithmetic

## Description
Provides checked arithmetic operations for 32-bit and 64-bit signed integers. These functions return a boolean value showing if the operation succeeded without overflow, and output the result.

## Complexity
- All operations: O(1) time, O(1) space.

## API Signature
- public static bool TryAdd.Run(int a, int b, out int res)
- public static bool TryAdd.Run(long a, long b, out long res)
- public static bool TrySub.Run(int a, int b, out int res)
- public static bool TrySub.Run(long a, long b, out long res)
- public static bool TryMul.Run(int a, int b, out int res)
- public static bool TryMul.Run(long a, long b, out long res)
- public static bool TryDiv.Run(int a, int b, out int res)
- public static bool TryDiv.Run(long a, long b, out long res)

## Usage Example
```csharp
using System;
using IAFahim.Math.Arithmetic;

public unsafe class Example
{
    public static void Main()
    {
        int a = 100;
        int b = 200;
        int res = 0;
        bool success = TryAdd.Run(a, b, out res);
    }
}
```