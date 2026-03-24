using System;
using System.IO;
using System.Threading.Tasks;
using Launcher.Core.Emulation;
using Launcher.Infrastructure;

namespace MelonDS.Plugin
{
    public sealed class MelonDSPlugin : IEmulatorPlugin
    {
        public EmulatorManifest Manifest { get; }

        public MelonDSPlugin()
        {
            Manifest = new EmulatorManifest
            {
                Id = "MelonDS",
                DisplayName = "MelonDS Plugin",
                System = "Nintendo DS",
                Executable = "melonds",
                SupportedExtensions = new[] { ".nds", ".zip", "7z" }
            };
            
            Console.WriteLine("[PLUGIN] MelonDSPlugin constructed");
            Console.WriteLine($"[PLUGIN] Executable path: {Manifest.Executable}");
        }

        public (string Executable, string Arguments) BuildLaunchCommand(string romPath)
        {
            if (string.IsNullOrWhiteSpace(romPath))
                throw new ArgumentException("ROM path required", nameof(romPath));
            
            string resolvedExecutable = PlatformServices.PathResolver.ResolveExecutable(Manifest.Executable);
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

            try
            {
                var (resolvedExecutable, arguments) = BuildLaunchCommand(romPath);
                string workingDir = Path.GetDirectoryName(romPath)!;

                Console.WriteLine($"[PLUGIN] Executable: {resolvedExecutable}");
                Console.WriteLine($"[PLUGIN] Arguments: {arguments}");
                Console.WriteLine($"[PLUGIN] WorkingDirectory: {workingDir}");

                int exitCode = await PlatformServices.ProcessRunner.RunAsync(
                    resolvedExecutable, 
                    arguments,
                    workingDir
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