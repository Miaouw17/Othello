using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(Board_MouseDown), true);
            GridGeneration(12, 12);
        }

        private void GridGeneration(int row, int column)
        {
            // CreateRow
            for (int i = 0; i < row+1; i++)
            {
                Board.RowDefinitions.Add(new RowDefinition());
                if (i != 0)
                {
                    Label rowLabel = new Label();
                    rowLabel.Content = $"{i}";
                    Grid.SetColumn(rowLabel, 0);
                    Grid.SetRow(rowLabel, i);
                    Board.Children.Add(rowLabel);
                }
            }

            // CreateColumn
            for (int j = 0; j < column+1; j++)
            {
                Board.ColumnDefinitions.Add(new ColumnDefinition());
                if (j != 0)
                {
                    Label columnLabel = new Label();
                    int index = 64 + j;
                    columnLabel.Content = Encoding.ASCII.GetString(new byte[] { (byte)index });
                    Grid.SetRow(columnLabel, 0);
                    Grid.SetColumn(columnLabel, j);
                    Board.Children.Add(columnLabel);
                }
            }

            for(int x=1; x <= row; x++)
            {
                for(int y=1; y <= column; y++)
                {
                    string name = $"case{x}{y}";
                    Rectangle rect = new Rectangle() { Name = name, Fill = new SolidColorBrush(System.Windows.Media.Colors.AliceBlue) };
                    rect.MouseLeftButtonDown += X_MouseDown;
                    Grid.SetRow(rect, x);
                    Grid.SetColumn(rect, y);
                    Board.Children.Add(rect);
                }
            }       
        }

        private void X_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Reaction au clic sur un rectangle
            var mouseWasDownOn = e.Source as FrameworkElement;
            Rectangle r = (Rectangle)mouseWasDownOn;
            if (r.Fill.ToString() == "#FFF0F8FF") 
            {
                r.Fill = new ImageBrush(new BitmapImage(new Uri(@"trump.png", UriKind.Relative)));
                //MessageBox.Show("yep");
            }
            else
            {
                //MessageBox.Show("nope");
            }
            string elementName = mouseWasDownOn.Name;
            //string selectedRect = elementName.ToString();
            MessageBox.Show(elementName);
           
            // throw new NotImplementedException();
        }

        private void Board_MouseDown(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(e.Source.ToString());
        }
    }
}
