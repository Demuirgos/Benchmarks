``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22621.1105)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2


```
|                  Method |     Mean |   Error |   StdDev |   Median |    Gen0 | Allocated |
|------------------------ |---------:|--------:|---------:|---------:|--------:|----------:|
|   OldWay_Valid_Bytecode | 123.1 μs | 5.71 μs | 16.84 μs | 118.3 μs |  7.8125 |  24.12 KB |
| OldWay_Invalid_Bytecode | 107.7 μs | 2.15 μs |  5.03 μs | 107.4 μs |  7.8125 |  24.12 KB |
|   NewWay_Valid_Bytecode | 195.0 μs | 2.68 μs |  2.51 μs | 195.5 μs | 29.7852 |  72.91 KB |
| NewWay_Invalid_Bytecode | 174.9 μs | 4.05 μs | 11.95 μs | 181.7 μs | 30.5176 |  74.19 KB |
