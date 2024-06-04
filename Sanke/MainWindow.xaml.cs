using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sanke
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };
        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid(); // Initialize gridImages in the constructor
            gameState = new GameState(rows, cols);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGrid();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
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

        private void DrawGrid()
        {
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                }
            }
        }
    }
}
