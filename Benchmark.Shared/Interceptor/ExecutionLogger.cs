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
using System.Text;

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
    public  long ExecutionTime { get; set; }
    public DateTime StartTime  { get; set; }
    public DateTime FinishTime { get; set; }
    public string ExceptionValue { get; set; }

    public string ToString(InterceptionMode mode, TimeUnit unit = TimeUnit.Milliseconds)
    {
        var sb = new StringBuilder();
        sb.Append($"MethodName = {MethodQualifiedName}, ");

        if (Status.HasFlag(MethodStatus.Completed))
        {
            var timeUnit = unit switch
            {
                TimeUnit.Milliseconds => "ms",
                TimeUnit.Seconds => "s",
                _ => String.Empty
            }; 

            sb = mode switch
            {
                InterceptionMode.CallCount      => sb.Append($"CallCount = {CallCount}, "),
                InterceptionMode.ExecutionTime  => sb.Append($"ExecutionTime = {ExecutionTime}{timeUnit}, "),
                InterceptionMode.MetadataLog    => sb.Append($"CallCount = {CallCount}, ExecutionTime = {ExecutionTime}{timeUnit}, ")
                                                     .Append($"PeriodTime = from {StartTime.ToString("hh:mm:ss.fff tt")} to {FinishTime.ToString("hh:mm:ss.fff tt")}, "),
            };

        }

        sb.Append("Status = "); var foundFlag = false;
        foreach(var statusValue in Enum.GetValues<MethodStatus>())
        {
            if(Status.HasFlag(statusValue))
            {
                sb.Append($"{( foundFlag ? " | " : "" )} { statusValue }");
                foundFlag = true;
            }
        }

        if(Status.HasFlag(MethodStatus.Failed) && !String.IsNullOrEmpty(ExceptionValue))
        {
            sb.Append(", ");
            sb.Append($"Exception = {ExceptionValue}");
        }
        return sb.ToString();
    }
}

[PSerializable]
[DebuggerStepThrough]
[AttributeUsage(AttributeTargets.Method)]
public class MonitorAttribute : OnGeneralMethodBoundaryAspect {

    [PNonSerialized] private (Assembly Assembly, string Function) CallSourceType;
    
    private InterceptionMode _interceptionMode { get; set; } 
    private LogDestination _logDestination { get; set; }
    private TimeUnit _timeUnit { get; set; }
    private int _periodBetweenLogsWait { get; set; }
    private string _logFilePath { get; set; }

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
    private string PropertyName => $"{CallSourceType.Function}{_interceptionMode}";
    public MonitorAttribute(InterceptionMode InterceptionMode, LogDestination LogDestination, String FilePath = null, TimeUnit TimeUnit = TimeUnit.Milliseconds ,int WaitInBetweenLogs = 100) 
    {
        _interceptionMode = InterceptionMode; 
        _logDestination = LogDestination; 
        _periodBetweenLogsWait = WaitInBetweenLogs; 
        _timeUnit = TimeUnit;
        if(LogDestination == LogDestination.File)
        {
            _logFilePath = FilePath;
        }
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
            StartTime = DateTime.Now,
            Status = MethodStatus.OnGoing,
            ExecutionTime = 0,
        };
        MetricsMetadata.CallCountKeeper[CallSourceType.Function]++;
        args.MethodExecutionTag = AttachedLog;
        AttachedLog.EmbeddedResource.Start();
    }

    public override void OnFailure(ExecutionArgs args)
    {
        var innerLogs = args.MethodExecutionTag as MetricsMetadata<Stopwatch>;
        innerLogs.Status = MethodStatus.Failed | MethodStatus.Aborted;
        innerLogs.ExceptionValue = TryExtractExceptionMessage(args);
        var message = $"Error :: {DateTime.Now} :>> Logs : \n {innerLogs.ToString(_interceptionMode)} \n";
        SendToLogDestination(innerLogs, message);
    }

    public override void OnCompletion(ExecutionArgs args)
    {
        var innerLogs = args.MethodExecutionTag as MetricsMetadata<Stopwatch>;
        innerLogs.EmbeddedResource.Stop();

        if (!ShouldLog(_periodBetweenLogsWait))
        {
            return;
        }

        innerLogs.FinishTime = DateTime.Now;
        innerLogs.ExecutionTime = _timeUnit switch
        {
            TimeUnit.Seconds => innerLogs.EmbeddedResource.Elapsed.Seconds,
            TimeUnit.Milliseconds => innerLogs.EmbeddedResource.Elapsed.Milliseconds,
            TimeUnit.Ticks => innerLogs.EmbeddedResource.Elapsed.Ticks,
        };
        innerLogs.Status = MethodStatus.Completed | (args.Exception is null ? MethodStatus.Succeeded : MethodStatus.Failed);
        innerLogs.ExceptionValue = TryExtractExceptionMessage(args);
        var message = $"Logs :: {DateTime.Now} :>> Logs : \n {innerLogs.ToString(_interceptionMode, _timeUnit)} \n";
        SendToLogDestination(innerLogs, message);
    }
    private string TryExtractExceptionMessage(ExecutionArgs args)
    {
        return args.Exception is not null ? $"{args.Exception?.GetType().Name} ({args.Exception?.HelpLink}) : from '{(IsAsyncMode ? CallSourceType.Function : args.Exception?.TargetSite)}' with message '{args.Exception?.Message}' at '{args.Exception?.Source}'" : String.Empty;
    }
    private void SendToLogDestination(MetricsMetadata<Stopwatch> innerLogs, string message)
    {

        switch (_logDestination)
        {
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
                    .SetValue(null, _interceptionMode switch
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