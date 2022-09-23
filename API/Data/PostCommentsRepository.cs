namespace API.Data;

public class PostCommentsRepository : IPostCommentsRepository
{
    private readonly DataContext _context;
    public PostCommentsRepository(DataContext context)
    {
        _context = context;
    }

    public void AddComment(PostComment postComment)
    {
        _context.PostsComments.Add(postComment);

    }
    public void DeletComment(PostComment postComment)
    {
        _context.PostsComments.Remove(postComment);
    }

    public async Task<PostComment> GetComment(int id)
    {
        return await _context.PostsComments
            .SingleOrDefaultAsync(x => x.Id == id);
    }
    public async Task<PostComment> GetPostComment(int sourcePostId, int likedUserId)
    {
        return await _context.PostsComments.FindAsync(sourcePostId, likedUserId);
    }
    public async Task<IEnumerable<PostCommentDto>> GetcommentOFPost(int postId)
    {
        var postComments = _context.PostsComments.AsQueryable();


        postComments = postComments.Where(comment => comment.SourcePostId == postId);
        var comments = await postComments.Select(comment => new PostCommentDto
        {
            Id = comment.Id,
            PostId = comment.CommentedUser.Id,
            Username = comment.CommentedUser.UserName,
            KnownAs = comment.CommentedUser.KnownAs,
            PhotoUrl = comment.CommentedUser.Photo.FirstOrDefault(p => p.IsMain).Url,
            Content = comment.Content,
            Role = comment.Role,
            CommentCreated = comment.CommentCreated

        }).ToListAsync();



        return comments;
    }
    public async Task<PagedList<PostDto>> GetDocPostsCommentedByUser(PostCommentsParams postCommentsParams)
    {

        var posts = _context.Posts
         .Where(m => m.Creater.UserName != postCommentsParams.Username)
         .OrderByDescending(m => m.PostCreated)
        .AsQueryable();

        var postsComments = _context.PostsComments.AsQueryable();

        postsComments = postsComments.Where(comment => comment.CommentedUserId == postCommentsParams.UserId);

        posts = postsComments.Select(like => like.SourcePost)
        .Where(u => u.CreaterId != postCommentsParams.UserId && u.Role == "Doctor")
         .OrderByDescending(m => m.PostCreated);


        var commentedPosts = posts.Select(post => new PostDto
        {
            CreaterUsername = post.CreaterUsername,
            CreaterKnownAs = post.Creater.KnownAs,
            CreaterPhotoUrl = post.Creater.Photo.FirstOrDefault(p => p.IsMain).Url,
            CreaterId = post.CreaterId,
            Content = post.Content,
            PostCreated = post.PostCreated,
            Id = post.Id
        });

        return await PagedList<PostDto>.CreateAsync(commentedPosts,
            postCommentsParams.PageNumber, postCommentsParams.PageSize);
    }

    public async Task<PagedList<PostDto>> GetPostsCommentedByUser(PostCommentsParams postCommentsParams)
    {


        var posts = _context.Posts
         .Where(m => m.Creater.UserName != postCommentsParams.Username)
         .OrderByDescending(m => m.PostCreated)
        .AsQueryable();

        var postsComments = _context.PostsComments.AsQueryable();

        postsComments = postsComments.Where(comment => comment.CommentedUserId == postCommentsParams.UserId);

        posts = postsComments.Select(like => like.SourcePost)
        .Where(u => u.CreaterId != postCommentsParams.UserId && u.Role != "Doctor")
         .OrderByDescending(m => m.PostCreated);


        var commentedPosts = posts.Select(post => new PostDto
        {
            CreaterUsername = post.CreaterUsername,
            CreaterKnownAs = post.Creater.KnownAs,
            CreaterPhotoUrl = post.Creater.Photo.FirstOrDefault(p => p.IsMain).Url,
            CreaterId = post.CreaterId,
            Content = post.Content,
            PostCreated = post.PostCreated,
            Id = post.Id
        });

        return await PagedList<PostDto>.CreateAsync(commentedPosts,
            postCommentsParams.PageNumber, postCommentsParams.PageSize);
    }


}
