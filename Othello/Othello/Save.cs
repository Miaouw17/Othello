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

        public Save(int[] values, bool isWhiteTurn, long tsWhitePlayer, long tsBlackPlayer)
        {
            this.values = values;
            this.isWhiteTurn = isWhiteTurn;
            this.tsWhitePlayer = tsWhitePlayer;
            this.tsBlackPlayer = tsBlackPlayer;
        }
    }
}
