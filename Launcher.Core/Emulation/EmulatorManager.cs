using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Launcher.Core.Emulation;

public class EmulatorManager
{
    private readonly Dictionary<string, List<IEmulatorPlugin>> _registry = new();

    // Register a new emulator plugin
    public void Register(IEmulatorPlugin plugin)
    {
        Console.WriteLine($"[EMULATOR MANAGER] Registering plugin:");
        Console.WriteLine($"  Id: {plugin.Manifest.Id}");
        Console.WriteLine($"  System: {plugin.Manifest.System}");
        Console.WriteLine($"  Extensions: {string.Join(", ", plugin.Manifest.SupportedExtensions)}");

        if (!_registry.ContainsKey(plugin.Manifest.System))
            _registry[plugin.Manifest.System] = new List<IEmulatorPlugin>();

        _registry[plugin.Manifest.System].Add(plugin);
    }


    // Get all emulators for a system
    public IEnumerable<IEmulatorPlugin> GetEmulators(string system)
        => _registry.TryGetValue(system, out var list) ? list : Enumerable.Empty<IEmulatorPlugin>();

    // Check if a ROM is supported by any registered emulator
    public bool IsSupportedRom(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();

        Console.WriteLine($"[CHECK] ROM: {path}");
        Console.WriteLine($"[CHECK] Extension: {ext}");

        foreach (var (system, plugins) in _registry)
        {
            Console.WriteLine($"[CHECK] System registered: {system}");

            foreach (var plugin in plugins)
            {
                Console.WriteLine(
                    $"[CHECK] Plugin {plugin.Manifest.Id} supports: {string.Join(", ", plugin.Manifest.SupportedExtensions)}"
                );

                if (plugin.Manifest.SupportedExtensions
                    .Any(e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("[CHECK] ✅ Supported!");
                    return true;
                }
            }
        }

        Console.WriteLine("[CHECK] ❌ No emulator supports this ROM");
        return false;
    }




    // Find the first emulator that can run this ROM
    public IEmulatorPlugin? FindForRom(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        foreach (var plugins in _registry.Values)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.Manifest.SupportedExtensions.Contains(ext))
                    return plugin;
            }
        }
        return null;
    }

    // List all registered systems
    public IEnumerable<string> RegisteredSystems() => _registry.Keys;
}