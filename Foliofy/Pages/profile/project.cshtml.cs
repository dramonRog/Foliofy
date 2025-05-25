using Foliofy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Foliofy.DataBase;
using Supabase;
using Microsoft.EntityFrameworkCore;

namespace Foliofy.Pages.profile
{
    [Authorize]
    public class ProjectModel : PageModel
    {
        private readonly Database db;
        private readonly Client supabase;

        public ProjectModel(Database db, Client supabase)
        {
            this.db = db;
            this.supabase = supabase;
        }

        public async Task<IActionResult> OnPostCreateProjectAsync(
            string projectName,
            string projectDescription,
            string projectStatus,
            string projectTags,
            IFormFile projectCoverImage,
            List<IFormFile> projectFiles)
        {
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !int.TryParse(claimUserId, out int userId))
                return RedirectToPage("/AccountActions/login");

            var user = await db.Users.Include(user => user.Projects).FirstOrDefaultAsync(user => user.Id == userId);
            if (user == null)
                return NotFound();

            if (user.Projects.Any(project => project.ProjectName == projectName))
            {
                ModelState.AddModelError("projectName", "That project name already exists in your projects!");
                return BadRequest(ModelState);
            }

            string slugifiedName = Slugify(projectName);
            string uniqueSlug = $"{slugifiedName}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            string basePath = $"{user.Username}/{uniqueSlug}";

            Project project = new Project
            {
                ProjectName = projectName,
                ProjectDescription = projectDescription,
                Status = Enum.TryParse<ProjectAccess>(projectStatus, out var status) ? status : ProjectAccess.Private,
                UserId = userId,
                CoverImage = ""
            };

            db.Projects.Add(project);
            await db.SaveChangesAsync();

            if (projectCoverImage != null && projectCoverImage.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(projectCoverImage.FileName)}";
                var storagePath = $"{basePath}/cover/{fileName}";

                using var ms = new MemoryStream();
                await projectCoverImage.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                var uploadResult = await supabase.Storage
                    .From("projects")
                    .Upload(fileBytes, storagePath, new Supabase.Storage.FileOptions
                    {
                        ContentType = projectCoverImage.ContentType,
                        Upsert = true
                    });

                if (!string.IsNullOrWhiteSpace(uploadResult))
                {
                    project.CoverImage = supabase.Storage
                        .From("projects")
                        .GetPublicUrl(storagePath);
                }
            }

            if (projectFiles != null && projectFiles.Any())
            {
                foreach (var file in projectFiles)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var storagePath = $"{basePath}/files/{fileName}";

                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();

                    var uploadResult = await supabase.Storage
                        .From("projects")
                        .Upload(fileBytes, storagePath, new Supabase.Storage.FileOptions
                        {
                            ContentType = file.ContentType,
                            Upsert = true
                        });

                    if (!string.IsNullOrWhiteSpace(uploadResult))
                    {
                        var publicUrl = supabase.Storage
                            .From("projects")
                            .GetPublicUrl(storagePath);

                        db.ProjectFiles.Add(new ProjectFile
                        {
                            ProjectId = project.Id,
                            Path = publicUrl
                        });
                    }
                }
            }

            var tagList = projectTags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var tag in tagList)
            {
                db.UserTags.Add(new UserTag
                {
                    ProjectId = project.Id,
                    TagName = tag,
                    Category = TagCategory.Tool
                });
            }

            await db.SaveChangesAsync();

            return new OkObjectResult(new { message = "Project created", id = project.Id });
        }

        private string Slugify(string input)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return new string(input
                .ToLowerInvariant()
                .Where(c => !invalidChars.Contains(c))
                .ToArray())
                .Replace(" ", "-")
                .Trim('-', '_');
        }
    }
}
