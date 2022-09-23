namespace API.Interfaces;

public interface IPostsRepository
{
    Task<PagedList<PostDto>> GetUsersPosts(PostParams postsParams);
    Task<PagedList<PostDto>> GetDoctorsPosts(PostParams postsParams);

    void AddPost(Post post);
    void DeletePost(Post post);
    Task<Post> GetPost(int id);
    Task<Post> GetlastPost(string creater);
}
