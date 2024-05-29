using Avalonia.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using AvaloniaApplication2.Model;
using MsBox.Avalonia.Enums;

namespace AvaloniaApplication2.Views;

public partial class FifthPageView : UserControl
{
  
    MainWindow mainWindow = new MainWindow(); // вывод окон
    SecondModel secondModel = new SecondModel(); // работа с бд и dbf
    public FifthPageView()
    {
        InitializeComponent();
    }
    async void Open(int position, ListBox Mylist)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Выберите DBF файл",
            AllowMultiple = false, // запрет множественного выбора 
            Filters = new List<FileDialogFilter>
                {
                   new FileDialogFilter { Name = "DBF Files", Extensions = new List<string> { "dbf"} } 
                }
        };
        string filename = (await openFileDialog.ShowAsync(mainWindow)).FirstOrDefault();

        if (filename is not null && StaticClass.file_pathDBF.Any() && StaticClass.file_pathDBF.ContainsKey(System.IO.Path.GetFullPath(filename)))
        {
            StaticClass.ShowMessageBox("Файл с таким именем уже был добавлен!", "Предупреждение", ButtonEnum.Ok);
        }
        else if (filename is not null)
        {
            StaticClass.file_pathDBF.Add(System.IO.Path.GetFullPath(filename), position); // вставка в словарь 
            Mylist.Items.Add(System.IO.Path.GetFileName(filename)); // добавление в список для отображения
        }
    }
    void Clear(int position, ListBox Mylist)
    {
        Mylist.Items.Clear(); // очистка
        foreach (var kvp in StaticClass.file_pathDBF.Where(pair => pair.Value == position).ToList())
        {
            StaticClass.file_pathDBF.Remove(kvp.Key);
        }
    }
    private void Button_ClickOpen1(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {Open(1, listBox);}
    private void Button_ClickOpen2(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {Open(2, listBox1);}
    private void Button_ClickOpen3(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {Open(3, listBox2);}
    private void Button_ClickClear1(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {Clear(1, listBox);}
    private void Button_ClickClear2(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {Clear(2, listBox1);}
    private void Button_ClickClear3(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {Clear(3, listBox2);}
    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e){

        secondModel.WriteDBFInSQL();
    }
}
