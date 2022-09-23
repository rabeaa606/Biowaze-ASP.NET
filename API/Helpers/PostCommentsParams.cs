namespace API.Helpers;

public class PostCommentsParams : PaginationParams
{
    //for getting user comments
    public int UserId { get; set; }
    public string Username { get; set; }
}
