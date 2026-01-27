// using System.ComponentModel;
//
// namespace Launcher.App.ViewModels;
//
// public class MainWindowViewModel : INotifyPropertyChanged
// {
//     public event PropertyChangedEventHandler? PropertyChanged;
//
//     private string _title = "Project Emulating My Childhood";
//
//     public string Title
//     {
//         get => _title;
//         set
//         {
//             if (_title != value)
//             {
//                 _title = value;
//                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
//             }
//         }
//     }
// }
using System.Collections.ObjectModel;

namespace Launcher.App.ViewModels;

public class MainWindowViewModel
{
    public ObservableCollection<GameViewModel> Games { get; } = new()
    {
        new GameViewModel("Super Mario 64"),
        new GameViewModel("Ocarina of Time"),
        new GameViewModel("Majora's Mask")
    };
}

