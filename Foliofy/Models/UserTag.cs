using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foliofy.Models
{
    public enum TagCategory
    {
        CreativeType,
        Tool
    }
    public class UserTag
    {
        [Key]
        public int Id { get; set; }
        public string TagName { get; set; }
        public TagCategory Category { get; set; } = TagCategory.CreativeType;

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public User? User { get; set; }
        [ForeignKey(nameof(Project))]
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}
