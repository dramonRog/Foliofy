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
    public class registerModel : PageModel
    {
        private readonly Database db;
        public registerModel(Database db)
        {
            this.db = db;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public string CreativeType { get; set; }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (await db.Users.AnyAsync(user => user.Username == User.Username))
                ModelState.AddModelError("Username", "This username is already taken!");

            if (await db.Users.AnyAsync(user => user.Email == User.Email))
                ModelState.AddModelError("Email", "This email is already taken!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hasher = new PasswordHasher<User>();
            User.Password = hasher.HashPassword(User, User.Password);

            db.Users.Add(User);
            await db.SaveChangesAsync();

            UserTag userTag = new UserTag
            {
                TagName = CreativeType,
                UserId = User.Id
            };

            db.UserTags.Add(userTag);
            await db.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, User.Id.ToString()),
                new Claim(ClaimTypes.Name, User.Username)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return new OkObjectResult(new { message = "Account was created successfully!" });
        }
    }
}
