namespace API.Interfaces;

public interface IPostLikesRepository
{
    void AddLike(PostLike postLike);
    Task<PostLike> GetPostLike(int sourcePostId, int likedUserId);
    Task<PagedList<PostDto>> GetPostsLikedByUser(PostLikesParams postLikesParams);
    Task<PagedList<PostDto>> GetDocPostsLikedByUser(PostLikesParams postLikesParams);
    Task<IEnumerable<PostLikeDto>> GetUsersLikedPost(int postId);
}
