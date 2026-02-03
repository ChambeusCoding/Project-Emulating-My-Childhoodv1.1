using System;
using System.IO;
using System.Threading.Tasks;
using Launcher.Core.Emulation;
using Launcher.Infrastructure.Linux;

namespace RetroArch.Plugin
{
    public sealed class RetroArchPlugin : IEmulatorPlugin
    {
        public EmulatorManifest Manifest { get; }

        public RetroArchPlugin()
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // RetroArch executable
            string defaultPath = Path.Combine(homeDir, ".local", "share", "emulators", "RetroArch", "retroarch");
            string executablePath = File.Exists(defaultPath) ? defaultPath : "retroarch";

            Manifest = new EmulatorManifest
            {
                Id = "retroarch",
                DisplayName = "RetroArch",
                System = "Nintendo 3DS",
                Executable = executablePath,
                SupportedExtensions = new[]
                {
                    ".3ds",
                    ".cia",
                    ".cci"
                }
            };

            Console.WriteLine("[PLUGIN] RetroArchPlugin constructed");
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
                // Absolute path to Citra core (adjust if your core is elsewhere)
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string citraCore = Path.Combine(homeDir, ".local", "share", "libretro", "citra_libretro.so");

                if (!File.Exists(citraCore))
                {
                    Console.WriteLine("[PLUGIN] Citra core not found at: " + citraCore);
                    return;
                }

                string args = $"--libretro \"{citraCore}\" \"{romPath}\"";

                await ProcessRunner.RunAsync(Manifest.Executable, args);
                Console.WriteLine("[PLUGIN] RetroArch process started successfully with Citra core");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PLUGIN] Failed to launch RetroArch: {ex.Message}");
            }
        }
    }
}
