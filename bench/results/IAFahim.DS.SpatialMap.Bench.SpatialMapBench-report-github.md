```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method          | N    | Mean     | Error | Allocated |
|---------------- |----- |---------:|------:|----------:|
| **BuildSpatialMap** | **64**   | **2.468 ms** |    **NA** |         **-** |
| **BuildSpatialMap** | **256**  | **2.485 ms** |    **NA** |         **-** |
| **BuildSpatialMap** | **1024** | **2.697 ms** |    **NA** |         **-** |
| **BuildSpatialMap** | **4096** | **3.871 ms** |    **NA** |         **-** |
