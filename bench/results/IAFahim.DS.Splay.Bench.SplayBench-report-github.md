```

BenchmarkDotNet v0.14.0, Ubuntu 25.10 (Questing Quokka)
Intel Core i5-4200U CPU 1.60GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.108
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method          | N    | Mean      | Error | Allocated |
|---------------- |----- |----------:|------:|----------:|
| **SplayOperations** | **512**  |  **1.836 ms** |    **NA** |         **-** |
| ReverseRange    | 512  | 20.874 ms |    NA |         - |
| **SplayOperations** | **2048** |  **1.909 ms** |    **NA** |         **-** |
| ReverseRange    | 2048 |  3.746 ms |    NA |         - |
