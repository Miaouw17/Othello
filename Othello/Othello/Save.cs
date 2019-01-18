using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    [Serializable]
    class Save
    {
        public int[] values;
        public bool isWhiteTurn;
        public long tsWhitePlayer;
        public long tsBlackPlayer;
        public Stack<Tuple<int[], bool>> stackUndo;
        public Stack<Tuple<int[], bool>> stackRedo;

        public Save(int[] values, bool isWhiteTurn, long tsWhitePlayer, long tsBlackPlayer, Stack<Tuple<int[], bool>> stackUndo, Stack<Tuple<int[], bool>> stackRedo)
        {
            this.values = values;
            this.isWhiteTurn = isWhiteTurn;
            this.tsWhitePlayer = tsWhitePlayer;
            this.tsBlackPlayer = tsBlackPlayer;
            this.stackUndo = stackUndo;
            this.stackRedo = stackRedo;
        }
    }
}
