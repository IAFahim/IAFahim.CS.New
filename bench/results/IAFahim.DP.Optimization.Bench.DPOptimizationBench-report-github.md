```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method                  | N    | Mean        | Error | Ratio | Allocated | Alloc Ratio |
|------------------------ |----- |------------:|------:|------:|----------:|------------:|
| **KnuthOptimization_Bench** | **64**   |    **903.9 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| LiChaoAddLine_Bench     | 64   |    659.8 μs |    NA |  0.73 |         - |          NA |
|                         |      |             |       |       |           |             |
| **KnuthOptimization_Bench** | **256**  |  **3,394.3 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| LiChaoAddLine_Bench     | 256  |    699.1 μs |    NA |  0.21 |         - |          NA |
|                         |      |             |       |       |           |             |
| **KnuthOptimization_Bench** | **1024** | **26,617.4 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| LiChaoAddLine_Bench     | 1024 |    758.2 μs |    NA |  0.03 |         - |          NA |
