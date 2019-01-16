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
        private bool isWhiteTurn = true;
        private Player p1;
        private Player p2;
        private static BitmapImage bmpEmpty = new BitmapImage(new Uri(@"frameempty.jpg", UriKind.Relative));
        private static BitmapImage bmpNWhite = new BitmapImage(new Uri(@"framenextwhite.jpg", UriKind.Relative));
        private static BitmapImage bmpNBlack = new BitmapImage(new Uri(@"framenextblack.jpg", UriKind.Relative));
        private static Uri uriWhite = new Uri(@"framewhite.jpg", UriKind.Relative);
        private static Uri uriBlack = new Uri(@"frameblack.jpg", UriKind.Relative);
        private OthelloBoard board;

        private const string NAME_PLAYER_1 = "White";
        private const string NAME_PLAYER_2 = "Black";

        public Game()
        {
            p1 = new Player(0, NAME_PLAYER_1, new BitmapImage(uriWhite));
            p2 = new Player(1, NAME_PLAYER_2, new BitmapImage(uriBlack));
            board = new OthelloBoard("Board", BOARD_WIDTH, BOARD_HEIGHT); //TODO : Change name dynamically, using save name !

            InitializeComponent();

            GridGeneration(BOARD_HEIGHT, BOARD_WIDTH);
            ScoreJ1.Content = board.GetWhiteScore();
            ScoreJ2.Content = board.GetBlackScore();
        }

        public Game(int[] values)
        {
            p1 = new Player(0, NAME_PLAYER_1, new BitmapImage(uriWhite));
            p2 = new Player(1, NAME_PLAYER_2, new BitmapImage(uriBlack));
            this.board = new OthelloBoard("Board",BOARD_WIDTH, BOARD_HEIGHT, values);

            InitializeComponent();
      
            GridGeneration(BOARD_HEIGHT, BOARD_WIDTH);
            ScoreJ1.Content = board.GetWhiteScore();
            ScoreJ2.Content = board.GetBlackScore();
        }

        private void GridGeneration(int row, int column)
        {
            // CreateRow
            for (int i = 0; i <= row; i++)
            {
                Board.RowDefinitions.Add(new RowDefinition());
                if (i != 0)
                {
                    Label rowLabel = new Label()
                    {
                        FontFamily = new FontFamily("Segoe UI Light"),
                        FontSize = 32,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Content = $"{i}",
                    };
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
                    int index = '@' + j;
                    Label columnLabel = new Label()
                    {
                        FontFamily = new FontFamily("Segoe UI Light"),
                        FontSize = 32,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Content = Encoding.ASCII.GetString(new byte[] { (byte)index }),

                    };
                    Grid.SetRow(columnLabel, 0);
                    Grid.SetColumn(columnLabel, j);
                    Board.Children.Add(columnLabel);
                }
            }
            Board.RowDefinitions[0].Height = new GridLength(50);
            Board.ColumnDefinitions[0].Width = new GridLength(50);

            // Fill board with clickable rectangle
            for (int y = 1; y <= row; y++)
            {
                for (int x = 1; x <= column; x++)
                {
                    Rectangle rect = new Rectangle() { DataContext = new Tuple<int, int>(x, y) };
                    rect.MouseLeftButtonDown += X_MouseDown;
                    rect.MouseMove += X_MouseMove;
                    Grid.SetRow(rect, y);
                    Grid.SetColumn(rect, x);
                    Board.Children.Add(rect);
                    caseList.Add(rect);
                }
            }
            DisplayBoard();
        }

        private void X_MouseMove(object sender, MouseEventArgs e)
        {
            //if(((Rectangle)sender). == nextFrameBlack)
            //TODO
        }

        /// <summary>
        /// Handles a click on a rectangle on the grid.
        /// </summary>
        /// <param name="sender">Rectangle object on which the click has happend on.</param>
        /// <param name="e">Event args</param>
        private void X_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tuple<int, int> coords = ((Tuple<int, int>)((Rectangle)sender).DataContext); //Extracting coords from Rectangle DataContext.
            int boardX = coords.Item1 - 1; //GridX to BoardX
            int boardY = coords.Item2 - 1; //GridY to BoardY

            if (board.IsPlayable(boardX, boardY, isWhiteTurn))
            {
                board.PlayMove(boardX, boardY, isWhiteTurn);
                isWhiteTurn ^= true; //Invert turn
            }

            DisplayBoard();
            UpdatePlayableCells();
            ScoreJ1.Content = board.GetWhiteScore();
            ScoreJ2.Content = board.GetBlackScore();
        }

        private void UpdatePlayableCells()
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (board.IsPlayable(x, y, isWhiteTurn))
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
                    Rectangle r_base = caseList.Find(rec => rec.DataContext.Equals(new Tuple<int, int>(x, y)));
                    r_base.Fill = new ImageBrush(bmpEmpty);


                    if (board[ix(x - 1, y - 1)] == 1) //Blanc
                    {
                        Rectangle r = caseList.Find(rec => rec.DataContext.Equals(new Tuple<int, int>(x, y)));
                        r.Fill = new ImageBrush(p1.getImage());
                    }
                    else if (board[ix(x - 1, y - 1)] == -1) //Noir
                    {
                        Rectangle r = caseList.Find(rec => rec.DataContext.Equals(new Tuple<int, int>(x, y)));
                        r.Fill = new ImageBrush(p2.getImage());
                    }
                    else if (board[ix(x - 1, y - 1)] == 0)
                    {
                        //if (board.IsPlayable(x - 1, y - 1, isWhiteTurn))
                        if(board.NextPossibleMoves.Contains((y-1)*board.Width+(x-1)))
                        {
                            Rectangle r = caseList.Find(rec => rec.DataContext.Equals(new Tuple<int, int>(x, y)));
                            r.Fill = new ImageBrush(isWhiteTurn ? bmpNWhite : bmpNBlack);
                        }
                    }
                }
            }

        }

        private void Board_MouseDown(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(e.Source.ToString());
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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Here you can resize every element of the page dependant of the mainwindow size

            // Column proportion
            boardColumn.Width = new GridLength(0.7 * Window.GetWindow(this).Width);
            infoColumn.Width = new GridLength(0.3 * Window.GetWindow(this).Width);
        }
    }
}
