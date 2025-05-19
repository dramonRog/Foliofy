using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Foliofy.DataBase;
using Foliofy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Foliofy.Pages.AccountActions
{
    public class logoutModel : PageModel
    {
        public async Task<IActionResult> OnGetLogoutAsync()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToPage("/home");
        }
    }
}
