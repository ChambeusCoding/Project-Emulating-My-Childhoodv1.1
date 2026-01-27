using Avalonia.Controls;
using Launcher.App.ViewModels;

namespace Launcher.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}