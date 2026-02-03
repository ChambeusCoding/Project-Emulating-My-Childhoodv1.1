using System;
using System.IO;

namespace Launcher.Infrastructure.Windows
{
    public static class WindowsFileSystem
    {
        public static bool Exists(string path) => File.Exists(path) || Directory.Exists(path);

        public static string Combine(params string[] paths) => Path.Combine(paths);

        public static string GetHomeDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}