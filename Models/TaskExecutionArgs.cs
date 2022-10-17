using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using System.Reflection;

public enum TaskFlowBehavior 
    {
        Default,
        Continue,
        RethrowException,
        Return,
        ThrowException,
    }

public sealed class ExecutionArgs : MethodExecutionArgs
    {
        public ExecutionArgs(MethodExecutionArgs args)
            : base(args.Instance, args.Arguments)
        {
            Method = args.Method;
            Arguments = args.Arguments;
            MethodExecutionTag = args.MethodExecutionTag;
            Exception = args.Exception;
            TaskFlowBehavior = TaskFlowBehavior.Default;
        }

        public ExecutionArgs(Task previousStateMachineTask, MethodExecutionArgs args)
            : base(args.Instance, args.Arguments)
        {
            Method = args.Method;
            Arguments = args.Arguments;
            MethodExecutionTag = args.MethodExecutionTag;
            Exception = previousStateMachineTask.Exception ?? args.Exception;
            TaskFlowBehavior = TaskFlowBehavior.Default;
            IsAsyncMode = previousStateMachineTask is not null;
        }

        public bool IsAsyncMode { get; set; } = false;
        public TaskFlowBehavior TaskFlowBehavior { get; set; }
    }