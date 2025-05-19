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
    public class LoginModel : PageModel
    {
        private readonly Database db;

        public LoginModel(Database db)
        {
            this.db = db;
        }

        [BindProperty]
        public User User { get; set; }
        public async Task<IActionResult> OnPostLoginAsync()
        {
            ModelState.Clear();
            User user = await db.Users.FirstOrDefaultAsync(user => user.Username == User.Username);

            if (user == null)
            {
                ModelState.AddModelError("Error", "Invalid username or password!");
                return BadRequest(ModelState);
            }

            var hasher = new PasswordHasher<User>();
            if (hasher.VerifyHashedPassword(user, user.Password, User.Password) == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, User.Username)
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);
                return new OkObjectResult(new { message = "You were logged in successfully!" });
            }

            ModelState.AddModelError("Error", "Invalid username or password!");
            return BadRequest(ModelState);
        }

    }
}
