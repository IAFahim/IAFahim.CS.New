```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry      : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  ShortRun : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

LaunchCount=1  

```
| Method                | Job      | IterationCount | RunStrategy | UnrollFactor | WarmupCount | N    | Mean            | Error      | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------- |--------- |--------------- |------------ |------------- |------------ |----- |----------------:|-----------:|----------:|------:|--------:|----------:|------------:|
| **DirectPointer**         | **Dry**      | **1**              | **ColdStart**   | **1**            | **1**           | **64**   |   **381,744.00 ns** |         **NA** |  **0.000 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| NativeArray_ReadWrite | Dry      | 1              | ColdStart   | 1            | 1           | 64   | 1,167,328.00 ns |         NA |  0.000 ns |  3.06 |    0.00 |         - |          NA |
|                       |          |                |             |              |             |      |                 |            |           |       |         |           |             |
| DirectPointer         | ShortRun | 3              | Default     | 16           | 3           | 64   |        46.61 ns |   1.125 ns |  0.062 ns |  1.00 |    0.00 |         - |          NA |
| NativeArray_ReadWrite | ShortRun | 3              | Default     | 16           | 3           | 64   |       195.78 ns |   1.226 ns |  0.067 ns |  4.20 |    0.00 |         - |          NA |
|                       |          |                |             |              |             |      |                 |            |           |       |         |           |             |
| **DirectPointer**         | **Dry**      | **1**              | **ColdStart**   | **1**            | **1**           | **1024** |   **420,333.00 ns** |         **NA** |  **0.000 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| NativeArray_ReadWrite | Dry      | 1              | ColdStart   | 1            | 1           | 1024 | 1,192,024.00 ns |         NA |  0.000 ns |  2.84 |    0.00 |         - |          NA |
|                       |          |                |             |              |             |      |                 |            |           |       |         |           |             |
| DirectPointer         | ShortRun | 3              | Default     | 16           | 3           | 1024 |       498.71 ns | 249.748 ns | 13.690 ns |  1.00 |    0.03 |         - |          NA |
| NativeArray_ReadWrite | ShortRun | 3              | Default     | 16           | 3           | 1024 |     3,155.69 ns |  84.215 ns |  4.616 ns |  6.33 |    0.15 |         - |          NA |
