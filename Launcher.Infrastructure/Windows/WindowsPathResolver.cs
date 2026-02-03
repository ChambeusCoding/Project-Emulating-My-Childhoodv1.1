using System;
using System.IO;
using Launcher.Infrastructure.Abstractions;

namespace Launcher.Infrastructure.Windows
{
    public sealed class WindowsPathResolver : IPathResolver
    {
        public string ResolveExecutable(string executableName)
        {
            if (!executableName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                executableName += ".exe";

            if (Path.IsPathRooted(executableName) && File.Exists(executableName))
                return executableName;

            var pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            string[] emulators =
            [
                "RetroArch",
                "Citra",
                "Mupen64Plus",
                "Dolphin",
                "Cemu",
                "Ryujinx",
                "Yuzu",
                "PCSX2",
                "DuckStation",
                "PPSSPP",
                "Mednafen",
                "MAME",
                "Flycast"
            ];

            foreach (var emulator in emulators)
            {
                var paths = new[]
                {
                    Path.Combine(pf, emulator, executableName),
                    Path.Combine(pf86, emulator, executableName)
                };

                foreach (var path in paths)
                {
                    if (File.Exists(path))
                        return path;
                }
            }

            return executableName;
        }
    }
}