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
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        public TagCategory Category { get; set; } = TagCategory.CreativeType;
    }
}
