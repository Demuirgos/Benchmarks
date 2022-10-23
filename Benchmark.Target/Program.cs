using BenchmarkDotNet.Attributes;
using Benchmarks.Interception;
using Benchmarks.Models;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using System.Diagnostics;
using System.Reflection;
using static TestFunc;
using BenchmarkDotNet.Running;

var i = SumSync1(0, 100);
var j = await SumAsync1(0, 100);
Console.WriteLine($"Results are : [{i}, {j}]");

//BenchmarkRunner.Run<TestFunc>();

[MemoryDiagnoser]
public class TestFunc {
    public IEnumerable<int> ValuesForlow => new int[] { 0, 100, 1000};
    public IEnumerable<int> ValuesForHigh => new int[] { 100, 1000, 1000000};

    [ParamsSource(nameof(ValuesForlow))]
    public int lowBound { get; set; }
    [ParamsSource(nameof(ValuesForHigh))]
    public int highBound { get; set; }
    [Benchmark] public int SumSyncAttr() => SumSync1(lowBound, highBound);
    [Benchmark] public async Task<int> SumAsyncAttr() => await SumAsync1(lowBound, highBound);

    [Monitor(InterceptionMode: InterceptionMode.ExecutionTime, LogDestination : LogDestination.Console)]
    public static int SumSync1(int n, int k)
    {
        return ActionSync(n, k);
    }

    [Monitor(InterceptionMode: InterceptionMode.Failures, LogDestination : LogDestination.Console)]
    public static async Task<int> SumAsync1(int n, int k)
    {
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
            if((new Random()).Next(0, 100) == 0)
            {
                throw new Exception("Random Exception");
            }
            res += await Task.FromResult(i);
        }
        return res;
    }
}