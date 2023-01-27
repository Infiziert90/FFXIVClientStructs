using System.Diagnostics;
using System.Text;
using FFXIVClientStructs.InteropGenerator;
using Microsoft.CodeAnalysis;

namespace FFXIVClientStructs.InteropSourceGenerators; 

[Generator]
public class VersionGenerator : ISourceGenerator {
    private uint version;
    
    private string GitCommand(string command) {
        var gitProcess = new Process() {
            StartInfo = new ProcessStartInfo() {
                CreateNoWindow = true, 
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = "git",
                Arguments = command
            }
        };

        var output = new StringBuilder();
        
        gitProcess.OutputDataReceived += (_, e) => {
            output.Append(e.Data);
        };
        
        gitProcess.Start();
        gitProcess.BeginOutputReadLine();
        gitProcess.WaitForExit();
        gitProcess.CancelOutputRead();
        return output.ToString();
    }
    
    public void Initialize(GeneratorInitializationContext context) {
        var hash = GitCommand("show -s --format=%H");
        var count = GitCommand($"rev-list --count {hash}");
        if (!uint.TryParse(count, out version)) version = 0;
    }

    public void Execute(GeneratorExecutionContext context) {
        var builder = new IndentedStringBuilder();
        builder.AppendLine("using System.Reflection;");
        builder.AppendLine($"[assembly: AssemblyVersion(\"1.0.0.{version}\")]");
        builder.AppendLine($"[assembly: AssemblyFileVersion(\"1.0.0.{version}\")]");
        builder.AppendLine($"[assembly: AssemblyInformationalVersion(\"1.0.0.{version}\")]");

        builder.AppendLine("namespace FFXIVClientStructs.Interop;");
        builder.AppendLine("public partial class Resolver {");
        using (builder.Indent()) {
            builder.AppendLine($"public const uint Version = {version};");
        }

        builder.AppendLine("}");
        context.AddSource("FFXIVClientStructs.Interop.Resolver.Version.g.cs", builder.ToString());
    }
}
