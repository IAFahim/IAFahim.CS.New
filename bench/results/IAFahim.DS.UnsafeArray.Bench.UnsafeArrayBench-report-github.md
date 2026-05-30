```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method      | N     | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|------------ |------ |-----------:|------:|------:|----------:|------------:|
| **AllocateInt** | **1024**  |   **485.6 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| FillAndSum  | 1024  |   460.2 μs |    NA |  0.95 |         - |          NA |
| MemCopy     | 1024  |   501.3 μs |    NA |  1.03 |         - |          NA |
|             |       |            |       |       |           |             |
| **AllocateInt** | **4096**  |   **583.5 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| FillAndSum  | 4096  |   504.7 μs |    NA |  0.86 |         - |          NA |
| MemCopy     | 4096  |   603.9 μs |    NA |  1.04 |         - |          NA |
|             |       |            |       |       |           |             |
| **AllocateInt** | **16384** |   **967.7 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| FillAndSum  | 16384 | 1,045.9 μs |    NA |  1.08 |         - |          NA |
| MemCopy     | 16384 | 1,404.3 μs |    NA |  1.45 |         - |          NA |
