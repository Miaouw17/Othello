using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    class OthelloBoard
    {
        private int[,] values;
        private int width;
        private int height;

        public int[,] Values { get => values; }
        public int Width { get => width;}
        public int Height { get => height;}

        //enum CellState { BLACK=-1, WHITE=1, EMPTY=0};
        public OthelloBoard(int width, int height)
        {
            this.height = height;
            this.width = width;
            values = new int[height, width];
            MakeInitialBoard();
        }

        private void MakeInitialBoard()
        {
            for (var y = 0; y < width; y++)
                for (var x = 0; x < height; x++)
                    values[y,x] = 0;
            values[height / 2 - 1, width / 2 - 1] = 1;
            values[height / 2 , width / 2 - 1] = -1;
            values[height / 2 - 1, width / 2] = -1;
            values[height / 2, width / 2] = 1;
        }

        public void Play(int x, int y, int value)
        {
            values[y, x] = value;
        }

        public void DisplayBoard()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write($"{values[y,x]}");
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.ReadLine();
        }

        public override string ToString()
        {
            StringBuilder debug = new StringBuilder();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write($"[{values[y,x]}]");
                    debug.Append($"[{values[y,x]}]");
                }
                debug.Append("\n");
            }
            Console.WriteLine(debug.ToString());
            return debug.ToString();
        }
    }
}
