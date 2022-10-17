using System;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
static class Log {
    private static int counter = 0;
    public static void LogResult(long value) {
        counter++;
    }
}

class DestructorCounter {
    private Stopwatch stopwatch = new Stopwatch();
    public DestructorCounter() {
        stopwatch.Start();
    }
    ~DestructorCounter() {
		Log.LogResult(stopwatch.ElapsedTicks);
    }
}

class DeferedCounter : Defer<Stopwatch> { 
    private DeferedCounter(Stopwatch res, Action<Stopwatch> start, Action<Stopwatch> end) : base(res, start, end) {}
    public DeferedCounter() : base(new Stopwatch(), (resource) => resource.Start(), (resource) => Log.LogResult(resource.ElapsedTicks)) {}
} 

class DeferedCounter2 : Defer<Stopwatch> { 
    private DeferedCounter2(Stopwatch res, Action<Stopwatch> start, Action<Stopwatch> end) : base(res, start, end) {}
    public DeferedCounter2() : base(new Stopwatch(), (resource) => resource.Start(), (resource) => Log.LogResult(resource.ElapsedTicks)) {}
} 

class Defer<T> : IDisposable { 
    public T Resource;
    internal Action<T> defered {get; set;}
    internal Action<T> urgent  {get; set;}
    public Defer() {}
    public Defer(T res, Action<T> onStart, Action<T> onEnd) {
        Resource = res;
        (urgent, defered) = (onStart, onEnd);
        
        urgent?.Invoke(Resource);
    }
    public void Dispose() {
       defered?.Invoke(Resource);
    }
}

[MemoryDiagnoser]
public class MonitorCompare
{
    static Action work = () => {
        int i= 0; 
        while( i< 10000) {
            if(i % 2 == 0) {
                i--;
            } else {
                i+=2;
            }
        }
    };
    
    [Benchmark]
    public void ManualMethod() => ManualMethod(work);

    void ManualMethod(Action someWork) {
        var watch = new Stopwatch();
        watch.Start();
        someWork();
        Log.LogResult(watch.ElapsedTicks);
    }

    [Benchmark]
    public void TryFinallyMethod() => TryFinallyMethod(work);
    void TryFinallyMethod(Action someWork) {
        var watch = new Stopwatch();
        try {
            someWork();
        } finally {
            Log.LogResult(watch.ElapsedTicks);
        }
    }

    [Benchmark]
    public void NestedTryFinallyMethod() => NestedTryFinallyMethod(work);
    void NestedTryFinallyMethod(Action someWork) {
        try {
            var watch = new Stopwatch();
            try {
                someWork();
            } finally {
                Log.LogResult(watch.ElapsedTicks);
            }
        } catch {
            // do nothing
        }
    }

    [Benchmark]
    public void DeferMethod() => ManualMethod(work);
    void DeferMethod(Action someWork) {
        var watch = new Stopwatch();
        using(var manager = new Defer<Stopwatch>(watch, (watch) => watch.Start(), (watch) => Log.LogResult(watch.ElapsedTicks))){
            someWork();
        }
    }

    [Benchmark]
    public void UsingMethod() => ManualMethod(work);
    void UsingMethod(Action someWork) {
        using(var counter = new DeferedCounter()) {
            someWork();
        }
    }

    [Benchmark]
    public void Using2Method() => ManualMethod(work);
    void Using2Method(Action someWork) {
        using var counter = new DeferedCounter2();
        someWork();
    }
}

/*
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;


Action work = () => {
    int i= 0; 
    while( i< 10000) {
        if(i % 2 == 0) {
            i--;
        } else {
            i+=2;
        }
    }
};

RunNTime(nameof(ManualMethod), 10, ManualMethod, work);
RunNTime(nameof(DeferedCounter), 10, UsingMethod, work);
RunNTime(nameof(DeferedCounter2), 10, Using2Method, work);
RunNTime(nameof(DeferMethod), 10, DeferMethod, work);

void RunNTime(string key, int n, Action<Action> monitor, Action work) {
    while(n-- > 0) {
        monitor(work);
    }
    
    var logResults = Log.logs[key];
    Console.WriteLine($"{key} : {logResults.Average()}");
}


static void ManualMethod(Action someWork) {
    var watch = new Stopwatch();
    watch.Start();
    someWork();
    Log.logResult(nameof(ManualMethod) ,watch.ElapsedTicks);
}

void UsingMethod(Action someWork) {
    using(var counter = new DeferedCounter()) {
        someWork();
    }
}

void Using2Method(Action someWork) {
    using var counter = new DeferedCounter2();
    someWork();
}

void DeferMethod(Action someWork) {
    var watch = new Stopwatch();
    using(var manager = new Defer<Stopwatch>(watch, (watch) => watch.Start(), (watch) => Log.logResult(nameof(DeferMethod) ,watch.ElapsedTicks))){
        someWork();
    }
}

class DestructorCounter {
    private Stopwatch stopwatch = new Stopwatch();
    public DestructorCounter() {
        stopwatch.Start();
    }
    ~DestructorCounter() {
        Log.logResult(nameof(DestructorCounter) ,stopwatch.ElapsedTicks);
    }
}

class DeferedCounter : Defer<Stopwatch> { 
    private DeferedCounter(Stopwatch res, Action<Stopwatch> start, Action<Stopwatch> end) : base(res, start, end) {}
    public DeferedCounter() : base(new Stopwatch(), (resource) => resource.Start(), (resource) => Log.logResult(nameof(DeferedCounter), resource.ElapsedTicks)) {}
} 

class DeferedCounter2 : Defer<Stopwatch> { 
    private DeferedCounter2(Stopwatch res, Action<Stopwatch> start, Action<Stopwatch> end) : base(res, start, end) {}
    public DeferedCounter2() : base(new Stopwatch(), (resource) => resource.Start(), (resource) => Log.logResult(nameof(DeferedCounter2), resource.ElapsedTicks)) {}
} 

class Defer<T> : IDisposable { 
    public T Resource;
    internal Action<T> defered {get; set;}
    internal Action<T> urgent  {get; set;}
    public Defer() {}
    public Defer(T res, Action<T> onStart, Action<T> onEnd) {
        Resource = res;
        (urgent, defered) = (onStart, onEnd);
        
        urgent?.Invoke(Resource);
    }
    public void Dispose() {
       defered?.Invoke(Resource);
    }
}

static class Log {
    public static void logResult(string key, long value) {
        if(logs.ContainsKey(key)) {
            logs[key].Add(value);
            return;
        }
        logs[key] = new List<long>{ value };
    }
    public static Dictionary<string, List<long>> logs = new();
}
*/