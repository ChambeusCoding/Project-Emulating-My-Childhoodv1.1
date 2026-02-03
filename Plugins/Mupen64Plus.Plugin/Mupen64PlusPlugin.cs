using System;
using System.IO;
using System.Threading.Tasks;
using Launcher.Core.Emulation;
using Launcher.Infrastructure.Linux;

namespace Mupen64Plus.Plugin
{
    public sealed class Mupen64PlusPlugin : IEmulatorPlugin
    {
        public EmulatorManifest Manifest { get; }

        public Mupen64PlusPlugin()
        {
            // Dynamically get the user's home directory
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Standard Linux path for Mupen64Plus inside ~/.local/share/emulators/mupen64plus
            string defaultExecutable = Path.Combine(homeDir, ".local", "share", "emulators", "mupen64plus", "mupen64plus");

            // If it exists, use it; otherwise, fall back to just "mupen64plus" (must be in PATH)
            string executablePath = File.Exists(defaultExecutable) ? defaultExecutable : "mupen64plus";

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
            Console.WriteLine($"[PLUGIN] Executable: {Manifest.Executable}");
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
                await ProcessRunner.RunAsync(Manifest.Executable, $"\"{romPath}\"");
                Console.WriteLine("[PLUGIN] Mupen64Plus process started successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PLUGIN] Failed to launch Mupen64Plus: {ex.Message}");
            }
        }
    }
}
