namespace API.Entities;

public class AppUser : IdentityUser<int>  //use dotnet identity package
{
    public DateTime DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime LastActive { get; set; } = DateTime.Now;
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public ICollection<Photo> Photo { get; set; }
    public ICollection<Message> MessagesSent { get; set; }
    public ICollection<Message> MessagesReceived { get; set; }
    public ICollection<AppUserRole> UserRoles { get; set; }
    public ICollection<Post> UserPosts { get; set; }
    public ICollection<UserGroup> Groups { get; set; }
    public ICollection<Food> UserFoods { get; set; }
    public ICollection<Feeling> UserFeelings { get; set; }

}
