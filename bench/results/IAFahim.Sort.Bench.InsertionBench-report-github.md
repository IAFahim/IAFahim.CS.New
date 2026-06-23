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
| **SpanSort**      | **64**   |   **512.8 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| InsertionSort | 64   |   567.5 μs |    NA |  1.11 |         - |          NA |
|               |      |            |       |       |           |             |
| **SpanSort**      | **256**  |   **516.2 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| InsertionSort | 256  |   944.0 μs |    NA |  1.83 |         - |          NA |
|               |      |            |       |       |           |             |
| **SpanSort**      | **1024** |   **560.8 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| InsertionSort | 1024 | 1,180.8 μs |    NA |  2.11 |         - |          NA |
