using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Sanke
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid(); // Initialize gridImages in the constructor
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image img = new()
                    {
                        Source = Images.Empty // Assuming Images.Empty is defined elsewhere
                    };

                    images[r, c] = img;
                    GameGrid.Children.Add(img);
                }
            }
            return images;
        }
    }
}
