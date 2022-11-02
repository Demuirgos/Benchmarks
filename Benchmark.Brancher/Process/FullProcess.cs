using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Concurrent;
using static FileProcess;
using static DotnetProcess;
using static GitProcess;

public static class BenchProcess
{
    public static Options Options { get; set; }
    public static ConcurrentDictionary<string, List<string>> Logs = new ConcurrentDictionary<string, List<string>>();
    public static string TempFolder => $"{Path.GetTempPath()}TEMP4BENCHMARK"; 
    [Marked]
    public static async Task Process(Options argOptions) {
        Options = argOptions;
        var (clonePath, currentPath, targetPath) = FileProcess.CreateTestContainer(argOptions.RepositorySource, argOptions.CurrentCommit, argOptions.TargetCommit);
        await Clone(argOptions.RepositorySource, clonePath);         

        Func<string, Task> ProcessFactory = (string path) => Task.Run(async () => {
            await Clone(clonePath, path);         
            await SwitchTo(currentPath, path);
            await Build($"{path}\\{argOptions.SolutionPath}");
            await Launch($"{path}\\{argOptions.ProjectPath}", "net6.0", 
                callback: (line) => {
                    if(line.TrimStart().StartsWith("Logs : took ")) {
                        string newValue = line.Substring(12).Replace("ms", "");
                        Logs.AddOrUpdate(path, new List<string>() { newValue }, (key, value) => {
                            lock (value)
                            {
                                value.Add(newValue);
                            }
                            return value;
                        });
                    }
                });
        });

        await Task.WhenAll(ProcessFactory(currentPath), ProcessFactory(targetPath));

        PrintLogs();
        FileProcess.CleanUp();
    }

    [Marked]
    public static void PrintLogs() {
        foreach(var log in Logs) {
            // get folder name
            var folderName = log.Key.Substring(log.Key.LastIndexOf("\\") + 1);
            Console.WriteLine($"\tCommit {folderName}(isCurrent = {Options.CurrentCommit == folderName}) : took on average {log.Value.Select(Int32.Parse).Average()}ms");
        }
    }
}