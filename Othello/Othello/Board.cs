using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    class OthelloBoard
    {
        private int[] values;
        private int width;
        private int height;

        public int this[int i] {
            get => values[i];
            set => values[i] = value;
        }
        public int Width { get => width;}
        public int Height { get => height;}

        //enum CellState { BLACK=-1, WHITE=1, EMPTY=0};
        public OthelloBoard(int width, int height)
        {
            this.height = height;
            this.width = width;
            values = new int[height * width];
            MakeInitialBoard();
        }

        private void MakeInitialBoard()
        {
            for (var y = 0; y < width; y++)
                for (var x = 0; x < height; x++)
                    values[ix(x, y)] = 0;
            int yMid = height >> 1;
            int xMid = width >> 1;
            values[ix(yMid - 1, xMid - 1)] = 1;
            values[ix(yMid, xMid - 1)] = -1;
            values[ix(yMid - 1, xMid)] = -1;
            values[ix(yMid, xMid)] = 1;
        }

        public void Play(int x, int y, int v)
        {
            values[ix(x, y)] = v;
            List<int> verticalValues = (values.Where((val, index) => (index) % width == x)).ToList();
            List<int> horizontalValues = (values.Where((val, index) => index / width == y)).ToList();
            List<int> negDiagonalValues;
            List<int> posDiagonalValues;
            if (x > y) negDiagonalValues = (values.Where((val, index) => index % (width + 1) == (x + width * y) % (width + 1) && index % width >= (x + width * y) % (width + 1))).ToList();
            else negDiagonalValues = (values.Where((val, index) => index % (width + 1) == (x + width * y) % (width + 1) && index % width < (x + width * y) % (width + 1))).ToList();
            if (x + y < width) posDiagonalValues = (values.Where((val, index) => index % (width - 1) == (x + width * y) % (width - 1) && index % width <= (x + width * y) % (width - 1) && index / width <= (x + width * y) % (width - 1))).ToList();
            else posDiagonalValues = (values.Where((val, index) => index % (width - 1) == (x + width * y) % (width - 1) && index % width > (x + width * y) % (width - 1) && index / width > (x + width * y) % (width - 1))).ToList();
            Console.WriteLine($"Play x:{x} y:{y} index:{x+width*y}");
            Console.WriteLine($"Count(diagMin):{negDiagonalValues.Count()}");
            foreach (int e in negDiagonalValues)
            {
                Console.Write($"[{e}]");
            }
            Console.WriteLine("");
        }

        public void DisplayBoard()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write($"{values[ix(x, y)]}");
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
                    debug.Append($"[{values[ix(x, y)]}]");
                }
                debug.Append("\n");
            }
            Console.WriteLine(debug.ToString());
            return debug.ToString();
        }
        private int ix(int x, int y)
        {
            return x + y * width;
        }
    }
}
