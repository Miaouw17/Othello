using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMichelPetroff.AITools
{
    class TreeNode
    {
        private static readonly int[,] weightMap9x7 = 
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

        private List<Tuple<int, int>> DiscPlayerWhiteList;
        private List<Tuple<int, int>> DiscPlayerBlackList;

        private List<Tuple<int, int>> CurrentPlayerDiscList { get { return CurrentValue == GameProperties.WHITE ? DiscPlayerWhiteList : DiscPlayerBlackList; } }
        private List<Tuple<int, int>> OpponentPlayerDiscList { get { return CurrentValue == GameProperties.BLACK ? DiscPlayerWhiteList : DiscPlayerBlackList; } }

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

            // CurrentPlayerDiscs Copy
            DiscPlayerWhiteList = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> disc in treeNode.DiscPlayerWhiteList)
                DiscPlayerWhiteList.Add(new Tuple<int, int>(disc.Item1, disc.Item2));

            // OpponentPlayerDiscs Copy
            DiscPlayerBlackList = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> disc in treeNode.DiscPlayerBlackList)
                DiscPlayerBlackList.Add(new Tuple<int, int>(disc.Item1, disc.Item2));

            GameIsFinished = treeNode.GameIsFinished;
        }

        public TreeNode(int[,] board, int currentPlayerValue)
        {
            GameIsFinished = false;
            Board = board;
            CurrentValue = currentPlayerValue;
            ListPossibleMoves = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            DiscPlayerWhiteList = new List<Tuple<int, int>>();
            DiscPlayerBlackList = new List<Tuple<int, int>>();
            for (int x = 0; x < Board.GetLength(0); ++x)
            {
                for (int y = 0; y < Board.GetLength(1); ++y)
                {
                    if (Board[x, y] == GameProperties.WHITE)
                        DiscPlayerWhiteList.Add(new Tuple<int, int>(x, y));
                    else if (Board[x, y] == GameProperties.BLACK)
                        DiscPlayerBlackList.Add(new Tuple<int, int>(x, y));
                }
            }

            UpdateListPossibleMoves();
        }

        public TreeNode Apply(Tuple<int, int> move)
        {
            if (GameIsFinished || !ListPossibleMoves.Keys.Contains(move))
            {
                throw new Exception("Can't play this move.");
            }

            TreeNode deepCopy = new TreeNode(this);

            foreach (Tuple<int, int> discsToReverse in deepCopy.ListPossibleMoves[move])
            {
                deepCopy.Board[discsToReverse.Item1, discsToReverse.Item2] = deepCopy.CurrentValue;
                if (!deepCopy.CurrentPlayerDiscList.Contains(discsToReverse))
                {
                    deepCopy.CurrentPlayerDiscList.Add(discsToReverse);
                }
                if (deepCopy.OpponentPlayerDiscList.Contains(discsToReverse)) // QMDP check if else
                {
                    deepCopy.OpponentPlayerDiscList.Remove(discsToReverse);
                }
            }

            deepCopy.PassToOtherPlayer();

            if (deepCopy.ListPossibleMoves.Count <= 0) // QMDP peut-être que ça marche sans
            {
                deepCopy.PassToOtherPlayer();
                if (deepCopy.ListPossibleMoves.Count <= 0)
                    deepCopy.GameIsFinished = true;
            }

            return deepCopy;
        }

        public void PassToOtherPlayer()
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
            if (GameIsFinished && CurrentPlayerDiscList.Count > OpponentPlayerDiscList.Count)
            {
                return int.MaxValue - 1;
            }
            if (GameIsFinished && CurrentPlayerDiscList.Count < OpponentPlayerDiscList.Count)
            {
                return int.MinValue + 1;
            }

            if (eval == null)
            {
                int score = 0;
                for (int y = 0; y < GameProperties.HEIGHT; y++)
                {
                    for (int x = 0; x < GameProperties.WIDTH; x++)
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

        public bool IsLeaf()
        {
            return GameIsFinished;
        }

        private void UpdateListPossibleMoves()
        {
            ListPossibleMoves = GameOperator.GetPossibleMoves(Board, CurrentValue == GameProperties.WHITE);
        }
    }
}
