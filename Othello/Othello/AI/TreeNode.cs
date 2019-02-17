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
                                                 { -5, -7,  1,  1,  1,  1,  1, -7, -5 },
                                                 {  7,  1,  3,  2,  2,  2,  3,  1,  7 },
                                                 {  5,  1,  2,  1,  1,  1,  2,  1,  5 },
                                                 {  7,  1,  3,  2,  2,  2,  3,  1,  7 },
                                                 { -5, -7,  1,  1,  1,  1,  1, -7, -5 },
                                                 { 10, -5,  7,  5,  5,  5,  7, -5, 10 }};

        private static readonly Tuple<int,int>[] DIRECTIONS =
        {
            new Tuple<int, int>(-1, -1), new Tuple<int, int>(0, -1), new Tuple<int, int>(1, -1),
            new Tuple<int, int>(-1,  0),                             new Tuple<int, int>(1,  0),
            new Tuple<int, int>(-1,  1), new Tuple<int, int>(0,  1), new Tuple<int, int>(1,  1),
        };

        public BoardState Board { get; private set; }
        public bool WhiteTurn { get; private set; }

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

        /// <summary>
        /// Returns the children of the current node.
        /// </summary>
        /// <param name="isWhiteTurn"></param>
        /// <returns></returns>
        public Dictionary<Tuple<int,int>, TreeNode> GetChildren(bool isWhiteTurn)
        {
            Dictionary<Tuple<int, int>, TreeNode> moves = new Dictionary<Tuple<int, int>, TreeNode>();
            if (isWhiteTurn)
            {
                foreach (Tuple<int, int> move in GetNextPossibleMoves(Board.Board, isWhiteTurn))
                {
                    moves.Add(move, new TreeNode(Board.NewBoardWithMove(move, GameProperties.WHITE), false));
                }
            }
            else
            {
                foreach (Tuple<int, int> move in GetNextPossibleMoves(Board.Board, !isWhiteTurn))
                {
                    moves.Add(move, new TreeNode(Board.NewBoardWithMove(move, GameProperties.BLACK), false));
                }
            }
            return moves;
        }

        private List<Tuple<int,int>> GetNextPossibleMoves(int[,] gameBoard, bool whiteTurn)
        {
            List<Tuple<int, int>> nextMoves = new List<Tuple<int, int>>();
            for (int y = 0; y < GameProperties.HEIGHT; y++)
            {
                for(int x = 0; x < GameProperties.WIDTH; x++)
                {
                    if (gameBoard[y,x] == GameProperties.EMPTY)
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

        /// <summary>
        /// Returns if the node is a leaf.
        /// </summary>
        /// <returns></returns>
        public bool IsLeaf()
        {
            return GetNextPossibleMoves(Board.Board, WhiteTurn).Count() == 0;
        }

        /// <summary>
        /// Cette fonction provient du groupe de Bastien Wermeille et Segan Salomon.
        /// Nous en avons eu besoin car nous avons codé notre Othello avec un board sous forme de tableau 1D,
        /// hors, la validation d'un move n'était pas du tout optimisée car elle utilisait Linq et on ne pouvait pas se déplacer
        /// dans les indices aussi facilement qu'avec un tableau 2D, vu qu'ici on utilise un tableau 2D, nous avons donc
        /// décidé de reprendre un algorithme de validation de move provenant d'un groupe qui avait déjà implémenté leur
        /// Othello avec un tableau 2D. Ainsi, la validation est plus simple grâce aux directions. On peut s'arrêter au
        /// premier disque retourné.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="vPlayer"></param>
        /// <returns></returns>
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
