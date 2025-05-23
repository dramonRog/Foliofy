using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foliofy.Models
{
    public class ProjectFile
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
