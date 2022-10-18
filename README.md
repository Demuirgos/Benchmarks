# Benchmarks
a bunch of experiments and benchmarks

# Usage : 
```csharp
[Monitor(InterceptionMode.ExecutionTime, LogDestination.Prometheus)]
public static int SumSync(int n, int k)
{
    int res = 0;
    for(int i = n; i < k; i++){
        res += GetIntSync(i);
    }
    return res;
}
```
```csharp
[Monitor(InterceptionMode.ExecutionTime, LogDestination.Console, 30000)]
private static async Task<int> GetIntAsync(int i) {
    await Task.Delay(10);
    return i;
}
``` 
# Sample Output (testing) : 
```logs
10/18/2022 8:29:04 PM :>> Logs :
 MetricsMetadata { EmbeddedResource = System.Diagnostics.Stopwatch, MethodQualifiedName = GetIntSync, CallCount = 1, Status = Completed, ExecutionTime = 17913, StartTime = 8:29:04 PM, FinishTime = 8:29:04 PM }

10/18/2022 8:29:04 PM :>> Logs :
 MetricsMetadata { EmbeddedResource = System.Diagnostics.Stopwatch, MethodQualifiedName = GetIntSync, CallCount = 2, Status = Completed, ExecutionTime = 15, StartTime = 8:29:04 PM, FinishTime = 8:29:04 PM }

// Metrics.cs Contents
SumSyncExecutionTime := 86181
SumAsyncExecutionTime := 16061189
GetIntSyncExecutionTime := 0
```
