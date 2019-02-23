using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMichelPetroff.AITools
{
    class GameOperator
    {
        private static readonly Tuple<int, int>[] DIRECTIONS =
        {
            new Tuple<int, int>(-1, -1), new Tuple<int, int>(0, -1), new Tuple<int, int>(1, -1),
            new Tuple<int, int>(-1,  0),                             new Tuple<int, int>(1,  0),
            new Tuple<int, int>(-1,  1), new Tuple<int, int>(0,  1), new Tuple<int, int>(1,  1),
        };

        public static List<Tuple<int, int>> GetDiscs(int[,] game, int value)
        {
            List<Tuple<int, int>> discs = new List<Tuple<int, int>>();
            for (int i = 0; i < game.GetLength(0); i++)
            {
                for (int j = 0; j < game.GetLength(1); j++)
                {
                    if (game[i, j] == value)
                        discs.Add(new Tuple<int, int>(i, j));
                }
            }
            return discs;
        }

        public static bool ValidateMove(int[,] board, Tuple<int, int> move, int vPlayer)
        {
            if (board[move.Item2, move.Item1] != GameProperties.EMPTY)
            {
                return false;
            }

            foreach (Tuple<int, int> direction in DIRECTIONS)
            {
                bool end = false;
                int nbTokenReturnedTemp = 0;

                Tuple<int, int> ij = move;
                ij = new Tuple<int, int>(ij.Item1 + direction.Item1, ij.Item2 + direction.Item2);

                while (ij.Item1 >= 0 && ij.Item1 < GameProperties.HEIGHT && ij.Item2 >= 0 && ij.Item2 < GameProperties.WIDTH && !end)
                {
                    int cellState = board[ij.Item1, ij.Item2];
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

        public static Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetPossibleMoves(int[,] game, bool isWhite)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> listPossibleMoves = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();

            int currentValue = isWhite ? GameProperties.WHITE : GameProperties.BLACK;
            int opponentValue = isWhite ? GameProperties.BLACK : GameProperties.WHITE;

            List<Tuple<int, int>> currentTokens = GetDiscs(game, currentValue);

            foreach (Tuple<int, int> tokenStart in currentTokens)
            {
                foreach (Tuple<int, int> direction in DIRECTIONS)
                {
                    Tuple<int, int> tokenPosition = null;
                    HashSet<Tuple<int, int>> toReverse = new HashSet<Tuple<int, int>>();
                    bool directionIsEligibleForAMove = true;
                    int i = 1;

                    while (true)
                    {
                        tokenPosition = new Tuple<int, int>(tokenStart.Item1 + i * direction.Item1, tokenStart.Item2 + i * direction.Item2);

                        if (!BoardContains(game, tokenPosition))
                        {
                            directionIsEligibleForAMove = false;
                            break;
                        }

                        int valueOnBoardAtTokenPosition = game[tokenPosition.Item1, tokenPosition.Item2];

                        // The direct direction neighbour is empty or the 
                        if (toReverse.Count == 0 && valueOnBoardAtTokenPosition == GameProperties.EMPTY || valueOnBoardAtTokenPosition == currentValue)
                        {
                            directionIsEligibleForAMove = false;
                            break;
                        }
                        else if (valueOnBoardAtTokenPosition == opponentValue)
                        {
                            toReverse.Add(tokenPosition);
                        }
                        else if (valueOnBoardAtTokenPosition == GameProperties.EMPTY)
                        {
                            toReverse.Add(tokenPosition);
                            break;
                        }
                        i++;
                    }

                    if (directionIsEligibleForAMove)
                    {
                        if (listPossibleMoves.ContainsKey(tokenPosition))
                            listPossibleMoves[tokenPosition].UnionWith(toReverse);
                        else
                            listPossibleMoves.Add(tokenPosition, toReverse);
                    }
                }
            }
            return listPossibleMoves;
        }

        public static bool BoardContains(int[,] game, Tuple<int, int> position)
        {
            return !(position.Item1 < 0 || position.Item2 < 0 || position.Item1 >= game.GetLength(0) || position.Item2 >= game.GetLength(1));
        }


    }
}

