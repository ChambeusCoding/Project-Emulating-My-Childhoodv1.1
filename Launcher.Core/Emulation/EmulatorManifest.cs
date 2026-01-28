namespace Launcher.Core.Emulation;

public sealed class EmulatorManifest
{
    public required string Id { get; init; }
    public required string DisplayName { get; init; }

    public required string System { get; init; }   // "Nintendo 64"
    public required string Executable { get; init; }

    public required IReadOnlyList<string> SupportedExtensions { get; init; }
}