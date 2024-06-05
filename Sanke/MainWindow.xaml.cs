using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sanke
{
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
        private readonly GameState gameState;
        private bool gameRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid(); // Initialize gridImages in the constructor
            gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            DrawGrid();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                gameState.Move();
                UpdateGrid();
                ScoreText.Text = $"Score: {gameState.Score}";
                await Task.Delay(100);
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
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

        private async Task ShowCountDown()
        {
            for (int i = 3; i > 0; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(1000);
            }
        }

        private async Task ShowGameOver()
        {
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Game Over";
            await Task.Delay(1000);
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}
