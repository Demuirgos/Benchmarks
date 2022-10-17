using System;
using System.Reflection;
using PostSharp.Aspects;
using System.Diagnostics;
using PostSharp.Serialization;
using PostSharp.Extensibility;
using System.ComponentModel;

[PSerializable]
public class OnGeneralMethodBoundaryAspect : OnMethodBoundaryAspect
{
    public virtual void OnCompletion(ExecutionArgs args) {}
    public virtual void OnSuccess(ExecutionArgs args) {}
    public virtual void OnFailure(ExecutionArgs args) {}
    private bool isAsyncMode = false;

    public sealed override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
    {
        var methodInfo = method as MethodInfo;
        if (methodInfo == null)
        {
            throw new Exception("MethodInfo is null");
        }

        isAsyncMode =    methodInfo.ReturnType == typeof(Task) 
                    ||  (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));

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
            var task = (dynamic)args.ReturnValue;
            args.ReturnValue = GetContinuation(args, task);
        }
    }

    private async  Task<TResult> GetContinuation<TResult>(MethodExecutionArgs args, Task<TResult> previousStateMachineTask)
        => await previousStateMachineTask.ContinueWith(
            t => RunTaskInContext(t, args),
            TaskContinuationOptions.ExecuteSynchronously);
            
    private async  Task GetContinuation(MethodExecutionArgs args, Task previousStateMachineTask)
        => await previousStateMachineTask.ContinueWith(
            t => RunTaskInContext(t, args),
            TaskContinuationOptions.ExecuteSynchronously);

    
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