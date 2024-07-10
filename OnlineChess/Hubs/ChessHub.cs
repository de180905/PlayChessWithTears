using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using OnlineChess.Controllers;
using OnlineChess.Data;
using OnlineChess.Models.Entities;
using OnlineChess.Models.Game;
using OnlineChess.Models.Game.Chessman;
using OnlineChess.Utils;
using System.Collections.Concurrent;
namespace OnlineChess.Hubs;

public class ChessHub : Hub
{
    private readonly IDbContextFactory<OnlineChessDbContext> _dbFactory;
    private static ConcurrentDictionary<string, List<string>> rooms = ChessHubExtensions.rooms;
    private static ConcurrentDictionary<string, Match> matches = ChessHubExtensions.matches;
    private readonly IHubContext<ChessHub> _hubContext;

    public ChessHub(IHubContext<ChessHub> hubContext, IDbContextFactory<OnlineChessDbContext> dbFactory)
    {
        _hubContext = hubContext;
        _dbFactory = dbFactory;
    }

    private async Task CallUsersOfRoom(string roomId, string method, object message)
    {
        foreach(string userId in rooms[roomId])
        {
            await Clients.Users(userId).SendAsync(method, message);
        }
    }

    private void EndMatch(string roomId, MatchResult result)
    {
        var Match = matches[roomId];

        //persist in db
        var blackId = Match.GetPlayerByColor(Player.Black).UserId;
        var whiteId = Match.GetPlayerByColor(Player.White).UserId;
        var matchSummary = new MatchSummary();
        matchSummary.BlackId = blackId;
        matchSummary.WhiteId = whiteId;
        matchSummary.Result = result;
        matchSummary.Time = DateTime.Now;
        var dbContext = _dbFactory.CreateDbContext();
        dbContext.MatchSummaries.Add(matchSummary);
        dbContext.SaveChanges();
        var curRoom = rooms[roomId];
        matches.TryRemove(roomId, out Match mvalue);
        rooms.TryRemove(roomId, out List<string> rvalue);
        string resultMessage = "";
        foreach (string userId in curRoom)
        {
            if(result == MatchResult.Drawn)
            {
                resultMessage = "Drawn!";
            }
            else if(userId == Match.GetPlayerByColor((Player)result).UserId)
            {
                resultMessage = "You win!";
            }
            else
            {
                resultMessage = "You lose!";
            }
            _hubContext.Clients.Users(userId).SendAsync("GameOver", resultMessage);
        }

    }

