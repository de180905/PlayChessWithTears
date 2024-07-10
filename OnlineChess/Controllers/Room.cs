using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using OnlineChess.Data;
using OnlineChess.Hubs;
using OnlineChess.Models;
using OnlineChess.Models.DTO;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.JavaScript;

namespace OnlineChess.Controllers
{
    [Authorize]
    public class Room : Controller
    {
        private readonly UserManager<OnlineChessUser> _userManager;
        private readonly ConcurrentDictionary<string, List<string>> _rooms = ChessHubExtensions.rooms;
        public Room(UserManager<OnlineChessUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            string roomId = ChessHubExtensions.GetRoomOfUser(userId);
            if (roomId == null)
            {
                return View("Error");
            }
            var room = _rooms[roomId];
            var player1 = _userManager.FindByIdAsync(room[0]).Result;
            RoomDTO roomInfo;
            if (room.Count() > 1)
            {
                var player2 = _userManager.FindByIdAsync(room[1]).Result;
                roomInfo = new RoomDTO(roomId, player1, player2);
            }
            else
            {
                roomInfo = new RoomDTO(roomId, player1);
            }
            roomInfo.Config(userId);
            return View("Index", roomInfo);
        }

    }
}
