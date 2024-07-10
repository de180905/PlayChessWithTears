using OnlineChess.Data;
using OnlineChess.Models.Game;

namespace OnlineChess.Models.Entities
{
    public class MatchSummary
    {
        public int Id { get; set; }

        public string BlackId { get; set; }
        public OnlineChessUser Black { get; set; }

        public string WhiteId { get; set; }
        public OnlineChessUser White { get; set; }

        public MatchResult Result { get; set; }

        public DateTime Time { get; set; }
    }

}
