```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Job-TELDJN : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry        : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2


```
| Method            | Job        | InvocationCount | IterationCount | LaunchCount | RunStrategy | UnrollFactor | WarmupCount | N    | Mean        | Error    | StdDev   | Allocated |
|------------------ |----------- |---------------- |--------------- |------------ |------------ |------------- |------------ |----- |------------:|---------:|---------:|----------:|
| **SortInts_Heapsort** | **Job-TELDJN** | **16**              | **3**              | **Default**     | **Default**     | **16**           | **2**           | **256**  |    **61.60 μs** | **32.32 μs** | **1.772 μs** |         **-** |
| SortInts_Heapsort | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 256  |   697.05 μs |       NA | 0.000 μs |         - |
| **SortInts_Heapsort** | **Job-TELDJN** | **16**              | **3**              | **Default**     | **Default**     | **16**           | **2**           | **4096** |   **245.38 μs** | **28.24 μs** | **1.548 μs** |         **-** |
| SortInts_Heapsort | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 4096 | 2,247.37 μs |       NA | 0.000 μs |         - |
