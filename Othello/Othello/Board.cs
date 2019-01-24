using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    /// <summary>
    /// This class represents an Othello game board and its rules
    /// </summary>
    class OthelloBoard : IPlayable.IPlayable
    {
        #region Attributes
        private String name;
        private int[] values;
        private int[] indices;
        private List<int> nextPossibleMoves;
        private int width;
        private int height;
        #endregion

        #region Properties
        public int this[int i] {
            get => Values[i];
            set => Values[i] = value;
        }
        public int Width { get => width;}
        public int Height { get => height;}
        public List<int> NextPossibleMoves { get => nextPossibleMoves; set => nextPossibleMoves = value; }
        public int[] Values { get => values; set => values = value; }
        #endregion

        #region Constructors
        public OthelloBoard(String name, int width, int height)
        {
            this.name = name;
            this.height = height;
            this.width = width;
            this.nextPossibleMoves = new List<int>();
            Values = new int[height * width];
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
            Values = new int[height * width];
            indices = new int[height * width];
            for (int i = 0; i < height * width; i++)
            {
                indices[i] = i;
            }

            for (int i = 0; i < height * width; i++)
            {
                this.Values[i] = board[i];
            }

            UpdateNextPossibleMoves(isWhiteTurn ? 1 : -1);
        }

        #endregion

        #region Private Methods
        private void MakeInitialBoard()
        {
            for (var i = 0; i < width * height; i++)
            {
                Values[i] = 0;
            }
            int yMid = height >> 1;
            int xMid = width >> 1;
            Values[ix(xMid - 1, yMid)] = 1;
            Values[ix(xMid, yMid)] = -1;
            Values[ix(xMid - 1, yMid + 1)] = -1;
            Values[ix(xMid, yMid + 1)] = 1;
            UpdateNextPossibleMoves(1); // 1 : White begins
        }

        private List<int> getAllFlips(int x, int y, int v)
        {
            List<int> verticalIndices = getVerticalIndices(x, y);       // Board indices in vertical line relative to (x,y) cell. 
            List<int> horizontalIndices = getHorizontalIndices(x, y);   // Board indices in horizontal line relative to (x,y) cell. 
            List<int> negativeDiagonalIndices = getNegativeDiagonalIndices(x, y); // Board indices in negative diagonal (\), relative to (x,y) cell.
            List<int> positiveDiagonalIndices = getPositiveDiagonalIndices(x, y); // Board indices in positive diagonal (/), relative to (x,y) cell.
            List<int> allFlips = new List<int>();
            allFlips.AddRange(getFlips(verticalIndices, getListOfValuesFromListOfIndices(verticalIndices), v, ix(x, y)));
            allFlips.AddRange(getFlips(horizontalIndices, getListOfValuesFromListOfIndices(horizontalIndices), v, ix(x, y)));
            allFlips.AddRange(getFlips(positiveDiagonalIndices, getListOfValuesFromListOfIndices(positiveDiagonalIndices), v, ix(x, y)));
            allFlips.AddRange(getFlips(negativeDiagonalIndices, getListOfValuesFromListOfIndices(negativeDiagonalIndices), v, ix(x, y)));
            return allFlips;
        }

        private List<int> getPositiveDiagonalIndices(int x, int y)
        {
            /* The following formulas may not be perfect nor optimal, but they seem to work for any width and height of the board.
             * At least they work for sure for our (X,Y = 9,7) case.
             * 
             * This version is shaped like python's list comprehensions but happens that C# Linq is way less efficient. 
             * It works well since the board is not too large, but in a general case we should find an other way to find thoses indices.
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
                if (width == height && x == width - 1 && y == height - 1) //Horrific but quickly fixes a bug for square maps
                {
                    posDiagonalIndices = new List<int>();
                    posDiagonalIndices.Add(x + y * height);
                }
                else
                {
                    posDiagonalIndices = (indices.Where((val, index) =>
                    index % (width - 1) == (x + width * y) % (width - 1) &&
                    index % width >= (x + width * y) % (width - 1) &&
                    index / width >= (x + width * y) % (width - 1)
                    )).ToList();
                }


            }
            else //x + y == width - 1
            {
                posDiagonalIndices = (indices.Where((val, index) =>
                index % (width - 1) == (x + width * y) % (width - 1) &&
                index % width >= (x + width * y) % (width - 1) &&
                index / width >= (x + width * y) % (width - 1) &&
                index != 0)).ToList();
                if (width == height) //Horrific but quickly fixes a bug for square maps
                {
                    posDiagonalIndices.RemoveAt(posDiagonalIndices.Count() - 1);
                }
            }
            return posDiagonalIndices;
        }

        private List<int> getNegativeDiagonalIndices(int x, int y)
        {
            /* The following formulas may not be perfect nor optimal, but they seem to work for any width and height of the board.
             * At least they work for sure for our (X,Y = 9,7) case.
             * 
             * This version is shaped like python's list comprehensions but happens that C# Linq is way less efficient. 
             * It works well since the board is not too large, but in a general case we should find an other way to find thoses indices.
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

        private List<int> getFlips(List<int> indices, List<int> values, int v, int index)
        {
            List<int> flips = new List<int>();

            int indexOfIndex = indices.IndexOf(index);

            List<int> sublist1 = values.GetRange(0, indexOfIndex);
            List<int> sublist2 = values.GetRange(indexOfIndex + 1, values.Count() - sublist1.Count() - 1);
            sublist1.Reverse();


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
            foreach (int index in listOfIndices)
            {
                listOfValues.Add(Values[index]);
            }
            return listOfValues;
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the list of possible next move according to the current turn.
        /// </summary>
        /// <param name="v"></param>
        public void UpdateNextPossibleMoves(int v)
        {
            NextPossibleMoves.Clear();
            for (int i = 0; i < width * height; i++)
            {
                if(Values[i]==0)
                {
                    /* TODO : improve
                    * This is easily improvable by stoping the getAllFlips function at first flip found.
                    * getAllFlips could  also be multithreaded as there is no concurrency possible since
                    * all lists and sublists are working on different indices.
                    */
                    if (getAllFlips(i % width, i / width, v).Count()>0)
                    {
                        NextPossibleMoves.Add(i);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a disk on the played cell and performs flipping operations on disks
        /// </summary>
        /// <param name="x">x-composant of the cell played</param>
        /// <param name="y">y-composant of the cell played</param>
        /// <param name="v">value of the current player (1 or -1)</param>
        public void Play(int x, int y, int v)
        {
            Values[ix(x, y)] = v;
            /* Here we assume that the move is valid and playable.
             * What we have to do is to find which cell will switch color.
             * To do so we need to save every indices that are in same lines (vertical and horizontal)
             * and diagonals (positive and negative) as the current cell (x,y).
             */
            List<int> listOfFlippedDisksIndices = getAllFlips(x, y, v);
            foreach(int diskIndex in listOfFlippedDisksIndices)
            {
                Values[diskIndex] = v;
            }
            UpdateNextPossibleMoves(v*-1); // Update the list of possible moves for next player (v*=-1)
        }    

        /// <summary>
        /// Displays the board values.
        /// </summary>
        /// <returns>String representing the board state.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sb.Append($"{Values[ix(x, y)]}");
                    if(x < width-1)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

       

        /// <summary>
        /// Deep copy of the board values
        /// </summary>
        public int[] GetValues()
        {
            int[] copy = new int[values.Count()];
            for(int i=0;i<values.Count();i++)
            {
                copy[i] = values[i];
            }
            return copy;
        }

       

        /// <summary>
        /// Get the name of the board.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return this.name;
        }


        /// <summary>
        /// Returns of the move is playable
        /// </summary>
        /// <param name="x">X coord. of the move</param>
        /// <param name="y">Y coord. of the move</param>
        /// <param name="isWhite">Current turn, true=white.</param>
        /// <returns></returns>
        public bool IsPlayable(int x, int y, bool isWhite)
        {
            //TODO : This has to be optimized ! (Works for now)
            return (Values[ix(x, y)] == 0) && (this.getAllFlips(x, y, isWhite ? 1 : -1).Count() != 0);
        }

        /// <summary>
        /// Plays the move.
        /// </summary>
        /// <param name="column">X coord of the move</param>
        /// <param name="line">Y coord. of the move</param>
        /// <param name="isWhite">Current turn, true=white.</param>
        /// <returns></returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            this.Play(column, line, isWhite ? 1 : -1);
            return true; //TODO : Find out what this method should return, ask olvier.husser@he-arc.ch
        }

        /// <summary>
        /// This method is used by our AI to find out what the next best move will be.
        /// NOTE : Not implemented yet !
        /// </summary>
        /// <param name="game">board state</param>
        /// <param name="level">depth of search for minmax algorithm</param>
        /// <param name="whiteTurn">Current turn, true=white</param>
        /// <returns></returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            //TODO for IA.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the board state
        /// </summary>
        /// <returns>Current board state in shape of 2D int array</returns>
        public int[,] GetBoard()
        {
            int[,] output = new int[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    output[i, j] = Values[i * width + j];
                }
            }
            return output;
        }

        /// <summary>
        /// Gets the white player's score
        /// </summary>
        /// <returns></returns>
        public int GetWhiteScore()
        {
            int score = 0;
            foreach (var v in Values)
            {
                if (v == 1)
                {
                    score++;
                }
            }
            return score;
        }

        /// <summary>
        /// Get the black player's score
        /// </summary>
        /// <returns></returns>
        public int GetBlackScore()
        {
            int score = 0;
            foreach (var v in Values)
            {
                if (v == -1)
                {
                    score++;
                }
            }
            return score;
        }

        #endregion
    }
}
