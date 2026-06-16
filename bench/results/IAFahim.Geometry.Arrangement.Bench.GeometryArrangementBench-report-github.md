```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method                | N    | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|---------------------- |----- |-----------:|------:|------:|----------:|------------:|
| **PointLocationBuild**    | **256**  |   **696.3 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| VerticalDecomposition | 256  | 1,237.2 μs |    NA |  1.78 |         - |          NA |
|                       |      |            |       |       |           |             |
| **PointLocationBuild**    | **1024** |   **750.7 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| VerticalDecomposition | 1024 | 1,532.7 μs |    NA |  2.04 |         - |          NA |
