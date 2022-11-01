using System.Diagnostics;
using static FileProcess;
using static DotnetProcess;
using static GitProcess;
public static class BenchProcess
{
    public static string TempFolder => $"{Path.GetTempPath()}TEMP4BENCHMARK"; 
    [Marked]
    public static async Task Process(string SlnPath, string csProjPath, string repoSource, string currentCommit, string targetCommit) {
        var (clonePath, currentPath, targetPath) = FileProcess.CreateTestContainer(repoSource, currentCommit, targetCommit);
        await Clone(repoSource, clonePath);         

        Func<string, Task> ProcessFactory = (path) => Task.Run(async () => {
            await Clone(clonePath, path);         
            await SwitchTo(currentPath, path);
            await Build($"{path}\\{SlnPath}");
            await Launch($"{path}\\{csProjPath}", "net6.0");
        });

        await Task.WhenAll(ProcessFactory(currentPath), ProcessFactory(targetPath));

        Console.WriteLine("Cleaning up...");
        //FileProcess.CleanUp();
    }
}