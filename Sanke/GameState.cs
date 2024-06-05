using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sanke
{
    public class GameState
    {
        // Public properties representing the game state
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Direction { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        // Private fields for internal state management
        private readonly LinkedList<Direction> directionBuffer = new();
        private readonly LinkedList<Position> snakePositions = new();
        private readonly Random random = new();

        // Constructor initializing the game state
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Direction = Direction.Right; // Initial direction is right
            Score = 0;
            GameOver = false;

            AddSnake(); // Add the initial snake to the grid
            AddFood();  // Add the initial food to the grid
        }

        // Adds the initial snake to the center of the grid
        public void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 0; c < 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        // Returns an enumerable of all empty positions on the grid
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

        // Adds food to a random empty position on the grid
        private void AddFood()
        {
            List<Position> emptyPositions = new(EmptyPosition());

            if (emptyPositions.Count == 0)
            {
                GameOver = true; // Game over if no empty positions are left
                return;
            }

            Position position = emptyPositions[random.Next(emptyPositions.Count)];
            Grid[position.Row, position.Col] = GridValue.Food;
        }

        // Returns the position of the snake's head
        public Position? HeadPosition()
        {
            return snakePositions.First?.Value;
        }

        // Returns the position of the snake's tail
        public Position? TailPosition()
        {
            return snakePositions.Last?.Value;
        }

        // Returns all positions occupied by the snake
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        // Adds a new head position for the snake
        private void AddHead(Position position)
        {
            snakePositions.AddFirst(position);
            Grid[position.Row, position.Col] = GridValue.Snake;
        }

        // Removes the tail position of the snake
        private void RemoveTail()
        {
            if (snakePositions.Last != null)
            {
                Position tail = snakePositions.Last.Value;
                Grid[tail.Row, tail.Col] = GridValue.Empty;
                snakePositions.RemoveLast();
            }
        }

        // Gets the last direction from the direction buffer
        private Direction GetLastDirection()
        {
            return directionBuffer.Count > 0 ? directionBuffer.Last.Value : Direction;
        }

        // Determines if the snake can change to the new direction
        private bool CanChangeDirection(Direction newDirection)
        {
            if (directionBuffer.Count == 2)
            {
                return false;
            }

            Direction lastDirection = GetLastDirection();
            return newDirection != lastDirection && newDirection != lastDirection.Opposite();
        }

        // Changes the direction of the snake if possible
        public void ChangeDirection(Direction direction)
        {
            if (CanChangeDirection(direction))
            {
                directionBuffer.AddLast(direction);
            }
        }

        // Checks if a position is out of the grid bounds
        private bool OutOfBounds(Position position)
        {
            return position.Row < 0 || position.Row >= Rows || position.Col < 0 || position.Col >= Cols;
        }

        // Detects collision at the new head position
        private GridValue CollisionDetection(Position newHeadPos)
        {
            if (OutOfBounds(newHeadPos))
            {
                return GridValue.outOfBounds;
            }

            if (newHeadPos.Equals(TailPosition()))
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        // Moves the snake in the current direction
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

        // Resets the game state
        internal void Reset()
        {
            Array.Clear(Grid, 0, Grid.Length);
            directionBuffer.Clear();
            snakePositions.Clear();

            Direction = Direction.Right;
            Score = 0;
            GameOver = false;

            AddSnake();
            AddFood();
        }
    }
}
