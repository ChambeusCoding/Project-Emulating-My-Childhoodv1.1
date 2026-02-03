namespace Launcher.Infrastructure.Linux;

public static class LinuxFileSystem
{
    public static bool Exists(string path) => File.Exists(path);
}