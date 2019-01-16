using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    class OthelloBoard : IPlayable.IPlayable
    {
        private String name;
        private int[] values;
        private int[] indices;
        private List<int> nextPossibleMoves;
        private int width;
        private int height;

        public int this[int i] {
            get => values[i];
            set => values[i] = value;
        }
        public int Width { get => width;}
        public int Height { get => height;}
        public List<int> NextPossibleMoves { get => nextPossibleMoves; set => nextPossibleMoves = value; }

        //enum CellState { BLACK=-1, WHITE=1, EMPTY=0};
        public OthelloBoard(String name, int width, int height)
        {
            this.name = name;
            this.height = height;
            this.width = width;
            this.nextPossibleMoves = new List<int>();
            values = new int[height * width];
            indices = new int[height * width];
            for(int i = 0; i< height * width; i++)
            {
                indices[i] = i;
            }
            MakeInitialBoard();
        }

        public OthelloBoard(String name, int width, int height, int[] board, bool isWhiteTurn)
        {
            this.name = name;
            this.height = height;
            this.width = width;
            this.nextPossibleMoves = new List<int>();
            values = new int[height * width];
            indices = new int[height * width];
            for (int i = 0; i < height * width; i++)
            {
                indices[i] = i;
            }

            for (int i = 0; i < height * width; i++)
            {
                this.values[i] = board[i];
            }

            UpdateNextPossibleMoves(isWhiteTurn ? 1 : -1);
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
            UpdateNextPossibleMoves(1); // 1 : White begins
        }

        private void UpdateNextPossibleMoves(int v)
        {
            NextPossibleMoves.Clear();
            for (int i = 0; i < width * height; i++)
            {
                if(values[i]==0)
                {
                    if (getAllFlips(i % width, i / width, v).Count()>0) //TODO : improve
                    {
                        NextPossibleMoves.Add(i);
                        Console.Write($" {i} ");
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Adds a disk on the played cell and performs flipping operations on disks
        /// </summary>
        /// <param name="x">x-composant of the cell played</param>
        /// <param name="y">y-composant of the cell played</param>
        /// <param name="v">value of the current player (1 or -1)</param>
        public void Play(int x, int y, int v)
        {
            int index = ix(x,y);
            Console.WriteLine($"Play x:{x} y:{y} index:{index}");

            /* Here we assume that the move is valid and doable.
             * What we have to do is to find which cell will switch color.
             * To do so we need to save every indices that are in same lines (vertical and horizontal)
             * and diagonals (positive and negative) as the current cell (x,y).
             */
            values[index] = v;
            

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

            List<int> listOfFlippedDisksIndices = getAllFlips(x, y, v);
            foreach(int diskIndex in listOfFlippedDisksIndices)
            {
                values[diskIndex] = v;
            }
            UpdateNextPossibleMoves(v*-1); // Update the list of possible moves for next player (v*=-1)
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

        public List<int> getAllFlips(int x, int y, int v)
        {
            List<int> verticalIndices = getVerticalIndices(x, y);       // Board indices in vertical line relative to (x,y) cell. 
            List<int> horizontalIndices = getHorizontalIndices(x, y);   // Board indices in horizontal line relative to (x,y) cell. 
            List<int> negativeDiagonalIndices = getNegativeDiagonalIndices(x, y); // Board indices in negative diagonal (\), relative to (x,y) cell.
            List<int> positiveDiagonalIndices = getPositiveDiagonalIndices(x, y); // Board indices in positive diagonal (/), relative to (x,y) cell.
            List<int> allFlips = new List<int>();
            allFlips.AddRange(getFlips(verticalIndices, getListOfValuesFromListOfIndices(verticalIndices), v, ix(x,y)));
            allFlips.AddRange(getFlips(horizontalIndices, getListOfValuesFromListOfIndices(horizontalIndices), v, ix(x, y)));
            allFlips.AddRange(getFlips(positiveDiagonalIndices, getListOfValuesFromListOfIndices(positiveDiagonalIndices), v, ix(x, y)));
            allFlips.AddRange(getFlips(negativeDiagonalIndices, getListOfValuesFromListOfIndices(negativeDiagonalIndices), v, ix(x, y)));
            return allFlips;
        }

        private List<int> getFlips(List<int> indices, List<int> values, int v, int index)
        {
            List<int> flips = new List<int>();
            //print("VerticalValues : ", values);
            //print("VerticalIndices : ", indices);

            int indexOfIndex = indices.IndexOf(index);
            //Console.WriteLine($"Index of index : {indexOfIndex}");

            List<int> sublist1 = values.GetRange(0, indexOfIndex);
            List<int> sublist2 = values.GetRange(indexOfIndex + 1, values.Count() - sublist1.Count() - 1);
            sublist1.Reverse();

            //print("SubList1 : ", sublist1);
            //print("SubList2 : ", sublist2);


            // Not happy at all with this version but works fine enough for now.
            List<int> tmpList = new List<int>();
            bool validated = false;
            int indexInSublist = 0;
            if (sublist1.Count() >= 2)
            {
                foreach (int value in sublist1)
                {
                    if (value == v * -1)
                    {
                        tmpList.Add(indices.ElementAt(indexOfIndex - indexInSublist - 1));
                    }
                    else if (value == 0)
                    {
                        break;
                    }
                    else if (value == v)
                    {
                        validated = true;
                        break;
                    }
                    indexInSublist++;
                }
                if (validated)
                {
                    foreach (int i in tmpList)
                    {
                        flips.Add(i);
                    }
                }
            }
            tmpList.Clear();
            validated = false;
            indexInSublist = 0;
            if (sublist2.Count() >= 2)
            {
                foreach (int value in sublist2)
                {
                    if (value == 0)
                    {
                        break;
                    }
                    else if (value == v * -1)
                    {
                        tmpList.Add(indices.ElementAt(indexOfIndex + 1 + indexInSublist));
                    }
                    else if (value == v)
                    {
                        validated = true;
                        break;
                    }
                    indexInSublist++;
                }
                if (validated)
                {
                    foreach (int i in tmpList)
                    {
                        flips.Add(i);
                    }
                }
            }

            return flips;
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

        public void DisplayBoardDebug()
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
                    debug.Append($"{values[ix(x, y)]}");
                    if(x < width-1)
                    {
                        debug.Append(",");
                    }
                }
                debug.Append("\n");
            }
            Console.WriteLine(debug.ToString());
            return debug.ToString();
        }

        public int[] GetValues()
        {
            return values;
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

        public string GetName()
        {
            return this.name;
        }

        public bool IsPlayable(int x, int y, bool isWhite)
        {
            //TODO : This has to be optimized ! (Works for now)
            return (values[ix(x, y)] == 0) && (this.getAllFlips(x, y, isWhite ? 1 : -1).Count() != 0);
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            this.Play(column, line, isWhite ? 1 : -1);
            return true; //TODO : Find out what this method should return ask olvier.husser@he-arc.ch
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            //TODO
            throw new NotImplementedException();
        }

        public int[,] GetBoard()
        {
            int[,] output = new int[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    output[i, j] = values[i * width + j];
                }
            }
            return output;
        }

        public int GetWhiteScore()
        {
            int score = 0;
            foreach (var v in values)
            {
                if (v == 1)
                {
                    score++;
                }
            }
            return score;
        }

        public int GetBlackScore()
        {
            int score = 0;
            foreach (var v in values)
            {
                if (v == -1)
                {
                    score++;
                }
            }
            return score;
        }
    }
}
