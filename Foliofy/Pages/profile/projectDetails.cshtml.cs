using Foliofy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Foliofy.DataBase;
using Microsoft.EntityFrameworkCore;
using Supabase;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Foliofy.Pages.profile
{
    [Authorize]
    public class projectDetailsModel : PageModel
    {
        private readonly Database db;
        private readonly Client supabase;

        public projectDetailsModel(Database db, Client supabase)
        {
            this.db = db;
            this.supabase = supabase;
        }

        public Project? Project { get; set; }
        public Models.User? CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            string? claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(claimUserId);

            CurrentUser = await db.Users.FirstOrDefaultAsync(user => user.Id == userId);

            Project = await db.Projects
                .Include(project => project.User)
                .Include(project => project.Tags)
                .Include(project => project.Files)
                .FirstOrDefaultAsync(project => project.Id == id);

            if (Project == null)
                return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveProjectAsync(int id)
        {
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !int.TryParse(claimUserId, out int userId))
                return RedirectToPage("/AccountActions/login");

            var project = await db.Projects
                .Include(p => p.Files)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null)
                return NotFound();

            // Remove from supabase
            var pathsToDelete = new List<string>();

            var uri = new Uri(project.CoverImage);
            var coverPath = uri.AbsolutePath
                              .Substring(uri.AbsolutePath.IndexOf("/projects/") + "/projects/".Length);
            pathsToDelete.Add(coverPath);

            foreach (var file in project.Files)
            {
                uri = new Uri(file.Path);
                var filePath = uri.AbsolutePath
                                  .Substring(uri.AbsolutePath.IndexOf("/projects/") + "/projects/".Length);
                pathsToDelete.Add(filePath);
            }

            if (pathsToDelete.Count > 0)
            {
                var deleteResult = await supabase.Storage.From("projects").Remove(pathsToDelete);
            }

            // remove from database
            db.UserTags.RemoveRange(project.Tags);
            db.ProjectFiles.RemoveRange(project.Files);
            db.Projects.Remove(project);

            await db.SaveChangesAsync();
            return RedirectToPage("profile");
        }
    }
}
