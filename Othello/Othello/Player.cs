using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Othello
{
    class Player
    {
        private int id;
        private string name;
        private BitmapImage image;
        //private List<Pion> listPion; peut être utile plutot que de juste placer des image sur board

        public Player(int id, string name, BitmapImage image)
        {
            this.id = id;
            this.name = name;
            this.image = image;
        }

        public BitmapImage getImage()
        {
            return this.image;
        }
    }
}
