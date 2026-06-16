```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry      : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

LaunchCount=1  

```
| Method                         | Job      | IterationCount | RunStrategy | UnrollFactor | WarmupCount | N    | Mean            | Error      | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------------- |--------- |--------------- |------------ |------------- |------------ |----- |----------------:|-----------:|----------:|------:|--------:|----------:|------------:|
| **UnsafeList_Add**                 | **Dry**      | **1**              | **ColdStart**   | **1**            | **1**           | **64**   | **1,159,585.00 ns** |         **NA** |  **0.000 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| UnsafeList_AddRange            | Dry      | 1              | ColdStart   | 1            | 1           | 64   |   998,934.00 ns |         NA |  0.000 ns |  0.86 |    0.00 |         - |          NA |
| UnsafeList_ResizeUninitialized | Dry      | 1              | ColdStart   | 1            | 1           | 64   | 1,272,144.00 ns |         NA |  0.000 ns |  1.10 |    0.00 |         - |          NA |
|                                |          |                |             |              |             |      |                 |            |           |       |         |           |             |
| UnsafeList_Add                 | ShortRun | 3              | Default     | 16           | 3           | 64   |       188.04 ns |  50.866 ns |  2.788 ns |  1.00 |    0.02 |         - |          NA |
| UnsafeList_AddRange            | ShortRun | 3              | Default     | 16           | 3           | 64   |       111.03 ns |   7.524 ns |  0.412 ns |  0.59 |    0.01 |         - |          NA |
| UnsafeList_ResizeUninitialized | ShortRun | 3              | Default     | 16           | 3           | 64   |        75.89 ns |   0.348 ns |  0.019 ns |  0.40 |    0.01 |         - |          NA |
|                                |          |                |             |              |             |      |                 |            |           |       |         |           |             |
| **UnsafeList_Add**                 | **Dry**      | **1**              | **ColdStart**   | **1**            | **1**           | **1024** | **1,134,748.00 ns** |         **NA** |  **0.000 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| UnsafeList_AddRange            | Dry      | 1              | ColdStart   | 1            | 1           | 1024 | 1,008,147.00 ns |         NA |  0.000 ns |  0.89 |    0.00 |         - |          NA |
| UnsafeList_ResizeUninitialized | Dry      | 1              | ColdStart   | 1            | 1           | 1024 | 1,314,118.00 ns |         NA |  0.000 ns |  1.16 |    0.00 |         - |          NA |
|                                |          |                |             |              |             |      |                 |            |           |       |         |           |             |
| UnsafeList_Add                 | ShortRun | 3              | Default     | 16           | 3           | 1024 |     2,837.21 ns | 293.571 ns | 16.092 ns |  1.00 |    0.01 |         - |          NA |
| UnsafeList_AddRange            | ShortRun | 3              | Default     | 16           | 3           | 1024 |     1,564.01 ns | 272.338 ns | 14.928 ns |  0.55 |    0.01 |         - |          NA |
| UnsafeList_ResizeUninitialized | ShortRun | 3              | Default     | 16           | 3           | 1024 |       774.91 ns |   5.079 ns |  0.278 ns |  0.27 |    0.00 |         - |          NA |
