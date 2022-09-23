namespace API.DTOs;

public class PostCommentDto
{
    public int Id { get; set; }

    public int PostId { get; set; }  //post id
    public string Username { get; set; }
    public string KnownAs { get; set; }
    public string PhotoUrl { get; set; }
    public string Content { get; set; }
    public string Role { get; set; }
    public DateTime CommentCreated { get; set; }



}
