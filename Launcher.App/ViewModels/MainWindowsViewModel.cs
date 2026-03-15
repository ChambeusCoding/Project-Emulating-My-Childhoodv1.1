using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text;
using Avalonia.Threading;
using Avalonia.Controls;
using Launcher.Core.Games;
using Launcher.Core.Emulation;
using Launcher.App.Common;

namespace Launcher.App.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly GameScanner _scanner;
        private readonly EmulatorManager _emulators;
        
        public ICommand CopyTerminalCommand { get; }
        public ICommand PasteTerminalCommand { get; }

        private bool _debugRunning;
        private readonly DispatcherTimer _terminalFlushTimer;
        private readonly DispatcherTimer _tickerTimer;

        public ICommand ExecuteTerminalCommandCommand { get; }
        public ICommand ScanGamesCommand { get; }
        public ICommand SelectSystemCommand { get; }
        public ICommand LaunchGameCommand { get; }
        public ICommand InstallSelectedEmulatorCommand { get; }
        
        private string _statusText = "Welcome to RetroRunner! Install emulators, scan ROMs, launch games. Trust the terminal output.";
        public string StatusText 
        { 
            get => _statusText; 
            set 
            {
                if (_statusText != value) 
                {
                    _statusText = value; 
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> EmulatorInstallers { get; } =
            new ObservableCollection<string> { "SNES9x", "Mupen64Plus", "Azahar", "MelonDS"};

        private string? _selectedEmulatorInstaller;
        public string? SelectedEmulatorInstaller
        {
            get => _selectedEmulatorInstaller;
            set
            {
                if (_selectedEmulatorInstaller == value) return;
                _selectedEmulatorInstaller = value;
                OnPropertyChanged();
            }
        }

        private string _terminalOutput = "";
        public string TerminalOutput
        {
            get => _terminalOutput;
            set
            {
                if (_terminalOutput == value) return;
                _terminalOutput = value;
                OnPropertyChanged();
            }
        }

        private string _terminalInput = "";
        public string TerminalInput
        {
            get => _terminalInput;
            set
            {
                if (_terminalInput == value) return;
                _terminalInput = value;
                OnPropertyChanged();
            }
        }
        
        private readonly StringBuilder _terminalBuffer = new();
        private readonly object _terminalLock = new();

        // Observable collections - declared here for completeness
        public ObservableCollection<GameEntry> Games { get; }
        public ObservableCollection<GameEntry> FilteredGames { get; }
        public ObservableCollection<string> Systems { get; }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private string _selectedSystem = "All";
        public string SelectedSystem
        {
            get => _selectedSystem;
            set
            {
                if (_selectedSystem == value) return;
                _selectedSystem = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public MainWindowViewModel(GameScanner scanner)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _emulators = scanner.EmulatorManager;

            Games = new ObservableCollection<GameEntry>();
            FilteredGames = new ObservableCollection<GameEntry>();
            Systems = new ObservableCollection<string>();

            ExecuteTerminalCommandCommand = new RelayCommand(() => _ = ExecuteTerminalCommand());
            ScanGamesCommand = new RelayCommand(ScanGames);
            SelectSystemCommand = new RelayCommand<string>(SelectSystem);
            LaunchGameCommand = new RelayCommand<GameEntry>(game => _ = LaunchGameAsync(game));
            InstallSelectedEmulatorCommand = new RelayCommand(() => _ = RunSelectedInstaller());
            CopyTerminalCommand = new RelayCommand(async () => await CopyTerminalOutput());
            PasteTerminalCommand = new RelayCommand(async () => await PasteToTerminal());

            _terminalFlushTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _terminalFlushTimer.Tick += (s, e) => FlushTerminalBuffer();
            _terminalFlushTimer.Start();

            _tickerTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _tickerTimer.Tick += TickerTick;

            LoadSystems();
            AppendTerminal("[INIT] RetroRunner ready! Use 'help' in terminal.");
        }

        private void TickerTick(object? sender, EventArgs e) { }
        private void UpdateTicker() { }

        public void StartNewsTicker()
        {
            _tickerTimer.Start();
            StatusText = "🎮 RetroRunner | Ready to scan & launch games";
        }

        // ================= TERMINAL =================
        private async Task CopyTerminalOutput()
        {
            try
            {
                if (string.IsNullOrEmpty(TerminalOutput)) return;

                if (Avalonia.Controls.TopLevel.GetTopLevel(null) is { Clipboard: { } clipboard })
                {
                    await clipboard.SetTextAsync(TerminalOutput);
                    AppendTerminal("[CLIPBOARD] Output copied!");
                    StatusText = "📋 Terminal copied";
                }
            }
            catch (Exception ex)
            {
                AppendTerminal($"[ERROR] Copy failed: {ex.Message}");
            }
        }

        private async Task PasteToTerminal()
        {
            try
            {
                if (Avalonia.Controls.TopLevel.GetTopLevel(null) is { Clipboard: { } clipboard })
                {
                    var text = await clipboard.GetTextAsync();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        TerminalInput = text;
                        AppendTerminal($"[PASTE] {text.Substring(0, Math.Min(50, text.Length))}...");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendTerminal($"[ERROR] Paste failed: {ex.Message}");
            }
        }

        public void AppendTerminal(string line)
        {
            if (ShouldFilter(line)) return;

            var timestamped = $"[{DateTime.Now:HH:mm:ss}] {line}\n";
            lock (_terminalLock)
            {
                _terminalBuffer.Append(timestamped);
            }
            StatusText = line;
        }

        private void FlushTerminalBuffer()
        {
            lock (_terminalLock)
            {
                if (_terminalBuffer.Length > 0)
                {
                    TerminalOutput += _terminalBuffer.ToString();
                    _terminalBuffer.Clear();
                }
            }
        }

        private static bool ShouldFilter(string line) =>
            line.Contains("[IME]") || 
            line.Contains("Tmds.DBus") || 
            line.Contains("org.freedesktop.DBus") ||
            line.Contains("IBus");

        public async Task ExecuteTerminalCommand()
        {
            var input = TerminalInput.Trim();
            TerminalInput = "";

            if (string.IsNullOrWhiteSpace(input)) return;

            AppendTerminal($"$ {input}");
            
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            switch (cmd)
            {
                case "help":
                    AppendTerminal("Commands: help, clear, scan, debug");
                    break;
                case "clear":
                    TerminalOutput = "";
                    break;
                case "scan":
                    ScanGames();
                    break;
                case "debug":
                    await RunDebugger();
                    break;
                default:
                    await RunShellCommand(input);
                    break;
            }
        }
        private async Task RunDebugger()
        {
            if (_debugRunning) 
            {
                AppendTerminal("[WARN] Debug already running");
                return;
            }
            _debugRunning = true;
            try
            {
                var exe = Environment.ProcessPath!;
                await RunShellCommand($"DOTNET_ENVIRONMENT=Development \"{exe}\"");
            }
            finally { _debugRunning = false; }
        }

        private async Task RunShellCommand(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-l -i -c \"{command}\"",  // 🔥 Added -l -i for login shell
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), // 🔥 HOME dir
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
        
                // 🔥 TERMINAL ENVIRONMENT - matches Rider exactly
                EnvironmentVariables =
                {
                    ["TERM"] = "xterm-256color",
                    ["COLORTERM"] = "truecolor", 
                    ["LANG"] = "en_US.UTF-8",
                    ["PATH"] = Environment.GetEnvironmentVariable("PATH") ?? "",
                    ["HOME"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                }
            };

            using var process = Process.Start(psi)!;
    
            async Task ReadStream(StreamReader reader)
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (line != null) AppendTerminal(line);
                    }
                }
                catch { }
            }

            await Task.WhenAll(
                ReadStream(process.StandardOutput),
                ReadStream(process.StandardError)
            );
            await process.WaitForExitAsync();
        }


        // ================= EMULATOR INSTALLERS =================
        private async Task RunSelectedInstaller()
{
    if (string.IsNullOrWhiteSpace(SelectedEmulatorInstaller))
    {
        AppendTerminal("[INSTALL] Select an emulator first");
        return;
    }

    var scriptName = SelectedEmulatorInstaller switch
    {
        "SNES9x" => "install_snes9x.sh",
        "Mupen64Plus" => "install_mupen64plus.sh",
        "Azahar" => "install_azahar.sh",
        "MelonDS" => "install_melonds.sh",
        _ => null
    };

    if (scriptName == null)
    {
        AppendTerminal($"[INSTALL] No script for {SelectedEmulatorInstaller}");
        return;
    }

    var scriptsDir = Path.Combine(AppContext.BaseDirectory, "Installers");
    var scriptPath = Path.Combine(scriptsDir, scriptName);

    if (!File.Exists(scriptPath))
    {
        AppendTerminal($"[INSTALL] Missing: {scriptPath}");
        return;
    }

    AppendTerminal($"[INSTALL] Running {scriptName}...");

    // 🔥 FIXED VERSION - This makes it identical to Rider terminal
    var psi = new ProcessStartInfo
    {
        FileName = "/bin/bash",
        Arguments = $"\"{scriptPath}\"",
        WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), // ← HOME dir
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        
        // 🔥 TERMINAL ENVIRONMENT - matches Rider exactly
        EnvironmentVariables =
        {
            ["TERM"] = "xterm-256color",
            ["COLORTERM"] = "truecolor",
            ["LANG"] = "en_US.UTF-8",
            ["PATH"] = Environment.GetEnvironmentVariable("PATH") ?? "",
            ["HOME"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        }
    };

    using var proc = Process.Start(psi)!;

    async Task ReadStream(StreamReader reader)
    {
        try
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line != null) AppendTerminal(line);
            }
        }
        catch { }
    }

    await Task.WhenAll(
        ReadStream(proc.StandardOutput),
        ReadStream(proc.StandardError)
    );

    await proc.WaitForExitAsync();
    AppendTerminal($"[INSTALL] Done (code: {proc.ExitCode})");
}



        // ================= GAMES =================
        private void ScanGames()
        {
            AppendTerminal("[SCAN] Scanning ~/Documents/ROMs...");
            
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var romsDir = Path.Combine(home, "Documents", "ROMs");
            
            Games.Clear();
            
            if (!Directory.Exists(romsDir))
            {
                AppendTerminal($"[SCAN] Create ~/Documents/ROMs first");
                return;
            }

            foreach (var game in _scanner.Scan(romsDir))
            {
                game.System ??= "Unknown";
                game.BoxArtPath ??= "avares://Launcher.App/Assets/placeholder.png";
                Games.Add(game);
                AppendTerminal($"[SCAN] + {game.Title}");
            }
            
            ApplyFilters();
            AppendTerminal($"[SCAN] Found {Games.Count} games");
        }

        private void SelectSystem(string system) => SelectedSystem = system;

        private async Task LaunchGameAsync(GameEntry game)
        {
            if (game?.EmulatorId == null)
            {
                AppendTerminal("[LAUNCH] Invalid game");
                return;
            }

            AppendTerminal($"[LAUNCH] {game.Title}...");
    
            var emulator = _emulators.GetEmulators(game.System)
                .FirstOrDefault(e => e.Manifest.Id == game.EmulatorId);

            if (emulator == null)
            {
                AppendTerminal($"[LAUNCH] No emulator: {game.EmulatorId}");
                return;
            }

            try
            {
                string romPath = game.FilePath;  // Use game.FilePath consistently
                string romDirectory = Path.GetDirectoryName(romPath)!;
        
                var (exe, args) = emulator.BuildLaunchCommand(romPath);
        
                AppendTerminal($"[LAUNCH] Executable: {exe}");
                AppendTerminal($"[LAUNCH] WorkingDirectory: {romDirectory}");
        
                await RunProcessAsync(exe, args, romDirectory);  // ✅ All 3 parameters defined!
            }
            catch (Exception ex)
            {
                AppendTerminal($"[LAUNCH] Error: {ex.Message}");
            }
        }


        private async Task RunProcessAsync(string fileName, string arguments, string workingDirectory)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,  // ← FIXED! Clean path from plugins
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                EnvironmentVariables =
                {
                    ["TERM"] = "xterm-256color",
                    ["LANG"] = "en_US.UTF-8", 
                    ["PATH"] = Environment.GetEnvironmentVariable("PATH") ?? ""
                }
            };

            using var process = Process.Start(psi)!;
    
            async Task ReadStream(StreamReader reader)
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (line != null) AppendTerminal(line);
                    }
                }
                catch { }
            }

            await Task.WhenAll(
                ReadStream(process.StandardOutput),
                ReadStream(process.StandardError)
            );
            await process.WaitForExitAsync();
        }


        private void ApplyFilters()
        {
            FilteredGames.Clear();
            foreach (var game in Games)
            {
                if ((SelectedSystem == "All" || game.System == SelectedSystem) &&
                    (string.IsNullOrEmpty(SearchText) || 
                     game.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                {
                    FilteredGames.Add(game);
                }
            }
        }

        private void LoadSystems()
        {
            Systems.Clear();
            Systems.Add("All");
            foreach (var system in _emulators.RegisteredSystems())
                Systems.Add(system);
        }
    }
}
