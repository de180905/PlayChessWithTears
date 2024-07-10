using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using OnlineChess.Data;
using OnlineChess.Models.DTO;
using OnlineChess.Models.Game;

namespace OnlineChess.Controllers
{
    [Authorize]
    public class History : Controller
    {
        private readonly UserManager<OnlineChessUser> _userManager;
        private readonly OnlineChessDbContext _dbContext;

        public History(OnlineChessDbContext dbContext, UserManager<OnlineChessUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            // Retrieve data from the database into memory
            var matchSummaries = _dbContext.MatchSummaries
                                            .Include(ms => ms.Black)
                                            .Include(ms => ms.White)
                                            .Where(ms => ms.WhiteId == userId || ms.BlackId == userId)
                                            .OrderByDescending(ms => ms.Time)
                                            .Take(15)
                                            .ToList();

            // Perform the conversion from enum to string in memory
            var matchDTOs = matchSummaries
                                .Select(ms => new MatchDTO(ms.Black.InGame, ms.White.InGame, ms.Result.ToString(), ms.Time))
                                .ToList();
            return View(matchDTOs);
        }
    }
}
