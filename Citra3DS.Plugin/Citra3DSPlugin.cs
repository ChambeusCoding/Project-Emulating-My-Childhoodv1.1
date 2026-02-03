using System;
using System.IO;
using System.Threading.Tasks;
using Launcher.Core.Emulation;
using Launcher.Infrastructure;

namespace RetroArch.Plugin
{
    public sealed class Citra3DSPlugin : IEmulatorPlugin
    {
        public EmulatorManifest Manifest { get; }

        public Citra3DSPlugin()
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Expected AppImage location (Linux)
            string appImagePath = Path.Combine(
                homeDir,
                ".local",
                "share",
                "emulators",
                "Citra3DS",
                "Citra.AppImage"
            );

            // Fallback name (Windows PATH or manual install)
            string executablePath = File.Exists(appImagePath)
                ? appImagePath
                : "citra-qt";

            Manifest = new EmulatorManifest
            {
                Id = "citra3ds",
                DisplayName = "Citra 3DS",
                System = "Nintendo 3DS",
                Executable = executablePath,
                SupportedExtensions = new[]
                {
                    ".3ds",
                    ".cia",
                    ".cci"
                }
            };

            Console.WriteLine("[PLUGIN] Citra3DSPlugin constructed");
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
                string resolvedExecutable =
                    PlatformServices.PathResolver.ResolveExecutable(Manifest.Executable);

                Console.WriteLine($"[PLUGIN] Resolved executable: {resolvedExecutable}");
                Console.WriteLine($"[PLUGIN] Arguments: \"{romPath}\"");

                int exitCode = await PlatformServices.ProcessRunner.RunAsync(
                    resolvedExecutable,
                    $"\"{romPath}\""
                );

                Console.WriteLine($"[PLUGIN] Citra exited with code {exitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[PLUGIN] Failed to launch Citra3DS");
                Console.WriteLine($"[PLUGIN] Exception: {ex}");
            }
        }
    }
}
