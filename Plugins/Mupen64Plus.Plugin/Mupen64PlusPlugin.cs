using Launcher.Core.Emulation;
using Launcher.Infrastructure.Linux;

namespace Mupen64Plus.Plugin;

public sealed class Mupen64PlusPlugin : IEmulatorPlugin
{
    public EmulatorManifest Manifest { get; } = new()
    {
        Id = "mupen64plus",
        DisplayName = "Mupen64Plus",
        System = "Nintendo 64",
        Executable = "mupen64plus",
        SupportedExtensions = new[] { ".n64", ".z64", ".v64" }
    };

    public Task LaunchAsync(string romPath)
    {
        ProcessRunner.Run(
            Manifest.Executable,
            $"\"{romPath}\""
        );

        return Task.CompletedTask;
    }
}