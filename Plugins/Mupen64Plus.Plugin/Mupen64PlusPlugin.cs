using System;
using System.Threading.Tasks;
using Launcher.Core.Emulation;
using Launcher.Infrastructure.Linux;

namespace Mupen64Plus.Plugin;

public sealed class Mupen64PlusPlugin : IEmulatorPlugin
{
    public EmulatorManifest Manifest { get; } = new EmulatorManifest
    {
        Id = "mupen64plus",
        DisplayName = "Mupen64Plus",
        System = "Nintendo 64",
        Executable = "/usr/games/mupen64plus",
        SupportedExtensions = new[]
        {
            ".n64",
            ".z64",
            ".v64"
        }
    };

    public Mupen64PlusPlugin()
    {
        // 🔥 CONFIRM PLUGIN LOAD
        Console.WriteLine("[PLUGIN] Mupen64PlusPlugin constructed");
        Console.WriteLine($"[PLUGIN] System: {Manifest.System}");
        Console.WriteLine($"[PLUGIN] Extensions: {string.Join(", ", Manifest.SupportedExtensions)}");
        Console.WriteLine($"[PLUGIN] Executable: {Manifest.Executable}");
    }

    public Task LaunchAsync(string romPath)
    {
        Console.WriteLine($"[PLUGIN] Launch requested");
        Console.WriteLine($"[PLUGIN] ROM path: {romPath}");

        ProcessRunner.Run(
            Manifest.Executable,
            $"\"{romPath}\""
        );

        Console.WriteLine("[PLUGIN] mupen64plus process started");

        return Task.CompletedTask;
    }
}