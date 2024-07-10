using OnlineChess.Models.Game.Chessman;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OnlineChess.Models.Game
{
    public class Match
    {
        public Board ChessBoard { get; set; } = Board.Initialize();
        public Player Turn { get; set; } = Player.White;
        public RealPlayer Player1 { get; set; }
        public RealPlayer Player2 { get; set; }
        public bool IsStarted { get; set; } = false;
        public Match(RealPlayer player1, RealPlayer player2)
        {
            Player1 = player1;
            Player2 = player2;
            IsStarted = true;
        }

        public Match()
        {

        }

        public bool makeMove(Position FromPos, Position ToPos)
        {
            Piece piece = ChessBoard[FromPos];
            Player mover = piece.Color;
            var riskMoves = piece.getMoves(FromPos, ChessBoard);
            Move move = FilterMoves(riskMoves).FirstOrDefault(move => move.ToPos.Equals(ToPos));
            if(move != null)
            {
                move.execute(ChessBoard);
                if(piece.Type == PieceType.Pawn && Math.Abs(FromPos.X - ToPos.X) == 2)
                {
                    ChessBoard.PassPos = FromPos + PlayerExtensions.Forward(piece.Color);
                }
                else
                {
                    ChessBoard.PassPos = null;
                }
                ChangeTurn();
                return true;
            }
            return false;
        }

        public IEnumerable<Move> FilterMoves(IEnumerable<Move> moves)
        {
            return moves.Where(move => TryMove(move));
        }

        private bool TryMove(Move move)
        {
            Board boardCopy = ChessBoard.duplicate();
            Player moveExecuter = boardCopy[move.FromPos].Color;
            move.execute(boardCopy);
            if (boardCopy.IsInCheck(moveExecuter))
            {
                return false;
            }
            return true;
        }

        private void ChangeTurn()
        {
            Turn = PlayerExtensions.Opponent(Turn);
        }

        public RealPlayer GetPlayerByUserId(string userId)
        {
            return userId == Player1.UserId ? Player1 : Player2;
        }
        public RealPlayer GetCurrentPlayer()
        {
            return Turn == Player1.Color? Player1 : Player2;
        }

        public RealPlayer GetWaitingPlayer()
        {
            return Turn == Player1.Color? Player2 : Player1;
        }

        public IEnumerable<Move> GetValidMoves(Position pos)
        {
            if (ChessBoard.isEmpty(pos))
            {
                return new List<Move>();
            }
            return FilterMoves(ChessBoard[pos].getMoves(pos, ChessBoard));
        }
        public MatchResult? CheckForCheckmate(Player executer)
        {
            Player opponent = PlayerExtensions.Opponent(executer);
            var opponentPiecesValidMoves = ChessBoard.PiecePositionsFor(opponent).SelectMany(pos => GetValidMoves(pos));
            if (!opponentPiecesValidMoves.Any())
            {
                if (ChessBoard.IsInCheck(opponent))
                {
                    return (MatchResult)executer;
                }
                else
                {
                    return MatchResult.Drawn;
                }
            }
            return null;
        }

        public RealPlayer GetPlayerByColor(Player color)
        {
            return Player1.Color == color? Player1 : Player2;
        }
        
    }
}
