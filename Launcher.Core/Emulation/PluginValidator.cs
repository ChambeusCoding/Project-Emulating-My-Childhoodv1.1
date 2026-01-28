using System;
using System.IO;
using System.Linq;

namespace Launcher.Core.Emulation
{
    public static class PluginValidator
    {
        public static void Validate(IEmulatorPlugin plugin)
        {
            var m = plugin.Manifest;

            if (string.IsNullOrWhiteSpace(m.Id))
                throw new InvalidOperationException($"Plugin {plugin.GetType().Name} missing Id.");

            if (string.IsNullOrWhiteSpace(m.DisplayName))
                throw new InvalidOperationException($"Plugin {plugin.GetType().Name} missing DisplayName.");

            if (string.IsNullOrWhiteSpace(m.System))
                throw new InvalidOperationException($"Plugin {plugin.GetType().Name} missing System.");

            if (string.IsNullOrWhiteSpace(m.Executable) || !File.Exists(m.Executable))
                throw new InvalidOperationException($"Plugin {plugin.GetType().Name} has invalid Executable path: {m.Executable}");

            if (m.SupportedExtensions == null || m.SupportedExtensions.Count == 0)
                throw new InvalidOperationException($"Plugin {plugin.GetType().Name} must define SupportedExtensions.");
        }
    }
}