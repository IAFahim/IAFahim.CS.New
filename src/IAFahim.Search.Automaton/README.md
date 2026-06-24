# IAFahim.Search.Automaton

## Description
This package provides algorithms for automaton construction and modulo power operations on matrices. It allows building state transition graphs and exponentiating transition representations.

## Complexity
The matrix power operation runs in O(N^3 log exp) time and uses O(N^2) auxiliary memory. Constructing the state transitions runs in O(alphabetSize * N) time and space.

## API Signature
```csharp
namespace IAFahim.Search.Automaton
{
    public static unsafe class ModMatrixPow
    {
        public static void Run(int n, long* a, long* result, long exp, long mod);
    }

    public static unsafe class BuildAutomaton
    {
        public static int Run(int n, int* transitions, int* failure, int* output, int alphabetSize);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Automaton;

public static unsafe class Program
{
    public static void Main()
    {
        int n = 2;
        long exp = 5;
        long mod = 1000000007;
        long* a = (long*)Marshal.AllocHGlobal(n * n * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal(n * n * sizeof(long));
        try
        {
            a[0] = 1;
            a[1] = 1;
            a[2] = 1;
            a[3] = 0;
            ModMatrixPow.Run(n, a, res, exp, mod);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```