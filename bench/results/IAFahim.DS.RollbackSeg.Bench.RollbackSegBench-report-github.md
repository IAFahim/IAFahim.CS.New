```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method         | N    | Mean     | Error | Allocated |
|--------------- |----- |---------:|------:|----------:|
| **BuildAndUpdate** | **1024** | **2.024 ms** |    **NA** |         **-** |
| QueryRangeSum  | 1024 | 1.911 ms |    NA |         - |
| **BuildAndUpdate** | **4096** | **1.998 ms** |    **NA** |         **-** |
| QueryRangeSum  | 4096 | 1.952 ms |    NA |         - |
