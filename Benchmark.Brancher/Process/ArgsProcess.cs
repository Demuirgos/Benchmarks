using System;
using System.Linq;

public class Options {
    public string SolutionPath { get; set; }
    public string ProjectPath { get; set; }
    public string RepositorySource { get; set; }
    public string CurrentCommit { get; set; }
    public string TargetCommit { get; set; }
}

public class ArgsProcess {
    public static Options Process(string[] args) {
        var options = new Options();
        var concatenatedArgs = string.Join(" ", args.Skip(1));
        var tokens = concatenatedArgs.Split(' ', '=');
        for (int i = 0; i < tokens.Length;)
        {
            var argName = tokens[i++];
            if(!argName.StartsWith("--"))
                throw new Exception($"Argument name must start with -- : {argName}");
            argName = argName.Substring(2);

            var argValue = tokens[i++];

            switch (argName)
            {
                case "solution":
                    options.SolutionPath = argValue;
                    break;
                case "project":
                    options.ProjectPath = argValue;
                    break;
                case "repo":
                    options.RepositorySource = argValue;
                    break;
                case "current":
                    options.CurrentCommit = argValue;
                    break;
                case "target":
                    options.TargetCommit = argValue;
                    break;
                default:
                    throw new Exception($"Unknown argument name : {argName}");
            }
        }
        return options;
    }
}