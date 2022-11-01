using System.Diagnostics;
using static FileProcess;
using static DotnetProcess;
using static GitProcess;
public static class BenchProcess
{
    public static async Task Process(string SlnPath, string csProjPath, string repoSource, string currentCommit, string targetCommit) {
        var (clonePath, currentPath, targetPath) = FileProcess.CreateTestContainer(repoSource, currentCommit, targetCommit);
        Console.WriteLine("Cloning Repo...");
        Clone(repoSource, clonePath);         

        Task CurrentPathThread = Task.Run(() => {
            Clone(clonePath, currentPath);         
            SwitchTo(currentPath, currentCommit);
            Build($"{TempFolder}\\{currentCommit}\\{SlnPath}");
            Run($"{TempFolder}\\{currentCommit}\\{csProjPath}");
        });

        Task TargetPathThread = Task.Run(() => {
            Clone(clonePath, targetPath);
            SwitchTo(targetPath, targetCommit);
            Build($"{TempFolder}\\{targetCommit}\\{SlnPath}");
            Run($"{TempFolder}\\{targetCommit}\\{csProjPath}");
        });

        await Task.WhenAll(CurrentPathThread, TargetPathThread);

        Console.WriteLine("Cleaning up...");
        //FileProcess.CleanUp();


        

    }
}