using System.Diagnostics;
using System.Reflection;
public static class DotnetProcess
{
    private static  ProcessBuilder RunDotnetWithCommands(string args, string workingDir = null) {
        var process = ProcessBuilder.Instance
            .Create("dotnet")
            .WithArguments(args)
            .WithRedirectStandardOutput(false)
            .WithStandaloneWindow(true)
            .WithWorkingDirectory(workingDir);
        return process;
    } 
    [Marked]
    public static async Task Build(string path) {
        await RunDotnetWithCommands($"build {path} -c Release /p:DefineConstants=MONITOR")
                .Run();
    }
    [Marked]
    public static async Task Run(string path) {
        await RunDotnetWithCommands($"run --no-build {path}")
            .WithRedirectStandardOutput(true, 
                (line) => {
                    if(line.StartsWith("Logs :") || true) {
                        var projectPath = path.Replace(Path.GetTempPath(), "");
                        Console.WriteLine($"\t{projectPath} ::> {line}");
                    }
                }
            ).Run();
    }

    [Marked]
    public static async Task Launch(string path, string netVersion, string arguments = null) {
        var containingFolder = Path.GetDirectoryName(path);
        var fileName = Path.GetFileNameWithoutExtension(path);
        await ProcessBuilder.Instance.Create($"{containingFolder}\\bin\\Release\\{netVersion}\\{fileName}.exe")
            .WithArguments(arguments)
            .WithRedirectStandardOutput(true, 
                (line) => {
                    if(line.StartsWith("Logs :") || true) {
                        var projectPath = path.Replace(Path.GetTempPath(), "");
                        Console.WriteLine($"\t{projectPath} ::> {line}");
                    }
                }
            ).Run();
    }
}