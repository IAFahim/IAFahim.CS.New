```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method     | N     | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|----------- |------ |-----------:|------:|------:|----------:|------------:|
| **FenwickAdd** | **1024**  |   **631.4 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| FenwickSum | 1024  |   563.5 μs |    NA |  0.89 |         - |          NA |
|            |       |            |       |       |           |             |
| **FenwickAdd** | **4096**  |   **878.2 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| FenwickSum | 4096  | 1,158.8 μs |    NA |  1.32 |         - |          NA |
|            |       |            |       |       |           |             |
| **FenwickAdd** | **16384** | **1,783.4 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| FenwickSum | 16384 | 1,568.4 μs |    NA |  0.88 |         - |          NA |
