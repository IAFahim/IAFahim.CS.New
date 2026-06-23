```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Job-TELDJN : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry        : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2


```
| Method                      | Job        | InvocationCount | IterationCount | LaunchCount | RunStrategy | UnrollFactor | WarmupCount | N    | Mean      | Error     | StdDev    | Allocated |
|---------------------------- |----------- |---------------- |--------------- |------------ |------------ |------------- |------------ |----- |----------:|----------:|----------:|----------:|
| **BallTree_Build**              | **Job-TELDJN** | **16**              | **3**              | **Default**     | **Default**     | **16**           | **2**           | **1024** |  **2.297 ms** | **0.3249 ms** | **0.0178 ms** |         **-** |
| BallTree_Nearest_AllQueries | Job-TELDJN | 16              | 3              | Default     | Default     | 16           | 2           | 1024 |  3.650 ms | 0.2582 ms | 0.0142 ms |         - |
| BallTree_Build              | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 1024 |  4.022 ms |        NA | 0.0000 ms |         - |
| BallTree_Nearest_AllQueries | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 1024 |  5.926 ms |        NA | 0.0000 ms |         - |
| **BallTree_Build**              | **Job-TELDJN** | **16**              | **3**              | **Default**     | **Default**     | **16**           | **2**           | **8192** |  **6.638 ms** | **0.3111 ms** | **0.0171 ms** |         **-** |
| BallTree_Nearest_AllQueries | Job-TELDJN | 16              | 3              | Default     | Default     | 16           | 2           | 8192 | 13.272 ms | 0.8278 ms | 0.0454 ms |         - |
| BallTree_Build              | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 8192 | 31.710 ms |        NA | 0.0000 ms |         - |
| BallTree_Nearest_AllQueries | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 8192 | 48.734 ms |        NA | 0.0000 ms |         - |
