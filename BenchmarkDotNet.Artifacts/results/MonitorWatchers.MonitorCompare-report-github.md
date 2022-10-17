``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.674)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.402
  [Host]     : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2


```
|       Method |     Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------- |---------:|----------:|----------:|-------:|----------:|
| ManualMethod | 6.111 μs | 0.1220 μs | 0.2547 μs |      - |      40 B |
|  DeferMethod | 5.743 μs | 0.1131 μs | 0.1302 μs | 0.0076 |      40 B |
|  UsingMethod | 5.642 μs | 0.1088 μs | 0.1525 μs | 0.0076 |      40 B |
| Using2Method | 5.743 μs | 0.1138 μs | 0.2165 μs | 0.0076 |      40 B |
