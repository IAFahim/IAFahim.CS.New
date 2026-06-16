```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method       | N     | Mean        | Error | Allocated |
|------------- |------ |------------:|------:|----------:|
| **Gcd_Bench**    | **100**   |    **584.6 μs** |    **NA** |         **-** |
| ModPow_Bench | 100   |    824.4 μs |    NA |         - |
| ModMul_Bench | 100   |    600.6 μs |    NA |         - |
| **Gcd_Bench**    | **10000** |  **2,001.6 μs** |    **NA** |         **-** |
| ModPow_Bench | 10000 | 61,850.7 μs |    NA |         - |
| ModMul_Bench | 10000 |  4,708.9 μs |    NA |         - |
