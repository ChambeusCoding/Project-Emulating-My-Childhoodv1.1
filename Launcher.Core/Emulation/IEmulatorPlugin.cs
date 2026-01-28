namespace Launcher.Core.Emulation;

public interface IEmulatorPlugin
{
    EmulatorManifest Manifest { get; }

    Task LaunchAsync(string romPath);
}