using System.Collections.Generic;

namespace Launcher.Core.Emulation;

public sealed class EmulatorManifest
{
    // Unique identifier for the emulator (e.g., "mupen64plus")
    public required string Id { get; init; }

    // Friendly display name (e.g., "Mupen64Plus")
    public required string DisplayName { get; init; }

    // Target system (e.g., "Nintendo 64")
    public required string System { get; init; }

    // Path to the emulator executable
    public required string Executable { get; init; }

    // Supported file extensions (normalized with dot, e.g., ".n64", ".z64")
    private IReadOnlyList<string> _supportedExtensions = new List<string>();
    public IReadOnlyList<string> SupportedExtensions
    {
        get => _supportedExtensions;
        init
        {
            var normalized = new List<string>();
            foreach (var ext in value)
            {
                var e = ext.Trim().ToLowerInvariant();
                if (!e.StartsWith(".")) e = "." + e;
                normalized.Add(e);
            }
            _supportedExtensions = normalized;
        }
    }
}