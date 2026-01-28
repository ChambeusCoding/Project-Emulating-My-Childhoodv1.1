using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Launcher.Core.Emulation;

namespace Launcher.App;

public partial class App : Application
{
    public static EmulatorManager EmulatorManager { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        EmulatorManager = new EmulatorManager();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Views.MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}