```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method             | N    | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|------------------- |----- |---------:|------:|------:|----------:|------------:|
| **SpanBinarySearch**   | **64**   | **592.6 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| LowerBound_Bench   | 64   | 584.6 μs |    NA |  0.99 |         - |          NA |
| UpperBound_Bench   | 64   | 570.3 μs |    NA |  0.96 |         - |          NA |
| BinarySearch_Bench | 64   | 597.1 μs |    NA |  1.01 |         - |          NA |
|                    |      |          |       |       |           |             |
| **SpanBinarySearch**   | **256**  | **596.5 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| LowerBound_Bench   | 256  | 553.0 μs |    NA |  0.93 |         - |          NA |
| UpperBound_Bench   | 256  | 591.6 μs |    NA |  0.99 |         - |          NA |
| BinarySearch_Bench | 256  | 565.2 μs |    NA |  0.95 |         - |          NA |
|                    |      |          |       |       |           |             |
| **SpanBinarySearch**   | **1024** | **796.2 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| LowerBound_Bench   | 1024 | 573.0 μs |    NA |  0.72 |         - |          NA |
| UpperBound_Bench   | 1024 | 548.8 μs |    NA |  0.69 |         - |          NA |
| BinarySearch_Bench | 1024 | 612.4 μs |    NA |  0.77 |         - |          NA |
