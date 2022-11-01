using System.Diagnostics;

var stopwatch = new Stopwatch();
stopwatch.Start();
int res = 0;
for (int i = 0; i < 1000; i++)
    res += i;
Console.WriteLine($"Result : {res}");
Console.WriteLine($"Logs : took {stopwatch.ElapsedMilliseconds}ms");