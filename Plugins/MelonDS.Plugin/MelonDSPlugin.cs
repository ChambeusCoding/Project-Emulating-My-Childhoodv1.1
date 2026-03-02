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
                Executable = "MelonDS.exe",
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
                Console.WriteLine($"[PLUGIN] File not found: {romPath}");
                return;
            }
            
            Console.WriteLine($"[PLUGIN] Launching ROM: {romPath}");

            try
            {
                var (resolvedExecutable, arguments) = BuildLaunchCommand(romPath);
                
                Console.WriteLine($"[PLUGIN] Executable path: {resolvedExecutable}");
                Console.WriteLine($"[PLUGIN] arguments: {arguments}");

                int exitCode = await PlatformServices.ProcessRunner.RunAsync(
                    resolvedExecutable,
                    arguments
                    );
                Console.WriteLine($"[Plugin] MelonDS exit code: {exitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PLUGIN] MelonDS exception: {ex.Message}");
            }
        }
    }
}