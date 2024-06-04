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
    }
}
