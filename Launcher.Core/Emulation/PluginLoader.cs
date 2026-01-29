using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Launcher.Core.Emulation;

public static class PluginLoader
{
    public static IEnumerable<IEmulatorPlugin> LoadPlugins(string folder)
    {
        Console.WriteLine($"[PluginLoader] Scanning folder: {folder}");

        if (!Directory.Exists(folder))
        {
            Console.WriteLine("[PluginLoader] Folder does not exist");
            yield break;
        }

        foreach (var subDir in Directory.GetDirectories(folder))
        {
            Console.WriteLine($"[PluginLoader] Checking plugin directory: {subDir}");

            var jsonPath = Path.Combine(subDir, "plugin.json");
            if (!File.Exists(jsonPath))
            {
                Console.WriteLine("[PluginLoader] plugin.json not found, skipping");
                continue;
            }

            EmulatorManifest? manifest;
            try
            {
                var json = File.ReadAllText(jsonPath);
                manifest = JsonSerializer.Deserialize<EmulatorManifest>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PluginLoader] Failed to read manifest: {ex.Message}");
                continue;
            }

            if (manifest == null)
            {
                Console.WriteLine("[PluginLoader] Manifest is null");
                continue;
            }

            Console.WriteLine($"[PluginLoader] Loaded manifest: {manifest.DisplayName}");

            foreach (var dll in Directory.GetFiles(subDir, "*.dll"))
            {
                Console.WriteLine($"[PluginLoader] Loading DLL: {dll}");

                Assembly asm;
                try
                {
                    asm = Assembly.LoadFrom(dll);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PluginLoader] Failed to load DLL: {ex.Message}");
                    continue;
                }

                foreach (var type in asm.GetTypes())
                {
                    if (!typeof(IEmulatorPlugin).IsAssignableFrom(type) || type.IsAbstract)
                        continue;

                    IEmulatorPlugin? plugin = null;

                    try
                    {
                        plugin = (IEmulatorPlugin)Activator.CreateInstance(type)!;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[PluginLoader] Failed to create plugin instance: {ex.Message}");
                        continue;
                    }

                    Console.WriteLine($"[PluginLoader] Loaded plugin: {plugin.Manifest.DisplayName}");
                    Console.WriteLine($"[PluginLoader] Extensions: {string.Join(", ", plugin.Manifest.SupportedExtensions)}");

                    // ✔ yield return OUTSIDE of try/catch
                    yield return plugin;
                }
            }
        }
    }
}
