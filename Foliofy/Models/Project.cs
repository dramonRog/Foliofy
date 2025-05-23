using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foliofy.Models
{
    public enum ProjectAccess
    {
        Public,
        Private
    }
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string CoverImage { get; set; }
        public ProjectAccess Status { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        [InverseProperty(nameof(UserTag.Project))]
        public ICollection<UserTag> Tags { get; set; } = new List<UserTag>();
        [InverseProperty(nameof(ProjectFile.Project))]
        public ICollection<ProjectFile> Files { get; set; } = new List<ProjectFile>();
    }
}
