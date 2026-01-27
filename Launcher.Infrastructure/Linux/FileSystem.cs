namespace Launcher.Infrastructure.Linux;

public static class FileSystem
{
    public static bool Exists(string path) => File.Exists(path);
}