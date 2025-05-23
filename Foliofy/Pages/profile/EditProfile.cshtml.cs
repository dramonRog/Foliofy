using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Foliofy.DataBase;
using Foliofy.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

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

        public User CurrentUser { get; set; }
        public async Task<IActionResult> OnGetAsync(User? CurrentUser)
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
            this.CurrentUser = CurrentUser;
            return Page();
        }

        [BindProperty]
        public IFormFile? UploadedIcon { get; set; }
        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            string? Username = Request.Form["Username"];
            string? Email = Request.Form["Email"];
            string? UserDescription = Request.Form["UserDescription"];
            string? CustomTags = Request.Form["CustomTags"];
            string? CreativeTypes = Request.Form["CreativeTypes"];

            var ClaimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (ClaimUserId == null || !int.TryParse(ClaimUserId, out int userId))
                return RedirectToPage("/AccountActions/login");

            User? cookieUser = await db.Users
                .Include(user => user.Tags)
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (cookieUser == null)
                return NotFound();

            if (await db.Users.AnyAsync(user => user.Username == Username && user.Id != cookieUser.Id))
                ModelState.AddModelError("Username", "That username is already taken!");
            if (await db.Users.AnyAsync(user => user.Email == Email && user.Id != cookieUser.Id))
                ModelState.AddModelError("Email", "That email is already taken!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string[] TagList = CustomTags == null || CustomTags == "" ? Array.Empty<string>() : CustomTags.Split(',');
            string[] CreativeTypeList = CreativeTypes == null || CreativeTypes == "" ? Array.Empty<string>() : CreativeTypes.Split(',');

            db.UserTags.RemoveRange(db.UserTags.Where(tag => tag.UserId == userId));
            foreach(string creativeType in CreativeTypeList)
            {
                db.UserTags.Add(new UserTag
                {
                    TagName = creativeType,
                    Category = TagCategory.CreativeType,
                    UserId = userId
                });
            }
            
            foreach(string customTag in TagList)
            {
                db.UserTags.Add(new UserTag
                {
                    TagName = customTag,
                    Category = TagCategory.Tool,
                    UserId = userId
                });
            }

            cookieUser.Username = Username;
            cookieUser.Email = Email;
            cookieUser.UserDescription = UserDescription;

            if (UploadedIcon != null && UploadedIcon.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(cookieUser.IconPath))
                {
                    var oldIconPath = Path.Combine("wwwroot", cookieUser.IconPath.TrimStart('/'));

                    if (System.IO.File.Exists(oldIconPath))
                    {
                        System.IO.File.Delete(oldIconPath);
                    }
                }


                var uploadsFolder = Path.Combine("wwwroot", "uploads", "icons");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"user_{userId}_{Guid.NewGuid()}{Path.GetExtension(UploadedIcon.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedIcon.CopyToAsync(fileStream);
                }

                cookieUser.IconPath = $"/uploads/icons/{uniqueFileName}";
            }

            await db.SaveChangesAsync();

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, cookieUser.Id.ToString()),
                new Claim(ClaimTypes.Name, cookieUser.Username),
                new Claim("icons", cookieUser.IconPath)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return new OkObjectResult(true);
        }
    }
}
