using System;
using System.IO;
using System.Threading.Tasks;
using Launcher.Core.Emulation;
using Launcher.Infrastructure;

namespace Azahar.Plugin
{
    public sealed class AzaharPlugin : IEmulatorPlugin
    {
        public EmulatorManifest Manifest { get; }

        public AzaharPlugin()
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string azaharDir = Path.Combine(homeDir, ".local", "share", "emulators", "Azahar");
            string appImage = Directory.GetFiles(azaharDir, "*AppImage", SearchOption.TopDirectoryOnly)
                .FirstOrDefault() ?? string.Empty;

            string executablePath = !string.IsNullOrEmpty(appImage) && File.Exists(appImage)
                ? appImage
                : "azahar"; // fallback if it is in PATH

            Manifest = new EmulatorManifest
            {
                Id = "azahar",
                DisplayName = "Azahar (3DS)",
                System = "Nintendo 3DS",
                Executable = executablePath,
                SupportedExtensions = new[]
                {
                    ".3ds",
                    ".cia",
                    ".cci"
                }
            };

            Console.WriteLine("[PLUGIN] AzaharPlugin constructed");
            Console.WriteLine($"[PLUGIN] System: {Manifest.System}");
            Console.WriteLine($"[PLUGIN] Extensions: {string.Join(", ", Manifest.SupportedExtensions)}");
            Console.WriteLine($"[PLUGIN] Raw Executable: {Manifest.Executable}");
        }

        public async Task LaunchAsync(string romPath)
        {
            if (!File.Exists(romPath))
            {
                Console.WriteLine($"[PLUGIN] ROM not found: {romPath}");
                return;
            }

            string resolved = PlatformServices.PathResolver.ResolveExecutable(Manifest.Executable);

            Console.WriteLine($"[PLUGIN] Launching AZAHAR: {resolved} \"{romPath}\"");

            try
            {
                int exitCode = await PlatformServices.ProcessRunner.RunAsync(
                    resolved,
                    $"\"{romPath}\""
                );

                Console.WriteLine($"[PLUGIN] Azahar exited with code {exitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[PLUGIN] Failed to launch Azahar");
                Console.WriteLine(ex);
            }
        }
    }
}
