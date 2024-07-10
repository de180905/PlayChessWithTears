using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthMVC.Data;

// Add profile data for application users by adding properties to the AuthMVCUser class
public class AuthMVCUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

