using System;
using System.IO;
using System.Linq;
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
            string homeDir =
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string[] searchDirs =
            {
                Path.Combine(homeDir, ".local", "share", "emulators", "azahar"),
                Path.Combine(homeDir, "Downloads")
            };

            string appImage =
                searchDirs
                    .Where(Directory.Exists)
                    .SelectMany(d =>
                        Directory.GetFiles(d, "azahar*.AppImage", SearchOption.TopDirectoryOnly)
                    )
                    .OrderByDescending(File.GetLastWriteTimeUtc)
                    .FirstOrDefault()
                ?? string.Empty;

            string executablePath;

            if (!string.IsNullOrEmpty(appImage) && File.Exists(appImage))
            {
                executablePath = appImage;
            }
            else
            {
                // FINAL fallback — only works if user actually installed it in PATH
                executablePath = "azahar";
            }

            Manifest = new EmulatorManifest
            {
                Id = "azahar",
                DisplayName = "Azahar (Nintendo 3DS)",
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
            Console.WriteLine($"[PLUGIN] Raw executable: {Manifest.Executable}");
        }

        public async Task LaunchAsync(string romPath)
        {
            if (!File.Exists(romPath))
            {
                Console.WriteLine($"[PLUGIN] ROM not found: {romPath}");
                return;
            }

            try
            {
                string resolved =
                    PlatformServices.PathResolver.ResolveExecutable(
                        Manifest.Executable
                    );

                Console.WriteLine("[PLUGIN] Launching Azahar");
                Console.WriteLine($"  Exec: {resolved}");
                Console.WriteLine($"  ROM : {romPath}");

                await PlatformServices.ProcessRunner.RunAsync(
                    resolved,
                    $"\"{romPath}\""
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("[PLUGIN] ❌ Failed to launch Azahar");
                Console.WriteLine("Make sure the AppImage is executable:");
                Console.WriteLine("  chmod +x azahar*.AppImage");
                Console.WriteLine(ex);
            }
        }
    }
}
