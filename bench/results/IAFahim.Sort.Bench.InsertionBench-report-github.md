```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method        | N    | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|-------------- |----- |-----------:|------:|------:|----------:|------------:|
| **SpanSort**      | **64**   |   **526.3 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| InsertionSort | 64   |   609.6 μs |    NA |  1.16 |         - |          NA |
|               |      |            |       |       |           |             |
| **SpanSort**      | **256**  |   **617.8 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| InsertionSort | 256  | 1,041.7 μs |    NA |  1.69 |         - |          NA |
|               |      |            |       |       |           |             |
| **SpanSort**      | **1024** |   **584.1 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| InsertionSort | 1024 | 1,247.5 μs |    NA |  2.14 |         - |          NA |
