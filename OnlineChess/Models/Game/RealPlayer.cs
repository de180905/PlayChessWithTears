using OnlineChess.Utils;

namespace OnlineChess.Models.Game
{
    public class RealPlayer
    {
        public CountdownTimer Timer { get; set; }
        public Player Color { get; set; }
        public string UserId {  get; set; }
        public Position? CurPos { get; set; }
        public Position? ToPos { get; set; }
        public RealPlayer(Player color, string userId, int seconds)
        {
            TimeSpan duration = TimeSpan.FromSeconds(seconds);
            CountdownTimer timer = new CountdownTimer(duration);
            Timer = timer;
            Color = color;
            UserId = userId;
        }
        public RealPlayer(Player color, string userId, Position curPos)
        {
            Color = color;
            UserId = userId;
            CurPos = curPos;
        }
        public bool IsPicking()
        {
            return CurPos == null;
        }
    }
}
