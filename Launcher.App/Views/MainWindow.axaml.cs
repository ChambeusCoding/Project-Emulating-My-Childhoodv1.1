using System;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;  // ← Already there for Trace
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Launcher.Core.Games;
using Launcher.Core.Emulation;
using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class MainWindow : Window
{
    private ScrollViewer? _terminalScrollViewer;
    private TextWriter? _originalOut;
    private TextWriter? _originalError;

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

        Console.WriteLine("[DEBUG] Registered Systems:");
        foreach (var sys in emulatorManager.RegisteredSystems())
        {
            Console.WriteLine($" - {sys}");
        }

        var scanner = new GameScanner(emulatorManager);
        DataContext = new MainWindowViewModel(scanner);

        // 🔥 Redirect Console output to UI terminal
        RedirectConsoleToTerminal();

        DataContextChanged += OnDataContextChanged;
    }

    private void RedirectConsoleToTerminal()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            _originalOut = Console.Out;
            _originalError = Console.Error;

            Console.SetOut(new TerminalTextWriter(vm.AppendTerminal));
            Console.SetError(new TerminalTextWriter(vm.AppendTerminal));
            
            // FIXED: Use Trace.Listeners instead of Debug.Listeners
            Trace.Listeners.Add(new TerminalTraceListener(vm.AppendTerminal));
            Trace.AutoFlush = true;
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(vm.TerminalOutput))
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        _terminalScrollViewer?.ScrollToEnd();
                    });
                }
            };

            _terminalScrollViewer = this.FindControl<ScrollViewer>("TerminalScrollViewer");
        }
    }

    private async void OnTerminalKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        e.Handled = true;

        if (DataContext is not MainWindowViewModel vm)
            return;

        var command = vm.TerminalInput.Trim();
        if (string.IsNullOrWhiteSpace(command))
            return;

        vm.TerminalInput = "";
        await vm.ExecuteTerminalCommand();
    }

    private void OnTerminalKeyDownPreview(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && DataContext is MainWindowViewModel vm)
        {
            vm.TerminalInput = "";
            e.Handled = true;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        // Restore original console writers
        if (_originalOut != null)
            Console.SetOut(_originalOut);
        if (_originalError != null)
            Console.SetError(_originalError);
        
        base.OnClosed(e);
    }
}

// Terminal redirector classes (same as before)
public class TerminalTextWriter : TextWriter
{
    private readonly Action<string> _appendTerminal;
    private readonly StringBuilder _lineBuilder = new();

    public TerminalTextWriter(Action<string> appendTerminal)
    {
        _appendTerminal = appendTerminal;
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        if (value == '\n' || value == '\r')
        {
            FlushLine();
        }
        else
        {
            _lineBuilder.Append(value);
        }
    }

    public override void Write(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            foreach (char c in value)
            {
                Write(c);
            }
        }
    }

    public override void WriteLine(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _appendTerminal(value);
        }
    }

    private void FlushLine()
    {
        if (_lineBuilder.Length > 0)
        {
            _appendTerminal(_lineBuilder.ToString());
            _lineBuilder.Clear();
        }
    }
}

public class TerminalTraceListener : TraceListener
{
    private readonly Action<string> _appendTerminal;

    public TerminalTraceListener(Action<string> appendTerminal)
    {
        _appendTerminal = appendTerminal;
    }

    public override void WriteLine(string? message)
    {
        _appendTerminal(message ?? "");
    }

    public override void Write(string? message)
    {
        _appendTerminal(message ?? "");
    }
}
