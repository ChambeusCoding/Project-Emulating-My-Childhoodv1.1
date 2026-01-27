using Launcher.Core.Games;

namespace Launcher.Core.Emulation;

public class EmulatorManager
{
    private readonly IEnumerable<IEmulatorPlugin> _plugins;

    public EmulatorManager(IEnumerable<IEmulatorPlugin> plugins)
    {
        _plugins = plugins;
    }

    public IEmulatorPlugin? FindPlugin(GameEntry game)
    {
        return _plugins.FirstOrDefault(p => p.CanHandle(game));
    }
}