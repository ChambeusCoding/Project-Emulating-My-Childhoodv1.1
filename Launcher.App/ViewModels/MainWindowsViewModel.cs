using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Launcher.Core.Games;       // GameEntry, GameScanner
using Launcher.Core.Emulation;   // EmulatorManager


namespace Launcher.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly GameScanner _scanner;
        private readonly EmulatorManager _emulators;

        public MainWindowViewModel(GameScanner scanner)
        {
            _scanner = scanner;
            _emulators = scanner.EmulatorManager; // make EmulatorManager accessible in scanner

            Games = new ObservableCollection<GameEntry>();
            FilteredGames = new ObservableCollection<GameEntry>();
            Systems = new ObservableCollection<string>();

            ScanGamesCommand = new RelayCommand(ScanGames);
            SelectSystemCommand = new RelayCommand<string>(system =>
            {
                SelectedSystem = system;
            });

            LaunchGameCommand = new RelayCommand<GameEntry>(async game =>
            {
                if (game.EmulatorId == null) return;
                var emulator = _emulators.GetEmulators(game.System)
                    .FirstOrDefault(e => e.Manifest.Id == game.EmulatorId);
                if (emulator != null)
                    await emulator.LaunchAsync(game.FilePath);
            });

            LoadSystems();
        }

        public ObservableCollection<GameEntry> Games { get; }
        public ObservableCollection<GameEntry> FilteredGames { get; }
        public ObservableCollection<string> Systems { get; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        private string _selectedSystem = "All";
        public string SelectedSystem
        {
            get => _selectedSystem;
            set
            {
                if (_selectedSystem != value)
                {
                    _selectedSystem = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        public ICommand ScanGamesCommand { get; }
        public ICommand SelectSystemCommand { get; }
        public ICommand LaunchGameCommand { get; }

        public void ScanGames()
        {
            // Replace with your actual games folder path
            string gamesFolder = @"C:\Games";

            Games.Clear();
            foreach (var game in _scanner.Scan(gamesFolder))
            {
                // Make sure System is populated in GameEntry
                game.System ??= _emulators.FindForRom(game.FilePath)?.Manifest.System ?? "Unknown";

                // Optional: set a placeholder BoxArtPath
                game.BoxArtPath ??= "avares://Launcher.App/Assets/placeholder.png";

                Games.Add(game);
            }
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            FilteredGames.Clear();
            foreach (var game in Games)
            {
                if ((SelectedSystem == "All" || game.System == SelectedSystem) &&
                    (string.IsNullOrWhiteSpace(SearchText) || game.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
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

    // Simple RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action execute) : this(o => execute(), null) { }

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add { }   // do nothing
            remove { } // do nothing
        }

    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T)parameter!) ?? true;

        public void Execute(object? parameter) => _execute((T)parameter!);

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }

        }
    }
}
