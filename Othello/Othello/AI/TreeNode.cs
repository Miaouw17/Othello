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
                        //if (new OthelloBoard("", GameProperties.WIDTH, GameProperties.HEIGHT, gameBoard.Cast<int>().ToArray(),whiteTurn).getAllFlips(i, j, whiteTurn?GameProperties.WHITE:GameProperties.BLACK).Count() > 0)
                        if (new OthelloBoard("", GameProperties.WIDTH, GameProperties.HEIGHT,FlatArray(gameBoard), whiteTurn).getAllFlips(x, y, whiteTurn ? GameProperties.WHITE : GameProperties.BLACK).Count() > 0)
                        {
                            nextMoves.Add(new Tuple<int, int>(x,y));
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
    }
}
