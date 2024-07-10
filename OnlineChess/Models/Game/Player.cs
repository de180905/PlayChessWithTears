namespace OnlineChess.Models.Game
{
    public enum Player
    {
        White, 
        Black
    }
    public static class PlayerExtensions
    {
        public static Player Opponent( this Player player ) { 
            if(player == Player.White) return Player.Black;
            return Player.White;
        }
        public static Vector Forward(this Player player)
        {
            if (player == Player.White) return Vector.North;
            return Vector.South;
        }
    }
}
