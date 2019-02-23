using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMichelPetroff.AITools
{
    class TreeNode
    {


        private int[,] weightMap9x7 = 
            {{  10,  -5,   7,   5,   7,  -6,  10},
             {  -5,  -7,   1,   1,   1,  -7,  -5},
             {   7,   1,   3,   2,   3,   1,   7},
             {   5,   1,   2,   1,   2,   1,   5},
             {   5,   1,   2,   1,   2,   1,   5},
             {   5,   1,   2,   1,   2,   1,   5},
             {   7,   1,   3,   2,   3,   1,   7},
             {  -5,  -7,   1,   1,   1,  -7,  -5},
             {  10,  -5,   7,   5,   7,  -5,  10}};

        private static readonly Tuple<int, int>[] DIRECTIONS =
        {
            new Tuple<int, int>(-1, -1), new Tuple<int, int>(0, -1), new Tuple<int, int>(1, -1),
            new Tuple<int, int>(-1,  0),                             new Tuple<int, int>(1,  0),
            new Tuple<int, int>(-1,  1), new Tuple<int, int>(0,  1), new Tuple<int, int>(1,  1),
        };

        public int[,] Board;
        public int CurrentValue;
        public int OpponentValue { get { return CurrentValue == GameProperties.WHITE ? GameProperties.BLACK : GameProperties.WHITE; } }

        private List<Tuple<int, int>> TokenPlayerWhite;
        private List<Tuple<int, int>> TokenPlayerBlack;

        private List<Tuple<int, int>> CurrentToken { get { return CurrentValue == GameProperties.WHITE ? TokenPlayerWhite : TokenPlayerBlack; } }
        private List<Tuple<int, int>> OpponentToken { get { return CurrentValue == GameProperties.BLACK ? TokenPlayerWhite : TokenPlayerBlack; } }

        private bool GameIsFinished;
        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> ListPossibleMoves { get; set; }

        private int? eval = null;

        public TreeNode(TreeNode treeNode)
        {
            // ListPossibleMove Copy
            ListPossibleMoves = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            foreach (KeyValuePair<Tuple<int, int>, HashSet<Tuple<int, int>>> entry in treeNode.ListPossibleMoves)
                ListPossibleMoves.Add(new Tuple<int, int>(entry.Key.Item1, entry.Key.Item2), new HashSet<Tuple<int, int>>(entry.Value));

            // Board copy
            Board = new int[treeNode.Board.GetLength(0), treeNode.Board.GetLength(1)];
            for (int x = 0; x < Board.GetLength(0); x++)
                for (int y = 0; y < Board.GetLength(1); y++)
                    Board[x, y] = treeNode.Board[x, y];

            // CurrentPlayerValue Copy
            CurrentValue = treeNode.CurrentValue;

            // CurrentPlayerTokens Copy
            TokenPlayerWhite = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> token in treeNode.TokenPlayerWhite)
                TokenPlayerWhite.Add(new Tuple<int, int>(token.Item1, token.Item2));

            // OpponentPlayerTokens Copy
            TokenPlayerBlack = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> token in treeNode.TokenPlayerBlack)
                TokenPlayerBlack.Add(new Tuple<int, int>(token.Item1, token.Item2));

            GameIsFinished = treeNode.GameIsFinished;
        }

        public TreeNode(int[,] board, int currentPlayerValue)
        {
            GameIsFinished = false;
            Board = board;
            CurrentValue = currentPlayerValue;
            ListPossibleMoves = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            TokenPlayerWhite = new List<Tuple<int, int>>();
            TokenPlayerBlack = new List<Tuple<int, int>>();
            for (int x = 0; x < Board.GetLength(0); ++x)
            {
                for (int y = 0; y < Board.GetLength(1); ++y)
                {
                    if (Board[x, y] == GameProperties.WHITE)
                        TokenPlayerWhite.Add(new Tuple<int, int>(x, y));
                    else if (Board[x, y] == GameProperties.BLACK)
                        TokenPlayerBlack.Add(new Tuple<int, int>(x, y));
                }
            }

            UpdateListPossibleMoves();
        }

        public List<Tuple<int, int>> PossibleMoves()
        {
            List<Tuple<int, int>> moves = ListPossibleMoves.Keys.ToList<Tuple<int, int>>();
            return moves;
        }

        public TreeNode Apply(Tuple<int, int> move)
        {
            if (GameIsFinished)
            {
                throw new Exception("Can't play this move, the game is finished");
            }

            if (!ListPossibleMoves.Keys.Contains(move))
            {
                throw new Exception("Can't play this move, not a possible move");
            }

            TreeNode copy = new TreeNode(this);

            foreach (Tuple<int, int> tokenToReverse in copy.ListPossibleMoves[move])
            {
                copy.Board[tokenToReverse.Item1, tokenToReverse.Item2] = copy.CurrentValue;
                if (!copy.CurrentToken.Contains(tokenToReverse))
                    copy.CurrentToken.Add(tokenToReverse);
                if (copy.OpponentToken.Contains(tokenToReverse))
                    copy.OpponentToken.Remove(tokenToReverse);
            }

            copy.SwitchPlayer();

            if (copy.ListPossibleMoves.Count <= 0) //turn skipped
            {
                copy.SwitchPlayer();
                if (copy.ListPossibleMoves.Count <= 0)
                    copy.GameIsFinished = true;
            }

            return copy;
        }

        public void SwitchPlayer()
        {
            CurrentValue = CurrentValue == GameProperties.WHITE ? GameProperties.BLACK : GameProperties.WHITE;
            UpdateListPossibleMoves();
        }

        public List<Tuple<int, int>> GetPossibleMoves()
        {
            List<Tuple<int, int>> moves = ListPossibleMoves.Keys.ToList<Tuple<int, int>>();
            return moves;
        }

        public int Evaluate(int playerValue)
        {
            if (IsVictory())
            {
                return int.MaxValue - 1;
            }
            if (IsDefeat())
            {
                return int.MinValue + 1;
            }

            if (eval == null)
            {
                int score = 0;
                for (int y = 0; y < GameProperties.WIDTH; y++)
                {
                    for (int x = 0; x < GameProperties.HEIGHT; x++)
                    {
                        if (Board[x, y] == playerValue)
                        {
                            score += weightMap9x7[x, y];
                        }
                    }
                }
                eval = score;
            }
            return (int)eval;
        }

        private bool IsVictory()
        {
            return GameIsFinished && CurrentToken.Count > OpponentToken.Count;
        }

        private bool IsDefeat()
        {
            return GameIsFinished && CurrentToken.Count < OpponentToken.Count;
        }


        private List<Tuple<int, int>> GetNextPossibleMoves(int[,] gameBoard, bool whiteTurn)
        {
            List<Tuple<int, int>> nextMoves = new List<Tuple<int, int>>();
            for (int y = 0; y < GameProperties.WIDTH; y++)
            {
                for (int x = 0; x < GameProperties.HEIGHT; x++)
                {
                    if (gameBoard[y, x] == GameProperties.EMPTY)
                    {
                        Tuple<int, int> move = new Tuple<int, int>(x, y);
                        if (ValidateMove(move, whiteTurn ? 0 : 1))
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
            return GameIsFinished;
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
        private bool ValidateMove(Tuple<int, int> move, int vPlayer)
        {
            if (Board[move.Item1, move.Item2] != GameProperties.EMPTY)
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
                    int cellState = Board[ij.Item1, ij.Item2];
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

        private void UpdateListPossibleMoves()
        {
            ListPossibleMoves = GameOperator.GetPossibleMoves(Board, CurrentValue == GameProperties.WHITE);
        }
    }
}
