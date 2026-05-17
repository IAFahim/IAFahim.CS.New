```

BenchmarkDotNet v0.14.0, Ubuntu 26.04 LTS (Resolute Raccoon)
Intel Core i9-14900K, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.107
  [Host]     : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2
  Job-UBJCFN : .NET 10.0.7 (10.0.726.21808), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

```
| Method        | N    | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------- |----- |-----------:|----------:|----------:|-----------:|------:|--------:|----------:|------------:|
| **SpanSort**      | **64**   |   **3.341 μs** | **0.0550 μs** | **0.0735 μs** |   **3.333 μs** |  **1.00** |    **0.03** |         **-** |          **NA** |
| InsertionSort | 64   |   5.410 μs | 0.1828 μs | 0.5186 μs |   5.326 μs |  1.62 |    0.16 |         - |          NA |
|               |      |            |           |           |            |       |         |           |             |
| **SpanSort**      | **256**  |  **23.258 μs** | **0.6925 μs** | **2.0420 μs** |  **22.718 μs** |  **1.01** |    **0.13** |         **-** |          **NA** |
| InsertionSort | 256  |  12.610 μs | 0.2438 μs | 0.6549 μs |  12.540 μs |  0.55 |    0.06 |         - |          NA |
|               |      |            |           |           |            |       |         |           |             |
| **SpanSort**      | **1024** |  **95.699 μs** | **1.8955 μs** | **4.9602 μs** |  **93.413 μs** |  **1.00** |    **0.07** |         **-** |          **NA** |
| InsertionSort | 1024 | 126.936 μs | 1.5533 μs | 1.2127 μs | 126.757 μs |  1.33 |    0.07 |         - |          NA |
