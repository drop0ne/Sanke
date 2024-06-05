using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanke
{
    public class Position
    {
        // Properties for row and column positions
        public int Row { get; }
        public int Col { get; }

        // Constructor to initialize position
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }

        // Translate the position based on the direction
        public Position Translate(Direction direction)
        {
            return new Position(Row + direction.RowOffset, Col + direction.ColOffset);
        }

        // Override Equals method for comparing positions
        public override bool Equals(object? obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Col == position.Col;
        }

        // Override GetHashCode for using Position in collections
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        // Equality operator to compare two positions
        public static bool operator ==(Position? left, Position? right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        // Inequality operator to compare two positions
        public static bool operator !=(Position? left, Position? right)
        {
            return !(left == right);
        }
    }
}