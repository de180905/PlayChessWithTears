
namespace OnlineChess.Models.Game.Chessman
{
    public class Knight : Piece
    {
        public override Player Color { get; }
        public override PieceType Type { get; } = PieceType.Knight;
        private Vector[] vectors =
        {
            new Vector(-2, 1),
            new Vector(-1, 2),
            new Vector(1, 2),
            new Vector(2, 1),
            new Vector(2, -1),
            new Vector(1, -2),
            new Vector(-1, -2),
            new Vector(-2, -1),
        };
        public Knight(Player color)
        {
            Color = color;
        }

        public override IEnumerable<Move> getMoves(Position curPos, Board board)
        {
            return NextPositions(curPos, board).Select(pos => new NormalMove(curPos, pos));
        }

        private IEnumerable<Position> NextPositions(Position curPos, Board board) 
        {
            return vectors
                .Select(vector => curPos + vector)
                .Where(pos => CanReach(pos, board));
        }

        public override Piece duplicate()
        {
            Piece newPiece = new Knight(Color);
            newPiece.Moved = this.Moved;
            return newPiece;
        }
    }
}
