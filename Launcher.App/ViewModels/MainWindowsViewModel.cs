using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Launcher.Core.Games;
using Launcher.Core.Emulation;
using Launcher.App.Common;

namespace Launcher.App.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly GameScanner _scanner;
        private readonly EmulatorManager _emulators;

        public MainWindowViewModel(GameScanner scanner)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _emulators = scanner.EmulatorManager;

            Games = new ObservableCollection<GameEntry>();
            FilteredGames = new ObservableCollection<GameEntry>();
            Systems = new ObservableCollection<string>();

            ScanGamesCommand = new RelayCommand(ScanGames);
            SelectSystemCommand = new RelayCommand<string>(SelectSystem);
            LaunchGameCommand = new RelayCommand<GameEntry>(
                game => _ = LaunchGameAsync(game)
            );

            LoadSystems();
        }

        // ========================
        // Collections
        // ========================

        public ObservableCollection<GameEntry> Games { get; }
        public ObservableCollection<GameEntry> FilteredGames { get; }
        public ObservableCollection<string> Systems { get; }

        // ========================
        // Properties
        // ========================

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

        // ========================
        // Commands
        // ========================

#pragma warning disable CS0414
        public ICommand ScanGamesCommand { get; }
        public ICommand SelectSystemCommand { get; }
        public ICommand LaunchGameCommand { get; }
#pragma warning restore CS0414


        // ========================
        // Command Logic
        // ========================

        private void ScanGames()
        {
            var gameFolders = new[]
            {
                "/home/chambeus/Documents/Emulators/N64",
                "/home/chambersj/Documents/Emulators/N64",
            };

            Games.Clear();

            foreach (var folder in gameFolders)
            {
                Console.WriteLine($"[ScanGames] Scanning folder: {folder}");

                if (!Directory.Exists(folder))
                {
                    Console.WriteLine($"[ScanGames] Folder does not exist: {folder}");
                    continue;
                }

                foreach (var game in _scanner.Scan(folder))
                {
                    game.System ??= "Unknown";
                    game.BoxArtPath ??= "avares://Launcher.App/Assets/placeholder.png";
                    Games.Add(game);
                }
            }

            ApplyFilters();
        }



        private void SelectSystem(string system)
        {
            SelectedSystem = system;
        }

        private async Task LaunchGameAsync(GameEntry game)
        {
            if (game?.EmulatorId == null || game.System == null)
                return;

            var emulator = _emulators
                .GetEmulators(game.System)
                .FirstOrDefault(e => e.Manifest.Id == game.EmulatorId);

            if (emulator != null)
                await emulator.LaunchAsync(game.FilePath);
        }

        // ========================
        // Helpers
        // ========================

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
