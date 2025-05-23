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
        public string IconPath { get; set; } = "";
        public string UserDescription { get; set; } = "";

        [InverseProperty(nameof(Follower.FollowerUser))]
        public ICollection<Follower> Followers { get; set; } = new List<Follower>();

        [InverseProperty(nameof(Follower.FollowedUser))]
        public ICollection<Follower> Followings { get; set; } = new List<Follower>();

        [InverseProperty(nameof(UserTag.User))]
        public ICollection<UserTag> Tags { get; set; } = new List<UserTag>();

        [InverseProperty(nameof(Project.User))]
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
