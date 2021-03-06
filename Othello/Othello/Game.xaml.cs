﻿using Microsoft.Win32;
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
    /// Game.xaml logical interactions
    /// </summary>
    public partial class Game : Page, INotifyPropertyChanged
    {
        #region Attributes
        private const int BOARD_WIDTH = 9;
        private const int BOARD_HEIGHT = 7;
        private List<Rectangle> caseList = new List<Rectangle>();
        private bool isWhiteTurn = true;
        private bool whiteBlocked = false;
        private bool blackBlocked = false;
        private Player p1;
        private Player p2;
        private bool IAGame = false;
        private Stack<Tuple<int[], bool>> stackUndo;
        private Stack<Tuple<int[], bool>> stackRedo;
        private Stopwatch swWhitePlayer;
        private Stopwatch swBlackPlayer;
        private TimeSpan tsWhitePlayer;
        private long timeSaveWhite=0;
        private TimeSpan tsBlackPlayer;
        private long timeSaveBlack=0;
        private Timer timer;
        private static BitmapImage bmpEmpty = new BitmapImage(new Uri(@"./img/frameempty.jpg", UriKind.Relative));
        private static BitmapImage bmpNWhite = new BitmapImage(new Uri(@"./img/framenextwhite.jpg", UriKind.Relative));
        private static BitmapImage bmpNBlack = new BitmapImage(new Uri(@"./img/framenextblack.jpg", UriKind.Relative));
        private static Uri uriWhite = new Uri(@"./img/framewhite.jpg", UriKind.Relative);
        private static Uri uriBlack = new Uri(@"./img/frameblack.jpg", UriKind.Relative);
        private OthelloBoard board;
        private const string NAME_PLAYER_1 = "White";
        private const string NAME_PLAYER_2 = "Black";
        #endregion

        #region Constructors
        public Game()
        {
            this.DataContext = this;

            p1 = new Player(0, NAME_PLAYER_1, new BitmapImage(uriWhite));
            p2 = new Player(1, NAME_PLAYER_2, new BitmapImage(uriBlack));
            this.board = new OthelloBoard("Board", BOARD_WIDTH, BOARD_HEIGHT); //TODO : Change name dynamically, using save name !

            stackUndo = new Stack<Tuple<int[], bool>>();
            stackRedo = new Stack<Tuple<int[], bool>>();

            InitializeComponent();

            GridGeneration(BOARD_HEIGHT, BOARD_WIDTH);

            UpdateScore();

            initTimer();
        }

        public Game(bool IAGame)
        {
            this.DataContext = this;

            p1 = new Player(0, NAME_PLAYER_1, new BitmapImage(uriWhite));
            p2 = new Player(1, "IA", new BitmapImage(uriBlack));
            this.IAGame = IAGame;
            this.board = new OthelloBoard("Board", BOARD_WIDTH, BOARD_HEIGHT); //TODO : Change name dynamically, using save name !

            stackUndo = new Stack<Tuple<int[], bool>>();
            stackRedo = new Stack<Tuple<int[], bool>>();

            InitializeComponent();

            GridGeneration(BOARD_HEIGHT, BOARD_WIDTH);

            UpdateScore();

            initTimer();
        }

        public Game(int[] values, bool isWhiteTurn, long timeSaveWhite, long timeSaveBlack, Stack<Tuple<int[],bool>> stackUndo, Stack<Tuple<int[],bool>> stackRedo)
        {
            this.DataContext = this;

            p1 = new Player(0, NAME_PLAYER_1, new BitmapImage(uriWhite));
            p2 = new Player(1, NAME_PLAYER_2, new BitmapImage(uriBlack));
            this.board = new OthelloBoard("Board", BOARD_WIDTH, BOARD_HEIGHT, values, isWhiteTurn);

            this.isWhiteTurn = isWhiteTurn;
            this.timeSaveWhite = timeSaveWhite;
            this.timeSaveBlack = timeSaveBlack;
            this.stackUndo = stackUndo;
            this.stackRedo = stackRedo;

            InitializeComponent();

            GridGeneration(BOARD_HEIGHT, BOARD_WIDTH);

            UpdateScore();

            initTimer();
        }
        #endregion

        #region Properties
        // BINDING
        public int WhiteScore
        {
            get { return board.GetWhiteScore(); }
            set { p1.Score = value; RaisePropertyChanged("WhiteScore"); }
        }

        public int BlackScore
        {
            get { return board.GetBlackScore(); }
            set { p2.Score = value; RaisePropertyChanged("BlackScore"); }
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
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

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


        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Binary File|*.bin";
            saveFileDialog.Title = "Save your game";
            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog.FileName != "")
            {
                Save save = new Save(board.Values, isWhiteTurn, swWhitePlayer.ElapsedMilliseconds + timeSaveWhite, swBlackPlayer.ElapsedMilliseconds + timeSaveBlack, stackUndo, stackRedo);
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

            double min = Math.Min(board_Border.ActualHeight / (nRows + 1), board_Border.ActualWidth / (nCols + 1));

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

            Board.Margin = new Thickness(w, h - 25, w, h);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (stackUndo.Count() > 0)
            {
                // push step on redo stack
                bool turn = isWhiteTurn;
                stackRedo.Push(new Tuple<int[], bool>(board.GetValues(), turn));

                // pull undo stack and apply
                Tuple<int[], bool> step = stackUndo.Pop();
                board.Values = step.Item1;
                isWhiteTurn = step.Item2;
                board.UpdateNextPossibleMoves(isWhiteTurn ? 1 : -1);
                DisplayBoard();
                UpdateScore();
            }
            else
            {
                Console.WriteLine("UndoStack EMPTY");
            }
            lblTurn.Content = isWhiteTurn ? "It's White's turn !" : "It's Black's turn !";
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (stackRedo.Count() > 0)
            {
                // push step into undo stack
                bool turn = isWhiteTurn;
                stackUndo.Push(new Tuple<int[], bool>(board.GetValues(), turn));

                // pull redo stack and apply
                Tuple<int[], bool> step = stackRedo.Pop();
                board.Values = step.Item1;
                isWhiteTurn = step.Item2;
                board.UpdateNextPossibleMoves(isWhiteTurn ? 1 : -1);
                DisplayBoard();
                UpdateScore();
            }
            else
            {
                Console.WriteLine("RedoStack EMPTY");
            }
            lblTurn.Content = isWhiteTurn ? "It's White's turn !" : "It's Black's turn !";
        }

        private void Board_MouseDown(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(e.Source.ToString());
        }


        #endregion

        #region Private Methods
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
                        Foreground = Brushes.Black,
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
                        Foreground = Brushes.Black,
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

        private void X_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tuple<int, int> coords = ((Tuple<int, int>)((Rectangle)sender).DataContext); //Extracting coords from Rectangle DataContext.
            int boardX = coords.Item1 - 1; //GridX to BoardX
            int boardY = coords.Item2 - 1; //GridY to BoardY

            if (board.IsPlayable(boardX, boardY, isWhiteTurn))
            {
                // push step on undostack
                stackUndo.Push(new Tuple<int[], bool>(board.GetValues(), isWhiteTurn));
                // clean redo stack
                stackRedo.Clear();

                board.PlayMove(boardX, boardY, isWhiteTurn);
                UpdateTurn();
                //board.UpdateNextPossibleMoves(isWhiteTurn ? 1 : -1);
                //DisplayBoard();

                if (IAGame)
                {
                    int[,] game = board.GetBoard();
                    Tuple<int, int> play = board.GetNextMove(game, 5, isWhiteTurn);
                    board.PlayMove(play.Item1, play.Item2, isWhiteTurn);
                    UpdateTurn();
                    board.UpdateNextPossibleMoves(isWhiteTurn ? 1 : -1);
                }

                // Blocked player part
                if(board.NextPossibleMoves.Count()==0)
                {
                    whiteBlocked = blackBlocked = true;
                    if (isWhiteTurn)
                    {
                        swWhitePlayer.Stop();
                    }
                    else
                    {
                        swBlackPlayer.Stop();
                    }
                }
                else
                {
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
                DisplayBoard();
                
            }
            UpdateScore();

            if (whiteBlocked ^ blackBlocked)
            {
                string turn = isWhiteTurn ? "White" : "Black";
                string notTurn = isWhiteTurn ? "Black" : "White";
                MessageBox.Show($"No possible move for {notTurn} !\n{turn} can keep playing.");
            }
            if (whiteBlocked && blackBlocked)
            {
                int sWhite = Convert.ToInt32(ScoreJ1.Content);
                int sBlack = Convert.ToInt32(ScoreJ2.Content);
                if (sWhite != sBlack)
                {
                    string winner = "";
                    winner = sWhite > sBlack ? "White" : "Black";
                    MessageBox.Show($"GAME !\n{winner} wins !!!");
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
            byte gray = (byte)(0.2 + (WhiteScore * 0.8 / (double)(WhiteScore+BlackScore))*255);
            bgBoard.Fill = new SolidColorBrush(Color.FromRgb(gray,gray,gray));
        }

        private void UpdateTimers()
        {
            WhiteTimer = TimeSpan.FromMilliseconds(swWhitePlayer.ElapsedMilliseconds + timeSaveWhite);
            BlackTimer = TimeSpan.FromMilliseconds(swBlackPlayer.ElapsedMilliseconds + timeSaveBlack);
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
                        r.Fill = new ImageBrush(p1.Image);
                    }
                    else if (board[ix(x - 1, y - 1)] == -1) //Noir
                    {
                        Rectangle r = caseList.Find(rec => rec.DataContext.Equals(new Tuple<int, int>(x, y)));
                        r.Fill = new ImageBrush(p2.Image);
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

            // Could be binding
            buttonUndo.IsEnabled = (stackUndo.Count() != 0);
            buttonRedo.IsEnabled = (stackRedo.Count() != 0);
        }

        
        private int ix(int x, int y)
        {
            return x + y * board.Width;
        }

        #endregion
    }
}
