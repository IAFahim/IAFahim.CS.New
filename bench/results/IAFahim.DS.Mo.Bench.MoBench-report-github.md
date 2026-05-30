```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method | Q    | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|------- |----- |---------:|------:|------:|----------:|------------:|
| **MoSort** | **1000** | **1.362 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
|        |      |          |       |       |           |             |
| **MoSort** | **5000** | **3.666 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
