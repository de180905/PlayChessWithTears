using OnlineChess.Models.Game.Chessman;

namespace OnlineChess.Models.Game
{
    public class PassMove : Move
    {
        public PassMove(Position fromPos, Position toPos) : base(fromPos, toPos)
        {
        }

        public override MoveType Type { get; set; } = MoveType.PassMove;

        public override void execute(Board board)
        {
            Piece cur = board[FromPos];
            board[FromPos] = null;
            board[ToPos] = cur;
            cur.Moved = true;
            board[ToPos + PlayerExtensions.Forward(PlayerExtensions.Opponent(cur.Color))] = null;
        }
    }
}
