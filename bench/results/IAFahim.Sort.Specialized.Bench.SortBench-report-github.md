```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method           | N    | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|----------------- |----- |-----------:|------:|------:|----------:|------------:|
| **SpanSort**         | **64**   |   **572.1 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| SortInts_Bench   | 64   |   614.5 μs |    NA |  1.07 |         - |          NA |
| SortInt64s_Bench | 64   |   573.9 μs |    NA |  1.00 |         - |          NA |
|                  |      |            |       |       |           |             |
| **SpanSort**         | **256**  |   **534.8 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| SortInts_Bench   | 256  |   902.5 μs |    NA |  1.69 |         - |          NA |
| SortInt64s_Bench | 256  | 1,068.1 μs |    NA |  2.00 |         - |          NA |
|                  |      |            |       |       |           |             |
| **SpanSort**         | **1024** |   **606.3 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| SortInts_Bench   | 1024 | 1,188.0 μs |    NA |  1.96 |         - |          NA |
| SortInt64s_Bench | 1024 | 1,100.9 μs |    NA |  1.82 |         - |          NA |
