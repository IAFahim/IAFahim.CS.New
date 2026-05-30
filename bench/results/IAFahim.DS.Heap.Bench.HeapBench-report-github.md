```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method   | N      | Mean        | Error | Ratio | Allocated | Alloc Ratio |
|--------- |------- |------------:|------:|------:|----------:|------------:|
| **HeapPush** | **1000**   |    **878.5 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HeapPop  | 1000   |  1,265.9 μs |    NA |  1.44 |         - |          NA |
|          |        |             |       |       |           |             |
| **HeapPush** | **10000**  |  **1,181.2 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HeapPop  | 10000  |  4,694.8 μs |    NA |  3.97 |         - |          NA |
|          |        |             |       |       |           |             |
| **HeapPush** | **100000** |  **5,425.4 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HeapPop  | 100000 | 42,725.7 μs |    NA |  7.88 |         - |          NA |
