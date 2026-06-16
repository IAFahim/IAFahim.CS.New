```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method                | M  | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|---------------------- |--- |---------:|------:|------:|----------:|------------:|
| **ProfileDp_Bench**       | **8**  | **1.205 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
| BrokenProfileDp_Bench | 8  | 1.167 ms |    NA |  0.97 |         - |          NA |
|                       |    |          |       |       |           |             |
| **ProfileDp_Bench**       | **12** | **3.100 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
| BrokenProfileDp_Bench | 12 | 3.078 ms |    NA |  0.99 |         - |          NA |
