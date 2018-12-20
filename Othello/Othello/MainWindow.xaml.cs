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
        private const int ROW = 7;
        private const int COLUMN = 9;
        private int[,] tabPlayer = new int[COLUMN+1, ROW+1];
        private List<Rectangle> caseList = new List<Rectangle>();
        private int turn = 0;
        private Player p1 = new Player(0, "Trump", new BitmapImage(new Uri(@"trump.png", UriKind.Relative)));
        private Player p2 = new Player(1, "Hilary", new BitmapImage(new Uri(@"hillary.png", UriKind.Relative)));

        public MainWindow()
        {
            InitializeComponent();
            //AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(Board_MouseDown), true);
            GridGeneration(ROW,COLUMN);
            SetStarterPawn();
            DisplayBoard();
        }

        private void GridGeneration(int row, int column)
        {
            // CreateRow
            for (int i = 0; i < row + 1; i++)
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
            for (int j = 0; j < column + 1; j++)
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

            for (int y = 1; y <= row; y++)
            {
                for (int x = 1; x <= column; x++)
                {
                    string name = $"c{x}{y}";
                    Rectangle rect = new Rectangle() { Name = name, Fill = new SolidColorBrush(System.Windows.Media.Colors.AliceBlue) };
                    rect.MouseLeftButtonDown += X_MouseDown;
                    Grid.SetRow(rect, y);
                    Grid.SetColumn(rect, x);
                    Board.Children.Add(rect);
                    caseList.Add(rect);
                }
            }
        }

        private void SetStarterPawn()
        {
            int start_x = (COLUMN / 2);
            int start_y = (ROW / 2);
            tabPlayer[start_x, start_y] = 1;
            tabPlayer[start_x+1, start_y+1] = 1;
            tabPlayer[start_x+1, start_y] = -1;
            tabPlayer[start_x, start_y+1] = -1;
        }

        private void X_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Reaction au clic sur un rectangle
            var mouseWasDownOn = e.Source as FrameworkElement;
            Rectangle r = (Rectangle)mouseWasDownOn;
            string elementName = mouseWasDownOn.Name;
            int x = Convert.ToInt32(elementName.Substring(1,1));
            int y = Convert.ToInt32(elementName.Substring(elementName.Length-1,1));
            //string selectedRect = elementName.ToString();
            //MessageBox.Show(elementName);

            if (IsPlayable(x,y,true))
            {
                if (turn % 2 == 0)
                {
                    tabPlayer[x, y] = 1;
                    //r.Fill = new ImageBrush(p1.getImage());
                }
                else
                {
                    tabPlayer[x, y] = -1;
                    //r.Fill = new ImageBrush(p2.getImage());
                }
                //r.Fill = new ImageBrush(new BitmapImage(new Uri(@"trump.png", UriKind.Relative)));
                //MessageBox.Show(elementName);
                Console.WriteLine(DebugTabPlayer());
            }
            else
            {
                //MessageBox.Show("nope");
            }

            // displayBoard
            DisplayBoard();

            // incremente turn
            turn += 1;

            // throw new NotImplementedException();
        }

        private void DisplayBoard()
        {
            for(int y=1; y<ROW+1; y++)
            {
                for(int x=1; x<COLUMN+1; x++)
                {
                    string name = $"c{x}{y}";
                   
                    if (tabPlayer[x,y] == 1)
                    {
                        Rectangle r = caseList.Find(rec => rec.Name == name);
                        //Rectangle r = (Rectangle)Board.FindName(name);
                        r.Fill = new ImageBrush(p1.getImage());
                    }
                    else if(tabPlayer[x,y] == -1)
                    {
                        Rectangle r = caseList.Find(rec => rec.Name == name);
                        //Rectangle r = (Rectangle)Board.FindName(name);
                        r.Fill = new ImageBrush(p2.getImage());
                    }
                }
            }
        }

        private void Board_MouseDown(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(e.Source.ToString());
        }

        private bool IsPlayable(int column, int row, bool isWhite)
        {
            if(tabPlayer[column, row] == 0)
            {
                // APPLIQUER REGLE DE JEU
                return true;
            }
            else
            {
                return false;
            }
        }

        private string DebugTabPlayer()
        {
            string debug = "";
            for(int y=1;y<ROW+1;y++)
            {
                for(int x=1;x<COLUMN+1;x++)
                {
                    debug += "[" + tabPlayer[x, y] + "]";
                }
                debug += "\n";
            }
            return debug;
        }
    }
}
