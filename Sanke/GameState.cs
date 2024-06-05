using System.Runtime.CompilerServices;

namespace Sanke
{
    public class GameState
    {
        // Properties for the game state
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Direction { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        // Buffers to store directions and snake positions
        private readonly LinkedList<Direction> directionBuffer = new();
        private readonly LinkedList<Position> snakePositions = new();
        private readonly Random random = new();

        // Constructor
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Direction = Direction.Right;
            Score = 0;
            GameOver = false;

            AddSnake(); // Initialize the snake on the grid
            AddFood(); // Add food on the grid
        }

        // Add the initial snake to the grid
        public void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 0; c < 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        // Get all empty positions on the grid
        private IEnumerable<Position> EmptyPosition()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        // Add food to a random empty position on the grid
        private void AddFood()
        {
            List<Position> emptyPositions = new(EmptyPosition());

            if (emptyPositions.Count == 0)
            {
                GameOver = true;
                return;
            }

            Position position = emptyPositions[random.Next(emptyPositions.Count)];
            Grid[position.Row, position.Col] = GridValue.Food;
        }

        // Get the current head position of the snake
        public Position? HeadPosition()
        {
            return snakePositions.First?.Value;
        }

        // Get the current tail position of the snake
        public Position? TailPosition()
        {
            return snakePositions.Last?.Value;
        }

        // Get all positions occupied by the snake
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        // Add a new head to the snake
        private void AddHead(Position position)
        {
            snakePositions.AddFirst(position);
            Grid[position.Row, position.Col] = GridValue.Snake;
        }

        // Remove the tail of the snake
        private void RemoveTail()
        {
            if (snakePositions.Last != null)
            {
                Position tail = snakePositions.Last.Value;
                Grid[tail.Row, tail.Col] = GridValue.Empty;
                snakePositions.RemoveLast();
            }
        }

        // Get the last direction from the buffer
        private Direction GetLastDirection()
        {
            return directionBuffer.Count > 0 ? directionBuffer.Last.Value : Direction;
        }

        // Check if the direction can be changed
        private bool CanChangeDirection(Direction newDirection)
        {
            if (directionBuffer.Count == 2)
            {
                return false;
            }

            Direction lastDirection = GetLastDirection();
            return newDirection != lastDirection && newDirection != lastDirection.Opposite();
        }

        // Change the direction of the snake
        public void ChangeDirection(Direction direction)
        {
            if (CanChangeDirection(direction))
            {
                directionBuffer.AddLast(direction);
            }
        }

        // Check if the position is out of the grid bounds
        private bool OutOfBounds(Position position)
        {
            return position.Row < 0 || position.Row >= Rows || position.Col < 0 || position.Col >= Cols;
        }

        // Detect collision at the new head position
        private GridValue CollisionDetection(Position newHeadPos)
        {
            if (OutOfBounds(newHeadPos))
            {
                return GridValue.outOfBounds;
            }

            if (newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        // Move the snake
        public void Move()
        {
            if (directionBuffer.Count > 0)
            {
                Direction = directionBuffer.First.Value;
                directionBuffer.RemoveFirst();
            }

            Position? newHeadPos = HeadPosition()?.Translate(Direction);

            if (newHeadPos == null)
            {
                GameOver = true;
                return;
            }

            GridValue collision = CollisionDetection(newHeadPos);

            if (collision == GridValue.Snake || collision == GridValue.outOfBounds)
            {
                GameOver = true;
                return;
            }
            else if (collision == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (collision == GridValue.Food)
            {
                Score++;
                AddHead(newHeadPos);
                AddFood();
            }
        }

        // Reset the game state
        internal void Reset()
        {
            Array.Clear(Grid, 0, Grid.Length);
            directionBuffer.Clear();
            snakePositions.Clear();

            Direction = Direction.Right;
            Score = 0;
            GameOver = false;

            AddSnake(); // Reinitialize the snake
            AddFood(); // Reinitialize the food
        }
    }
}
