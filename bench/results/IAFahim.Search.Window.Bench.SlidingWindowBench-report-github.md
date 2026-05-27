```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method                 | N    | WindowSize | Mean     | Error | Allocated |
|----------------------- |----- |----------- |---------:|------:|----------:|
| **SlidingWindowMin_Bench** | **1024** | **16**         | **736.1 μs** |    **NA** |         **-** |
| SlidingWindowMax_Bench | 1024 | 16         | 768.0 μs |    NA |         - |
| **SlidingWindowMin_Bench** | **1024** | **64**         | **763.1 μs** |    **NA** |         **-** |
| SlidingWindowMax_Bench | 1024 | 64         | 777.9 μs |    NA |         - |
| **SlidingWindowMin_Bench** | **4096** | **16**         | **797.0 μs** |    **NA** |         **-** |
| SlidingWindowMax_Bench | 4096 | 16         | 797.7 μs |    NA |         - |
| **SlidingWindowMin_Bench** | **4096** | **64**         | **769.2 μs** |    **NA** |         **-** |
| SlidingWindowMax_Bench | 4096 | 64         | 784.1 μs |    NA |         - |
