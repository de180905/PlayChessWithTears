
namespace OnlineChess.Models.Game.Chessman
{
    public class Bishop : Piece
    {
        public override Player Color { get; }
        public override PieceType Type { get; } = PieceType.Bishop;
        private static readonly Vector[] vectors =
        {
                Vector.NorthWest,
                Vector.NorthEast,
                Vector.SouthEast,
                Vector.SouthWest,

        };
        public Bishop(Player color)
        {
            Color = color;
        }

        public override IEnumerable<Move> getMoves(Position curPos, Board board)
        {
            return NextPositionsInMultiVectors(curPos, vectors, board).Select(toPos => new NormalMove(curPos, toPos));
        }

        public override Piece duplicate()
        {
            Piece newPiece = new Bishop(Color);
            newPiece.Moved = this.Moved;
            return newPiece;
        }
    }
}