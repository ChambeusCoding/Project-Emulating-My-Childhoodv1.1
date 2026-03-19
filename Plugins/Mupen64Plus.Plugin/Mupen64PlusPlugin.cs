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
        public string[] Executables { get; set; } = Array.Empty<string>();

        public Mupen64PlusPlugin()
        {
            Manifest = new EmulatorManifest
            {
                Id = "mupen64plus",
                DisplayName = "Mupen64Plus", 
                System = "Nintendo 64",
                Executables = new[] { 
                    "mupen64plus-ui-console.exe",    // Windows
                    "mupen64plus",               // Linux/macOS  
                    "mupen64plus.exe"            // Windows fallback
                },
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
                string workingDir = Path.GetDirectoryName(romPath)!;

                Console.WriteLine($"[PLUGIN] Launching: {resolvedExecutable} {arguments}");
                Console.WriteLine($"[PLUGIN] WorkingDirectory: {workingDir}");

                // Fire and forget - DON'T wait for emulator exit
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = resolvedExecutable,
                        Arguments = arguments,
                        WorkingDirectory = workingDir,
                        UseShellExecute = false,
                        CreateNoWindow = false  // Let Mupen have its console window
                    }
                };

                process.Start();
                Console.WriteLine($"[PLUGIN] Mupen64Plus launched (PID: {process.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PLUGIN] Launch failed: {ex.Message}");
            }
        }

    }
}
