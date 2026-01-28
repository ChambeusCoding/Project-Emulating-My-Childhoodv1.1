using System.Collections.Generic;

namespace Launcher.Core.Emulation;

public class EmulatorManager
{
    private readonly Dictionary<string, List<IEmulatorPlugin>> _registry = new();

    public void Register(IEmulatorPlugin plugin)
    {
        if (!_registry.ContainsKey(plugin.Manifest.System))
            _registry[plugin.Manifest.System] = new List<IEmulatorPlugin>();

        _registry[plugin.Manifest.System].Add(plugin);
    }

    public IEnumerable<IEmulatorPlugin> GetEmulators(string system)
        => _registry.TryGetValue(system, out var list) ? list : new List<IEmulatorPlugin>();

    public bool IsSupportedRom(string path)
    {
        foreach (var emus in _registry.Values)
        {
            foreach (var e in emus)
            {
                foreach (var ext in e.Manifest.SupportedExtensions)
                {
                    if (path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
        }
        return false;
    }

    public IEmulatorPlugin? FindForRom(string path)
    {
        foreach (var emus in _registry.Values)
        {
            foreach (var e in emus)
            {
                foreach (var ext in e.Manifest.SupportedExtensions)
                {
                    if (path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                        return e;
                }
            }
        }
        return null;
    }

    public IEnumerable<string> RegisteredSystems() => _registry.Keys;
}