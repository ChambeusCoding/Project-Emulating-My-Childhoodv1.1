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
                Executable = "mupen64plus",  // Just the name - PATH finds /usr/games/mupen64plus
                SupportedExtensions = new[] { ".n64", ".z64", ".v64" }
            };

            Console.WriteLine("[PLUGIN] Mupen64PlusPlugin constructed");
            Console.WriteLine($"[PLUGIN] System: {Manifest.System}");
            Console.WriteLine($"[PLUGIN] Extensions: {string.Join(", ", Manifest.SupportedExtensions)}");
            Console.WriteLine($"[PLUGIN] Executable (raw): {Manifest.Executable}");
        }


        public (string Executable, string Arguments) BuildLaunchCommand(string romPath)
        {
            if (string.IsNullOrWhiteSpace(romPath))
                throw new ArgumentException("ROM path must not be empty.", nameof(romPath));

            // Let PathResolver do normal PATH lookup like before
            string resolvedExecutable = PlatformServices.PathResolver.ResolveExecutable(Manifest.Executable);
            string args = $"\"{romPath}\"";

            Console.WriteLine($"[PLUGIN] BuildLaunchCommand: exe='{resolvedExecutable}', args={args}");
            return (resolvedExecutable, args);
        }


        // Existing behavior kept for other callers
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

                Console.WriteLine($"[PLUGIN] Mupen64Plus exited with code {exitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PLUGIN] Failed to launch Mupen64Plus");
                Console.WriteLine($"[PLUGIN] Exception: {ex}");
            }
        }
    }
}
