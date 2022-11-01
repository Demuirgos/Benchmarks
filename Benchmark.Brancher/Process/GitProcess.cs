using System.Diagnostics;
public static class GitProcess
{
    private static ProcessBuilder RunGitWithCommands(string args, string workingDir = null) {
        var process = ProcessBuilder.Instance
            .Create("git")
            .WithArguments(args)
            .WithRedirectStandardOutput(
                LogsHandle : Console.WriteLine
            )
            .WithStandaloneWindow(false)
            .WithWorkingDirectory(workingDir);
        return process;
    } 

    [Marked]
    public static async Task SwitchTo(string path, string commitHash) {
        await RunGitWithCommands($"checkout {commitHash}", path).Run();
    }

    [Marked]
    public static async Task Clone(string repoSource, string folderPath) {
        await RunGitWithCommands($"clone {repoSource} {folderPath}").Run();
    }
}