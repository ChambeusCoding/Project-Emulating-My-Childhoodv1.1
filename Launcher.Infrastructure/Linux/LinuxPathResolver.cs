using System;
using System.IO;
using Launcher.Infrastructure.Abstractions;

namespace Launcher.Infrastructure.Linux
{
    public sealed class LinuxPathResolver : IPathResolver
    {
        public string ResolveExecutable(string executableName)
        {
            if (Path.IsPathRooted(executableName) && File.Exists(executableName))
                return executableName;

            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string[] emulatorDirs =
            [
                ".local/share/emulators",
                ".local/bin"
            ];

            string[] emulators =
            [
                "retroarch",
                "citra",
                "mupen64plus",
                "dolphin-emu",
                "cemu",
                "ryujinx",
                "yuzu",
                "pcsx2",
                "duckstation",
                "ppsspp",
                "mednafen",
                "mame",
                "flycast"
            ];

            foreach (var dir in emulatorDirs)
            {
                foreach (var emulator in emulators)
                {
                    var candidate = Path.Combine(home, dir, emulator, executableName);
                    if (File.Exists(candidate))
                        return candidate;
                }
            }

            string[] systemPaths =
            [
                Path.Combine("/usr/bin", executableName),
                Path.Combine("/usr/local/bin", executableName)
            ];

            foreach (var path in systemPaths)
            {
                if (File.Exists(path))
                    return path;
            }

            // PATH fallback
            return executableName;
        }
    }
}