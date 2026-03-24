using System.Diagnostics;
using System.Threading.Tasks;
using Launcher.Infrastructure.Abstractions;

namespace Launcher.Infrastructure.Linux
{
    public sealed class LinuxProcessRunner : IProcessRunner
    {
        public async Task<int> RunAsync(
            string executable,
            string arguments,
            string? workingDirectory = null)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = arguments,
                UseShellExecute = false,
                WorkingDirectory = workingDirectory ?? string.Empty
            };

            using var process = Process.Start(startInfo);

            if (process == null)
                return -1;

            await process.WaitForExitAsync();
            return process.ExitCode;
        }
    }
}