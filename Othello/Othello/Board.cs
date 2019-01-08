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
        private int[] indices;
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
            indices = new int[height * width];
            for(int i = 0; i< height * width; i++)
            {
                indices[i] = i;
            }
            MakeInitialBoard();
        }

        private void MakeInitialBoard()
        {
            for (var i = 0; i < width * height; i++)
            {
                values[i] = 0;
            }
            int yMid = height >> 1;
            int xMid = width >> 1;
            values[ix(xMid-1, yMid-1)] = 1;
            values[ix(xMid, yMid-1)] = -1;
            values[ix(xMid-1, yMid)] = -1;
            values[ix(xMid, yMid)] = 1;
        }

        /// <summary>
        /// Adds a disk on the played cell and performs flipping operations on disks
        /// </summary>
        /// <param name="x">x-composant of the cell played</param>
        /// <param name="y">y-composant of the cell played</param>
        /// <param name="v">value of the current player (1 or -1)</param>
        public void Play(int x, int y, int v)
        {
            //Console.WriteLine($"Play x:{x} y:{y} index:{x+width*y}");

            /* Here we assume that the move is valid and doable.
             * What we have to do is to find which cell will switch color.
             * To do so we need to save every indices that are in same lines (vertical and horizontal)
             * and diagonals (positive and negative) as the current cell (x,y).
             */
            values[ix(x, y)] = v;
            List<int> verticalIndices = getVerticalIndices(x, y);       // Board indices in vertical line relative to (x,y) cell. 
            List<int> horizontalIndices = getHorizontalIndices(x, y);   // Board indices in horizontal line relative to (x,y) cell. 
            List<int> negativeDiagonalIndices = getNegativeDiagonalIndices(x, y); // Board indices in negative diagonal (\), relative to (x,y) cell.
            List<int> positiveDiagonalIndices = getPositiveDiagonalIndices(x, y); // Board indices in positive diagonal (/), relative to (x,y) cell.

            /* Now that all indices are saved, we need to define which disk will be flipped.
             * The rule for a valid move is the following :
             * 
             * "You can play a disc when you flank one or more opponents discs between your new disc 
             * and any other of your own discs, in the same horizontal, vertical or diagonal line."
             * - source : https://www.yourturnmyturn.com/rules/reversi.php
             * 
             * Assuming the move in x,y is already valid, we only need to define which disk will be flipped
             * and switch their owner.
             */

            List<int> listOfFlippedDisksIndices = getFlippedDisksIndices(verticalIndices, horizontalIndices, negativeDiagonalIndices, positiveDiagonalIndices, v);
            foreach(int diskIndex in listOfFlippedDisksIndices)
            {
                values[diskIndex] = v;
            }

        }

        private List<int> getPositiveDiagonalIndices(int x, int y)
        {
            /* The following formulas may not be perfect nor optimal, but they seem to work for any width and height of the board.
             * At least they work for sure for our (X,Y = 9,7) case.
             */
            List<int> posDiagonalIndices;
            if (x + y < width - 1)
            {
                posDiagonalIndices = (indices.Where((val, index) =>
                index % (width - 1) == (x + width * y) % (width - 1) &&
                index % width <= (x + width * y) % (width - 1) &&
                index / width <= (x + width * y) % (width - 1)
                )).ToList();
            }
            else if ((x + y > width - 1))
            {
                posDiagonalIndices = (indices.Where((val, index) =>
                index % (width - 1) == (x + width * y) % (width - 1) &&
                index % width >= (x + width * y) % (width - 1) &&
                index / width >= (x + width * y) % (width - 1)
                )).ToList();
            }
            else //x + y == width - 1
            {
                posDiagonalIndices = (indices.Where((val, index) =>
                index % (width - 1) == (x + width * y) % (width - 1) &&
                index % width >= (x + width * y) % (width - 1) &&
                index / width >= (x + width * y) % (width - 1) &&
                index != 0)).ToList();
            }
            return posDiagonalIndices;
        }

        private List<int> getNegativeDiagonalIndices(int x, int y)
        {
            /* The following formulas may not be perfect nor optimal, but they seem to work for any width and height of the board.
             * At least they work for sure for our (X,Y = 9,7) case.
             */
            List<int> negDiagonalIndices;
            if (x >= y)
            {
                negDiagonalIndices = (indices.Where((val, index) =>
                index % (width + 1) == (x + width * y) % (width + 1) &&
                index % width >= (x + width * y) % (width + 1)
                )).ToList();
            }
            else
            {
                negDiagonalIndices = (indices.Where((val, index) =>
                index % (width + 1) == (x + width * y) % (width + 1) &&
                index % width < (x + width * y) % (width + 1)
                )).ToList();
            }
            return negDiagonalIndices;
        }

        private List<int> getHorizontalIndices(int x, int y)
        {
            return (indices.Where((val, index) => index / width == y)).ToList();
        }

        private List<int> getVerticalIndices(int x, int y)
        {
            return (indices.Where((val, index) => (index) % width == x)).ToList();
        }

        private List<int> getFlippedDisksIndices(List<int> verticalIndices, List<int> horizontalIndices, List<int> negDiagonalIndices, List<int> posDiagonalIndices, int v)
        {
            List<int> listOfFlippedDisksIndices = new List<int>();
            List<int> verticalValues = getListOfValuesFromListOfIndices(verticalIndices);
            List<int> horizontalValues = getListOfValuesFromListOfIndices(horizontalIndices);
            List<int> negDiagonalValues = getListOfValuesFromListOfIndices(negDiagonalIndices);
            List<int> posDiagonalValues = getListOfValuesFromListOfIndices(posDiagonalIndices);

            //print("VerticalIndices : ", verticalIndices); //OK
            //print("VerticalValues : ", verticalValues); //OK

            //TODO

            //listOfFlippedDisksIndices.Add(22);
            return listOfFlippedDisksIndices;
        }

        private List<int> getListOfValuesFromListOfIndices(List<int> listOfIndices)
        {
            List<int> listOfValues = new List<int>();
            foreach(int index in listOfIndices)
            {
                listOfValues.Add(values[index]);
            }
            return listOfValues;
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

        private static void print(List<int> l)
        {
            foreach (var item in l)
            {
                Console.Write($"[{item}]");
            }
            Console.WriteLine("");
        }

        private static void print(String s, List<int> l)
        {
            Console.Write(s);
            print(l);
        }
    }
}
