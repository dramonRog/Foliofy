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

        public Project? CurrentProject { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                CurrentProject = null;
            }
            else
            {
                CurrentProject = await db.Projects
                    .Include(project => project.Tags)
                    .Include(project => project.Files)
                    .FirstOrDefaultAsync(project => project.Id == id);

                if (CurrentProject == null)
                {
                    return NotFound();
                }
            }
            return Page();
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

            var cleanFileName = Path.GetFileNameWithoutExtension(projectName);
            var ext = Path.GetExtension(projectName);
            var fileNaming = $"{Slugify(cleanFileName)}_{Guid.NewGuid().ToString().Substring(0, 4)}{ext}";
            string basePath = $"{user.Username}/{fileNaming}";

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
                var originalName = Path.GetFileNameWithoutExtension(projectCoverImage.FileName);
                var extension = Path.GetExtension(projectCoverImage.FileName);
                var fileName = $"{Slugify(originalName)}_{Guid.NewGuid().ToString().Substring(0, 4)}{extension}";
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
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{Slugify(fileNameWithoutExtension)}_{Guid.NewGuid().ToString().Substring(0, 4)}{extension}";
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

            var tagList = projectTags.Split(',');
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

        public async Task<IActionResult> OnPostUpdateProjectAsync(
            int id,
            string projectName,
            string projectDescription,
            string projectStatus,
            string projectTags,
            IFormFile? projectCoverImage,
            List<IFormFile>? projectFiles)
        {
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !int.TryParse(claimUserId, out int userId))
                return RedirectToPage("/AccountActions/login");

            var project = await db.Projects
                .Include(p => p.Files)
                .Include(p => p.Tags)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
                return NotFound();

            var oldProjectName = project.ProjectName;
            var newSlug = Slugify(projectName);
            var basePath = $"{project.User.Username}/{newSlug}_{project.Id}";

            project.ProjectName = projectName;
            project.ProjectDescription = projectDescription;
            project.Status = Enum.TryParse<ProjectAccess>(projectStatus, out var parsedStatus) ? parsedStatus : ProjectAccess.Private;

            if (projectCoverImage != null && projectCoverImage.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(project.CoverImage))
                {
                    var oldPath = GetStoragePathFromUrl(project.CoverImage);
                    await supabase.Storage.From("projects").Remove(new List<string> { oldPath });
                }

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(projectCoverImage.FileName);
                var extension = Path.GetExtension(projectCoverImage.FileName);
                var fileName = $"{Slugify(fileNameWithoutExtension)}_{Guid.NewGuid().ToString().Substring(0, 4)}{extension}";
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

                project.CoverImage = supabase.Storage.From("projects").GetPublicUrl(storagePath);
            }

            var incomingTags = projectTags.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();
            var existingTags = project.Tags.Select(t => t.TagName).ToList();

            var tagsToAdd = incomingTags.Except(existingTags).ToList();
            var tagsToRemove = existingTags.Except(incomingTags).ToList();

            foreach (var tag in tagsToAdd)
            {
                db.UserTags.Add(new UserTag
                {
                    ProjectId = project.Id,
                    TagName = tag,
                    Category = TagCategory.Tool
                });
            }

            var tagsToDelete = project.Tags.Where(t => tagsToRemove.Contains(t.TagName)).ToList();
            db.UserTags.RemoveRange(tagsToDelete);

            var incomingFileNames = Request.Form["existingFileNames"].ToString().Split(',').Where(f => !string.IsNullOrWhiteSpace(f)).ToList();

            var filesToDelete = project.Files.Where(f => !incomingFileNames.Contains(Path.GetFileName(f.Path))).ToList();

            foreach (var file in filesToDelete)
            {
                var filePath = GetStoragePathFromUrl(file.Path);
                await supabase.Storage.From("projects").Remove(new List<string> { filePath });
                db.ProjectFiles.Remove(file);
            }

            if (projectFiles != null && projectFiles.Any())
            {
                foreach (var file in projectFiles)
                {
                    var fileName = $"{Slugify(Path.GetFileNameWithoutExtension(file.FileName))}_{Guid.NewGuid().ToString().Substring(0, 4)}{Path.GetExtension(file.FileName)}";
                    var storagePath = $"{basePath}/files/{fileName}";

                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();

                    var uploadResult = await supabase.Storage.From("projects").Upload(fileBytes, storagePath, new Supabase.Storage.FileOptions
                    {
                        ContentType = file.ContentType,
                        Upsert = true
                    });

                    if (!string.IsNullOrWhiteSpace(uploadResult))
                    {
                        var publicUrl = supabase.Storage.From("projects").GetPublicUrl(storagePath);
                        db.ProjectFiles.Add(new ProjectFile
                        {
                            ProjectId = project.Id,
                            Path = publicUrl
                        });
                    }
                }
            }

            await db.SaveChangesAsync();
            return new OkObjectResult(new { message = "Project updated", id = project.Id });
        }


        private string GetStoragePathFromUrl(string url)
        {
            var index = url.IndexOf("/projects/");
            return url.Substring(index + "/projects/".Length);
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
