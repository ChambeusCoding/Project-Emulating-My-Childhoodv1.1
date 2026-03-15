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

            string resolvedExecutable = PlatformServices.PathResolver.ResolveExecutable(Manifest.Executable);

            // 🔧 FIX: snes9x-gtk requires -n flag for ROM loading
            string args = $"-n \"{romPath}\"";

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

            try
            {
                var (resolvedExecutable, arguments) = BuildLaunchCommand(romPath);
                string workingDir = Path.GetDirectoryName(romPath)!;  // ← ADD THIS

                Console.WriteLine($"[PLUGIN] Executable: {resolvedExecutable}");
                Console.WriteLine($"[PLUGIN] Arguments: {arguments}");
                Console.WriteLine($"[PLUGIN] WorkingDirectory: {workingDir}");  // ← ADD LOGGING

                int exitCode = await PlatformServices.ProcessRunner.RunAsync(
                    resolvedExecutable, 
                    arguments,
                    workingDir  // ← PASS THIS
                );

                Console.WriteLine($"[PLUGIN] Emulator exited with code {exitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PLUGIN] Launch failed: {ex.Message}");
            }
        }

    }
}
