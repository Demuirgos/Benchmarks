using System.Diagnostics;

int i = 1000;
while (i-- > 0)
{
	NewMethod();
}

static void NewMethod()
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    int res = 1000 * 1001 / 2;
    Console.WriteLine($"Result : {res}");
    Console.WriteLine($"Logs : took {stopwatch.ElapsedMilliseconds}ms");
}