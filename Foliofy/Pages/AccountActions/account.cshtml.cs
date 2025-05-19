using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Foliofy.DataBase;
using Foliofy.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;

namespace Foliofy.Pages.AccountActions
{
    public class accountModel : PageModel
    {
        private readonly Database db;
        public accountModel(Database db) { this.db = db; }

        [BindProperty]
        public User User { get; set; }

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

            return new OkObjectResult(new { message = "Account was created successfully!" });
        }

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
                return new OkObjectResult(new { message = "You were logged in successfully!" });
            }

            ModelState.AddModelError("Error", "Invalid username or password!");
            return BadRequest(ModelState);
        }
    }
}
