using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Foliofy.Models;

namespace Foliofy.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CreativeType { get; set; }
        public string IconPath { get; set; } = "";
        [InverseProperty(nameof(Follower.FollowerUser))]
        public ICollection<Follower> Followers { get; set; }
        [InverseProperty(nameof(Follower.FollowedUser))]
        public ICollection<Follower> Followings { get; set; }
    }
}
