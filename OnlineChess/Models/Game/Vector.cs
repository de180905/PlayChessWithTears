namespace OnlineChess.Models.Game
{
    public class Vector : Position
    {
        public readonly static Vector North = new Vector(-1, 0);
        public readonly static Vector South = new Vector(1, 0);
        public readonly static Vector West = new Vector(0, -1);
        public readonly static Vector East = new Vector(0, 1);
        public readonly static Vector NorthEast = North+East;
        public readonly static Vector NorthWest = North+West;
        public readonly static Vector SouthWest = South+West;
        public readonly static Vector SouthEast = South+East;    
        public Vector(int x, int y) : base(x, y)
        {
            
        }
        public static Vector operator +(Vector a, Vector b)
        {
            // Define the behavior of the + operator for your class
            return new Vector(a.X+b.X, a.Y+b.Y);
        }
        public static Vector operator *(Vector a, int scalar)
        {
            // Define the behavior of the + operator for your class
            return new Vector(a.X*scalar, a.Y*scalar);
        }
    }
}
