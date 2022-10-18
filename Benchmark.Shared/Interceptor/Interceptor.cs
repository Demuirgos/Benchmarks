using System;
using System.Threading.Tasks;
using System.Reflection;
using PostSharp.Aspects;
using System.Diagnostics;
using PostSharp.Serialization;
using PostSharp.Extensibility;
using System.ComponentModel;

using Benchmarks.Models;

namespace Benchmarks.Extensions;


[PSerializable]
public class OnGeneralMethodBoundaryAspect : OnMethodBoundaryAspect
{
    public virtual void OnEmition(MethodBase args) {}
    public virtual void OnCompletion(ExecutionArgs args) {}
    public virtual void OnSuccess(ExecutionArgs args) {}
    public virtual void OnFailure(ExecutionArgs args) {}
    private bool isAsyncMode = false;

    public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
    {
        var methodInfo = method as MethodInfo;
        if (methodInfo == null)
        {
            throw new Exception("MethodInfo is null");
        }

        isAsyncMode =    methodInfo.ReturnType == typeof(Task) 
                    ||  (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
        OnEmition(method);
        base.CompileTimeInitialize(method, aspectInfo);
    }

    public sealed override void OnSuccess(MethodExecutionArgs args)
        => OnSuccess(new ExecutionArgs(args));
    public sealed override void OnException(MethodExecutionArgs args)
        => OnFailure(new ExecutionArgs(args));
    
    public sealed override void OnExit(MethodExecutionArgs args)
    {
        bool isEndOfStateMachine = isAsyncMode && !typeof(Task).IsAssignableFrom(args.ReturnValue.GetType()) || !isAsyncMode;
        if(isEndOfStateMachine) {
            OnCompletion(new ExecutionArgs(args));
        } else {
            Console.WriteLine();
            var task = (Task)args.ReturnValue;
            args.ReturnValue = GetContinuation(args, task);
        }
    }

    private async  Task<TResult> GetContinuation<TResult>(MethodExecutionArgs args, Task<TResult> previousStateMachineTask)
    {
        Console.WriteLine($"GetContinuation<{typeof(TResult).Name}>");
        return await previousStateMachineTask.ContinueWith(
            t => RunTaskInContext(t, args),
            TaskContinuationOptions.ExecuteSynchronously);
    }
            
    private async  Task GetContinuation(MethodExecutionArgs args, Task previousStateMachineTask)
    {
        Console.WriteLine($"GetContinuation<>");
        await previousStateMachineTask.ContinueWith(
            t => RunTaskInContext(t, args),
            TaskContinuationOptions.ExecuteSynchronously);
    }

    
    private TResult? RunTaskInContext<TResult>(Task<TResult> previousStateMachineTask, MethodExecutionArgs args)
    {
        var taskArgs = new ExecutionArgs(previousStateMachineTask, args);

        if (    previousStateMachineTask.IsCompleted 
            && !previousStateMachineTask.IsFaulted)
        {
            taskArgs.ReturnValue = previousStateMachineTask.Result;
        }

        HandleTaskExecution(previousStateMachineTask, taskArgs);

        return (TResult)taskArgs.ReturnValue;
    }

    private void RunTaskInContext(Task previousStateMachineTask, MethodExecutionArgs args)
    {
        var taskArgs = new ExecutionArgs(previousStateMachineTask, args);
        HandleTaskExecution(previousStateMachineTask, taskArgs);
    }

    private void HandleTaskExecution(Task previousStateMachineTask, ExecutionArgs taskArgs)
    {
        try
        {
            if (previousStateMachineTask.IsFaulted)
            {
                taskArgs.TaskFlowBehavior = TaskFlowBehavior.Default;
                OnFailure(taskArgs);

                switch (taskArgs.TaskFlowBehavior)
                {
                    case TaskFlowBehavior.ThrowException:
                        // Throw given exception in preserving stack trace.
                        throw new AggregateException(taskArgs.Exception);
                    case TaskFlowBehavior.Default:
                    case TaskFlowBehavior.RethrowException:
                        // Rethrow exception.
                        previousStateMachineTask.Wait();
                        break;
                    case TaskFlowBehavior.Continue:
                    case TaskFlowBehavior.Return:
                        // Swallow exception and continue with execution.
                        break;
                }
            }
            else if (previousStateMachineTask.IsCompleted)
            {
                taskArgs.TaskFlowBehavior = TaskFlowBehavior.Default;
                OnSuccess(taskArgs);
            }
        }
        finally
        {
            taskArgs.TaskFlowBehavior = TaskFlowBehavior.Default;
            OnCompletion(taskArgs);
        }
    }

}