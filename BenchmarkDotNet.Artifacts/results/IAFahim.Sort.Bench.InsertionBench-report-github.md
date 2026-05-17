```

BenchmarkDotNet v0.14.0, Ubuntu 26.04 LTS (Resolute Raccoon)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.107
  [Host]     : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2
  Job-EVPLKS : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

```
| Method        | N    | Mean       | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------- |----- |-----------:|----------:|----------:|------:|--------:|----------:|------------:|
| **SpanSort**      | **64**   |   **3.363 μs** | **0.0648 μs** | **0.0541 μs** |  **1.00** |    **0.02** |         **-** |          **NA** |
| InsertionSort | 64   |   5.509 μs | 0.2585 μs | 0.7248 μs |  1.64 |    0.22 |         - |          NA |
|               |      |            |           |           |       |         |           |             |
| **SpanSort**      | **256**  |  **23.207 μs** | **0.6556 μs** | **1.9229 μs** |  **1.01** |    **0.12** |         **-** |          **NA** |
| InsertionSort | 256  |  12.879 μs | 0.2574 μs | 0.5862 μs |  0.56 |    0.05 |         - |          NA |
|               |      |            |           |           |       |         |           |             |
| **SpanSort**      | **1024** |  **94.772 μs** | **1.7900 μs** | **4.4577 μs** |  **1.00** |    **0.06** |         **-** |          **NA** |
| InsertionSort | 1024 | 129.251 μs | 1.3859 μs | 1.2286 μs |  1.37 |    0.06 |         - |          NA |
