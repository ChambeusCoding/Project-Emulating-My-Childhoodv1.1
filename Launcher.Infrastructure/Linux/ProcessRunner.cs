using System.Diagnostics;

namespace Launcher.Infrastructure.Linux;

public static class ProcessRunner
{
    public static void Run(string executable, string args)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = executable,
            Arguments = args,
            UseShellExecute = false
        });
    }
}