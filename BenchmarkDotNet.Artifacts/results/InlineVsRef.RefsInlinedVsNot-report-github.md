``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.978/21H2)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-rc.1.22431.12
  [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT AVX2


```
|                            Method |     Mean |     Error |    StdDev |   Gen0 | Allocated |
|---------------------------------- |---------:|----------:|----------:|-------:|----------:|
|       PassRefStructByValueInlined | 4.428 ns | 0.0449 ns | 0.0398 ns | 0.0102 |      32 B |
|    PassRefStructByValueNotInlined | 3.102 ns | 0.0651 ns | 0.0609 ns | 0.0102 |      32 B |
|                PassRefStructByRef | 3.091 ns | 0.0297 ns | 0.0278 ns | 0.0102 |      32 B |
|    PassNormalStructByValueInlined | 2.824 ns | 0.0865 ns | 0.0849 ns | 0.0102 |      32 B |
| PassNormalStructByValueNotInlined | 2.827 ns | 0.0568 ns | 0.0531 ns | 0.0102 |      32 B |
|             PassNormalStructByRef | 2.842 ns | 0.0550 ns | 0.0514 ns | 0.0102 |      32 B |
