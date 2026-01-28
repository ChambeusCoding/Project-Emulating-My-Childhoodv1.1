using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Launcher.Core.Emulation;

public static class PluginLoader
{
    public static IEnumerable<IEmulatorPlugin> LoadPlugins(string folder)
    {
        if (!Directory.Exists(folder))
            yield break;

        foreach (var subDir in Directory.GetDirectories(folder))
        {
            var jsonPath = Path.Combine(subDir, "plugin.json");
            if (!File.Exists(jsonPath))
                continue;

            var json = File.ReadAllText(jsonPath);
            var manifest = JsonSerializer.Deserialize<EmulatorManifest>(json);
            if (manifest == null)
                continue;

            // Dynamically load the plugin class from the DLL
            var dllFiles = Directory.GetFiles(subDir, "*.dll");
            foreach (var dll in dllFiles)
            {
                var asm = System.Reflection.Assembly.LoadFrom(dll);
                foreach (var type in asm.GetTypes())
                {
                    if (typeof(IEmulatorPlugin).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        var plugin = (IEmulatorPlugin)Activator.CreateInstance(type)!;
                        yield return plugin;
                    }
                }
            }
        }
    }
}