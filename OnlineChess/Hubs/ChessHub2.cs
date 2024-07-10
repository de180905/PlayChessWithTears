//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.SignalR;
//using Newtonsoft.Json;
//using OnlineChess.Models.Game;
//using OnlineChess.Models.Game.Chessman;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//namespace OnlineChess.Hubs;
//[Authorize]
//public class ChessHub2 : Hub
//{
//    private static List<Room> Rooms { get; set; } = new List<Room>();

//    private static readonly ConcurrentDictionary<string, string> userConnectionMap = new ConcurrentDictionary<string, string>();

//    public override async Task OnConnectedAsync()
//    {
//        var userId = Context.User.Identity.Name;
//        var connectionId = Context.ConnectionId;

//        userConnectionMap.TryAdd(userId, connectionId);

//        await base.OnConnectedAsync();
//    }

//    public override async Task OnDisconnectedAsync(Exception exception)
//    {
//        var userId = Context.User.Identity.Name;
//        var connectionId = Context.ConnectionId;

//        string removedConnectionId;
//        userConnectionMap.TryRemove(userId, out removedConnectionId);

//        await base.OnDisconnectedAsync(exception);
//    }


//    public Room? GetRoomOfUser(string Username, List<Room> Rooms)
//    {
//        return Rooms.FirstOrDefault((Room) => Room.RoomMem1.Username == Username || Room.RoomMem2.Username == Username);
//    }
//    public async Task CreateRoom()
//    {
//        string Username = Context.User.Identity.Name;
//        if (GetRoomOfUser(Username, Rooms) == null) return;
//        string? roomId = GenerateAvailRoomId(100000);
//        if (roomId == null)
//        {
//            //thong bao client rang da full phong
//        }
//        Rooms.Add(new Room(roomId, new RoomMem(Username)));
//        await Clients.Caller.SendAsync("RoomCreated", roomId, Username+"from hub2");
//    }

//    public async Task JoinRoom(string roomId, string playerName)
//    {
//        System.Diagnostics.Debug.WriteLine(rooms.ContainsKey(roomId));
//        System.Diagnostics.Debug.WriteLine(rooms[roomId].Count);
//        if (rooms.ContainsKey(roomId) && rooms[roomId].Count == 1)
//        {
//            rooms[roomId].Add(Context.ConnectionId);
//            connections.TryAdd(Context.ConnectionId, roomId);
//            connNames.TryAdd(Context.ConnectionId, playerName);
//            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
//            await Clients.Group(roomId).SendAsync("PlayerJoined", roomId);
//        }
//        else
//        {
//            await Clients.Caller.SendAsync("JoinRoomFailed", "Room is full or does not exist");
//        }
//    }

//    public async Task LeaveRoom(string roomId)
//    {
//        System.Diagnostics.Debug.WriteLine("is leaving");
//        if (rooms.ContainsKey(roomId))
//        {
//            rooms[roomId].Remove(Context.ConnectionId);
//            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
//            await Clients.Group(roomId).SendAsync("PlayerLeft");
//        }
//    }


//    private string? GenerateAvailRoomId(int maxId)
//    {
//        for (int i = 0; i < maxId; i++)
//        {
//            string availRoomId = i.ToString("D5");
//            if (!rooms.ContainsKey(availRoomId))
//            {
//                return availRoomId;
//            }
//        }
//        return null;
//    }

//    public async Task StartGame()
//    {
//        string starterId = Context.ConnectionId;
//        string roomId = connections[starterId];
//        if (rooms.ContainsKey(roomId) && rooms[roomId].Count == 2 && rooms[roomId][0] == Context.ConnectionId)
//        {
//            Player random = Player.White;
//            RealPlayer player1 = new RealPlayer(random, rooms[roomId][0]);
//            RealPlayer player2 = new RealPlayer(PlayerExtensions.Opponent(random), rooms[roomId][1]);
//            var Match = new Match(player1, player2);
//            matches.TryAdd(roomId, Match);
//            string jsonMatch = JsonConvert.SerializeObject(matches[roomId]);
//            // Send the ChessMatch to both players

//            foreach (var connectionId in rooms[roomId])
//            {
//                await Clients.Client(connectionId).SendAsync("GameStarted", jsonMatch, Match.GetPlayerByConnectionId(connectionId));
//            }
//        }
//        else
//        {
//            await Clients.Caller.SendAsync("StartGameFailed", "You are not the room creator or there are not enough players");
//        }
//    }

//    public async Task makeMove(int x0, int y0, int x1, int y1)
//    {
//        string executerId = Context.ConnectionId;
//        var roomId = connections[executerId];
//        var Match = matches[roomId];
//        Piece piece = Match.ChessBoard[x0, y0];
//        Match.makeMove(new Position(x0, y0), new Position(x1, y1));
//    }

//    public async Task SelectCell(Position curPos)
//    {
//        string connectionId = Context.ConnectionId;
//        string roomId = connections[connectionId];
//        var Match = matches[roomId];
//        var player = Match.GetPlayerByConnectionId(connectionId);
//        if (player.Color == Match.Turn && Match.ChessBoard.isInside(curPos))
//        {
//            Piece piece = Match.ChessBoard[curPos];
//            if (!Match.ChessBoard.isEmpty(curPos) && piece.Color == player.Color)
//            {
//                player.CurPos = curPos;
//                var nextMoves = Match.GetValidMoves(curPos);
//                await Clients.Caller.SendAsync("ShowNextMoves", nextMoves);
//            }
//            else if (player.CurPos != null)
//            {
//                player.ToPos = curPos;
//                Match.makeMove(player.CurPos, player.ToPos);
//                if (Match.Ended())
//                {
//                    await Clients.Group(roomId).SendAsync("GameOver", Match.Result);
//                }
//                else
//                {
//                    player.CurPos = null;
//                    player.ToPos = null;
//                    string jsonMatch = JsonConvert.SerializeObject(matches[roomId]);
//                    await Clients.Group(roomId).SendAsync("MakeMoveSuccessfully", jsonMatch);
//                }
//            }
//        }
//    }
//    private async Task GetNextMoves(string connectionId, Position curPos)
//    {
//        string roomId = connections[connectionId];
//        var Match = matches[roomId];
//        if (Match.GetPlayerByConnectionId(connectionId).Color == Match.Turn)
//        {
//            var nextMoves = Match.ChessBoard[curPos].getMoves(curPos, Match.ChessBoard);
//            await Clients.Group(roomId).SendAsync("ShowNextMoves", nextMoves);
//        }
//    }

//    private bool ValidateCreator(Dictionary<string, List<string>> rooms)
//    {
//        var creatorId = Context.ConnectionId;
//        foreach (var pair in rooms.Values)
//        {
//            if (pair.Contains(creatorId))
//            {
//                return false;
//            }
//        }
//        return true;
//    }
//    // Add other methods for handling chess moves and game logic
//}
