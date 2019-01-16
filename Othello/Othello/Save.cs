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
        public TimeSpan tsJ1;
        public TimeSpan tsJ2;
    }
}
