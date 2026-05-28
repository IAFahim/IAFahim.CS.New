```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method   | N      | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|--------- |------- |-----------:|------:|------:|----------:|------------:|
| **DsuUnion** | **1000**   |   **653.1 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| DsuFind  | 1000   |   446.0 μs |    NA |  0.68 |         - |          NA |
|          |        |            |       |       |           |             |
| **DsuUnion** | **10000**  | **1,596.8 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| DsuFind  | 10000  |   695.4 μs |    NA |  0.44 |         - |          NA |
|          |        |            |       |       |           |             |
| **DsuUnion** | **100000** | **3,711.6 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| DsuFind  | 100000 |   734.0 μs |    NA |  0.20 |         - |          NA |
