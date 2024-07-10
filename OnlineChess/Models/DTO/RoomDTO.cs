using Newtonsoft.Json;
using OnlineChess.Data;
using OnlineChess.Hubs;
using OnlineChess.Models.Game;

namespace OnlineChess.Models.DTO
{
    public class RoomDTO
    {
        public string ID { get; set; }
        public OnlineChessUser Player1 { get; set; }
        public OnlineChessUser Player2 { get; set;}
        public bool StartGame { get; set; }
        public bool Kick { get; set; }
        public bool Leave { get; set; }
        public void Config(string userId)
        {
            StartGame = Kick = 
            (Player1.Id == userId)&&(Player2 != null)&&(!ChessHubExtensions.matches.ContainsKey(ID));
            Leave = !ChessHubExtensions.matches.ContainsKey(ID);
        }
        public RoomDTO(string iD, OnlineChessUser player1, OnlineChessUser player2)
        {
            ID = iD;
            this.Player1 = player1;
            this.Player2 = player2;
        }
        public RoomDTO(string iD, OnlineChessUser player1)
        {
            ID = iD;
            this.Player1 = player1;
        }
    }
}
