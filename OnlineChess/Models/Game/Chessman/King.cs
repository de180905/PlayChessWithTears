using System.Linq;
namespace OnlineChess.Models.Game.Chessman
{
    public class King : Piece
    {
        public override Player Color { get; }
        public override PieceType Type { get; } = PieceType.King;
        public Position RookLeftPos { get; }
        public Position RookRightPos { get; }
        public Position KingPos { get; }
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
        public King(Player color)
        {
            Color = color;
            if(Color == Player.White)
            {
                RookLeftPos = new Position(7, 0);
                RookRightPos = new Position(7, 7);
                KingPos = new Position(7, 4);
            }
            else
            {
                RookLeftPos = new Position(0, 0);
                RookRightPos = new Position(0, 7);
                KingPos = new Position(0, 4);
            }
        }

        private IEnumerable<Position> NextPositions(Position curPos, Board board)
        {
            return vectors.Select(vector => curPos + vector).Where(pos => board.isInside(pos) && !IsAlly(pos, board));
        }

        private IEnumerable<Position> CastlingPositions(Board board)
        {
            if(this.Moved == true || board.IsInCheck(Color))
            {
                yield break;
            }
            if(!board.hasObstacle(KingPos, RookLeftPos))
            {
                yield return KingPos + new Vector(0, -2);
            }
            if (!board.hasObstacle(KingPos, RookRightPos))
            {
                yield return KingPos + new Vector(0, 2);
            }
        }
        private IEnumerable<Move> getCastlingMoves(Board board)
        {
            return CastlingPositions(board).Select(pos => new CastlingMove(KingPos, pos));
        }
        public IEnumerable<Move> getNormalMoves(Position curPos, Board board)
        {
            return NextPositions(curPos, board).Select(pos => new NormalMove(curPos, pos));
        }
        public override IEnumerable<Move> getMoves(Position curPos, Board board)
        {
            return getNormalMoves(curPos, board).Concat(getCastlingMoves(board));
        }

        public override Piece duplicate()
        {
            Piece newPiece = new King(Color);
            newPiece.Moved = this.Moved;
            return newPiece;
        }

        public override bool canCaptureOpponentKing(Position cur, Board board)
        {
            return getNormalMoves(cur, board).Any(move => { return !board.isEmpty(move.ToPos) && board[move.ToPos].Type == PieceType.King; });
        }
    }
}
