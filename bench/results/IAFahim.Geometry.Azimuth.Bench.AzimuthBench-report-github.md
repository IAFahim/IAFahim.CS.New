```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method                  | N     | Mean     | Error | Allocated |
|------------------------ |------ |---------:|------:|----------:|
| CartesianAzimuth_Bench  | 10000 | 1.079 ms |    NA |         - |
| SphericalAzimuth_Bench  | 10000 | 1.838 ms |    NA |         - |
| SphericalDistance_Bench | 10000 | 1.847 ms |    NA |         - |
