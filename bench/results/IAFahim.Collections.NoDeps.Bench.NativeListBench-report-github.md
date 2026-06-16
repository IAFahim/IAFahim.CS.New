```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry      : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

LaunchCount=1  

```
| Method                         | Job      | IterationCount | RunStrategy | UnrollFactor | WarmupCount | N    | Mean            | Error      | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------------- |--------- |--------------- |------------ |------------- |------------ |----- |----------------:|-----------:|---------:|------:|--------:|----------:|------------:|
| **NativeList_Add**                 | **Dry**      | **1**              | **ColdStart**   | **1**            | **1**           | **64**   | **1,251,404.00 ns** |         **NA** | **0.000 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| NativeList_AddThenRemoveAt     | Dry      | 1              | ColdStart   | 1            | 1           | 64   | 1,362,781.00 ns |         NA | 0.000 ns |  1.09 |    0.00 |         - |          NA |
| NativeList_ResizeUninitialized | Dry      | 1              | ColdStart   | 1            | 1           | 64   | 1,410,383.00 ns |         NA | 0.000 ns |  1.13 |    0.00 |         - |          NA |
|                                |          |                |             |              |             |      |                 |            |          |       |         |           |             |
| NativeList_Add                 | ShortRun | 3              | Default     | 16           | 3           | 64   |       273.02 ns |   4.067 ns | 0.223 ns |  1.00 |    0.00 |         - |          NA |
| NativeList_AddThenRemoveAt     | ShortRun | 3              | Default     | 16           | 3           | 64   |       788.38 ns | 148.076 ns | 8.117 ns |  2.89 |    0.03 |         - |          NA |
| NativeList_ResizeUninitialized | ShortRun | 3              | Default     | 16           | 3           | 64   |        93.13 ns |   1.059 ns | 0.058 ns |  0.34 |    0.00 |         - |          NA |
|                                |          |                |             |              |             |      |                 |            |          |       |         |           |             |
| **NativeList_Add**                 | **Dry**      | **1**              | **ColdStart**   | **1**            | **1**           | **1024** | **1,247,627.00 ns** |         **NA** | **0.000 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| NativeList_AddThenRemoveAt     | Dry      | 1              | ColdStart   | 1            | 1           | 1024 | 1,477,853.00 ns |         NA | 0.000 ns |  1.18 |    0.00 |         - |          NA |
| NativeList_ResizeUninitialized | Dry      | 1              | ColdStart   | 1            | 1           | 1024 | 1,422,590.00 ns |         NA | 0.000 ns |  1.14 |    0.00 |         - |          NA |
|                                |          |                |             |              |             |      |                 |            |          |       |         |           |             |
| NativeList_Add                 | ShortRun | 3              | Default     | 16           | 3           | 1024 |     3,820.53 ns | 137.531 ns | 7.539 ns |  1.00 |    0.00 |         - |          NA |
| NativeList_AddThenRemoveAt     | ShortRun | 3              | Default     | 16           | 3           | 1024 |    32,143.30 ns | 174.942 ns | 9.589 ns |  8.41 |    0.01 |         - |          NA |
| NativeList_ResizeUninitialized | ShortRun | 3              | Default     | 16           | 3           | 1024 |       795.93 ns |  31.569 ns | 1.730 ns |  0.21 |    0.00 |         - |          NA |
