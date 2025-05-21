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


        [BindProperty]
        public IFormFile? UploadedIcon { get; set; }
        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            string? Username = Request.Form["Username"];
            string? Email = Request.Form["Email"];
            string? UserDescription = Request.Form["UserDescription"];
            string? AddCustomTags = Request.Form["AddCustomTags"];
            string? RemoveCustomTags = Request.Form["RemoveCustomTags"];
            string? AddCreativeTypes = Request.Form["AddCreativeTypes"];
            string? RemoveCreativeTypes = Request.Form["RemoveCreativeTypes"];

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

            string[] AddTagList = AddCustomTags == null ? Array.Empty<string>() : AddCustomTags.Split(',');
            string[] AddCreativeTypeList = AddCreativeTypes == null ? Array.Empty<string>() : AddCreativeTypes.Split(',');
            string[] RemoveTagList = RemoveCustomTags == null ? Array.Empty<string>() : RemoveCustomTags.Split(',');
            string[] RemoveCreativeTypeList = RemoveCreativeTypes == null ? Array.Empty<string>() : RemoveCreativeTypes.Split(',');

            if (RemoveCreativeTypeList[0].Trim() != "")
            {
                foreach (string removeCreativeType in RemoveCreativeTypeList)
                {
                    var tagToRemove = cookieUser.Tags.FirstOrDefault(tag => tag.TagName == removeCreativeType && tag.Category == TagCategory.CreativeType);
                    if (tagToRemove != null)
                        cookieUser.Tags.Remove(tagToRemove);
                }
            }
            if (AddCreativeTypeList[0].Trim() != "") { 
                foreach (string addCreativeType in AddCreativeTypeList)
                {
                    db.UserTags.Add(new UserTag
                    {
                        TagName = addCreativeType,
                        Category = TagCategory.CreativeType,
                        UserId = cookieUser.Id
                    });
                }
            }
            if (RemoveTagList[0].Trim() != "")
            {
                foreach (string removeTag in RemoveTagList)
                {
                    var tagToRemove = cookieUser.Tags.FirstOrDefault(tag => tag.TagName == removeTag && tag.Category == TagCategory.Tool);
                }
            }
            if (AddTagList[0].Trim() != "")
            {
                foreach (string addTag in AddTagList)
                {
                    //if (addTag.Trim() != "")
                    //{
                    db.UserTags.Add(new UserTag
                    {
                        TagName = addTag,
                        Category = TagCategory.Tool,
                        UserId = cookieUser.Id
                    });
                    //}
                }
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
            return new OkObjectResult(true);
        }
    }
}
