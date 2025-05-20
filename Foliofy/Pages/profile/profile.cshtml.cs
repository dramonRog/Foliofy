using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Foliofy.DataBase;
using Foliofy.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Foliofy.Pages
{
    [Authorize]
    public class profileModel : PageModel
    {
        private readonly Database db;

        public profileModel(Database db)
        {
            this.db = db;
        }

        [BindProperty]
        public User? CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToPage("/AccountActions/login");
            }

            CurrentUser = await db.Users
                .Include(user => user.Followers)
                .Include(user => user.Followings)
                .Include(user => user.Tags)
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (CurrentUser == null)
                return NotFound();
            return Page();
        }
    }
}
