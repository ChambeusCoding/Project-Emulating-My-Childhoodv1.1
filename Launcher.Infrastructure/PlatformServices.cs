using System.Runtime.InteropServices;
using Launcher.Infrastructure.Abstractions;
using Launcher.Infrastructure.Windows;
using Launcher.Infrastructure.Linux;

namespace Launcher.Infrastructure
{
    public static class PlatformServices
    {
        public static IProcessRunner ProcessRunner =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WindowsProcessRunner()
                : new LinuxProcessRunner();

        public static IPathResolver PathResolver =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WindowsPathResolver()
                : new LinuxPathResolver();
    }
}