namespace API.Entities;

public class Post
{
    public int Id { get; set; }
    public AppUser Creater { get; set; }
    public int CreaterId { get; set; }
    public string CreaterUsername { get; set; }
    public string Content { get; set; }
    public string Role { get; set; }
    public MemberGroup Group { get; set; }
    public int SourceGroupId { get; set; }
    public string SourceGroupName { get; set; }

    public DateTime PostCreated { get; set; } = DateTime.UtcNow;
    public ICollection<PostLike> LikedByUsers { get; set; }
    public ICollection<PostComment> CommentedByUsers { get; set; }
    public ICollection<PostPhoto> Photos { get; set; }

}
