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

        public async Task<IActionResult> OnPostCheckTagAsync(string CustomTagName, string removedTagsList)
        {
            var userClaimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userClaimId == null || !int.TryParse(userClaimId, out int userId))
                return RedirectToPage("/AccountActions/login");

            string[] removedTags = removedTagsList == null ? Array.Empty<string>() : removedTagsList.Split(',');
            if (await db.UserTags.AnyAsync(tag => tag.TagName == CustomTagName && tag.UserId == userId
                && !removedTags.Contains(CustomTagName) && tag.Category == TagCategory.Tool))
                return new JsonResult(false);
            return new JsonResult(true);
        }

        public async Task<IActionResult> OnPostCheckCreativeTypeAsync(string CreativeType, string removeCreativeTagsList)
        {
            var userClaimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userClaimId == null || !int.TryParse(userClaimId, out int userId))
                return RedirectToPage("/AccountActions/login");

            string[] removeTagsList = removeCreativeTagsList == null ? Array.Empty<string>() : removeCreativeTagsList.Split(',');

            if (await db.UserTags.AnyAsync(tag => tag.TagName == CreativeType && tag.UserId == userId && !removeTagsList.Contains(CreativeType)))
                return new JsonResult(false);
            return new JsonResult(true);
        }
    }
}
