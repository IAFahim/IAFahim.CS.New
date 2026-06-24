```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method        | N    | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|-------------- |----- |-----------:|------:|------:|----------:|------------:|
| **SpanSort**      | **64**   |   **538.9 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| InsertionSort | 64   |   585.1 μs |    NA |  1.09 |         - |          NA |
|               |      |            |       |       |           |             |
| **SpanSort**      | **256**  |   **541.5 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| InsertionSort | 256  |   961.4 μs |    NA |  1.78 |         - |          NA |
|               |      |            |       |       |           |             |
| **SpanSort**      | **1024** |   **619.6 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| InsertionSort | 1024 | 1,212.3 μs |    NA |  1.96 |         - |          NA |
