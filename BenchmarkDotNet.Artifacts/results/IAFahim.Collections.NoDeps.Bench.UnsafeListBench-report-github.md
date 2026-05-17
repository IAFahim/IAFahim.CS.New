```

BenchmarkDotNet v0.14.0, Ubuntu 26.04 LTS (Resolute Raccoon)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.107
  [Host]   : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                         | N    | Mean      | Error     | StdDev   | Ratio | Allocated | Alloc Ratio |
|------------------------------- |----- |----------:|----------:|---------:|------:|----------:|------------:|
| **UnsafeList_Add**                 | **64**   |  **83.28 ns** |  **0.240 ns** | **0.013 ns** |  **1.00** |         **-** |          **NA** |
| UnsafeList_AddRange            | 64   |  42.02 ns |  1.526 ns | 0.084 ns |  0.50 |         - |          NA |
| UnsafeList_ResizeUninitialized | 64   |  30.90 ns |  0.230 ns | 0.013 ns |  0.37 |         - |          NA |
|                                |      |           |           |          |       |           |             |
| **UnsafeList_Add**                 | **1024** | **969.13 ns** | **98.209 ns** | **5.383 ns** |  **1.00** |         **-** |          **NA** |
| UnsafeList_AddRange            | 1024 | 460.00 ns | 75.475 ns | 4.137 ns |  0.47 |         - |          NA |
| UnsafeList_ResizeUninitialized | 1024 | 365.50 ns |  3.321 ns | 0.182 ns |  0.38 |         - |          NA |
