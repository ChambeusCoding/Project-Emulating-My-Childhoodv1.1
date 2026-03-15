using System.Diagnostics;
using System.Threading.Tasks;
using Launcher.Infrastructure.Abstractions;

namespace Launcher.Infrastructure.Windows
{
    public sealed class WindowsProcessRunner : IProcessRunner
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
                UseShellExecute = true,          // you had this before
                CreateNoWindow = false,          // you had this before
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