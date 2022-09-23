namespace API.Entities;
public class PostPhoto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string PublicId { get; set; }

    public Post SourcePost { get; set; }
    public int SourcePostId { get; set; }

}
