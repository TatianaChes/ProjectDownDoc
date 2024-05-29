using Avalonia.Media;
using AvaloniaApplication2.Model;
using AvaloniaApplication2.Views;
using DynamicData;
using ReactiveUI;
using System;
using System.Windows.Input;

namespace AvaloniaApplication2.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        _CurrentPage = Pages[0];
        MessageBackground = brushes[0];
        ContentButton = "Войти";
        isButtonVisible = true;
        var canNavNext = this.WhenAnyValue(x => x.CurrentPage.CanNavigateNext);
        var canNavPrev = this.WhenAnyValue(x => x.CurrentPage.CanNavigatePrevious);
        NavigateNextCommand = ReactiveCommand.Create(NavigateNext, canNavNext);
        NavigatePreviousCommand = ReactiveCommand.Create(NavigatePrevious, canNavPrev);
    }


    private string? _ContentButton;
    public string? ContentButton
    {
        get { return _ContentButton; }
        set { this.RaiseAndSetIfChanged(ref _ContentButton, value); }
    }

    private IBrush _MessageBackground;
    public IBrush MessageBackground
    {
        get { return _MessageBackground; }
        set
        {
            this.RaiseAndSetIfChanged(ref _MessageBackground, value);
        }
    }
    private bool _isButtonVisible;
    public bool isButtonVisible
    {
        get { return _isButtonVisible; }
        set { this.RaiseAndSetIfChanged(ref _isButtonVisible, value); }
    }

    private bool _isButtonPrevVisible;
    public bool isButtonPrevVisible
    {
        get { return _isButtonPrevVisible; }
        set { this.RaiseAndSetIfChanged(ref _isButtonPrevVisible, value); }
    }

    SolidColorBrush[] brushes = new[] {
    new SolidColorBrush(Colors.Green),
    new SolidColorBrush(Colors.Blue),
    new SolidColorBrush(Colors.Blue),
    new SolidColorBrush(Colors.Transparent),
    };

    private readonly PageViewModelBase[] Pages =
    {
        new SecondPageViewModel(),
        new FifthPageViewModel(),
        new ThirdPageViewModel(),
        new FourthPageViewModel()
    };

    private PageViewModelBase _CurrentPage;
    public PageViewModelBase CurrentPage
    {
        get { return _CurrentPage; }
        set { this.RaiseAndSetIfChanged(ref _CurrentPage, value); }
    }


    public ICommand NavigateNextCommand { get; }
    private void NavigateNext()
    {
        var index = Pages.IndexOf(CurrentPage) + 1;
        MessageBackground = brushes[index];
        CurrentPage = Pages[index];
        ContentButton = "Далее";
        if (index == 3)
        { isButtonVisible = false; }
        if (index == 2)
        { isButtonPrevVisible = true; }
        else { isButtonPrevVisible = false; }

    }
    public ICommand NavigatePreviousCommand { get; }

    private void NavigatePrevious()
    {
        var index = Pages.IndexOf(CurrentPage) - 1;
        CurrentPage = Pages[index];
        if (index == 2)
        { isButtonPrevVisible = true; }
        else { isButtonPrevVisible = false; }
    }

}
