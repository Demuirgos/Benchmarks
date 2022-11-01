using System.Diagnostics;
public static class GitProcess
{
    private static  void RunGitWithCommands(string args) {
        var process = new Process();
        process.StartInfo.FileName = "git";
        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();
        process.WaitForExit();
        string line;        
        while ((line = process.StandardOutput.ReadLine()) != null)
        {
            Console.WriteLine(line);
        }
        process.Close();
    } 

    public static void SwitchTo(string path, string commitHash) {
        RunGitWithCommands($"--git-dir={path}/.git --work-tree={path} checkout {commitHash}");
    }

    public static void Clone(string repoSource, string folderPath) {
        RunGitWithCommands($"clone {repoSource} {folderPath}");
    }
}