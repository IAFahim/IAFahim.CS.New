```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method                  | N     | Mean           | Error | Ratio | Allocated | Alloc Ratio |
|------------------------ |------ |---------------:|------:|------:|----------:|------------:|
| **Knapsack01_Bench**        | **100**   |     **1,785.4 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| KnapsackUnbounded_Bench | 100   |     2,118.3 μs |    NA |  1.19 |         - |          NA |
| KnapsackBounded_Bench   | 100   |     6,410.9 μs |    NA |  3.59 |         - |          NA |
| SubsetSum_Bench         | 100   |     1,317.8 μs |    NA |  0.74 |         - |          NA |
| BitsetSubsetSum_Bench   | 100   |       773.5 μs |    NA |  0.43 |         - |          NA |
|                         |       |                |       |       |           |             |
| **Knapsack01_Bench**        | **1000**  |    **18,866.1 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| KnapsackUnbounded_Bench | 1000  |    10,635.4 μs |    NA |  0.56 |         - |          NA |
| KnapsackBounded_Bench   | 1000  |    90,667.6 μs |    NA |  4.81 |         - |          NA |
| SubsetSum_Bench         | 1000  |    15,006.5 μs |    NA |  0.80 |         - |          NA |
| BitsetSubsetSum_Bench   | 1000  |     1,948.5 μs |    NA |  0.10 |         - |          NA |
|                         |       |                |       |       |           |             |
| **Knapsack01_Bench**        | **10000** |   **834,729.5 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| KnapsackUnbounded_Bench | 10000 |   936,474.5 μs |    NA |  1.12 |         - |          NA |
| KnapsackBounded_Bench   | 10000 | 9,048,967.1 μs |    NA | 10.84 |         - |          NA |
| SubsetSum_Bench         | 10000 |   354,701.1 μs |    NA |  0.42 |         - |          NA |
| BitsetSubsetSum_Bench   | 10000 |    81,685.9 μs |    NA |  0.10 |         - |          NA |
