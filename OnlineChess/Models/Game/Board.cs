using OnlineChess.Models.Game.Chessman;

namespace OnlineChess.Models.Game
{
    public class Board
    {
        public Piece[,] pieces = new Piece[8,8];
        public Position PassPos { get; set; }

        public Piece this[int x, int y]
        {
            get { return pieces[x,y]; }
            set { pieces[x, y] = value; }
        }

        public Piece this[Position pos]
        {
           
            get { return pieces[pos.X, pos.Y]; }
            set { pieces[pos.X, pos.Y] = value;}
        }

        private void setUp()
        {
            this[0, 0] = new Rook(Player.Black);
            this[0, 1] = new Knight(Player.Black);
            this[0, 2] = new Bishop(Player.Black);
            this[0, 3] = new Queen(Player.Black);
            this[0, 4] = new King(Player.Black);
            this[0, 5] = new Bishop(Player.Black);
            this[0, 6] = new Knight(Player.Black);
            this[0, 7] = new Rook(Player.Black);
            this[7, 0] = new Rook(Player.White);
            this[7, 1] = new Knight(Player.White);
            this[7, 2] = new Bishop(Player.White);
            this[7, 3] = new Queen(Player.White);
            this[7, 4] = new King(Player.White);
            this[7, 5] = new Bishop(Player.White);
            this[7, 6] = new Knight(Player.White);
            this[7, 7] = new Rook(Player.White);
            for (int i = 0; i < 8; i++)
            {
                this[1, i] = new Pawn(Player.Black);
                this[6, i] = new Pawn(Player.White);
            }
        }

        public static Board Initialize()
        {
            Board board = new Board();
            board.setUp();
            return board;
        }

        public bool isInside(int x, int y)
        {
            return x>=0 && x<=7 && y>=0 && y<=7;
        }

        public bool isInside(Position pos)
        {
            return isInside(pos.X, pos.Y);
        }

        public bool isEmpty(Position pos)
        {
            return this[pos] == null;
        }
        public bool hasObstacle(Position from, Position to)
        {
            int x = to.X - from.X;
            int y = to.Y - from.Y;
            int divisor = x != 0 ? Math.Abs(x) : Math.Abs(y);
            Vector dir = new Vector(x/divisor, y/divisor);
            Position itr = from+dir;
            while(!itr.Equals(to))
            {
                if (!isEmpty(itr) || IsTargeted(itr, PlayerExtensions.Opponent(this[from].Color)))
                {
                    return true;
                }
                itr += dir;
            }
            return false;
        }
        public IEnumerable<Position> PiecePositions()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++) {
                    Position pos = new Position(i, j);
                    if (!isEmpty(pos))
                    {
                        yield return pos;
                    }
                }
            }
        } 
        public IEnumerable<Position> PiecePositionsFor(Player player)
        {
            return PiecePositions().Where(pos => this[pos].Color == player);
        }

        public Board duplicate()
        {
            Board dup = new Board();
            foreach(Position pos in PiecePositions())
            {
                dup[pos] = this[pos].duplicate();
            }
            return dup;
        }

        public bool IsInCheck(Player player)
        {
            return PiecePositionsFor(PlayerExtensions.Opponent(player)).Any(pos =>
            {
                
                Piece opponentPiece = this[pos];
                return opponentPiece.canCaptureOpponentKing(pos, this);
            });
        }

        public bool IsTargeted(Position curPos, Player enemy)
        {
            return PiecePositionsFor(enemy).Any(pos =>
            {
                Piece opponentPiece = this[pos];
                IEnumerable<Move> moves;
                if(opponentPiece.Type == PieceType.King)
                {
                    moves = ((King)opponentPiece).getNormalMoves(pos, this);
                }
                else 
                {
                    moves = opponentPiece.getMoves(pos, this);
                }
                return moves.Any(move => move.ToPos.Equals(curPos));
            });
        }
        public IEnumerable<Move>? getMovesOfCurPos(Position curPos)
        {
            if (this.isEmpty(curPos)) { return null; }
            return this[curPos].getMoves(curPos, this);
        }
    }
}
