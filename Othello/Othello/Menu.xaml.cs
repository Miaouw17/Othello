using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Game());
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            // Readfile and load a game
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary file|*.bin";
            openFileDialog.Title = "Load your game";

            if(openFileDialog.ShowDialog() == true)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                Save save = (Save)formatter.Deserialize(stream);
                stream.Close();
                this.NavigationService.Navigate(new Game(save.values, save.isWhiteTurn, save.tsWhitePlayer, save.tsBlackPlayer, save.stackUndo, save.stackRedo));
            }
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
