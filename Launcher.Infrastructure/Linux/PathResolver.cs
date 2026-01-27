namespace Launcher.Infrastructure.Linux;

public static class PathResolver
{
    public static string Expand(string path)
        => path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
}