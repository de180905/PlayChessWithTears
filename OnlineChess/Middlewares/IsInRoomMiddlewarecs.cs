using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using OnlineChess.Hubs;
using System.Security.Claims;
using System.Threading.Tasks;

public class IsInRoomMiddleware
{
    private readonly RequestDelegate _next;

    public IsInRoomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestPath = context.Request.Path;
        if(!requestPath.StartsWithSegments("/Room") && !requestPath.StartsWithSegments("/hubs/chessHub") && context.User.Identity.IsAuthenticated)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(ChessHubExtensions.GetRoomOfUser(userId) != null)
            {
                // Redirect to room page if the user is already in a match
                context.Response.Redirect("/Room");
                return;
            }
        }

        // Continue to the next middleware if the condition is met
        await _next(context);
    }
}

