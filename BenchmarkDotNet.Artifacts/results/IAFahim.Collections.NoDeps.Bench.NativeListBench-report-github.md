```

BenchmarkDotNet v0.14.0, Ubuntu 26.04 LTS (Resolute Raccoon)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.107
  [Host]   : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                         | N    | Mean         | Error        | StdDev     | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------------- |----- |-------------:|-------------:|-----------:|------:|--------:|----------:|------------:|
| **NativeList_Add**                 | **64**   |     **82.38 ns** |     **1.124 ns** |   **0.062 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| NativeList_AddThenRemoveAt     | 64   |    284.17 ns |     4.183 ns |   0.229 ns |  3.45 |    0.00 |         - |          NA |
| NativeList_ResizeUninitialized | 64   |     30.64 ns |     1.627 ns |   0.089 ns |  0.37 |    0.00 |         - |          NA |
|                                |      |              |              |            |       |         |           |             |
| **NativeList_Add**                 | **1024** |    **977.44 ns** |    **93.606 ns** |   **5.131 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| NativeList_AddThenRemoveAt     | 1024 | 12,482.77 ns | 2,096.932 ns | 114.940 ns | 12.77 |    0.12 |         - |          NA |
| NativeList_ResizeUninitialized | 1024 |    377.35 ns |   206.840 ns |  11.338 ns |  0.39 |    0.01 |         - |          NA |
