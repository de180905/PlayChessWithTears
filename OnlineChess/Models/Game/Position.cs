using System.Drawing;

namespace OnlineChess.Models.Game
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static Position operator +(Position p, Vector v)
        {
            // Define the behavior of the + operator for your class
            return new Position(p.X + v.X, p.Y + v.Y);
        }
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Position pos = (Position)obj;
                return (X == pos.X) && (Y == pos.Y);
            }
        }
        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
}
