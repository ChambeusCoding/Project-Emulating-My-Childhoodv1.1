using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;

using Launcher.Core.Games;
using Launcher.Core.Emulation;
using Launcher.App.Common;

namespace Launcher.App.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly GameScanner _scanner;
        private readonly EmulatorManager _emulators;

        private bool _debugRunning;

        public ICommand ExecuteTerminalCommandCommand { get; }
        public ICommand ScanGamesCommand { get; }
        public ICommand SelectSystemCommand { get; }
        public ICommand LaunchGameCommand { get; }

        // NEW: installer dropdown + command
        public ObservableCollection<string> EmulatorInstallers { get; } =
            new ObservableCollection<string> { "SNES9x", "Mupen64Plus", "Azahar" };

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

        public ICommand InstallSelectedEmulatorCommand { get; }

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

        public MainWindowViewModel(GameScanner scanner)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _emulators = scanner.EmulatorManager;

            Games = new ObservableCollection<GameEntry>();
            FilteredGames = new ObservableCollection<GameEntry>();
            Systems = new ObservableCollection<string>();

            ExecuteTerminalCommandCommand =
                new RelayCommand(() => _ = ExecuteTerminalCommand());

            ScanGamesCommand = new RelayCommand(ScanGames);
            SelectSystemCommand = new RelayCommand<string>(SelectSystem);
            LaunchGameCommand = new RelayCommand<GameEntry>(
                game => _ = LaunchGameAsync(game)
            );

            // NEW: hook up installer command
            InstallSelectedEmulatorCommand =
                new RelayCommand(() => _ = RunSelectedInstaller());

            LoadSystems();
        }

        // ================= TERMINAL =================

        private async Task RunShellCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;

            AppendTerminal($"$ {command}");

            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi)!;

            async Task ReadStreamAsync(System.IO.StreamReader reader)
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (line != null)
                        AppendTerminal(line);
                }
            }

            await Task.WhenAll(
                ReadStreamAsync(process.StandardOutput),
                ReadStreamAsync(process.StandardError)
            );

            await process.WaitForExitAsync();
        }

        // generic process runner that streams output into the UI terminal
        private async Task RunProcessAsync(string fileName, string arguments)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            AppendTerminal($"$ {fileName} {arguments}");

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi)!;

            async Task ReadStreamAsync(System.IO.StreamReader reader)
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (line != null)
                        AppendTerminal(line);
                }
            }

            await Task.WhenAll(
                ReadStreamAsync(process.StandardOutput),
                ReadStreamAsync(process.StandardError)
            );

            await process.WaitForExitAsync();
        }

        private async Task RunDebugger()
        {
            if (_debugRunning)
            {
                AppendTerminal("[WARN] Debug instance already running.");
                return;
            }

            _debugRunning = true;

            try
            {
                AppendTerminal("[DEBUG] Relaunching in Development mode...");
                var exe = Environment.ProcessPath!;
                await RunShellCommand($"DOTNET_ENVIRONMENT=Development \"{exe}\"");
            }
            finally
            {
                _debugRunning = false;
            }
        }

        public async Task ExecuteTerminalCommand()
        {
            AppendTerminal("[DEBUG] ExecuteTerminalCommand() invoked");

            var input = TerminalInput.Trim();
            TerminalInput = "";

            if (string.IsNullOrWhiteSpace(input))
                return;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            switch (cmd)
            {
                case "help":
                    AppendTerminal("Commands:");
                    AppendTerminal("  help   - show commands");
                    AppendTerminal("  clear  - clear terminal");
                    AppendTerminal("  debug  - relaunch app in dev mode");
                    break;

                case "clear":
                    TerminalOutput = "";
                    break;

                case "debug":
                    await RunDebugger();
                    break;

                default:
                    await RunShellCommand(input);
                    break;
            }
        }

        public void AppendTerminal(string line)
        {
            Dispatcher.UIThread.Post(() =>
            {
                TerminalOutput +=
                    $"[{DateTime.Now:HH:mm:ss}] {line}{Environment.NewLine}";
            });
        }

        // ================= EMULATOR INSTALLERS (NEW) =================

        private async Task RunSelectedInstaller()
        {
            if (string.IsNullOrWhiteSpace(SelectedEmulatorInstaller))
            {
                AppendTerminal("[INSTALL] No emulator selected.");
                return;
            }

            var name = SelectedEmulatorInstaller;

            var scriptName = name switch
            {
                "SNES9x"      => "install_snes9x.sh",
                "Mupen64Plus" => "install_mupen64plus.sh",
                "Azahar"      => "install_azahar.sh",
                _             => null
            };

            if (scriptName is null)
            {
                AppendTerminal($"[INSTALL] No script mapped for '{name}'.");
                return;
            }

            var scriptsDir = System.IO.Path.Combine(AppContext.BaseDirectory, "Installers");
            var scriptPath = System.IO.Path.Combine(scriptsDir, scriptName);

            if (!System.IO.File.Exists(scriptPath))
            {
                AppendTerminal($"[INSTALL] Script not found: {scriptPath}");
                return;
            }

            AppendTerminal($"[INSTALL] Running {scriptName}...");

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "/usr/bin/env",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                psi.ArgumentList.Add("bash");
                psi.ArgumentList.Add(scriptPath);

                using var proc = Process.Start(psi)!;

                async Task ReadStreamAsync(System.IO.StreamReader reader)
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (line != null)
                            AppendTerminal(line);
                    }
                }

                await Task.WhenAll(
                    ReadStreamAsync(proc.StandardOutput),
                    ReadStreamAsync(proc.StandardError)
                );

                await proc.WaitForExitAsync();
                AppendTerminal($"[INSTALL] Exit code: {proc.ExitCode}");
            }
            catch (Exception ex)
            {
                AppendTerminal($"[INSTALL] Failed: {ex.Message}");
            }
        }

        // ================= GAMES =================

        public ObservableCollection<GameEntry> Games { get; }
        public ObservableCollection<GameEntry> FilteredGames { get; }
        public ObservableCollection<string> Systems { get; }

        private string _searchText = string.Empty;
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

        private void ScanGames()
        {
            // Cross‑platform user home
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var gameFolders = new[]
            {
                Path.Combine(home, "Documents"),
                Path.Combine(home, "Documents", "ROMs"),
            };

            Games.Clear();

            foreach (var folder in gameFolders.Where(Directory.Exists))
            {
                foreach (var game in _scanner.Scan(folder))
                {
                    game.System ??= "Unknown";
                    game.BoxArtPath ??= "avares://Launcher.App/Assets/placeholder.png";
                    Games.Add(game);
                }
            }

            ApplyFilters();
        }


        private void SelectSystem(string system) => SelectedSystem = system;

        // uses plugin BuildLaunchCommand + RunProcessAsync
        private async Task LaunchGameAsync(GameEntry game)
        {
            if (game?.EmulatorId == null || game.System == null)
                return;

            var emulator = _emulators
                .GetEmulators(game.System)
                .FirstOrDefault(e => e.Manifest.Id == game.EmulatorId);

            if (emulator == null)
                return;

            var (exe, args) = emulator.BuildLaunchCommand(game.FilePath);

            await RunProcessAsync(exe, args);
        }

        private void ApplyFilters()
        {
            FilteredGames.Clear();

            foreach (var game in Games)
            {
                if ((SelectedSystem == "All" || game.System == SelectedSystem) &&
                    (string.IsNullOrWhiteSpace(SearchText) ||
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
