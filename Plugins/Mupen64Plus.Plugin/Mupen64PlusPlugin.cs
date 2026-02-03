using System;
using System.IO;
using System.Threading.Tasks;
using Launcher.Core.Emulation;
using Launcher.Infrastructure;

namespace Mupen64Plus.Plugin
{
    public sealed class Mupen64PlusPlugin : IEmulatorPlugin
    {
        public EmulatorManifest Manifest { get; }

        public Mupen64PlusPlugin()
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string defaultExecutable = Path.Combine(
                homeDir,
                ".local",
                "share",
                "emulators",
                "mupen64plus",
                "mupen64plus"
            );

            string executablePath = File.Exists(defaultExecutable)
                ? defaultExecutable
                : "mupen64plus";

            Manifest = new EmulatorManifest
            {
                Id = "mupen64plus",
                DisplayName = "Mupen64Plus",
                System = "Nintendo 64",
                Executable = executablePath,
                SupportedExtensions = new[]
                {
                    ".n64",
                    ".z64",
                    ".v64"
                }
            };

            Console.WriteLine("[PLUGIN] Mupen64PlusPlugin constructed");
            Console.WriteLine($"[PLUGIN] System: {Manifest.System}");
            Console.WriteLine($"[PLUGIN] Extensions: {string.Join(", ", Manifest.SupportedExtensions)}");
            Console.WriteLine($"[PLUGIN] Executable (raw): {Manifest.Executable}");
        }

        public async Task LaunchAsync(string romPath)
        {
            if (!File.Exists(romPath))
            {
                Console.WriteLine($"[PLUGIN] ROM not found: {romPath}");
                return;
            }

            Console.WriteLine($"[PLUGIN] Launching ROM: {romPath}");

            try
            {
                // Resolve executable per-OS (Linux PATH / Windows .exe)
                string resolvedExecutable =
                    PlatformServices.PathResolver.ResolveExecutable(Manifest.Executable);

                Console.WriteLine($"[PLUGIN] Resolved executable: {resolvedExecutable}");
                Console.WriteLine($"[PLUGIN] Arguments: \"{romPath}\"");

                int exitCode = await PlatformServices.ProcessRunner.RunAsync(
                    resolvedExecutable,
                    $"\"{romPath}\""
                );

                Console.WriteLine($"[PLUGIN] Mupen64Plus exited with code {exitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PLUGIN] Failed to launch Mupen64Plus");
                Console.WriteLine($"[PLUGIN] Exception: {ex}");
            }
        }
    }
}
