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
        private const int BOARD_WIDTH = 8;
        private const int BOARD_HEIGHT = 8;
        private List<Rectangle> caseList = new List<Rectangle>();
        private int turn = 0;
        private Player p1;
        private Player p2;
        OthelloBoard board;

        public MainWindow()
        {
            p1 = new Player(0, "Trump", new BitmapImage(new Uri(@"trump.png", UriKind.Relative)));
            p2 = new Player(1, "Hilary", new BitmapImage(new Uri(@"hillary.png", UriKind.Relative)));
            board = new OthelloBoard(BOARD_WIDTH, BOARD_HEIGHT);
            InitializeComponent();
            //AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(Board_MouseDown), true);
            GridGeneration(BOARD_HEIGHT,BOARD_WIDTH);
            
        }

        private void GridGeneration(int row, int column)
        {
            // CreateRow
            for (int i = 0; i <= row; i++)
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
            for (int j = 0; j <= column; j++)
            {
                Board.ColumnDefinitions.Add(new ColumnDefinition());
                if (j != 0)
                {
                    Label columnLabel = new Label();
                    int index = 'A'- 1 + j;
                    columnLabel.Content = Encoding.ASCII.GetString(new byte[] { (byte)index });
                    Grid.SetRow(columnLabel, 0);
                    Grid.SetColumn(columnLabel, j);
                    Board.Children.Add(columnLabel);
                }
            }

            // Fill board with rectangle clickabley
            for (int y = 1; y <= row; y++)
            {
                for (int x = 1; x <= column; x++)
                {
                    string name = $"c{x}{y}";
                    Rectangle rect = new Rectangle() { Name = name, Fill = new SolidColorBrush(Colors.AliceBlue)};
                    rect.MouseLeftButtonDown += X_MouseDown;
                    Grid.SetRow(rect, y);
                    Grid.SetColumn(rect, x);
                    Board.Children.Add(rect);
                    caseList.Add(rect);
                }
            }
            DisplayBoard();
        }

        private void X_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Reaction au clic sur un rectangle
            string elementName = (e.Source as FrameworkElement).Name; //This is bad
            int gridX = Convert.ToInt32(elementName.Substring(1,1)); // Works only for x and y < 10
            int gridY = Convert.ToInt32(elementName.Substring(elementName.Length-1,1));
            int boardX = gridX - 1;
            int boardY = gridY - 1;
            //MessageBox.Show(elementName);

            if (IsPlayable(boardX, boardY, true))
            {
                if (turn % 2 == 0)
                {
                    board.Play(boardX, boardY, 1);
                    //r.Fill = new ImageBrush(p1.getImage());
                }
                else
                {
                    board.Play(boardX, boardY, -1);
                    //r.Fill = new ImageBrush(p2.getImage());
                }
                //r.Fill = new ImageBrush(new BitmapImage(new Uri(@"trump.png", UriKind.Relative)));
                //MessageBox.Show(elementName);
                Console.WriteLine(board.ToString());
                //board.DisplayBoard();
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
            for(int y=1; y<=board.Height; y++)
            {
                for(int x=1; x<=board.Width; x++)
                {
                    string name = $"c{x}{y}";
                   
                    if (board.Values[y-1,x-1] == 1) //Blanc
                    {
                        Rectangle r = caseList.Find(rec => rec.Name == name);
                        //Rectangle r = (Rectangle)Board.FindName(name);
                        r.Fill = new ImageBrush(p1.getImage());
                    }
                    else if(board.Values[y-1,x-1] == -1) //Noir
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

        private bool IsPlayable(int y, int x, bool isWhite)
        {
            if(board.Values[y, x] == 0)
            {
                // APPLIQUER REGLE DE JEU
                return true;
            }
            return false;
        }
    }
}
