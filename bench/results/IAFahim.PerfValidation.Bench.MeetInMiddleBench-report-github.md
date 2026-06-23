```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Job-TELDJN : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry        : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2


```
| Method                      | Job        | InvocationCount | IterationCount | LaunchCount | RunStrategy | UnrollFactor | WarmupCount | Mean       | Error   | StdDev  | Allocated |
|---------------------------- |----------- |---------------- |--------------- |------------ |------------ |------------- |------------ |-----------:|--------:|--------:|----------:|
| MeetInMiddle_SubsetSumCount | Job-TELDJN | 16              | 3              | Default     | Default     | 16           | 2           |   327.6 μs | 8.20 μs | 0.45 μs |         - |
| MeetInMiddle_SubsetSumCount | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 1,735.3 μs |      NA | 0.00 μs |         - |
