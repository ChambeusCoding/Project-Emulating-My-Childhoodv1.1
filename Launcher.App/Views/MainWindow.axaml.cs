using System; //Kept for learning reference
using System.IO;//Kept for learning reference
using System.Diagnostics;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Launcher.Core.Games;
using Launcher.Core.Emulation;
using Launcher.App.ViewModels;
using Avalonia.Interactivity;
using System.ComponentModel; //Kept for learning reference

namespace Launcher.App.Views;

public partial class MainWindow : Window
{
    private ScrollViewer? _terminalScrollViewer;
    private TextWriter? _originalOut;
    private TextWriter? _originalError;
    private TextBox? _terminalInputBox;
    

    public MainWindow()
    {
        InitializeComponent();
        
        var emulatorManager = new EmulatorManager();
        
        
        _terminalScrollViewer = this.FindControl<ScrollViewer>("TerminalScrollViewer");
        
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
        
        RedirectConsoleToTerminal();

        DataContextChanged += OnDataContextChanged;
    }
    
    private void MainWindow_OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            if (DataContext is MainWindowViewModel vm)
            {
            }
        }
    }

    private void OnTerminalKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;

        // BLOCK Ctrl+C/V clipboard crashes
        if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Handled = true;
            vm.CopyTerminalCommand.Execute(null);
            return;
        }
    
        if (e.Key == Key.V && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Handled = true;
            vm.PasteTerminalCommand.Execute(null);
            return;
        }
    
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            _ = vm.ExecuteTerminalCommand();
            return;
        }
    
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            vm.TerminalInput = "";
        }
    }

    
    private void RedirectConsoleToTerminal()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            _originalOut = Console.Out;
            _originalError = Console.Error;

            Console.SetOut(new TerminalTextWriter(vm.AppendTerminal));
            Console.SetError(new TerminalTextWriter(vm.AppendTerminal));
            
            Trace.Listeners.Add(new TerminalTraceListener(vm.AppendTerminal));
            Trace.AutoFlush = true;
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.PropertyChanged += async (s, args) =>
            {
                if (args.PropertyName == nameof(vm.TerminalOutput))
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        _terminalScrollViewer?.ScrollToEnd();
                    });
                }
            };

            _terminalScrollViewer = this.FindControl<ScrollViewer>("TerminalScrollViewer");
            _terminalInputBox = this.FindControl<TextBox>("TerminalInputBox");
        }
    }
    
    private void OnTerminalOutputTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_terminalScrollViewer != null)
        {
            DispatcherTimer.RunOnce(() => _terminalScrollViewer.ScrollToEnd(), 
                TimeSpan.FromMilliseconds(10));
        }
    }


    private void OnMainWindowLoaded(object? sender, RoutedEventArgs e)
    {
        _terminalInputBox ??= this.FindControl<TextBox>("TerminalInputBox");
        _terminalInputBox?.Focus();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (_originalOut is not null)
            Console.SetOut(_originalOut);
        if (_originalError is not null)
            Console.SetError(_originalError);

        base.OnClosed(e);
    }
}

public class TerminalTextWriter : TextWriter
{
    private readonly Action<string> _appendTerminal;
    private readonly StringBuilder _buffer = new();

    public TerminalTextWriter(Action<string> appendTerminal)
    {
        _appendTerminal = appendTerminal;
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        if (value is '\n' or '\r')
        {
            FlushLine();
        }
        else
        {
            _buffer.Append(value);
        }
    }

    public override void Write(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            foreach (char ch in value)
                Write(ch);
        }
    }

    public override void WriteLine(string? value)
    {
        if (!string.IsNullOrEmpty(value))
            _appendTerminal(value);
    }

    private void FlushLine()
    {
        if (_buffer.Length == 0)
            return;

        _appendTerminal(_buffer.ToString());
        _buffer.Clear();
    }
}

public class TerminalTraceListener : TraceListener
{
    private readonly Action<string> _appendTerminal;

    public TerminalTraceListener(Action<string> appendTerminal)
    {
        _appendTerminal = appendTerminal;
    }

    public override void Write(string? message) =>
        _appendTerminal(message ?? "");

    public override void WriteLine(string? message) =>
        _appendTerminal(message ?? "");
}
