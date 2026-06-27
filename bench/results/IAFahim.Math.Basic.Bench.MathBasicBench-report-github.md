```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method         | N       | Mean       | Error | Allocated |
|--------------- |-------- |-----------:|------:|----------:|
| **CeilDiv_Bench**  | **1000**    |   **543.6 μs** |    **NA** |         **-** |
| FloorDiv_Bench | 1000    |   506.6 μs |    NA |         - |
| AbsInt_Bench   | 1000    |   680.8 μs |    NA |         - |
| MinInt_Bench   | 1000    |   489.2 μs |    NA |         - |
| MaxInt_Bench   | 1000    |   481.5 μs |    NA |         - |
| Clamp_Bench    | 1000    |   524.8 μs |    NA |         - |
| **CeilDiv_Bench**  | **1000000** | **1,167.8 μs** |    **NA** |         **-** |
| FloorDiv_Bench | 1000000 | 1,190.9 μs |    NA |         - |
| AbsInt_Bench   | 1000000 |   518.2 μs |    NA |         - |
| MinInt_Bench   | 1000000 | 1,118.5 μs |    NA |         - |
| MaxInt_Bench   | 1000000 | 1,103.9 μs |    NA |         - |
| Clamp_Bench    | 1000000 | 1,800.3 μs |    NA |         - |
