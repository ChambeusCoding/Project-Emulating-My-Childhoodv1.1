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
namespace Launcher.App.ViewModels;

public class MainWindowViewModel
{
    public string Title => "EMCbetav1.2 🎮";
}
