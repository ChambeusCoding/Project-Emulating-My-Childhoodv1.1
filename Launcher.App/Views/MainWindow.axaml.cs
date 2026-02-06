using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Input;

using Launcher.Core.Games;
using Launcher.Core.Emulation;
using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // 1️⃣ Create the emulator manager
        var emulatorManager = new EmulatorManager();

        // 2️⃣ Load all plugins from Plugins folder
        var pluginsPath = Path.Combine(AppContext.BaseDirectory, "Plugins");
        Console.WriteLine($"[DEBUG] Loading plugins from {pluginsPath}");

        if (Directory.Exists(pluginsPath))
        {
            foreach (var plugin in PluginLoader.LoadPlugins(pluginsPath))
            {
                Console.WriteLine(
                    $"[DEBUG] Registering plugin: {plugin.Manifest.DisplayName} ({plugin.Manifest.System})");
                emulatorManager.Register(plugin);
            }
        }
        else
        {
            Console.WriteLine("[DEBUG] Plugins folder does not exist!");
        }

        // 2.1️⃣ Debug: list all registered systems
        Console.WriteLine("[DEBUG] Registered Systems:");
        foreach (var sys in emulatorManager.RegisteredSystems())
        {
            Console.WriteLine($" - {sys}");
        }

        // 3️⃣ Create the GameScanner
        var scanner = new GameScanner(emulatorManager);

        // 4️⃣ Set DataContext
        DataContext = new MainWindowViewModel(scanner);
    }

    private async void OnTerminalKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        if (DataContext is not MainWindowViewModel vm)
            return;

        if (string.IsNullOrWhiteSpace(vm.TerminalInput))
            return;

        var command = vm.TerminalInput.Trim();
        vm.TerminalInput = "";

        vm.AppendTerminal($"> {command}");

        await RunTerminalCommand(command);
    }

    private async Task RunTerminalCommand(string command)
    {
        if (DataContext is not MainWindowViewModel vm)
            return;

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            var stdout = await process.StandardOutput.ReadToEndAsync();
            var stderr = await process.StandardError.ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(stdout))
                vm.AppendTerminal(stdout.TrimEnd());

            if (!string.IsNullOrWhiteSpace(stderr))
                vm.AppendTerminal(stderr.TrimEnd());
        }
        catch (Exception ex)
        {
            vm.AppendTerminal($"[error] {ex.Message}");
        }
    }
}
