namespace API.Interfaces;

public interface IPostCommentsRepository
{
    void AddComment(PostComment postLike);
    void DeletComment(PostComment postComment);
    Task<PostComment> GetComment(int id);
    Task<PostComment> GetPostComment(int sourcePostId, int likedUserId);
    Task<PagedList<PostDto>> GetPostsCommentedByUser(PostCommentsParams postCommentsParams);
    Task<PagedList<PostDto>> GetDocPostsCommentedByUser(PostCommentsParams postCommentsParams);
    Task<IEnumerable<PostCommentDto>> GetcommentOFPost(int postId);
}
