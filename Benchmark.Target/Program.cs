using Benchmarks.Interception;
using Benchmarks.Models;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using System.Reflection;
using Plugin;
using static TestFunc;

SumSync(0, 100);
await SumAsync(0, 100);

var prometheusSinkType = typeof(Plugin.Metrics);
foreach (var property in prometheusSinkType?.GetProperties(BindingFlags.Public | BindingFlags.Static))
{
    var value = property.GetValue(null, null);
    Console.WriteLine($"{property.Name} := {value}");
}
class TestFunc {
    [Monitor(InterceptionMode.ExecutionTime, LogDestination.Prometheus)]
    public static int SumSync(int n, int k)
    {
        int res = 0;
        for(int i = n; i < k; i++){
            res += GetIntSync(i);
        }
        return res;
    }

    [Monitor(InterceptionMode.ExecutionTime, LogDestination.Prometheus)]
    public static async Task<int> SumAsync(int n, int k) {
        int res = 0;
        for(int i = n; i < k; i++){
            res += await GetIntAsync(i);
        }
        return res;
    }

    [Monitor(InterceptionMode.ExecutionTime, LogDestination.Console, 30000)]
    private static int GetIntSync(int i)
    {
        return i;
    }

    private static async Task<int> GetIntAsync(int i) {
        await Task.Delay(10);
        return i;
    }
}