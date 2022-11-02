using System;
using System.Diagnostics;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Serialization;
using Benchmarks.Extensions;
using Benchmarks.Models;
using System.Linq;
using System.Text.Json;

namespace Benchmarks.Interception;

[PSerializable]
[DebuggerStepThrough]
[AttributeUsage(AttributeTargets.Method)]
public class MarkedAttribute : OnGeneralMethodBoundaryAspect
{

    public MarkedAttribute()
    {
    }
    public override MetricsMetadata OnStarting(MethodInterceptionArgs args)
    {
        MetricsMetadata AttachedLog = args;
        AttachedLog.MethodQualifiedName = args.Method.Name;
        Console.WriteLine($"Running : {args.Method.Name} ");
        return AttachedLog;
    }
    public override void OnCompletion(MetricsMetadata logs)
    {
        Console.WriteLine($"Exiting : {logs.MethodQualifiedName} with Status: {logs.Status}");
    }
}