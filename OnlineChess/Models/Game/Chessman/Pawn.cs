
namespace OnlineChess.Models.Game.Chessman
{
    public class Pawn : Piece
    {
        public override Player Color { get; }
        public override PieceType Type { get; } = PieceType.Pawn;
        private static readonly Vector[] vectors =
        {
                Vector.NorthWest,
                Vector.NorthEast,
                Vector.North,
        };
        public Pawn(Player color)
        {
            Color = color;
        }

        public override IEnumerable<Move> getMoves(Position curPos, Board board)
        {
            return GetNormalPositions(curPos, board)
           .Select(pos =>
            {
                Move move;
                if (pos.X == 7 || pos.X == 0)
                {
                    move = new ProMove(curPos, pos);
                }
                else if (pos.Equals(board.PassPos))
                {
                    move = new PassMove(curPos, pos);
                }
                else
                {
                    move = new NormalMove(curPos, pos);
                }
                return move;
            });
        }

        public override Piece duplicate()
        {
            Piece newPiece = new Pawn(Color);
            newPiece.Moved = this.Moved;
            return newPiece;
        }
        public IEnumerable<Position> GetNormalPositions(Position curPos, Board board)
        {
            Vector dir = board[curPos].Color == Player.White ? Vector.North : Vector.South;
            Position nextPos = curPos + dir;
            if (board.isInside(nextPos) && board.isEmpty(nextPos))
            {
                yield return nextPos;
                nextPos = curPos + dir * 2;
                if (this.Moved == false && board.isEmpty(nextPos))
                {
                    yield return nextPos;
                }

            }
            nextPos = curPos + (dir+Vector.West);
            if (board.isInside(nextPos) && !board.isEmpty(nextPos) && board[nextPos].Color != this.Color)
            {
                yield return nextPos;
            }
            if (nextPos.Equals(board.PassPos))
            {
                yield return nextPos;
            }
            nextPos = curPos + (dir+Vector.East);
            if (board.isInside(nextPos) && !board.isEmpty(nextPos) && board[nextPos].Color != this.Color)
            {
                yield return nextPos;
            }
            if (nextPos.Equals(board.PassPos))
            {
                yield return nextPos;
            }

        }



    }
}
