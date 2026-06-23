```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method                  | Bits | Mean       | Error | Allocated |
|------------------------ |----- |-----------:|------:|----------:|
| **GrayCodeGenerate_Bench**  | **8**    |   **586.0 μs** |    **NA** |         **-** |
| GrayCodeToAndFrom_Bench | 8    |   510.8 μs |    NA |         - |
| NextPermutation_Bench   | 8    | 1,069.0 μs |    NA |         - |
| CartesianProduct_Bench  | 8    |   599.0 μs |    NA |         - |
| **GrayCodeGenerate_Bench**  | **10**   |   **560.8 μs** |    **NA** |         **-** |
| GrayCodeToAndFrom_Bench | 10   |   518.7 μs |    NA |         - |
| NextPermutation_Bench   | 10   | 5,244.9 μs |    NA |         - |
| CartesianProduct_Bench  | 10   |   576.9 μs |    NA |         - |
