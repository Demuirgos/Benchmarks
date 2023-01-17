``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22621.1105)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2


```
|                             Method |      Mean |    Error |   StdDev |    Gen0 | Allocated |
|----------------------------------- |----------:|---------:|---------:|--------:|----------:|
|      Parallel_Stack_Valid_Bytecode | 231.06 μs | 3.473 μs | 3.249 μs | 66.1621 | 172.84 KB |
|    Parallel_Stack_Invalid_Bytecode | 226.25 μs | 3.461 μs | 3.237 μs | 57.1289 | 153.74 KB |
|     Parallel_Pooled_Valid_Bytecode | 155.19 μs | 1.593 μs | 1.412 μs | 37.8418 |  94.04 KB |
|   Parallel_Pooled_Invalid_Bytecode | 150.54 μs | 1.417 μs | 1.256 μs | 37.3535 |  91.59 KB |
|    Sequential_Stack_Valid_Bytecode |  55.81 μs | 0.768 μs | 0.718 μs |  7.8735 |  24.12 KB |
|  Sequential_Stack_Invalid_Bytecode |  55.79 μs | 0.682 μs | 0.533 μs |  7.8735 |  24.12 KB |
|   Sequential_Pooled_Valid_Bytecode |  38.55 μs | 0.747 μs | 0.734 μs |  5.2490 |  16.09 KB |
| Sequential_Pooled_Invalid_Bytecode |  37.99 μs | 0.489 μs | 0.408 μs |  5.2490 |  16.09 KB |
