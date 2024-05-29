using Avalonia.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using AvaloniaApplication2.Model;
using MsBox.Avalonia.Enums;

namespace AvaloniaApplication2.Views;

public partial class FifthPageView : UserControl
{
  
    MainWindow mainWindow = new MainWindow(); // ����� ����
    SecondModel secondModel = new SecondModel(); // ������ � �� � dbf
    public FifthPageView()
    {
        InitializeComponent();
    }
    async void Open(int position, ListBox Mylist)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "�������� DBF ����",
            AllowMultiple = false, // ������ �������������� ������ 
            Filters = new List<FileDialogFilter>
                {
                   new FileDialogFilter { Name = "DBF Files", Extensions = new List<string> { "dbf"} } 
                }
        };
        string filename = (await openFileDialog.ShowAsync(mainWindow)).FirstOrDefault();

        if (filename is not null && StaticClass.file_pathDBF.Any() && StaticClass.file_pathDBF.ContainsKey(System.IO.Path.GetFullPath(filename)))
        {
            StaticClass.ShowMessageBox("���� � ����� ������ ��� ��� ��������!", "��������������", ButtonEnum.Ok);
        }
        else if (filename is not null)
        {
            StaticClass.file_pathDBF.Add(System.IO.Path.GetFullPath(filename), position); // ������� � ������� 
            Mylist.Items.Add(System.IO.Path.GetFileName(filename)); // ���������� � ������ ��� �����������
        }
    }
    void Clear(int position, ListBox Mylist)
    {
        Mylist.Items.Clear(); // �������
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
