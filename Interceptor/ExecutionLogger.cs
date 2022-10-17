using System;
using System.Diagnostics;
using System.Reflection;

using PostSharp.Aspects;
using PostSharp.Serialization;
using PostSharp.Extensibility;

[PSerializable]
[DebuggerStepThrough]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
public class MonitorExecutionTimeAttribute : OnGeneralMethodBoundaryAspect {
    public override void OnEntry(MethodExecutionArgs args) {
        args.MethodExecutionTag = Stopwatch.StartNew();
    }

    public override void OnCompletion(ExecutionArgs args) {
        var stopwatch = (Stopwatch)args.MethodExecutionTag;
        stopwatch.Stop();
        var executionTime = stopwatch.ElapsedMilliseconds;
        var message = string.Format("{0}.{1} executed in {2} ms", args.Method.DeclaringType.Name, args.Method.Name, executionTime);
        Console.WriteLine(message);
    }
}