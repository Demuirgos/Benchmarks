// get commandLineArgs from the command line
// example : 
// dotnet run "ConsoleApp1.sln" "ConsoleApp1\ConsoleApp1.csproj" "D:\Projects\benchmarks\Benchmark.Target" "7b5f9e3748b35c25a00560f2cc8665018c689b41" "a7f58cc97c0052da29b3cd78c21797e5c1aa0145"
var commandLineArgs = Environment.GetCommandLineArgs();
var (slnPath, csprojPath, repoSource, currentCommit, targetCommit)  = (commandLineArgs[1], commandLineArgs[2], commandLineArgs[3], commandLineArgs[4], commandLineArgs[5]);
await BenchProcess.Process(slnPath, csprojPath, repoSource, currentCommit, targetCommit);

