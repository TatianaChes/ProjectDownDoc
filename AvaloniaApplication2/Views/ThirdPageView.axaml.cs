using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication2.Model;
using AvaloniaApplication2.ViewModels;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AvaloniaApplication2.Views
{
   public partial class ThirdPageView : UserControl
    {
        MainWindow mainWindow = new MainWindow(); // ��� ����������� ���� ������ �����
        ThirdModel thirdModel = new ThirdModel(); // ������ � excel ������
        SecondModel secondModel = new SecondModel(); // ������ � ��
        string[] string_array; // ������ ��� ����������� 
        // ������� ������������ ������ (key:���� � ����� - value:����� ������, � ������� ����������) 
        Dictionary<string, int> file_path = new Dictionary<string, int>();
        int owner; // ��������� ���������� ����������� �� comboBox

        public ThirdPageView()
        {
            InitializeComponent();
            string_array = new string[] { "����", "1191", "���. ��������" }; // ���������� combobox ��� �������� ���� 
            WrapPanel? wrapPannel = this.FindControl<WrapPanel>("wrapPannel");
            Button? buttonOk = this.FindControl<Button>("buttonOk");
            Button? buttonRead = this.FindControl<Button>("buttonRead");
            TextBox? textBox = this.FindControl<TextBox>("number");
            buttonOk.Click += AddItemsBbutton_Click;
            buttonRead.Click += ReadFiles_Click;
        }

        private void AddItemsBbutton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (int.TryParse(number.Text?.ToString(), out int num)) {
                wrapPannel.Children.Clear(); // ������� ����� ������������ ���������� 
                file_path.Clear(); // �� ������, ���� ������������ ������ �������� ���������� � ��� ��������� �����
                int countInput = Convert.ToInt32(number.Text?.ToString()); // ��������� ���������� 
                create_elements(countInput); // ����� ���������� ��������� �� ����� 
            }
            else {
                StaticClass.ShowMessageBox("������� ����� � ���� ����������!", "��������������", ButtonEnum.Ok);
            } 
      
        }

        void create_elements(int count)
        {
            if (count > 6)
            {
                StaticClass.ShowMessageBox("�������� ����� �� ������ ��������� 6!", "�����������", ButtonEnum.Ok);
            }
            else
            {
                for (int i = 0; i < count; ++i)
                {
                    Button button = new Button()
                    {
                        Content = string.Format("�����", i),
                        Tag = i,
                        Width = 85,
                        Height = 30,
                        HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Margin = new Avalonia.Thickness(5, 0, 0, 0)
                    };

                    Button buttonClear = new Button()
                    {
                        Content = string.Format("��������", i),
                        Tag = i,
                        Width = 85,
                        Height = 30,
                        Margin = new Avalonia.Thickness(5, 0, 0, 0),
                        HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.DarkGray)
                    };

                    ComboBox comboBox = new ComboBox()
                    {
                        Name = string.Format("ComboBox{0}", i),
                        Width = 140,
                        Height = 27,
                        ItemsSource = string_array,
                        Margin = new Avalonia.Thickness(0, 0, 5, 0)

                    };
                    ListBox listbox = new ListBox()
                    {
                        Name = string.Format("listbox{0}", i),
                        Width = 450,
                        Height = 80,
                        Margin = new Avalonia.Thickness(2, 2, 2, 2)
                    };

                    wrapPannel.Children.Add(comboBox);
                    wrapPannel.Children.Add(listbox);
                    button.Click += buttonOpenFile_Click; // ���������� ������� ��� ������ 
                    wrapPannel.Children.Add(button);
                    wrapPannel.Children.Add(buttonClear);
                    buttonClear.Click += button_Clear_Click; // ���������� ������� ��� ������ 

                }
            }
        }
        void button_Clear_Click(object sender, RoutedEventArgs e)
        {
            int position = Convert.ToInt32(string.Format("{0}", (sender as Button).Tag)); // ������� ������ � ���� 
            var listBox = wrapPannel.Children.OfType<ListBox>().ElementAt(position); // ������ � �������� �� ������� 
            listBox.Items.Clear(); // �������
            foreach (var kvp in file_path.Where(pair => pair.Value == position).ToList())
            {
                file_path.Remove(kvp.Key);
            }
        }
        async void buttonOpenFile_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "�������� excel ����",
                AllowMultiple = true, // ������������� ����� ������
                Filters = new List<FileDialogFilter>
                {
                   new FileDialogFilter { Name = "Excel Files", Extensions = new List<string> { "xlsx", "xls" } } 
                }
            };

           string[] result = await openFileDialog.ShowAsync(mainWindow); // ��������� ��������� ������
           
           foreach (string filename in result)
           {
               int position = Convert.ToInt32(string.Format("{0}", (sender as Button).Tag)); // ������� ������ � ���� 
               var listBox = wrapPannel.Children.OfType<ListBox>().ElementAt(position); // ������ � �������� �� ������� 

               if (file_path.ContainsKey(System.IO.Path.GetFullPath(filename)))
               {
                   StaticClass.ShowMessageBox("���� � ����� ������ ��� ��� ��������!", "��������������", ButtonEnum.Ok);
               }
               else if (file_path.Where(pair => pair.Value == position).Select(pair1 => pair1.Value).Count() >= 2)
               {
                    StaticClass.ShowMessageBox("�� �� ������ �������� ������ 2 ������ �� ���� �����������!", "��������������", ButtonEnum.Ok);
               }
               else
               {
                   file_path.Add(System.IO.Path.GetFullPath(filename), position); // ������� � ������� 
                   listBox.Items.Add(System.IO.Path.GetFileName(filename)); // ���������� � ������ ��� �����������
               }
           }
        }
        private void ReadFiles_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            StaticClass.LetRead = true;
            if (file_path.Any())
            {
                foreach (var files in file_path) // ���������� ������ � ���������� ������ 
                {
                    var comboBox = wrapPannel.Children.OfType<ComboBox>().ElementAt(files.Value); // ������ � �������� �� ������� 
                    if (comboBox.SelectedIndex == -1)
                    {
                        StaticClass.ShowMessageBox("�� �� ������� �����������!", "��������������", ButtonEnum.Ok);
                        StaticClass.datas.Clear();
                        return;
                    }
                    else
                    {
                        switch (comboBox.SelectedItem.ToString())
                        {
                            case "����":
                                owner = 12;
                                break;
                            case "1191":
                                owner = 28;
                                break;
                            case "���. ��������":
                                owner = 18;
                                break;
                        }

                        thirdModel.WorkWithExcel(files.Key, owner); // ���������� excel ����� � ���������� � datas
                    }
                }
                // ��� ����� �� ������ �������� ������� �� ViewModel � ���������� ������
                if (DataContext is ThirdPageViewModel viewModel && StaticClass.LetRead == true && StaticClass.datas.Count > 0 && secondModel.WriteToDataBase()) // letread ��  thirdModel.WorkWithExcel
                {
                    // ������ � ��
                    viewModel.OnButtonClicked();
                    buttonRead.IsEnabled = false; // ������ �� ����������� ������
                }
                else {
                    StaticClass.datas.Clear();
                }
            }
            else
            {
                StaticClass.ShowMessageBox("���������� ���������� � �������� �����!", "��������������", ButtonEnum.Ok); 
            }
        }
    }
}
