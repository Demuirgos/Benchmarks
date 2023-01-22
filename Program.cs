using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// BenchmarkRunner.Run<MyBenchmark>();

var benchy = new MyBenchmark();
benchy.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
    .Where(meth => meth.CustomAttributes.Any(attr => attr.AttributeType == typeof(BenchmarkAttribute)))
    //.Where(meth => meth.Name.Contains("No_Bit_Manip"))
    .ToList()
    .ForEach(method => {
        Console.Write($"{method.Name} : ");
        Console.WriteLine(method.Invoke(benchy, null));
    });
