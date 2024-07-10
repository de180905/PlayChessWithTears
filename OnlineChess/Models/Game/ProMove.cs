using OnlineChess.Models.Game.Chessman;

namespace OnlineChess.Models.Game
{
    public class ProMove : Move
    {
        public ProMove(Position fromPos, Position toPos) : base(fromPos, toPos)
        {
        }

        public override MoveType Type { get; set; } = MoveType.Promote;

        public override void execute(Board board)
        {
            Move move = new NormalMove(FromPos, ToPos);
            move.execute(board);
            board[ToPos] = new Queen(board[ToPos].Color);
        }
    }
}
