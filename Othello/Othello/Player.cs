using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Othello
{
    /// <summary>
    /// This class represents a Player
    /// </summary>
    class Player
    {
        #region Attributes
        private int id;
        private string name;
        private int score;
        private BitmapImage image;
        #endregion

        #region Properties
        public int Score { get => score; set => score = value; }
        public BitmapImage Image { get => image; set => image = value; }
        #endregion

        #region Constructor
        public Player(int id, string name, BitmapImage image)
        {
            this.id = id;
            this.name = name;
            this.Image = image;
        }
        #endregion

    }
}
