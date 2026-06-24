# IAFahim.Search.MeetInMiddle

## Description
This package implements search algorithms using the meet-in-the-middle technique, splitting search sets to solve subset sum problems.

## Complexity
Subset sum search runs in O(2^(N/2) * log(2^(N/2))) time where N is set size. Space complexity is O(2^(N/2)).

## API Signature
```csharp
namespace IAFahim.Search.MeetInMiddle
{
    public static unsafe class MeetInMiddle
    {
        public static int SubsetSumCount(int* values, int len, int target);
        public static bool HasSubsetSum(int* values, int len, int target);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.MeetInMiddle;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 4;
        int target = 9;
        int* values = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            values[0] = 2;
            values[1] = 4;
            values[2] = 5;
            values[3] = 10;
            bool found = MeetInMiddle.HasSubsetSum(values, len, target);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)values);
        }
    }
}
```