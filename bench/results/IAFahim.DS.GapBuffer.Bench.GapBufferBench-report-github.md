```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method          | N     | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|---------------- |------ |---------:|------:|------:|----------:|------------:|
| **GapBufferInsert** | **1024**  | **552.2 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| GapBufferDelete | 1024  | 578.2 μs |    NA |  1.05 |         - |          NA |
|                 |       |          |       |       |           |             |
| **GapBufferInsert** | **4096**  | **614.5 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| GapBufferDelete | 4096  | 765.3 μs |    NA |  1.25 |         - |          NA |
|                 |       |          |       |       |           |             |
| **GapBufferInsert** | **16384** | **576.2 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| GapBufferDelete | 16384 | 565.7 μs |    NA |  0.98 |         - |          NA |
