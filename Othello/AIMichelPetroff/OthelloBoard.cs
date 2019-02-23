using IPlayable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMichelPetroff.AITools;

namespace AIMichelPetroff
{
    class OthelloBoard : IPlayable.IPlayable
    {
        private int[,] board;
        private int AIValueOnBoard;

    
        public OthelloBoard()
        {
            board = new int[GameProperties.WIDTH, GameProperties.HEIGHT];
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = GameProperties.EMPTY;
                }
            }

            int px = GameProperties.WIDTH / 2 - 1;
            int py = GameProperties.HEIGHT / 2;

            board[px, py + 1] = GameProperties.BLACK;
            board[px + 1, py] = GameProperties.BLACK;
            board[px, py] = GameProperties.WHITE;
            board[px + 1, py + 1] = GameProperties.WHITE;
        }

        public int GetBlackScore()
        {
            return GameOperator.GetDiscs(board, GameProperties.BLACK).Count;
        }

        public int[,] GetBoard()
        {
            return board;
        }

        public string GetName()
        {
            return "Michel & Petroff AI";
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            AIValueOnBoard = (whiteTurn) ? GameProperties.WHITE : GameProperties.BLACK;
            return AlphaBeta(game, level);
        }

        public int GetWhiteScore()
        {
            return GameOperator.GetDiscs(board, GameProperties.WHITE).Count;
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            return GameOperator.ValidateMove(board, new Tuple<int,int>(column,line), isWhite?GameProperties.WHITE:GameProperties.BLACK);
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> listPossibleMoves = GameOperator.GetPossibleMoves(board, isWhite);
            Tuple<int, int> position = new Tuple<int, int>(column, line);
            if (!listPossibleMoves.ContainsKey(position))
            {
                return false;
            }

            HashSet<Tuple<int, int>> discsToFlip = listPossibleMoves[position];
            int value = isWhite ? GameProperties.WHITE : GameProperties.BLACK;
            foreach (Tuple<int, int> disc in discsToFlip)
            {
                board[disc.Item1, disc.Item2] = value;
            }
            return true;
        }


        #region AlphaBeta
        private Tuple<int, int> AlphaBeta(int[,] game, int depth)
        {
            TreeNode root = new TreeNode(game, AIValueOnBoard);

            Tuple<int, Tuple<int, int>> res = _AlphaBeta(root, depth);

            if (res.Item2 == null)
            {
                return new Tuple<int, int>(-1, -1);
            }
            return res.Item2;
        }

        private Tuple<int, Tuple<int, int>> _AlphaBeta(TreeNode node, int depth, int alpha = int.MinValue, int beta = int.MaxValue, bool maximizingPlayer = true)
        {
            Tuple<int, int> move = null;
            int value;

            if (depth <= 0 || node.IsLeaf())
                return new Tuple<int, Tuple<int, int>>(node.Evaluate(AIValueOnBoard), null);

            if (maximizingPlayer)
            {
                value = int.MinValue;
                foreach (Tuple<int, int> _move in node.GetPossibleMoves())
                {
                    TreeNode child = node.Apply(_move);
                    Tuple<int, Tuple<int, int>> result = _AlphaBeta(child, depth - 1, alpha, beta, GetMinOrMax(child));
                    if (result.Item1 > value)
                    {
                        value = result.Item1;
                        move = _move;
                    }
                    if (value > alpha)
                    {
                        alpha = value;
                    }
                    if (alpha >= beta)
                        break;
                }
            }
            else
            {
                value = int.MaxValue;
                foreach (Tuple<int, int> _move in node.GetPossibleMoves())
                {
                    TreeNode child = node.Apply(_move);
                    Tuple<int, Tuple<int, int>> result = _AlphaBeta(child, depth - 1, alpha, beta, GetMinOrMax(child));


                    if (result.Item1 < value)
                    {
                        value = result.Item1;
                        move = _move;
                    }
                    if (value < alpha)
                    {
                        beta = value;
                    }
                    if (alpha >= beta)
                        break;
                }
            }
            return new Tuple<int, Tuple<int, int>>(value, move);
        }

        private bool GetMinOrMax(TreeNode treeNode)
        {
            return treeNode.CurrentValue == AIValueOnBoard;
        }

        #endregion
    }
}
