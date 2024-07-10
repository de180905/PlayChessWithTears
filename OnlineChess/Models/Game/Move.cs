namespace OnlineChess.Models.Game
{
    public abstract class Move
    {
        public Position FromPos { get; set; }
        public Position ToPos { get; set; }
        public abstract MoveType Type { get; set; }
        public abstract void execute(Board board);
        public Move(Position fromPos, Position toPos)
        {
            FromPos = fromPos;
            ToPos = toPos;
        }
        public override string ToString()
        {
            return "("+FromPos+", "+ToPos+")";
        }
    }
}
