using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            //EventHandler += 
        }
       
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new Menu());
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            frame.Width = this.Width;
            frame.Height = this.Height;
        }

        /*private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = e.NewSize.Width;
            this.Height = e.NewSize.Height;

            double xChange = 1, yChange = 1;

            if (e.PreviousSize.Width != 0)
                xChange = (e.NewSize.Width / e.PreviousSize.Width);

            if (e.PreviousSize.Height != 0)
                yChange = (e.NewSize.Height / e.PreviousSize.Height);

            foreach (FrameworkElement fe in this.Children)
            {
                //because I didn't want to resize the grid I'm having inside the canvas in this particular instance. (doing that from xaml)
                if (fe is Grid == false)
                {
                    fe.Height = fe.ActualHeight * yChange;
                    fe.Width = fe.ActualWidth * xChange;

                    Canvas.SetTop(fe, Canvas.GetTop(fe) * yChange);
                    Canvas.SetLeft(fe, Canvas.GetLeft(fe) * xChange);

                }
            }
        }*/
    }

    

    /* Logique : Il ne devrait pas y avoir de fonction "IsPlayable". On devrait plutôt ne laisser le user cliquer que sur les cases qui sont playable.
     * de cette façon il y a pas besoin de checker si elles le sont .
     */
}
