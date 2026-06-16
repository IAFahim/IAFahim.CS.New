```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method      | N     | Mean       | Error | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------ |------ |-----------:|------:|------:|--------:|----------:|------------:|
| **ZAlgorithm**  | **100**   |   **707.3 μs** |    **NA** |  **1.00** |    **0.00** |         **-** |          **NA** |
| ManacherOdd | 100   |   693.8 μs |    NA |  0.98 |    0.00 |         - |          NA |
|             |       |            |       |       |         |           |             |
| **ZAlgorithm**  | **1000**  |         **NA** |    **NA** |     **?** |       **?** |        **NA** |           **?** |
| ManacherOdd | 1000  |   672.6 μs |    NA |     ? |       ? |         - |           ? |
|             |       |            |       |       |         |           |             |
| **ZAlgorithm**  | **10000** | **1,129.2 μs** |    **NA** |  **1.00** |    **0.00** |         **-** |          **NA** |
| ManacherOdd | 10000 | 1,093.8 μs |    NA |  0.97 |    0.00 |         - |          NA |

Benchmarks with issues:
  StringBench.ZAlgorithm: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=1000]
