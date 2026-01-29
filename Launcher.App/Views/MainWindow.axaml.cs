using Launcher.Core.Games;
using Launcher.Core.Emulation;
using Avalonia.Controls;
using Launcher.App.ViewModels; // must match namespace

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
            Console.WriteLine($"[DEBUG] Plugins folder does not exist!");
        }

        // 2.1️⃣ Debug: list all registered systems
        Console.WriteLine("[DEBUG] Registered Systems:");
        foreach (var sys in emulatorManager.RegisteredSystems())
        {
            Console.WriteLine($" - {sys}");
        }

        // 3️⃣ Create the GameScanner
        var scanner = new GameScanner(emulatorManager);

        // 4️⃣ Set the DataContext for MVVM binding
        DataContext = new MainWindowViewModel(scanner);
    }
}



