namespace API.Entities;

public class PostComment
{
    public int Id { get; set; }

    public Post SourcePost { get; set; }
    public int SourcePostId { get; set; }

    public AppUser CommentedUser { get; set; }
    public int CommentedUserId { get; set; }
    public string Content { get; set; }
    public string Role { get; set; }
    public DateTime CommentCreated { get; set; } = DateTime.UtcNow;

}
