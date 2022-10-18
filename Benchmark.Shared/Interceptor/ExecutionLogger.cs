using System;
using System.Diagnostics;
using System.Reflection;

using PostSharp.Aspects;
using PostSharp.Serialization;
using PostSharp.Extensibility;

using Benchmarks.Extensions;
using Benchmarks.Models;
using System.Collections.Generic;
using System.Linq;

namespace Benchmarks.Interception;

[Serializable]
public static class MetricsMetadata {
    public static Dictionary<string, long> CallCountKeeper = new();
}
public record MetricsMetadata<TResource> {
    public TResource EmbeddedResource { get; set; }
    public string MethodQualifiedName { get; set; }
    public long CallCount => MetricsMetadata.CallCountKeeper[MethodQualifiedName];
    public MethodStatus Status { get; set; }
    public long ExecutionTime { get; set; }
    public string StartTime  { get; set; }
    public string FinishTime { get; set; }
}

[PSerializable]
[DebuggerStepThrough]
[AttributeUsage(AttributeTargets.Method)]
public class MonitorAttribute : OnGeneralMethodBoundaryAspect {
    [PNonSerialized]
    private (Assembly Assembly, string Function) CallSourceType;
    private InterceptionMode InterceptionMode { get; set; } 
    private LogDestination LogDestination { get; set; }
    private int PeriodBetweenLogsWait { get; set; }

    private DateTime? previousLogTime;
    private bool ShouldLog(int periodTicks)
    {
        if (previousLogTime is null)
        {
            previousLogTime = DateTime.UtcNow;
            return true;
        }

        var now = DateTime.UtcNow;
        if (DateTime.UtcNow.Ticks - previousLogTime.Value.Ticks > periodTicks)
        {
            previousLogTime = now;
            return true;
        }

        return false;
    }

    private bool IsInitialized = false;
    private string PropertyName => $"{CallSourceType.Function}{InterceptionMode}";
    public MonitorAttribute(InterceptionMode interceptionMode, LogDestination logDestination, int TicksWaitInBetweenLogs = 100) 
    {
        InterceptionMode = interceptionMode; LogDestination = logDestination; PeriodBetweenLogsWait = TicksWaitInBetweenLogs; 

    }
    public override void OnEntry(MethodExecutionArgs args) {
        if(!IsInitialized)
        {
            var method = args.Method;
            CallSourceType = (method.Module.Assembly, method.Name);
            if (!MetricsMetadata.CallCountKeeper.ContainsKey(CallSourceType.Function))
            {
                MetricsMetadata.CallCountKeeper[CallSourceType.Function] = 0;
            }
            IsInitialized = true;
        }

        var AttachedLog = new MetricsMetadata<Stopwatch> {
            EmbeddedResource = new Stopwatch(),
            MethodQualifiedName = (args.Method as MethodInfo).Name,
            StartTime = DateTime.Now.ToLongTimeString(),
            Status = MethodStatus.OnGoing,
            ExecutionTime = 0,
        };
        MetricsMetadata.CallCountKeeper[CallSourceType.Function]++;
        args.MethodExecutionTag = AttachedLog;
        AttachedLog.EmbeddedResource.Start();
    }

    public override void OnCompletion(ExecutionArgs args) {
        var innerLogs = args.MethodExecutionTag as MetricsMetadata<Stopwatch>;
        innerLogs.EmbeddedResource.Stop();

        if (!ShouldLog(PeriodBetweenLogsWait))
        {
            return;
        }

        innerLogs.FinishTime = DateTime.Now.ToLongTimeString();
        innerLogs.ExecutionTime = innerLogs.EmbeddedResource.ElapsedTicks;
        innerLogs.Status = MethodStatus.Completed;
        var message = $"{DateTime.Now} :>> Logs : \n {innerLogs} \n";
        switch (LogDestination) {
            case LogDestination.Console:
                Console.WriteLine(message);
                break;
            case LogDestination.Debug:
                Debug.WriteLine(message);
                break;
            case LogDestination.Prometheus:
                var prometheusSinkType = CallSourceType.Assembly.GetTypes().Where(t => t.FullName == "Plugin.Metrics").Single();
                prometheusSinkType?
                    .GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Static)
                    .SetValue(null, InterceptionMode switch
                    {
                        InterceptionMode.ExecutionTime => innerLogs.ExecutionTime,
                        InterceptionMode.CallCount => innerLogs.CallCount,
                        InterceptionMode.MetadataLog => throw new NotImplementedException()
                    });
                break;
            default:
                break;
        }
    }
}