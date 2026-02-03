using System.Collections.Generic;

namespace Launcher.Core.Emulation;

public sealed class EmulatorManifest
{

    public required string Id { get; init; }


    public required string DisplayName { get; init; }


    public required string System { get; init; }


    public required string Executable { get; init; }


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