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
using System.Windows.Shapes;

namespace miny
{
    /// <summary>
    /// Interakční logika pro Start.xaml
    /// </summary>
    public partial class Start : Window
    {
        public Start()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Otevře hlavní okno hry
            MainWindow gameWindow = new MainWindow();
            gameWindow.Show();

            // Zavře okno menu
            this.Close();
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            // Ukončí celou aplikaci
            Application.Current.Shutdown();
        }
    }
}

