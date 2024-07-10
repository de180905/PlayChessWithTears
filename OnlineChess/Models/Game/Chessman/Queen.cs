namespace OnlineChess.Models.Game.Chessman
{
    public class Queen : Piece
    {
        public override Player Color { get; }
        public override PieceType Type { get; } = PieceType.Queen;
        private static readonly Vector[] vectors =
        { 
                Vector.NorthWest,
                Vector.NorthEast, 
                Vector.SouthEast, 
                Vector.SouthWest,
                Vector.North,
                Vector.South,
                Vector.West,
                Vector.East
        };
        public Queen(Player color)
        {
            Color = color;
        }
        public override IEnumerable<Move> getMoves(Position curPos, Board board)
        {
            return NextPositionsInMultiVectors(curPos, vectors, board).Select(toPos => new NormalMove(curPos, toPos));
        }

        public override Piece duplicate()
        {
            Piece newPiece = new Queen(Color);
            newPiece.Moved = this.Moved;
            return newPiece;
        }
    }
}
