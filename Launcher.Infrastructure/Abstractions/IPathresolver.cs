namespace Launcher.Infrastructure.Abstractions
{
    public interface IPathResolver
    {
        string ResolveExecutable(string name);
    }
}