```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method        | N    | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|-------------- |----- |---------:|------:|------:|----------:|------------:|
| **LinkCutAccess** | **100**  | **611.0 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| LinkCutQuery  | 100  | 716.6 μs |    NA |  1.17 |         - |          NA |
|               |      |          |       |       |           |             |
| **LinkCutAccess** | **1000** | **665.0 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| LinkCutQuery  | 1000 | 737.0 μs |    NA |  1.11 |         - |          NA |
|               |      |          |       |       |           |             |
| **LinkCutAccess** | **5000** | **692.3 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| LinkCutQuery  | 5000 | 819.3 μs |    NA |  1.18 |         - |          NA |
