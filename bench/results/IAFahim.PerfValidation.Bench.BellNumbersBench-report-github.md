```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Job-TELDJN : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry        : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2


```
| Method                   | Job        | InvocationCount | IterationCount | LaunchCount | RunStrategy | UnrollFactor | WarmupCount | N   | Mean         | Error     | StdDev    | Allocated |
|------------------------- |----------- |---------------- |--------------- |------------ |------------ |------------- |------------ |---- |-------------:|----------:|----------:|----------:|
| **BellNumbers_BellTriangle** | **Job-TELDJN** | **16**              | **3**              | **Default**     | **Default**     | **16**           | **2**           | **50**  |     **5.327 μs** | **12.077 μs** | **0.6620 μs** |         **-** |
| BellNumbers_BellTriangle | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 50  |   770.215 μs |        NA | 0.0000 μs |         - |
| **BellNumbers_BellTriangle** | **Job-TELDJN** | **16**              | **3**              | **Default**     | **Default**     | **16**           | **2**           | **500** |   **435.298 μs** |  **5.985 μs** | **0.3281 μs** |         **-** |
| BellNumbers_BellTriangle | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 500 | 1,240.642 μs |        NA | 0.0000 μs |         - |
