``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22621.1105)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2


```
|                             Method |       Mean |    Error |    StdDev |     Median |    Gen0 | Allocated |
|----------------------------------- |-----------:|---------:|----------:|-----------:|--------:|----------:|
|      Parallel_Stack_Valid_Bytecode |   861.0 μs | 16.86 μs |  14.95 μs |   862.0 μs | 80.0781 |  250639 B |
|    Parallel_Stack_Invalid_Bytecode |   414.0 μs |  8.15 μs |  17.89 μs |   415.6 μs | 41.5039 |  129813 B |
|     Parallel_Pooled_Valid_Bytecode |   426.3 μs |  8.39 μs |  17.14 μs |   429.4 μs |  0.9766 |    3392 B |
|   Parallel_Pooled_Invalid_Bytecode |   207.4 μs |  4.01 μs |   6.37 μs |   206.5 μs |  0.9766 |    3406 B |
|    Sequential_Stack_Valid_Bytecode | 1,916.3 μs | 53.19 μs | 147.40 μs | 1,946.3 μs | 78.1250 |  246906 B |
|  Sequential_Stack_Invalid_Bytecode | 1,080.2 μs | 22.78 μs |  63.88 μs | 1,078.9 μs | 41.0156 |  131694 B |
|   Sequential_Pooled_Valid_Bytecode | 1,008.8 μs | 20.10 μs |  51.16 μs | 1,006.6 μs |       - |     301 B |
| Sequential_Pooled_Invalid_Bytecode |   530.1 μs | 15.38 μs |  43.88 μs |   542.4 μs |       - |     299 B |
