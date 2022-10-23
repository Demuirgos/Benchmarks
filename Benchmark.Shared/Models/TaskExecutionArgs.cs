using System;
using System.Collections.Generic;
using System.Text;
using PostSharp.Aspects;

namespace Benchmarks.Models;
[Flags] public enum TimeUnit { Temporal, Ticks }
[Flags] public enum LogDestination { Debug, Console, Prometheus, File, None }
[Flags] public enum InterceptionMode { ExecutionTime, CallCount, Failures, MetadataLog, None }
[Flags] public enum MethodStatus { Failed = 1, Succeeded = 2, Completed = 4, Aborted = 8, OnGoing = 16, Halted = 32 }

[Serializable]
public static class MetricsMetadataExtensions {
    public static Dictionary<string, long> CallCountKeeper = new();
    public static Dictionary<string, long> ExceptionCountKeeper = new();
}
public class MetricsMetadata {
    public Object EmbeddedResource { get; set; }
    public string MethodQualifiedName { get; set; }
    public long CallCount => MetricsMetadataExtensions.CallCountKeeper[MethodQualifiedName];
    public long Failures => MetricsMetadataExtensions.ExceptionCountKeeper[MethodQualifiedName];
    public MethodStatus Status { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public DateTime StartTime  { get; set; }
    public DateTime FinishTime { get; set; }
    public Exception Exception { get; set; }

    public static implicit operator MetricsMetadata(MethodInterceptionArgs args) => new() {
        MethodQualifiedName = args.Method.Name,
    };

    public string ToString(InterceptionMode mode, TimeUnit unit = TimeUnit.Temporal)
    {
        var sb = new StringBuilder();
        sb.Append($"MethodName = {MethodQualifiedName}, ");

        if (Status.HasFlag(MethodStatus.Completed))
        {
            string executionTime = unit switch
            {
                TimeUnit.Temporal => $"{ExecutionTime}ms",
                TimeUnit.Ticks => ExecutionTime.Ticks.ToString(),
            };

            sb = mode switch
            {
                InterceptionMode.CallCount      => sb.Append($"CallCount = {CallCount}, "),
                InterceptionMode.Failures       => sb.Append($"Failures = {Failures}, "),
                InterceptionMode.ExecutionTime  => sb.Append($"ExecutionTime = {executionTime}, "),
                InterceptionMode.MetadataLog    => sb.Append($"CallCount = {CallCount}, ExecutionTime = {executionTime}, ")
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


        if(Status.HasFlag(MethodStatus.Failed) && Exception is not null )
        {
            var ExceptionValue = Exception is not null ? $"{Exception?.GetType().Name} ({Exception?.HelpLink}) : from '{Exception?.TargetSite}' with message '{Exception?.Message}' at '{Exception?.Source}'" : String.Empty;
            sb.Append(", ");
            sb.Append($"Exception = {ExceptionValue}");
        }
        return sb.ToString();
    }
}