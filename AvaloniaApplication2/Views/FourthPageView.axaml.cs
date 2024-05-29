using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Linq;
using AvaloniaApplication2.Model;

namespace AvaloniaApplication2.Views
{
    public partial class FourthPageView : UserControl
    {
        //public static Dictionary<string, decimal>? result;
        public FourthPageView()
        {
            InitializeComponent();
            StackPanel stackPannel = this.FindControl<StackPanel>("stackPannel");
            //result = StaticClass.resultReader;

            for (int i = 0; i < StaticClass.resultReader.Count; ++i)
            {
                TextBlock textBox = new TextBlock()
                {
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    FontWeight = FontWeight.Bold,
                    FontSize = 13
                };
                TextBlock textBox2 = new TextBlock()
                {
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    FontSize = 12,
                    Margin = new Thickness(0, 5, 0, 15)
                };
                stackPannel.Children.Add(textBox);
                stackPannel.Children.Add(textBox2);
            }

            fillingText();
        }

            private void fillingText()
            {
            int i = 0;
            int k = 1;
            foreach (var res in StaticClass.resultReader)
            {
                var textBox = stackPannel.Children.OfType<TextBlock>().ElementAt(i); // ������ � �������� �� ������� 0 2 4
                textBox.Text = "����������� ���� - " + res.Key;

                var textBoxSum = stackPannel.Children.OfType<TextBlock > ().ElementAt(k); // ������ � �������� �� �������   1 3 5
                textBoxSum.Text = "� ���������� ���������� �������� ����� ������� �� ��������� ���������: " + res.Value.ToString();
                i += 2;
                k += 2;
            }
          
        }
    }
}
