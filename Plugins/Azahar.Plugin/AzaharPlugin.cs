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
                // Fallback — only works if user actually installed it in PATH
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

        // NEW: used by your ViewModel to run Azahar and capture stdout/stderr
        public (string Executable, string Arguments) BuildLaunchCommand(string romPath)
        {
            if (string.IsNullOrWhiteSpace(romPath))
                throw new ArgumentException("ROM path must not be empty.", nameof(romPath));

            // Resolve executable (AppImage path or PATH)
            string resolved =
                PlatformServices.PathResolver.ResolveExecutable(
                    Manifest.Executable
                );

            string args = $"\"{romPath}\"";

            Console.WriteLine($"[PLUGIN] BuildLaunchCommand: exe='{resolved}', args={args}");

            return (resolved, args);
        }

        // Kept for other callers; now delegates to BuildLaunchCommand
        public async Task LaunchAsync(string romPath)
        {
            if (!File.Exists(romPath))
            {
                Console.WriteLine($"[PLUGIN] ROM not found: {romPath}");
                return;
            }

            try
            {
                var (resolved, args) = BuildLaunchCommand(romPath);

                Console.WriteLine("[PLUGIN] Launching Azahar");
                Console.WriteLine($"  Exec: {resolved}");
                Console.WriteLine($"  ROM : {romPath}");

                await PlatformServices.ProcessRunner.RunAsync(
                    resolved,
                    args
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
