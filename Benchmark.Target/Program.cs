using Benchmarks.Interception;
using Benchmarks.Models;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using System.Reflection;
using Plugin;
using static TestFunc;

try
{
    int i = SumSync(0, 100);
    int j =  await SumAsync(0, 100);
    Console.WriteLine($"results : {i} == {j}");
}
catch { }


var prometheusSinkType = typeof(Plugin.Metrics);
foreach (var property in prometheusSinkType?.GetProperties(BindingFlags.Public | BindingFlags.Static))
{
    var value = property.GetValue(null, null);
    Console.WriteLine($"{property.Name} := {value}");
}
class TestFunc {

    [Monitor(InterceptionMode.MetadataLog, LogDestination.Console)]
    public static int SumSync(int n, int k)
    {
        try
        {
            int res = 0;
            for(int i = n; i <= k; i++){
                res += GetIntSync(i);
            }
            return res;
        } catch
        {
            return 0;
        }
    }

    [Monitor(InterceptionMode.MetadataLog, LogDestination.Console)]
    public static async Task<int> SumAsync(int n, int k) {
        int res = 0;
        for (int i = n; i <= k; i++)
        {
            res += await GetIntAsync(i);
        }
        return res;
    }
    [Monitor(InterceptionMode.CallCount, LogDestination.Prometheus)]
    private static int GetIntSync(int i)
    {
        return i;
    }

    [Monitor(InterceptionMode.ExecutionTime, LogDestination.Prometheus)]
    private static async Task<int> GetIntAsync(int i) {
        await Task.Delay(10);
        return (new Random()).Next(0, 50) == 1 ? throw new Exception("testing async exceptions") : i;
    }
}