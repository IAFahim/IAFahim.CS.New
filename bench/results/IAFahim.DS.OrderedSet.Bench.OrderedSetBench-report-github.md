```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method           | N    | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|----------------- |----- |-----------:|------:|------:|----------:|------------:|
| **OrderedSetInsert** | **64**   |   **812.3 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| OrderedSetRank   | 64   |   530.1 μs |    NA |  0.65 |         - |          NA |
|                  |      |            |       |       |           |             |
| **OrderedSetInsert** | **256**  |   **857.1 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| OrderedSetRank   | 256  |   728.0 μs |    NA |  0.85 |         - |          NA |
|                  |      |            |       |       |           |             |
| **OrderedSetInsert** | **1024** | **2,179.4 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| OrderedSetRank   | 1024 |   526.4 μs |    NA |  0.24 |         - |          NA |
