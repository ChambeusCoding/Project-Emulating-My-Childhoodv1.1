using System;
using System.Threading.Tasks;
using Launcher.Core.Emulation;
using Launcher.Core.Games;
using Launcher.Infrastructure.Linux;

namespace Mupen64Plus.Plugin;

public class Mupen64PlusPlugin : IEmulatorPlugin
{
    public string Id => "mupen64plus";
    public string DisplayName => "Mupen64Plus";

    public bool CanHandle(GameEntry game)
    {
        return game.FilePath.EndsWith(".n64", StringComparison.OrdinalIgnoreCase);
    }

    public Task LaunchAsync(GameEntry game)
    {
        ProcessRunner.Run("mupen64plus", $"\"{game.FilePath}\"");
        return Task.CompletedTask;
    }
}