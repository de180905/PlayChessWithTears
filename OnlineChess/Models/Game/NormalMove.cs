using OnlineChess.Models.Game.Chessman;

namespace OnlineChess.Models.Game
{
    public class NormalMove : Move
    {
        public override MoveType Type { get; set; } = MoveType.Normal;

        public override void execute(Board board)
        {
            Piece cur = board[FromPos];
            board[FromPos] = null;
            board[ToPos] = cur;
            cur.Moved = true;
        }

        public NormalMove(Position fromPos, Position toPos) : base(fromPos, toPos)
        {

        }
    }
}
