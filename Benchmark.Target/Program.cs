using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using Benchmarks.Interception;
using Benchmarks.Models;
using BenchmarkDotNet.Running;
using static TestFunc;

BenchmarkRunner.Run<TestFunc>();

[MemoryDiagnoser]
public class TestFunc {
    public IEnumerable<int> ValuesForlow => new int[] { 0, 100, 1000};
    public IEnumerable<int> ValuesForHigh => new int[] { 100, 1000, 1000000};
    [ParamsSource(nameof(ValuesForlow))] public int lowBound { get; set; }
    [ParamsSource(nameof(ValuesForHigh))] public int highBound { get; set; }

    [Benchmark] public int SumSyncAttr() => SumSync1(lowBound, highBound);
    [Benchmark] public async Task<int> SumAsyncAttr() => await SumAsync1(lowBound, highBound);
    [Benchmark] public int SumSyncClass() => SumSync2(lowBound, highBound);
    [Benchmark] public async Task<int> SumAsyncClass() => await SumAsync2(lowBound, highBound);


    [Monitor(InterceptionMode: InterceptionMode.ExecutionTime, LogDestination : LogDestination.Console)]
    public static int SumSync1(int n, int k)
        => ActionSync(n, k);

    [Monitor(InterceptionMode: InterceptionMode.ExecutionTime, LogDestination : LogDestination.Console)]
    public static async Task<int> SumAsync1(int n, int k) 
        => await ActionAsync(n, k);

    public static int SumSync2(int n, int k)
    {
        using DisposableTimer? timer = new DisposableTimer();
        return ActionSync(n, k);
    }

    public static async Task<int> SumAsync2(int n, int k)
    {
        using DisposableTimer? timer = new DisposableTimer();
        return await ActionAsync(n, k);
    }

    public static int ActionSync(int n, int k)
    {
        int res = 0;
        for (int i = n; i <= k; i++)
        {
            res += i;
        }
        return res;
    }
    public static async Task<int> ActionAsync(int n, int k)
    {
        int res = 0;
        for (int i = n; i <= k; i++)
        {
            res += await Task.FromResult(i);
        }
        return res;
    }
}

public class DisposableTimer :IDisposable
{
    private Stopwatch watch = new();
    public DisposableTimer() => watch.Start();
    public void Dispose() => Console.WriteLine(watch.Elapsed.TotalMilliseconds);
}