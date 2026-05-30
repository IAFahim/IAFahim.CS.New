```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method           | N     | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|----------------- |------ |-----------:|------:|------:|----------:|------------:|
| **SparseTableBuild** | **1024**  |   **654.5 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
|                  |       |            |       |       |           |             |
| **SparseTableBuild** | **4096**  | **1,132.5 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
|                  |       |            |       |       |           |             |
| **SparseTableBuild** | **16384** | **1,774.9 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
