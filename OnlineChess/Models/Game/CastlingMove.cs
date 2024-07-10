using OnlineChess.Models.Game.Chessman;

namespace OnlineChess.Models.Game
{
    public class CastlingMove : Move
    {
        public CastlingMove(Position fromPos, Position toPos) : base(fromPos, toPos)
        {
        }

        public override MoveType Type { get; set; } = MoveType.Castling;

        public override void execute(Board board)
        {
            Position kingPos = FromPos;
            King king = (King)board[kingPos];
            Position rookPos;
            Vector rookOffset;
            //far castling
            if(ToPos.Y < kingPos.Y)
            {
                rookOffset = Vector.East * 3;
                rookPos = king.RookLeftPos;
            }
            //near castling
            else
            {
                rookOffset = Vector.West * 2;
                rookPos = king.RookRightPos;
            }
            Position rookNextPos = rookPos + rookOffset;
            Position kingNextPos = ToPos;
            board[kingNextPos] = board[kingPos];
            board[kingPos] = null;
            board[rookNextPos] = board[rookPos];
            board[rookPos] = null;
        }
    }
}
