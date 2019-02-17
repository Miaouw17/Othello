using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello.AI
{
    class BoardState
    {
        public int[,] Board { get; private set; }
        public BoardState(int[,] board)
        {
            Board = board;
        }

        public BoardState NewBoardWithMove(Tuple<int, int> move, int vPlayer)
        {
            int width = GameProperties.WIDTH;
            int height = GameProperties.HEIGHT;
            int[,] newBoard = new int[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newBoard[y, x] = Board[y, x];
                }
            }
            newBoard[move.Item2, move.Item1] = vPlayer; //?
            return new BoardState(newBoard);
        }

        public static BoardState InitialState()
        {
            int width = GameProperties.WIDTH;
            int height = GameProperties.HEIGHT;
            int[,] newBoard = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    newBoard[i, j] = GameProperties.EMPTY;
                }
            }
            int yMid = height >> 1;
            int xMid = width >> 1;
            newBoard[xMid - 1, yMid] = GameProperties.WHITE;
            newBoard[xMid, yMid] = GameProperties.BLACK;
            newBoard[xMid - 1, yMid + 1] = GameProperties.BLACK;
            newBoard[xMid, yMid + 1] = GameProperties.WHITE;
            return new BoardState(newBoard);
        }
    }
}
