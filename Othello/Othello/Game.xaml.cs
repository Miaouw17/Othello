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
using System.Diagnostics;
using System.Timers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour Game.xaml
    /// </summary>
    public partial class Game : Page, INotifyPropertyChanged
    {
        private const int BOARD_WIDTH = 9;
        private const int BOARD_HEIGHT = 7;
        private List<Rectangle> caseList = new List<Rectangle>();
        private bool isWhiteTurn = true;
        private bool whiteBlocked = false;
        private bool blackBlocked = false;
        private Player p1;
        private Player p2;

       
        private Stopwatch swWhitePlayer;
        private Stopwatch swBlackPlayer;
        private TimeSpan tsWhitePlayer;
        private long timeSaveWhite=0;
        private TimeSpan tsBlackPlayer;
        private long timeSaveBlack=0;
        private Timer timer;

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
            this.DataContext = this;

            p1 = new Player(0, NAME_PLAYER_1, new BitmapImage(uriWhite));
            p2 = new Player(1, NAME_PLAYER_2, new BitmapImage(uriBlack));
            this.board = new OthelloBoard("Board", BOARD_WIDTH, BOARD_HEIGHT); //TODO : Change name dynamically, using save name !

            InitializeComponent();

            GridGeneration(BOARD_HEIGHT, BOARD_WIDTH);

            UpdateScore();

            initTimer();
        }

        public Game(int[] values, bool isWhiteTurn, long timeSaveWhite, long timeSaveBlack)
        {
            this.DataContext = this;

            p1 = new Player(0, NAME_PLAYER_1, new BitmapImage(uriWhite));
            p2 = new Player(1, NAME_PLAYER_2, new BitmapImage(uriBlack));
            this.board = new OthelloBoard("Board", BOARD_WIDTH, BOARD_HEIGHT, values, isWhiteTurn);

            this.isWhiteTurn = isWhiteTurn;
            this.timeSaveWhite = timeSaveWhite;
            this.timeSaveBlack = timeSaveBlack;

            InitializeComponent();

            GridGeneration(BOARD_HEIGHT, BOARD_WIDTH);

            UpdateScore();

            initTimer();
        }

        // BINDING
        public int WhiteScore
        {
            get { return board.GetWhiteScore(); }
            set { p1.SetScore(value); RaisePropertyChanged("WhiteScore"); }
        }

        public int BlackScore
        {
            get { return board.GetBlackScore(); }
            set { p2.SetScore(value); RaisePropertyChanged("BlackScore"); }
        }

        public TimeSpan WhiteTimer
        {
            get { return tsWhitePlayer; }
            set { tsWhitePlayer = value; RaisePropertyChanged("WhiteTimer"); }
        }

        public TimeSpan BlackTimer
        {
            get { return tsBlackPlayer; }
            set { tsBlackPlayer = value; RaisePropertyChanged("BlackTimer"); }
        }

        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
       
        public event PropertyChangedEventHandler PropertyChanged;

        private void initTimer()
        {
            swWhitePlayer = new Stopwatch();
            swBlackPlayer = new Stopwatch();
            tsWhitePlayer = swWhitePlayer.Elapsed;
            tsBlackPlayer = swBlackPlayer.Elapsed;
            timer = new Timer();
            timer.Interval = 0.1;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke((Action)delegate ()
                {
                    UpdateTimers();
                });
            }
            catch
            {
                // try catch to avoid crash on windows close
            }
        }

        private void GridGeneration(int row, int column)
        {
            // CreateRow
            for (int i = 0; i <= row; i++)
            {
                Board.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(65) });
                if (i > 0)
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
                Board.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(65) });
                if (j > 0)
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
            Board.RowDefinitions[0].Height = new GridLength(20);
            Board.ColumnDefinitions[0].Width = new GridLength(20);
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
                UpdateTurn();
                board.UpdateNextPossibleMoves(isWhiteTurn ? 1 : -1);
                DisplayBoard();
                if (isWhiteTurn)
                {
                    swWhitePlayer.Start();
                    swBlackPlayer.Stop();
                }
                else
                {
                    swWhitePlayer.Stop();
                    swBlackPlayer.Start();
                }
            }

            UpdatePlayableCells();
            UpdateScore();

            Console.WriteLine($"whiteblocked:{whiteBlocked}, blackblocked:{blackBlocked}");

            if (whiteBlocked ^ blackBlocked)
            {
                string turn = isWhiteTurn ? "White" : "Black";
                string notTurn = isWhiteTurn ? "Black" : "White";
                MessageBox.Show($"No possible move for {notTurn} !\n{turn} can keep playing.");
            }
            if (whiteBlocked && blackBlocked || !board.Values.ToList().Contains(0))
            {
                //End of game !
                int sWhite = Convert.ToInt32(ScoreJ1.Content);
                int sBlack = Convert.ToInt32(ScoreJ2.Content);
                if (sWhite != sBlack)
                {
                    string winner = "";
                    winner = sWhite > sBlack ? "White" : "Black";
                    MessageBox.Show($"End of the game !\n{winner} wins !!!");
                }
                else
                {
                    MessageBox.Show("Tie game !");
                }
            }

        }

        private void UpdateTurn()
        {
            if (board.NextPossibleMoves.Count() == 0)
            {
                string turn = isWhiteTurn ? "White" : "Black";
                string notTurn = isWhiteTurn ? "Black" : "White";
                if (isWhiteTurn)
                {
                    blackBlocked = true;
                }
                else
                {
                    whiteBlocked = true;
                }
            }
            if (board.NextPossibleMoves.Count() > 0)
            {
                if (isWhiteTurn)
                {
                    blackBlocked = false;
                }
                else
                {
                    whiteBlocked = false;
                }
                isWhiteTurn ^= true; //Invert turn
            }
            lblTurn.Content = isWhiteTurn ? "It's White's turn !" : "It's Black's turn !";
        }

        private void UpdateScore()
        {
            WhiteScore = board.GetWhiteScore();
            BlackScore = board.GetBlackScore();
        }

        private void UpdateTimers()
        {
            WhiteTimer = TimeSpan.FromMilliseconds(swWhitePlayer.ElapsedMilliseconds + timeSaveWhite);
            BlackTimer = TimeSpan.FromMilliseconds(swBlackPlayer.ElapsedMilliseconds + timeSaveBlack);
        }

        private void UpdatePlayableCells()
        {
            //Not used yet
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
            saveFileDialog.Filter = "Binary File|*.bin";
            saveFileDialog.Title = "Save your game";
            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog.FileName != "")
            {
                Save save = new Save(board.GetValues(), isWhiteTurn, swWhitePlayer.ElapsedMilliseconds+timeSaveWhite, swBlackPlayer.ElapsedMilliseconds+timeSaveBlack);
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, save);
                stream.Close();
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
            
            //infoColumn.Width = new GridLength(.3f * Window.GetWindow(this).Width);
            infoColumn.Width = new GridLength(300);
            boardColumn.Width = new GridLength(Window.GetWindow(this).ActualWidth - 300);


            int nRows = Board.RowDefinitions.Count;
            int nCols = Board.ColumnDefinitions.Count;

            double min = Math.Min(board_Border.ActualHeight / (nRows+1), board_Border.ActualWidth / (nCols+1));

            GridLength gl = new GridLength((int)min, GridUnitType.Pixel);
            Board.ColumnDefinitions[0].Width = new GridLength(50);
            Board.RowDefinitions[0].Height = new GridLength(50);
            for (int i = 1; i < nCols; i++)
            {
                Board.ColumnDefinitions[i].Width = gl;
            }
            for (int i = 1; i < nRows; i++)
            {
                Board.RowDefinitions[i].Height = gl;
            }

            double w = (board_Border.ActualWidth - min * nCols) / 2;
            double h = (board_Border.ActualHeight - min * nRows) / 2;

            Board.Margin = new Thickness(w, h-25, w, h);
        }

        
    }
}
