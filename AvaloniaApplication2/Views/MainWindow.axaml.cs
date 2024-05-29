using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication2.Model;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        // InitializeComponent();
        AvaloniaXamlLoader.Load(this);
        DataContext = new MainWindowViewModel();
        // Инициализация заголовка окона
        StaticClass.Initialize(this);

    }

}
