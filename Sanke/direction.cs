namespace Sanke
{
    public class Direction
    {
        // Predefined static directions
        public readonly static Direction Left = new(0, -1);
        public readonly static Direction Right = new(0, 1);
        public readonly static Direction Up = new(-1, 0);
        public readonly static Direction Down = new(1, 0);

        // Properties for row and column offsets
        public int RowOffset { get; }
        public int ColOffset { get; }

        // Constructor to initialize direction with row and column offsets
        public Direction(int rowOffset, int colOffset)
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }

        // Method to get the opposite direction
        public Direction Opposite()
        {
            return new Direction(-RowOffset, -ColOffset);
        }

        // Override Equals method for comparing directions
        public override bool Equals(object? obj)
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColOffset == direction.ColOffset;
        }

        // Override GetHashCode for using Direction in collections
        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        // Equality operator to compare two directions
        public static bool operator ==(Direction left, Direction right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        // Inequality operator to compare two directions
        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }
    }
}