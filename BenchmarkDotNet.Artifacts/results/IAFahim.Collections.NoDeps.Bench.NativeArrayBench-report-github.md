```

BenchmarkDotNet v0.14.0, Ubuntu 26.04 LTS (Resolute Raccoon)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.107
  [Host]   : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                | N    | Mean        | Error      | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------- |----- |------------:|-----------:|---------:|------:|--------:|----------:|------------:|
| **DirectPointer**         | **64**   |    **33.24 ns** |   **2.130 ns** | **0.117 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| NativeArray_ReadWrite | 64   |    89.10 ns |   6.987 ns | 0.383 ns |  2.68 |    0.01 |         - |          NA |
|                       |      |             |            |          |       |         |           |             |
| **DirectPointer**         | **1024** |   **339.72 ns** | **131.138 ns** | **7.188 ns** |  **1.00** |    **0.03** |         **-** |          **NA** |
| NativeArray_ReadWrite | 1024 | 1,133.51 ns |  18.007 ns | 0.987 ns |  3.34 |    0.06 |         - |          NA |
