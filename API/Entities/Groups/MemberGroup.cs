namespace API.Entities;

public class MemberGroup
{
    public int Id { get; set; }
    public string GroupName { get; set; }
    public AppUser Admin { get; set; }
    public int AdminId { get; set; }
    public string AdminUsername { get; set; }
    public string Specialization { get; set; }
    public virtual ICollection<Post> GroupPosts { get; set; }
    public ICollection<UserGroup> Users { get; set; }

}
