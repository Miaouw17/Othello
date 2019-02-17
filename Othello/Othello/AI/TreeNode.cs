using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello.AI
{
    class TreeNode
    {
        private int[,] weightMap9x7 = new int[,]{{ 10, -5,  7,  5,  5,  5,  7, -5, 10 },
                                                 { -5, -5,  1,  1,  1,  1,  1, -5, -5 },
                                                 {  7,  1,  3,  1,  1,  1,  3,  1,  7 },
                                                 {  5,  1,  1,  1,  1,  1,  1,  1,  5 },
                                                 {  7,  1,  3,  1,  1,  1,  3,  1,  7 },
                                                 { -5, -5,  1,  1,  1,  1,  1, -5, -5 },
                                                 { 10, -5,  7,  5,  5,  5,  7, -5, 10 }};
        private static readonly Tuple<int,int>[] DIRECTIONS =
        {
            new Tuple<int, int>(-1, -1), new Tuple<int, int>(0, -1), new Tuple<int, int>(1, -1),
            new Tuple<int, int>(-1,  0),                             new Tuple<int, int>(1,  0),
            new Tuple<int, int>(-1,  1), new Tuple<int, int>(0,  1), new Tuple<int, int>(1,  1),
        };
        public BoardState Board { get; private set; }
        public bool WhiteTurn { get; private set; }

        private Dictionary<Tuple<int, int>, TreeNode> whiteMoves;
        private Dictionary<Tuple<int, int>, TreeNode> blackMoves;
        private int? eval = null;

        public TreeNode(BoardState gameBoard, bool isWhiteTurn)
        {
            Board = gameBoard;
            WhiteTurn = isWhiteTurn;
        }

        public int Eval(bool isWhite)
        {
            if (eval == null)
            {
                eval = DotProduct(Board.Board, weightMap9x7);
                if (!isWhite)
                {
                    eval *= -1;
                }
            }
            return (int)eval;
        }

        private int? DotProduct(int[,] a, int[,] b)
        {
            int s = 0;
            for (int y = 0; y < GameProperties.HEIGHT; y++)
            {
                for (int x = 0; x < GameProperties.WIDTH; x++)
                {
                    s += a[y,x] * b[y, x];
                }
            }
            return s;
        }

        public Dictionary<Tuple<int,int>, TreeNode> Children(bool whiteTurn)
        {
            if (whiteTurn && whiteMoves == null)
            {
                whiteMoves = new Dictionary<Tuple<int,int>,TreeNode> ();
                foreach (Tuple<int, int> move in GetNextPossibleMoves(Board.Board, whiteTurn))
                {
                    BoardState bs = Board.NewBoardWithMove(move, GameProperties.WHITE);
                    whiteMoves.Add(move, new TreeNode(bs, false));
                }
            }
            else if (!whiteTurn && blackMoves == null)
            {
                blackMoves = new Dictionary<Tuple<int, int>, TreeNode>();
                foreach (Tuple<int, int> move in GetNextPossibleMoves(Board.Board, !whiteTurn))
                {
                    BoardState bs = Board.NewBoardWithMove(move, GameProperties.BLACK);
                    blackMoves.Add(move, new TreeNode(bs, false));
                }
            }
            return whiteTurn ? whiteMoves : blackMoves;
        }

        private List<Tuple<int,int>> GetNextPossibleMoves(int[,] gameBoard, bool whiteTurn)
        {
            List<Tuple<int, int>> nextMoves = new List<Tuple<int, int>>();
            for (int y = 0; y < GameProperties.HEIGHT; y++)
            {
                for(int x = 0; x < GameProperties.WIDTH; x++)
                {
                    if (gameBoard[y,x] == 0)
                    {
                        Tuple<int, int> move = new Tuple<int, int>(x, y);
                        if (ValidateMove(move, whiteTurn?-1:1))
                        {
                            nextMoves.Add(move);
                        }
                    }
                }
            }
            return nextMoves;
        }

        public bool IsTerminal()
        {
            return GetNextPossibleMoves(Board.Board, WhiteTurn).Count() == 0;
        }

        private int[] FlatArray(int[,] arr)
        {
            int[] r = new int[63];
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    r[y * 9 + x] = arr[y, x];
                }
            }
            return r;
        }

        private bool ValidateMove(Tuple<int,int> move, int vPlayer)
        {
            if (Board.Board[move.Item2, move.Item1] != GameProperties.EMPTY)
            {
                return false;
            }
            
            foreach (Tuple<int, int> direction in DIRECTIONS)
            {
                bool end = false;
                int nbTokenReturnedTemp = 0;

                Tuple<int, int> ij = move;
                ij = new Tuple<int, int>(ij.Item1 + direction.Item1, ij.Item2 + direction.Item2);

                while (ij.Item1 >= 0 && ij.Item1 < GameProperties.WIDTH && ij.Item2 >= 0 && ij.Item2 < GameProperties.HEIGHT && !end)
                {
                    int cellState = Board.Board[ij.Item2, ij.Item1];
                    if (cellState == vPlayer)
                    {
                        end = true;
                        if (nbTokenReturnedTemp > 0)
                            return true;
                    }
                    else if (cellState == GameProperties.EMPTY)
                    {
                        end = true;
                    }
                    else
                    {
                        nbTokenReturnedTemp++;
                    }
                    ij = new Tuple<int, int>(ij.Item1 + direction.Item1, ij.Item2 + direction.Item2);
                }
            }
            return false;
        }
    }
}
