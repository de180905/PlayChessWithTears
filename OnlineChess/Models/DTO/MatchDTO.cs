using OnlineChess.Models.Game;

namespace OnlineChess.Models.DTO
{
    public class MatchDTO
    {
        public string Black { get; set; }
        public string White { get; set; }
        public string Result { get; set; }
        public DateTime Time { get; set; }

        public MatchDTO(string black, string white, string result, DateTime time)
        {
            Black = black;
            White = white;
            Result = result;
            Time = time;
        }
    }
}
