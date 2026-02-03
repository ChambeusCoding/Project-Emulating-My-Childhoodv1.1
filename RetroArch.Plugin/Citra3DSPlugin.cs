// using System;
// using System.IO;
// using System.Threading.Tasks;
// using Launcher.Core.Emulation;
// using Launcher.Infrastructure.Linux;
//
// namespace RetroArch.Plugin
// {
//     public sealed class Citra3DSPlugin : IEmulatorPlugin
//     {
//         public EmulatorManifest Manifest { get; }
//
//         public Citra3DSPlugin()
//         {
//             // Get the current user's home directory
//             string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
//
//             // Standard AppImage location
//             string appImagePath = Path.Combine(homeDir, ".local", "share", "emulators", "Citra3DS", "Citra.AppImage");
//
//             // Use AppImage if it exists, otherwise fallback to PATH
//             string executablePath = File.Exists(appImagePath) ? appImagePath : "citra3ds";
//
//             Manifest = new EmulatorManifest
//             {
//                 Id = "citra3ds",
//                 DisplayName = "Citra3DS",
//                 System = "Nintendo 3DS",
//                 Executable = executablePath,
//                 SupportedExtensions = new[]
//                 {
//                     ".3ds",
//                     ".cia",
//                     ".cci"
//                 }
//             };
//
//             // Debug logging
//             Console.WriteLine("[PLUGIN] Citra3DSPlugin constructed");
//             Console.WriteLine($"[PLUGIN] System: {Manifest.System}");
//             Console.WriteLine($"[PLUGIN] Extensions: {string.Join(", ", Manifest.SupportedExtensions)}");
//             Console.WriteLine($"[PLUGIN] Executable: {Manifest.Executable}");
//         }
//
//         public async Task LaunchAsync(string romPath)
//         {
//             if (!File.Exists(romPath))
//             {
//                 Console.WriteLine($"[PLUGIN] ROM not found: {romPath}");
//                 return;
//             }
//
//             Console.WriteLine($"[PLUGIN] Launching ROM: {romPath}");
//
//             try
//             {
//                 await ProcessRunner.RunAsync(Manifest.Executable, $"\"{romPath}\"");
//                 Console.WriteLine("[PLUGIN] Citra3DS process started successfully");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"[PLUGIN] Failed to launch Citra3DS: {ex.Message}");
//             }
//         }
//     }
// }
