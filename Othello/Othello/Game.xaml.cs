using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logique d'interaction pour Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        private const int BOARD_WIDTH = 9;
        private const int BOARD_HEIGHT = 7;
        private List<Rectangle> caseList = new List<Rectangle>();
        private int turn = 0;
        private Player p1;
        private Player p2;
        private BitmapImage emptyFrame;
        private BitmapImage nextFrameBlack;
        private BitmapImage nextFrameWhite;
        OthelloBoard board;

        public Game()
        {
            p1 = new Player(0, "Trump", new BitmapImage(new Uri(@"framewhite.jpg", UriKind.Relative)));
            p2 = new Player(1, "Hilary", new BitmapImage(new Uri(@"frameblack.jpg", UriKind.Relative)));
            emptyFrame = new BitmapImage(new Uri(@"frameempty.jpg", UriKind.Relative));
            nextFrameBlack = new BitmapImage(new Uri(@"framenextblack.jpg", UriKind.Relative));
            nextFrameWhite = new BitmapImage(new Uri(@"framenextwhite.jpg", UriKind.Relative));
            board = new OthelloBoard(BOARD_WIDTH, BOARD_HEIGHT);
            InitializeComponent();
            //AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(Board_MouseDown), true);
            GridGeneration(BOARD_HEIGHT, BOARD_WIDTH);
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
                    int index = 'A' - 1 + j;
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
                    Rectangle rect = new Rectangle() { Name = name, Fill = new SolidColorBrush(Colors.LightGreen) };
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
            Console.Write("ALO");
            // Reaction au clic sur un rectangle
            string elementName = (e.Source as FrameworkElement).Name; //This is bad
            int gridX = Convert.ToInt32(elementName.Substring(1, 1)); // Works only for x and y < 10
            int gridY = Convert.ToInt32(elementName.Substring(elementName.Length - 1, 1));
            int boardX = gridX - 1;
            int boardY = gridY - 1;

            if (IsPlayable(boardX, boardY))
            {
                board.Play(boardX, boardY, turn % 2 == 0 ? 1 : -1);
                // incremente turn
                turn += 1;
            }

            DisplayBoard();
            UpdatePlayableCells();
        }

        private void UpdatePlayableCells()
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (IsPlayable(x, y))
                    {

                    }
                }
            }
        }

        private void DisplayBoard()
        {
            for (int y = 1; y <= board.Height; y++)
            {
                for (int x = 1; x <= board.Width; x++)
                {
                    string name = $"c{x}{y}";
                    Rectangle r_base = caseList.Find(rec => rec.Name == name);
                    r_base.Fill = new ImageBrush(emptyFrame);


                    if (board[ix(x - 1, y - 1)] == 1) //Blanc
                    {
                        Rectangle r = caseList.Find(rec => rec.Name == name);
                        r.Fill = new ImageBrush(p1.getImage());
                    }
                    else if (board[ix(x - 1, y - 1)] == -1) //Noir
                    {
                        Rectangle r = caseList.Find(rec => rec.Name == name);
                        r.Fill = new ImageBrush(p2.getImage());
                    }
                    else if (board[ix(x - 1, y - 1)] == 0)
                    {
                        if (IsPlayable(x - 1, y - 1))
                        {
                            Rectangle r = caseList.Find(rec => rec.Name == name);
                            r.Fill = new ImageBrush(turn % 2 == 0 ? nextFrameWhite : nextFrameBlack);
                        }
                    }
                }
            }
        }

        private void Board_MouseDown(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(e.Source.ToString());
        }

        private bool IsPlayable(int x, int y)
        {
            return (board[ix(x, y)] == 0) && (board.getAllFlips(x, y, turn % 2 == 0 ? 1 : -1).Count() != 0);
        }

        private int ix(int x, int y)
        {
            return x + y * board.Width;
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file|*.txt|Csv file|*.csv";
            saveFileDialog.Title = "Save your game";
            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog.FileName != "")
            {
                using (StreamWriter writetext = new StreamWriter(saveFileDialog.FileName))
                {
                    writetext.WriteLine(board);
                }
            }
        }

        private void BackMenu_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Menu());
        }
    }
}
