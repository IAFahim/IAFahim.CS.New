```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method           | N    | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|----------------- |----- |-----------:|------:|------:|----------:|------------:|
| **WalshHadamardXor** | **256**  |   **667.6 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| SubsetZeta       | 256  |   810.3 μs |    NA |  1.21 |         - |          NA |
|                  |      |            |       |       |           |             |
| **WalshHadamardXor** | **1024** |   **704.6 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| SubsetZeta       | 1024 | 1,088.1 μs |    NA |  1.54 |         - |          NA |
|                  |      |            |       |       |           |             |
| **WalshHadamardXor** | **4096** | **1,168.8 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| SubsetZeta       | 4096 | 1,297.6 μs |    NA |  1.11 |         - |          NA |
