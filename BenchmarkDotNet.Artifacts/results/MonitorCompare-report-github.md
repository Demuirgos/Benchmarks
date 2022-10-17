``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.674)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.402
  [Host]     : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2


```
|                 Method |     Mean |     Error |    StdDev |   Gen0 | Allocated |
|----------------------- |---------:|----------:|----------:|-------:|----------:|
|           ManualMethod | 5.658 μs | 0.1127 μs | 0.2634 μs |      - |      40 B |
|       TryFinallyMethod | 6.041 μs | 0.1190 μs | 0.2457 μs | 0.0076 |      40 B |
| NestedTryFinallyMethod | 6.159 μs | 0.1048 μs | 0.1434 μs | 0.0076 |      40 B |
|            DeferMethod | 5.669 μs | 0.1114 μs | 0.1668 μs | 0.0076 |      40 B |
|            UsingMethod | 5.909 μs | 0.1033 μs | 0.1782 μs | 0.0076 |      40 B |
|           Using2Method | 5.645 μs | 0.1086 μs | 0.1207 μs | 0.0076 |      40 B |
