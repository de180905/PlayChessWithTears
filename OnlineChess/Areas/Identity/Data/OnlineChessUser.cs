using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OnlineChess.Data;

// Add profile data for application users by adding properties to the OnlineChessUser class
public class OnlineChessUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string InGame { get; set; }
}

