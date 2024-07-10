namespace OnlineChess.Models.Game.Chessman
{
    public abstract class Piece
    {
        public abstract Player Color { get; }
        public abstract PieceType Type { get; }
        public bool Moved { get; set; } = false;
        public abstract IEnumerable<Move> getMoves(Position curPos, Board board);
        protected IEnumerable<Position> NextPositionsInVector(Position curPos, Vector vector, Board board)
        {
            for(Position pos = curPos+vector; board.isInside(pos); pos += vector)
            {
                if (board.isEmpty(pos))
                {
                    yield return pos;
                    continue;
                }
                if (board[pos].Color != Color)
                {
                    yield return pos;
                }
                yield break;
            }
        }
        protected virtual IEnumerable<Position> NextPositionsInMultiVectors(Position curPos, Vector[] vectors, Board board)
        {
            return vectors.SelectMany(vector => NextPositionsInVector(curPos, vector, board));
        }
        public abstract Piece duplicate();
        public virtual bool canCaptureOpponentKing(Position cur, Board board)
        {
            return getMoves(cur, board).Any(move => {return !board.isEmpty(move.ToPos) && board[move.ToPos].Type == PieceType.King; });
        }
        public bool CanReach(Position des, Board board)
        {
            return board.isInside(des) && (board.isEmpty(des) || board[des].Color != this.Color) ;
        }
        public bool IsAlly(Position pos, Board board)
        {
            if(board.isEmpty(pos) || board[pos].Color != Color)
            {
                return false;
            }
            return true;
        }
    }
}
