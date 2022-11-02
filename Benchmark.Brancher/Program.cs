// dotnet run --repo="D:\Projects\benchmarks\Benchmark.Target" --solution="ConsoleApp1.sln" --project="ConsoleApp1\ConsoleApp1.csproj"  --target="e43e4304c06d1b0457c83da5dd8c591ff1116ca2" --current="d5334deb26802658ca8d22696546d05f3d252703"
await BenchProcess.Process(ArgsProcess.Process(Environment.GetCommandLineArgs()));

