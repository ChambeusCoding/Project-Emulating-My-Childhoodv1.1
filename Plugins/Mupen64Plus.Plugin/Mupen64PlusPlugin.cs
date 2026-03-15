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
            Manifest = new EmulatorManifest
            {
                Id = "mupen64plus",
                DisplayName = "Mupen64Plus", 
                System = "Nintendo 64",
                Executable = "mupen64plus",
                SupportedExtensions = new[] { ".n64", ".z64", ".v64" }
            };

            Console.WriteLine("[PLUGIN] Mupen64PlusPlugin constructed");
            Console.WriteLine($"[PLUGIN] Executable: {Manifest.Executable}");
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
