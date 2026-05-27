```

BenchmarkDotNet v0.14.0, Ubuntu 25.10 (Questing Quokka)
Intel Core i5-4200U CPU 1.60GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.108
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method      | N    | Mean      | Error | Ratio | Allocated | Alloc Ratio |
|------------ |----- |----------:|------:|------:|----------:|------------:|
| **PointDot**    | **256**  | **28.077 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
| PointCross  | 256  |  8.868 ms |    NA |  0.32 |         - |          NA |
| PolygonArea | 256  |  8.459 ms |    NA |  0.30 |         - |          NA |
|             |      |           |       |       |           |             |
| **PointDot**    | **1024** |  **1.935 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
| PointCross  | 1024 |  2.141 ms |    NA |  1.11 |         - |          NA |
| PolygonArea | 1024 |  1.599 ms |    NA |  0.83 |         - |          NA |
