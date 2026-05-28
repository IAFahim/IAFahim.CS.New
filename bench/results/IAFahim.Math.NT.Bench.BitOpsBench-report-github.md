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
| **BitCount**   | **1000**  |   **612.9 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| BitLength  | 1000  |   596.8 μs |    NA |  0.97 |         - |          NA |
| HighestBit | 1000  |   558.1 μs |    NA |  0.91 |         - |          NA |
|            |       |            |       |       |           |             |
| **BitCount**   | **10000** | **1,783.8 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| BitLength  | 10000 | 1,886.0 μs |    NA |  1.06 |         - |          NA |
| HighestBit | 10000 |   937.4 μs |    NA |  0.53 |         - |          NA |
