namespace OnlineChess.Models.Game.Chessman
{
    public class Rook : Piece
    {
        public override Player Color { get; }

        public override PieceType Type { get; } = PieceType.Rook;
        private static readonly Vector[] vectors =
        {
                Vector.North,
                Vector.South,
                Vector.West,
                Vector.East
        };
        public Rook(Player color)
        {
            Color = color;
        }
        public override IEnumerable<Move> getMoves(Position curPos, Board board)
        {
            return NextPositionsInMultiVectors(curPos, vectors, board).Select(toPos => new NormalMove(curPos, toPos));
        }

        public override Piece duplicate()
        {
            Piece newPiece = new Rook(Color);
            newPiece.Moved = this.Moved;
            return newPiece;
        }
    }
}
