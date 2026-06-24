```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method                   | N    | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|------------------------- |----- |---------:|------:|------:|----------:|------------:|
| **PointPlaneDistance**       | **256**  | **573.3 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| SegmentPlaneIntersection | 256  | 828.0 μs |    NA |  1.44 |         - |          NA |
|                          |      |          |       |       |           |             |
| **PointPlaneDistance**       | **1024** | **756.4 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| SegmentPlaneIntersection | 1024 | 639.7 μs |    NA |  0.85 |         - |          NA |
