```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Job-TELDJN : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry        : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2


```
| Method                    | Job        | InvocationCount | IterationCount | LaunchCount | RunStrategy | UnrollFactor | WarmupCount | Mean      | Error    | StdDev   | Allocated |
|-------------------------- |----------- |---------------- |--------------- |------------ |------------ |------------- |------------ |----------:|---------:|---------:|----------:|
| Patternized_LastSeenTable | Job-TELDJN | 16              | 3              | Default     | Default     | 16           | 2           |  11.87 μs | 3.567 μs | 0.196 μs |         - |
| Patternized_LastSeenTable | Dry        | Default         | 1              | 1           | ColdStart   | 1            | 1           | 936.99 μs |       NA | 0.000 μs |         - |
