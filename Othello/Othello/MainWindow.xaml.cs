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
        private const int BOARD_WIDTH = 6;
        private const int BOARD_HEIGHT = 6;
        private List<Rectangle> caseList = new List<Rectangle>();
        private int turn = 0;
        private Player p1;
        private Player p2;
        private BitmapImage emptyFrame;
        private BitmapImage nextFrameBlack;
        private BitmapImage nextFrameWhite;
        OthelloBoard board;

        public MainWindow()
        {
            p1 = new Player(0, "Trump", new BitmapImage(new Uri(@"framewhite.jpg", UriKind.Relative)));
            p2 = new Player(1, "Hilary", new BitmapImage(new Uri(@"frameblack.jpg", UriKind.Relative)));
            emptyFrame = new BitmapImage(new Uri(@"frameempty.jpg", UriKind.Relative));
            nextFrameBlack = new BitmapImage(new Uri(@"framenextblack.jpg", UriKind.Relative));
            nextFrameWhite = new BitmapImage(new Uri(@"framenextwhite.jpg", UriKind.Relative));
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
                    Label rowLabel = new Label() {
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
                    Label columnLabel = new Label() {
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
                    Rectangle rect = new Rectangle() {DataContext = new Tuple<int,int>(x,y)};
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
            Console.Write("ALO");
            // Reaction au clic sur un rectangle
            Tuple<int,int> coords = ((Tuple<int,int>)((Rectangle)sender).DataContext);
            int boardX = coords.Item1 - 1;
            int boardY = coords.Item2 - 1;
            
            if(IsPlayable(boardX,boardY))
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
                    if (IsPlayable(x,y))
                    {
                        
                    }
                }
            }
        }

        private void DisplayBoard()
        {
            for(int y=1; y<=board.Height; y++)
            {
                for(int x=1; x<=board.Width; x++)
                {
                    Rectangle r_base = caseList.Find(rec => rec.DataContext.Equals(new Tuple<int,int>(x,y)));
                    r_base.Fill = new ImageBrush(emptyFrame);
                    
                   
                    if (board[ix(x-1, y-1)] == 1) //Blanc
                    {
                        Rectangle r = caseList.Find(rec => rec.DataContext.Equals(new Tuple<int, int>(x, y)));
                        r.Fill = new ImageBrush(p1.getImage());
                    }
                    else if(board[ix(x-1, y-1)] == -1) //Noir
                    {
                        Rectangle r = caseList.Find(rec => rec.DataContext.Equals(new Tuple<int, int>(x, y)));
                        r.Fill = new ImageBrush(p2.getImage());
                    }
                    else if (board[ix(x - 1, y - 1)] == 0)
                    {
                        if(IsPlayable(x-1, y-1))
                        {
                            Rectangle r = caseList.Find(rec => rec.DataContext.Equals(new Tuple<int, int>(x, y)));
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
            return (board[ix(x, y)] == 0) && (board.getAllFlips(x,y,turn%2==0?1:-1).Count() != 0);
        }

        private int ix(int x, int y)
        {
            return x + y * board.Width;
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackMenu_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    /* Logique : Il ne devrait pas y avoir de fonction "IsPlayable". On devrait plutôt ne laisser le user cliquer que sur les cases qui sont playable.
     * de cette façon il y a pas besoin de checker si elles le sont .
     */
}
