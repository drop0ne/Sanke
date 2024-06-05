using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sanke
{
    public partial class MainWindow : Window
    {
        // Maps grid values to their corresponding image sources
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        // Maps directions to their corresponding rotation values
        private readonly Dictionary<Direction, int> dirToRatation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 }
        };

        // Number of rows and columns in the grid
        private readonly int rows = 15, cols = 15;
        // 2D array to hold the grid images
        private readonly Image[,] gridImages;
        // Holds the game state
        private readonly GameState gameState;
        // Flag to track if the game is running
        private bool gameRunning = false;

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid(); // Initialize grid images
            gameState = new GameState(rows, cols); // Initialize game state
        }

        // Main game loop
        private async Task RunGame()
        {
            DrawGrid();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            // Reset the game state for a new game
            gameState.Reset();
        }

        // Handle key press events
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // If the overlay is visible, ignore the key press
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            // If the game is not running, start it
            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        // Handle direction change on key down
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            // Change direction based on the key pressed
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

        // Main game loop
        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                gameState.Move();
                UpdateGrid();
                ScoreText.Text = $"Score: {gameState.Score}";
                await Task.Delay(100); // Delay for game speed
            }
        }

        // Setup the grid images
        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image img = new()
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5),
                    };

                    images[r, c] = img;
                    GameGrid.Children.Add(img); // Add image to the grid
                }
            }
            return images;
        }

        // Draw the entire grid
        private void DrawGrid()
        {
            UpdateGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        // Update the grid based on the game state
        private void UpdateGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity; // Reset transform
                }
            }
        }

        // Draw the snake head with the correct orientation
        private void DrawSnakeHead()
        {
            // Retrieve the current head position from the game state
            Position? headPosition = gameState.HeadPosition();

            // Ensure the head position is within the grid bounds
            if (headPosition != null && headPosition.Row >= 0 && headPosition.Row < rows && headPosition.Col >= 0 && headPosition.Col < cols)
            {
                // Get the image element at the head position in the grid
                Image headImage = gridImages[headPosition.Row, headPosition.Col];

                // Set the source to the head image
                headImage.Source = Images.Head;

                // Retrieve the rotation value based on the current direction
                int rotation = dirToRatation[gameState.Direction];

                // Apply the rotation transform to the image
                headImage.RenderTransform = new RotateTransform(rotation);
            }
        }

        // Show a countdown before the game starts
        private async Task ShowCountDown()
        {
            for (int i = 3; i > 0; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(1000); // Wait for 1 second
            }
        }

        // Show the game over screen
        private async Task ShowGameOver()
        {
            await Task.Delay(1000); // Wait for 1 second
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Game Over";
            await Task.Delay(2000); // Wait for 2 seconds
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}
