using System;
namespace circularRefs;

public class circularRefTest
{
    public void Test() => Console.WriteLine(FirstRef.Prop);

}
public class FirstRef {
    public static int Prop => SecondRef.Prop; 
}

public class SecondRef {
    public static int Prop => FirstRef.Prop; 
}
