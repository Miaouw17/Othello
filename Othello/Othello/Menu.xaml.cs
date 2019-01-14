﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
            openFileDialog.Filter = "Text file|*.txt|Csv file|*.csv";
            openFileDialog.Title = "Load your game";

            if(openFileDialog.ShowDialog() == true)
            {
                int height = 0;
                int width = 0;
                int[] board;
                string filename = openFileDialog.FileName;
                //Console.Write(File.ReadAllText(filename));
                using (var reader = new StreamReader(filename))
                {
                    int index = 0;
                    height = File.ReadAllLines(filename).Count();
                    var firstval = reader.ReadLine().Split(',');
                    width = firstval.Count();
                    board = new int[height * width];

                    // parce que le reader est deja a la fin de la premiere ligne
                    foreach (var v in firstval)
                    {
                        if (v != "")
                        {
                            board[index] = Convert.ToInt32(v);
                            index++;
                        }
                    }

                    // débute à la 2eme ligne (moyen d'opti mais j'aimerais que ça marche avant)
                    while (!reader.EndOfStream)
                    {
                        foreach(var v in reader.ReadLine().Split(','))
                        {
                            if (v != "")
                            {
                                board[index] = Convert.ToInt32(v);
                                index++;
                            }
                        }
                    }
                    Console.WriteLine(width);
                    Console.WriteLine(height-1);
                    for (int i = 0; i < index; i++)
                    {
                        Console.Write(board[i]);
                        if((i+1) % width == 0)
                        {
                            Console.WriteLine();
                        }
                    }
                   this.NavigationService.Navigate(new Game(board));
                }
            }
        }
    }
}
