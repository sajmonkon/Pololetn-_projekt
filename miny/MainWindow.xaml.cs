using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace miny
{
    public partial class MainWindow : Window
    {
        private const int Rows = 10;
        private const int Columns = 10;
        private const int MineCount = 15;

        private Button[,] buttons;
        private bool[,] mines;
        private bool[,] revealed;
        private bool[,] flagged;
        private bool firstClick = true; // Přidáno pro kontrolu prvního tahu

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();

            buttons = new Button[Rows, Columns];
            mines = new bool[Rows, Columns];
            revealed = new bool[Rows, Columns];
            flagged = new bool[Rows, Columns];
            firstClick = true;

            BitmapImage image = new BitmapImage(new Uri("H:\\projekt miny\\miny\\sydney.jpg", UriKind.Relative));

            for (int i = 0; i < Rows; i++)
                GameGrid.RowDefinitions.Add(new RowDefinition());
            for (int j = 0; j < Columns; j++)
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var button = new Button
                    {
                        Tag = new Tuple<int, int>(i, j),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                    };

                    var rect = new ImageBrush
                    {
                        ImageSource = image,
                        Viewbox = new Rect(j / (double)Columns, i / (double)Rows, 1.0 / Columns, 1.0 / Rows),
                        ViewboxUnits = BrushMappingMode.RelativeToBoundingBox
                    };
                    button.Background = rect;

                    button.Click += Button_Click;
                    button.MouseRightButtonDown += Button_RightClick;

                    buttons[i, j] = button;
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    GameGrid.Children.Add(button);
                }
            }
        }

        private void PlaceMines(int excludeRow, int excludeCol)
        {
            Random random = new Random();
            int placedMines = 0;

            while (placedMines < MineCount)
            {
                int row = random.Next(Rows);
                int col = random.Next(Columns);

                // Zabrání umístění min na políčko prvního kliknutí a jeho okolí
                if (Math.Abs(row - excludeRow) <= 1 && Math.Abs(col - excludeCol) <= 1)
                    continue;

                if (!mines[row, col])
                {
                    mines[row, col] = true;
                    placedMines++;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var (row, col) = (Tuple<int, int>)button.Tag;

                if (flagged[row, col]) return;

                if (firstClick)
                {
                    firstClick = false;
                    PlaceMines(row, col); // Umístí miny, vynechá oblast kolem prvního kliknutí
                    RevealSafeArea(row, col); // Odhalí bezpečnou oblast kolem prvního kliknutí
                }

                if (mines[row, col])
                {
                    button.Background = Brushes.Red;
                    var explosionWindow = new Window1();
                    explosionWindow.ShowDialog();
                    InitializeGame();
                }
                else
                {
                    RevealCell(row, col);
                    CheckWinCondition();
                }
            }
        }

        private void Button_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                var (row, col) = (Tuple<int, int>)button.Tag;

                if (revealed[row, col]) return;

                flagged[row, col] = !flagged[row, col];
                button.Content = flagged[row, col] ? "🚩" : null;
            }
        }

        private void RevealSafeArea(int startRow, int startCol)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int row = startRow + i;
                    int col = startCol + j;

                    if (row >= 0 && row < Rows && col >= 0 && col < Columns)
                    {
                        RevealCell(row, col);
                    }
                }
            }
        }

        private void RevealCell(int row, int col)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Columns || revealed[row, col] || flagged[row, col])
                return;

            revealed[row, col] = true;
            buttons[row, col].IsEnabled = false;

            int mineCount = CountAdjacentMines(row, col);
            if (mineCount > 0)
            {
                buttons[row, col].Content = mineCount.ToString();
                buttons[row, col].Background = Brushes.LightGray;
            }
            else
            {
                buttons[row, col].Background = Brushes.LightGray;

                RevealCell(row - 1, col);
                RevealCell(row + 1, col);
                RevealCell(row, col - 1);
                RevealCell(row, col + 1);
                RevealCell(row - 1, col - 1);
                RevealCell(row - 1, col + 1);
                RevealCell(row + 1, col - 1);
                RevealCell(row + 1, col + 1);
            }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int r = row + i;
                    int c = col + j;

                    if (r >= 0 && r < Rows && c >= 0 && c < Columns && mines[r, c])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void CheckWinCondition()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (!mines[i, j] && !revealed[i, j])
                    {
                        return; // Not all safe cells revealed
                    }
                }
            }

            MessageBox.Show("Gratulujeme! Vyhráli jste!");
            InitializeGame();
        }

        private void ReturnToMenu_Click(object sender, RoutedEventArgs e)
        {
            Start startWindow = new Start();
            startWindow.Show();
            this.Close();
        }
    }
}
