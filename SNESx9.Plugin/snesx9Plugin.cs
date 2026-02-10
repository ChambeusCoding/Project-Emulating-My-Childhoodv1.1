using System;
using System.IO;
using System.Threading.Tasks;
using Launcher.Core.Emulation;
using Launcher.Infrastructure;

namespace SNESx9.Plugin
{
    public sealed class Snesx9Plugin : IEmulatorPlugin
    {
        public EmulatorManifest Manifest { get; }

        public Snesx9Plugin()
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            
            string defaultExecutable = Path.Combine(
                homeDir,
                ".local",
                "share",
                "emulators",
                "snes9x",
                "snes9x"
            );

            string executablePath = File.Exists(defaultExecutable)
                ? defaultExecutable
                : "snes9x-gtk"; 


            Manifest = new EmulatorManifest
            {
                Id = "snesx9",           
                DisplayName = "SNESx9",     
                System = "Super Nintendo",  
                Executable = executablePath,
                SupportedExtensions = new[]
                {
                    ".smc",
                    ".sfc",
                    ".fig",
                    ".swc"
                }
            };

            Console.WriteLine("[PLUGIN] Snesx9Plugin constructed");
            Console.WriteLine($"[PLUGIN] System: {Manifest.System}");
            Console.WriteLine($"[PLUGIN] Extensions: {string.Join(", ", Manifest.SupportedExtensions)}");
            Console.WriteLine($"[PLUGIN] Executable (raw): {Manifest.Executable}");
        }

        public (string Executable, string Arguments) BuildLaunchCommand(string romPath)
        {
            if (string.IsNullOrWhiteSpace(romPath))
                throw new ArgumentException("ROM path must not be empty.", nameof(romPath));

            string resolvedExecutable =
                PlatformServices.PathResolver.ResolveExecutable(Manifest.Executable);

            string args = $"\"{romPath}\"";

            Console.WriteLine($"[PLUGIN] BuildLaunchCommand: exe='{resolvedExecutable}', args={args}");

            return (resolvedExecutable, args);
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
                var (resolvedExecutable, arguments) = BuildLaunchCommand(romPath);

                Console.WriteLine($"[PLUGIN] Resolved executable: {resolvedExecutable}");
                Console.WriteLine($"[PLUGIN] Arguments: {arguments}");

                int exitCode = await PlatformServices.ProcessRunner.RunAsync(
                    resolvedExecutable,
                    arguments
                );

                Console.WriteLine($"[PLUGIN] SNESx9 exited with code {exitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[PLUGIN] Failed to launch SNESx9");
                Console.WriteLine($"[PLUGIN] Exception: {ex}");
            }
        }
    }
}
