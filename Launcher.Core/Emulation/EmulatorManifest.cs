namespace Launcher.Core.Emulation;

public class EmulatorManifest
{
    public string Id { get; init; } = "";
    public string Executable { get; init; } = "";
    public string[] SupportedExtensions { get; init; } = [];
}