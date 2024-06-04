using System.Runtime.CompilerServices;

namespace Sanke
{
    public class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Direction { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Position> snakePositions = new();
        private readonly Random random = new();

        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Direction = Direction.Right;
            Score = 0;
            GameOver = false;

            AddSnake();
            AddFood();
        }

        public void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 0; c < 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

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

        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        private void AddHead(Position position)
        {
            snakePositions.AddFirst(position);
            Grid[position.Row, position.Col] = GridValue.Snake;
        }
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        public void changeDirection(Direction direction)
        {
            if (Direction.Opposite() == direction)
            {
                return;
            }
            Direction = direction;
        }

        private bool OutOfBounds(Position position)
        {
            return position.Row < 0 || position.Row >= Rows || position.Col < 0 || position.Col >= Cols;
        }

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

        public void Move()
        {
            Position newHeadPos = HeadPosition().Translate(Direction);

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
            }else if (collision == GridValue.Food)
            {
                Score++;
                AddHead(newHeadPos);
                AddFood();
            }

        }

    }

}
