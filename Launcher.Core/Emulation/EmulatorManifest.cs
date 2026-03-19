using System.Collections.Generic;

namespace Launcher.Core.Emulation;

public sealed class EmulatorManifest
{

    public string Id { get; init; }


    public string DisplayName { get; init; }


    public string System { get; init; }


    public string Executable { get; init; } = string.Empty;  // Legacy single
    public string[] Executables { get; init; } = Array.Empty<string>();  // Multi
    public string[] GetAllExecutables() => 
        Executables.Length > 0 ? Executables : new[] { Executable };
    
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