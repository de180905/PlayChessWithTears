// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineChess.Data;

namespace OnlineChess.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<OnlineChessUser> _userManager;
        private readonly SignInManager<OnlineChessUser> _signInManager;

        public IndexModel(
            UserManager<OnlineChessUser> userManager,
            SignInManager<OnlineChessUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Firstname")]
            public string Firstname { get; set; }
            [Display(Name = "Lastname")]
            public string Lastname { get; set; }
            [Display(Name = "Ingame")]
            public string Ingame { get; set; }
        }

        private async Task LoadAsync(OnlineChessUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstname = user.FirstName;
            var lastname = user.LastName;
            var ingame = user.InGame;

            Username = userName;

            Input = new InputModel
            {
                Firstname = firstname,
                Lastname = lastname,
                Ingame = ingame
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var firstname = user.FirstName;
            if (Input.Firstname != firstname)
            {
                user.FirstName = Input.Firstname;
                var setFirstnameResult = await _userManager.UpdateAsync(user);
                if (!setFirstnameResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set firstname.";
                    return RedirectToPage();
                }
            }

            var lastname = user.LastName;
            if (Input.Lastname != lastname)
            {
                user.LastName = Input.Lastname;
                var setLastnameResult = await _userManager.UpdateAsync(user);
                if (!setLastnameResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set lastname.";
                    return RedirectToPage();
                }
            }

            var ingame = user.InGame;
            if (Input.Ingame != ingame)
            {
                user.InGame = Input.Ingame;
                var setIngameResult = await _userManager.UpdateAsync(user);
                if (!setIngameResult.Succeeded)
                {
                    StatusMessage = "Ingame was taken or there was unexpected error when trying to set ingame.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
