```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method | N     | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|------- |------ |-----------:|------:|------:|----------:|------------:|
| **Build**  | **1024**  |   **315.1 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| Query  | 1024  |   610.8 μs |    NA |  1.94 |         - |          NA |
|        |       |            |       |       |           |             |
| **Build**  | **4096**  |   **343.0 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| Query  | 4096  | 1,284.0 μs |    NA |  3.74 |         - |          NA |
|        |       |            |       |       |           |             |
| **Build**  | **16384** |   **446.8 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| Query  | 16384 | 6,164.8 μs |    NA | 13.80 |         - |          NA |
