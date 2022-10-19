using Benchmarks.Interception;
using Benchmarks.Models;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using System.Reflection;
using Plugin;
using static TestFunc;

int i = SumSync(0, 100);
int j =  await SumAsync(0, 100);

Console.WriteLine($"results : {i} == {j}");

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

    [Monitor(InterceptionMode.ExecutionTime, LogDestination.Prometheus)]
    public static async Task<int> SumAsync(int n, int k) {
        try
        {
            int res = 0;
            for (int i = n; i <= k; i++)
            {
                res += await GetIntAsync(i);
            }
            return res;
        }
        catch { 
            return 0;
        }
    }

    [Monitor(InterceptionMode.ExecutionTime, LogDestination.Console)]
    private static int GetIntSync(int i)
    {
        return (new Random()).Next(0, 50) == 1 ? throw new Exception("testing sync exceptions") : i;
    }

    private static async Task<int> GetIntAsync(int i) {
        await Task.Delay(10);
        return (new Random()).Next(0, 50) == 1 ? throw new Exception("testing async exceptions") : i;
    }
}