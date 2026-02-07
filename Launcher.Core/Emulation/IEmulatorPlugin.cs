namespace Launcher.Core.Emulation;

public interface IEmulatorPlugin
{
    EmulatorManifest Manifest { get; }

    // Existing behavior (you can still use this elsewhere if you want)
    Task LaunchAsync(string romPath);

    // New: lets the UI build & run the process so it can capture stdout/stderr
    (string Executable, string Arguments) BuildLaunchCommand(string romPath);
}