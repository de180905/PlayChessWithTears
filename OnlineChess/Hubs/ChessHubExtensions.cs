using Microsoft.AspNetCore.SignalR;
using OnlineChess.Models.Game;
using OnlineChess.Utils;
using System.Collections.Concurrent;

namespace OnlineChess.Hubs
{
    public class ChessHubExtensions
    {
        public static ConcurrentDictionary<string, List<string>> rooms = new ConcurrentDictionary<string, List<string>>();
        public static ConcurrentDictionary<string, Match> matches = new ConcurrentDictionary<string, Match>();

        public static string? GenerateAvailRoomId(int maxId)
        {
            for (int i = 0; i < maxId; i++)
            {
                string availRoomId = i.ToString("D5");
                if (!rooms.ContainsKey(availRoomId))
                {
                    return availRoomId;
                }
            }
            return null;
        }
        public static bool IsValidatedCreator(string creatorId, ConcurrentDictionary<string, List<string>> rooms)
        {
            return !rooms.Values.Any((pair) => pair.Contains(creatorId));
        }
        public static string GetRoomOfUser(string userId)
        {
            return (rooms.FirstOrDefault((room) => room.Value.Any((innerUserId) => innerUserId == userId))).Key;
        }
        public static string GetInRoom(string userId)
        {
            string roomId = GetRoomOfUser(userId);
            if (roomId == null)
            {
                roomId = GenerateAvailRoomId(100000);
                if (roomId != null)
                {
                    rooms.TryAdd(roomId, new List<string> { userId });
                }
                else
                {
                    throw new Exception("The server is overloaded");
                }
            }
            return roomId;
        }

        public static string CreateRoom(string userId)
        {
            if (!IsValidatedCreator(userId, rooms))
            {
                throw new Exception("You're already in a room");
            }
            var roomId = GenerateAvailRoomId(100000);
            if (roomId == null)
            {
                throw new Exception("The server is overloaded");
            }
            rooms.TryAdd(roomId, new List<string> { userId });
            return roomId;
        }

        public static string GetInRoom(string userId, string roomId)
        {
            if (!IsValidatedCreator(userId, rooms))
            {
                throw new Exception("You're already in a room");
            }
            if (!rooms.ContainsKey(roomId) || rooms[roomId].Count != 1)
            {
                throw new Exception("Room does not exist or is full");
            }
            rooms[roomId].Add(userId);
            return roomId;
        }

        public static string LeaveRoom(string userId)
        {
            string roomId = GetRoomOfUser(userId);
            if(roomId == null)
            {
                throw new Exception("Not in a room");
            }
            rooms[roomId].Remove(userId);
            return roomId;
        }

        public static string KickRommate(string userId)
        {
            string roomId = GetRoomOfUser(userId);
            if(roomId != null && rooms[roomId][0] == userId)
            {
                rooms[roomId].RemoveAt(1);
                return roomId;
            }
            throw new Exception("Cannot handle this request");
        }

        public static bool IsCreator(string userId)
        {
            string roomId = GetRoomOfUser(userId);
            if(roomId == null) { return false; }
            return rooms[roomId][0] == userId;
        }

        public static bool IsEmpty(string roomId)
        {
            if (rooms.ContainsKey(roomId))
            {
                return rooms[roomId].Count == 0;
            }
            throw new Exception("There's no room with id "+roomId);
        }

        public static void DeleteRoom(string roomId)
        {
            rooms.TryRemove(roomId, out List<string> removedValue);
        }

        public static Match? GetMatchOfUser(string userId)
        {
            var roomId = GetRoomOfUser(userId);
            if(roomId != null && matches.ContainsKey(roomId))
            {
                return matches[roomId];
            }
            return null;
        }

        public void EndMatch()
        {
            
        }
       
        
    }
}
