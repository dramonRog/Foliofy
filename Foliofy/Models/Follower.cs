using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foliofy.Models
{
    public class Follower
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(FollowerUser))]
        public int FollowerId { get; set; }
        public User FollowerUser { get; set; }
        [ForeignKey(nameof(FollowedUser))]
        public int FollowedId { get; set; }
        public User FollowedUser { get; set; }
    }
}
