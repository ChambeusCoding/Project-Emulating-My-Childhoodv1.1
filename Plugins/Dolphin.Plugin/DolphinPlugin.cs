using System; //Kept for learning reference
using System.IO; //Kept for learning reference
using System.Threading.Tasks; //Kept for learning reference
using Launcher.Core.Emulation;
using Launcher.Infrastructure;

namespace Dolphin.Plugin
{
    public sealed class DolphinPLugin : IEmulatorPlugin
    {
        public EmulatorManifest Manifest { get; }

        public DolphinPLugin()
        {
            Manifest = new EmulatorManifest
            {
                Id = "Dolphin",
                DisplayName = "Dolphin",
                System = "Wii",
                Executable = "dolphin",
                SupportedExtensions = new[] { "" }
            };
            
            Console.WriteLine("[PLUGIN] Dolphin plugin contructed");
            Console.WriteLine($"[PLUGIN] Executable path: {Manifest.Executable}");
        }

        public (string Executable, string Arguments) BuildLaunchCommand(string romPath)
        {
            if (string.IsNullOrWhiteSpace(romPath))
                throw new ArgumentException("ROM path required",  nameof(romPath));
            
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

