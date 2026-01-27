using Launcher.Core.Games;

namespace Launcher.Core.Emulation;

public interface IEmulatorPlugin
{
    string Id { get; }
    string DisplayName { get; }

    bool CanHandle(GameEntry game);

    Task LaunchAsync(GameEntry game);
}