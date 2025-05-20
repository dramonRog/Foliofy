using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Foliofy.DataBase;
using Foliofy.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Foliofy.Pages.profile
{
    [Authorize]
    public class EditProfileModel : PageModel
    {
        private readonly Database db;

        public EditProfileModel(Database db)
        {
            this.db = db;
        }

        [BindProperty]
        public User? CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claimUserId == null || !int.TryParse(claimUserId, out var userId))
                return RedirectToPage("../AccountActions/login");

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
