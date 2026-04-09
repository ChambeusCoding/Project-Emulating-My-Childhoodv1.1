using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Launcher.Core.Games;

public sealed class GameEntry : INotifyPropertyChanged
{
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;

    public string? System { get; set; }
    public string? EmulatorId { get; set; }

    private string? _boxArtPath;
    public string? BoxArtPath
    {
        get => _boxArtPath;
        set
        {
            if (_boxArtPath != value)
            {
                _boxArtPath = value;
                OnPropertyChanged();
            }
        }
    }
    
    public string? _selectedGame { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}