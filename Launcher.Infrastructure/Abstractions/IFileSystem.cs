namespace Launcher.Infrastructure.Abstractions
{
    public interface IFileSystem
    {
        bool Exists(string path);
        void EnsureDirectory(string path);
        string Combine(params string[] paths);
        string HomeDirectory { get; }
    }
}