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
            ModelState.Clear();

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
    }
}
