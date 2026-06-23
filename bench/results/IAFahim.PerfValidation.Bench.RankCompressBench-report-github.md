```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Job-TELDJN : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry        : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2


```
| Method                               | Job        | InvocationCount | IterationCount | LaunchCount | RunStrategy | UnrollFactor | WarmupCount | N    | Mean        | Error     | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------------------- |----------- |---------------- |--------------- |------------ |------------ |------------- |------------ |----- |------------:|----------:|---------:|------:|--------:|----------:|------------:|
| **RankCompress_Heapsort**                | **Job-TELDJN** | **16**              | **3**              | **Default**     | **Default**     | **16**           | **2**           | **256**  |   **106.22 μs** | **108.98 μs** | **5.974 μs** |  **1.77** |    **0.12** |         **-** |          **NA** |
| RankCompress_InsertionSort_Reference | Job-TELDJN | 16              | 3              | Default     | Default     | 16           | 2           | 256  |    60.25 μs |  60.93 μs | 3.340 μs |  1.00 |    0.07 |         - |          NA |
|                                      |            |                 |                |             |             |              |             |      |             |           |          |       |         |           |             |
| RankCompress_Heapsort                | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 256  | 1,015.52 μs |        NA | 0.000 μs |  1.02 |    0.00 |         - |          NA |
| RankCompress_InsertionSort_Reference | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 256  |   996.62 μs |        NA | 0.000 μs |  1.00 |    0.00 |         - |          NA |
|                                      |            |                 |                |             |             |              |             |      |             |           |          |       |         |           |             |
| **RankCompress_Heapsort**                | **Job-TELDJN** | **16**              | **3**              | **Default**     | **Default**     | **16**           | **2**           | **2048** |   **211.46 μs** |  **51.76 μs** | **2.837 μs** |  **0.28** |    **0.00** |         **-** |          **NA** |
| RankCompress_InsertionSort_Reference | Job-TELDJN | 16              | 3              | Default     | Default     | 16           | 2           | 2048 |   761.20 μs |  24.94 μs | 1.367 μs |  1.00 |    0.00 |         - |          NA |
|                                      |            |                 |                |             |             |              |             |      |             |           |          |       |         |           |             |
| RankCompress_Heapsort                | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 2048 | 1,855.34 μs |        NA | 0.000 μs |  0.94 |    0.00 |         - |          NA |
| RankCompress_InsertionSort_Reference | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 2048 | 1,975.66 μs |        NA | 0.000 μs |  1.00 |    0.00 |         - |          NA |
