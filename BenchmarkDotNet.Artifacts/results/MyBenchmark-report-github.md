``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22621.1105)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2


```
|                                          Method |     Mean |     Error |    StdDev |     Gen0 | Allocated |
|------------------------------------------------ |---------:|----------:|----------:|---------:|----------:|
|            Sequential_Pooled_Valid_Bytecode_Old | 3.204 ms | 0.0311 ms | 0.0291 ms | 777.3438 | 2448838 B |
|          Sequential_Pooled_Invalid_Bytecode_Old | 1.661 ms | 0.0151 ms | 0.0133 ms | 388.6719 | 1224567 B |
|   Sequential_Pooled_Valid_Bytecode_No_Bit_Manip | 2.126 ms | 0.0206 ms | 0.0193 ms |        - |     351 B |
| Sequential_Pooled_Invalid_Bytecode_No_Bit_Manip | 1.097 ms | 0.0179 ms | 0.0256 ms |        - |     323 B |
|         Sequential_Pooled_Valid_Bytecode_Struct | 2.364 ms | 0.0321 ms | 0.0300 ms |        - |     351 B |
|       Sequential_Pooled_Invalid_Bytecode_Struct | 1.228 ms | 0.0103 ms | 0.0091 ms |        - |     323 B |
|            Sequential_Pooled_Valid_Bytecode_New |       NA |        NA |        NA |        - |         - |
|          Sequential_Pooled_Invalid_Bytecode_New |       NA |        NA |        NA |        - |         - |

Benchmarks with issues:
  MyBenchmark.Sequential_Pooled_Valid_Bytecode_New: DefaultJob
  MyBenchmark.Sequential_Pooled_Invalid_Bytecode_New: DefaultJob
