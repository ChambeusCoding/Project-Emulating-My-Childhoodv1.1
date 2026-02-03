using System.Diagnostics;
using System.Threading.Tasks;

namespace Launcher.Infrastructure.Linux;

public static class ProcessRunner
{
    public static Task RunAsync(string file, string args = "")
    {
        var psi = new ProcessStartInfo
        {
            FileName = file,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = Process.Start(psi)!;
        return process.WaitForExitAsync();
    }
}