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
        if (!_registry.ContainsKey(plugin.Manifest.System))
            _registry[plugin.Manifest.System] = new List<IEmulatorPlugin>();

        _registry[plugin.Manifest.System].Add(plugin);

        Console.WriteLine($"Registered emulator: {plugin.Manifest.DisplayName} for system {plugin.Manifest.System}");
    }

    // Get all emulators for a system
    public IEnumerable<IEmulatorPlugin> GetEmulators(string system)
        => _registry.TryGetValue(system, out var list) ? list : Enumerable.Empty<IEmulatorPlugin>();

    // Check if a ROM is supported by any registered emulator
    public bool IsSupportedRom(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("[DEBUG] Path is null or empty");
            return false;
        }

        var ext = Path.GetExtension(path).ToLowerInvariant(); // normalize extension
        Console.WriteLine($"[DEBUG] Checking file: {path}, extracted extension: {ext}");

        foreach (var plugins in _registry.Values)
        {
            foreach (var plugin in plugins)
            {
                Console.WriteLine($"[DEBUG] Checking against emulator: {plugin.Manifest.DisplayName}");
                Console.WriteLine($"[DEBUG] Supported extensions: {string.Join(", ", plugin.Manifest.SupportedExtensions)}");

                if (plugin.Manifest.SupportedExtensions.Contains(ext))
                {
                    Console.WriteLine($"[DEBUG] Match found! File {path} is supported by {plugin.Manifest.DisplayName}");
                    return true;
                }
            }
        }

        Console.WriteLine($"[DEBUG] No emulator supports the file: {path}");
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