    public async Task CreateRoom()
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            await Clients.Caller.SendAsync("UnauthorizedAccess", "You need to login");
            return;
        }
        var userId = Context.UserIdentifier;
        try 
        {
            ChessHubExtensions.CreateRoom(userId);
            await Clients.User(userId).SendAsync("RoomUpdated");
        }
        catch(Exception ex)
        {
            await Clients.User(userId).SendAsync("ExceptionHandler", ex.Message);
        }
    }
    public async Task SendMatch()
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            await Clients.Caller.SendAsync("UnauthorizedAccess", "You need to login");
            return;
        }
        var userId = Context.UserIdentifier;
        var match = ChessHubExtensions.GetMatchOfUser(userId);
        if(match != null)
        {
            string jsonMatch = JsonConvert.SerializeObject(match);
            await Clients.User(userId).SendAsync("GameStarted", jsonMatch, match.GetPlayerByUserId(userId));
        }
    }

    public async Task JoinRoom(string roomId)
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            await Clients.Caller.SendAsync("UnauthorizedAccess", "You need to login");
            return;
        }
        string userId = Context.UserIdentifier;
        try
        {
            ChessHubExtensions.GetInRoom(userId, roomId);
            foreach (var user in rooms[roomId])
            {
                await Clients.User(user).SendAsync("RoomUpdated");
            }
        }
        catch(Exception ex)
        {
            await Clients.User(userId).SendAsync("ExceptionHandler", ex.Message);
        }
    }

    public async Task StartGame()
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            await Clients.Caller.SendAsync("UnauthorizedAccess", "You need to login");
            return;
        }
        string starterId = Context.UserIdentifier;
        string roomId = ChessHubExtensions.GetRoomOfUser(starterId);
        if (rooms.ContainsKey(roomId) && rooms[roomId].Count == 2 && rooms[roomId][0] == starterId && !matches.ContainsKey(roomId))
        {
            Player random = Player.White;
            int seconds = 30;
            RealPlayer player1 = new RealPlayer(random, rooms[roomId][0], seconds);
            RealPlayer player2 = new RealPlayer(PlayerExtensions.Opponent(random), rooms[roomId][1], seconds);
            var Match = new Match(player1, player2);
            matches.TryAdd(roomId, Match);
            player1.Timer.TimerElapsed += async (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Ended");
                EndMatch(roomId, (MatchResult)player2.Color);
            };
            player2.Timer.TimerElapsed += async (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Ended");
                EndMatch(roomId, (MatchResult)player1.Color);
            };
            Match.GetCurrentPlayer().Timer.Start();
            Match.GetCurrentPlayer().Timer.Pause();
            Match.GetWaitingPlayer().Timer.Start();
            Match.GetWaitingPlayer().Timer.Pause();
            Match.GetCurrentPlayer().Timer.Resume();

            // Send the ChessMatch to both players
            string jsonMatch = JsonConvert.SerializeObject(Match);
            foreach(var userId in rooms[roomId])
            {
                await Clients.User(userId).SendAsync("RoomUpdated");
            }

        }
        else
        {
            await Clients.User(starterId).SendAsync("StartGameFailed", "You are not the room creator or there are not enough players");
        }
    }

    public async Task SelectCell(Position curPos)
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            await Clients.Caller.SendAsync("UnauthorizedAccess", "You need to login");
            return;
        }
        string userId = Context.UserIdentifier;
        string roomId = ChessHubExtensions.GetRoomOfUser(userId);
        var Match = matches[roomId];
        var player = Match.GetPlayerByUserId(userId);
        if (player.Color == Match.Turn && Match.ChessBoard.isInside(curPos))
        {
            Piece piece = Match.ChessBoard[curPos];
            if (!Match.ChessBoard.isEmpty(curPos) && piece.Color == player.Color)
            {
                player.CurPos = curPos; 
                var nextMoves = Match.GetValidMoves(curPos);
                await Clients.User(userId).SendAsync("ShowNextMoves", nextMoves);
            }
            else if(player.CurPos != null)
            {
                player.ToPos = curPos;
                bool executed = Match.makeMove(player.CurPos, player.ToPos);
                if (executed)
                {
                    Match.GetCurrentPlayer().Timer.Resume();
                    Match.GetWaitingPlayer().Timer.Pause();
                    player.CurPos = null;
                    player.ToPos = null;
                    string jsonMatch = JsonConvert.SerializeObject(matches[roomId]);
                    await CallUsersOfRoom(roomId, "MakeMoveSuccessfully", (string)jsonMatch);
                }
                var result = Match.CheckForCheckmate(player.Color);
                if (result != null)
                {
                    EndMatch(roomId, (MatchResult)result);
                }
            }
        }
    }

    public async Task LeaveRoom()
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            Clients.Caller.SendAsync("UnauthorizedAccess", "You need to login");
            return;
        }
        string leaverId = Context.UserIdentifier;
        var roomId = ChessHubExtensions.GetRoomOfUser(leaverId);
        if (roomId != null && !matches.ContainsKey(roomId))
        {
            rooms[roomId].Remove(leaverId);
            await Clients.User(leaverId).SendAsync("RedirectToHome");
            if (ChessHubExtensions.IsEmpty(roomId))
            {
                ChessHubExtensions.DeleteRoom(roomId);
            }
            else
            {
                await Clients.User(rooms[roomId][0]).SendAsync("RoomUpdated");
            }
        }
    }

    public async Task KickRoommate()
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            Clients.Caller.SendAsync("UnauthorizedAccess", "You need to login");
            return;
        }
        var userId = Context.UserIdentifier;
        var roomId = ChessHubExtensions.GetRoomOfUser(userId);
        if (roomId != null && userId == rooms[roomId][0] && rooms[roomId].Count > 1 && !matches.ContainsKey(roomId))
        {
            string userBeingKicked = rooms[roomId][1];
            rooms[roomId].RemoveAt(1);
            await Clients.User(userBeingKicked).SendAsync("RedirectToHome");
            await Clients.User(userId).SendAsync("RoomUpdated");
        }
    }
}

    // Add other methods for handling chess moves and game logic

