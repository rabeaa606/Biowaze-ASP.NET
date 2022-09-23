namespace API.Data;

public class PostRepository : IPostsRepository
{
    private readonly DataContext _context;

    public PostRepository(DataContext context)
    {
        _context = context;
    }

    public void AddPost(Post post)
    {
        _context.Posts.Add(post);
    }

    public void DeletePost(Post post)
    {
        _context.Posts.Remove(post);
    }

    public async Task<Post> GetPost(int id)
    {
        return await _context.Posts
        .Include(u => u.Creater)
        .Include(c => c.Creater.Photo)
        .Include(c => c.Photos)
        .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Post> GetlastPost(string creater)
    {
        return await _context.Posts
        .Where(c => c.CreaterUsername == creater)
        .Include(u => u.Creater)
        .Include(c => c.Creater.Photo)
        .Include(c => c.Photos)
        .FirstOrDefaultAsync(p => p.PostCreated == _context.Posts.Max(x => x.PostCreated));
    }

    public async Task<PagedList<PostDto>> GetUsersPosts(PostParams postsParams)
    {
        var posts = _context.Posts
                   .OrderByDescending(m => m.PostCreated)
                   .AsQueryable();

        posts = postsParams.Core switch
        {
            "My" => posts.Where(m => m.Creater.UserName == postsParams.Username && m.Role != "Doctor" && m.Role != "Group"),
            "Others" =>
            posts.Where(m => m.Creater.UserName != postsParams.Username
            && m.Role != "Doctor" && m.Role != "Group"),
            _ => throw new ArgumentOutOfRangeException(nameof(postsParams.Core)),
        };

        var postsDto = posts.Select(post => new PostDto
        {
            CreaterId = post.CreaterId,
            CreaterUsername = post.Creater.UserName,
            CreaterKnownAs = post.Creater.KnownAs,
            CreaterPhotoUrl = post.Creater.Photo.FirstOrDefault(x => x.IsMain).Url,
            Content = post.Content,
            PostCreated = post.PostCreated,
            Id = post.Id,
        });

        return await PagedList<PostDto>.CreateAsync(postsDto,
                  postsParams.PageNumber, postsParams.PageSize);
    }

    public async Task<PagedList<PostDto>> GetDoctorsPosts(PostParams postsParams)
    {

        var posts = _context.Posts
                   .OrderByDescending(m => m.PostCreated)
                   .Include(p => p.Photos)
                   .AsQueryable();


        posts = postsParams.Core switch
        {
            "My" => posts.Where(m => m.Creater.UserName == postsParams.Username && m.Role == "Doctor" && m.Role != "Group"),
            "Others" =>
            posts.Where(m => m.Creater.UserName != postsParams.Username
            && m.Role == "Doctor" && m.Role != "Group"),
            _ => throw new ArgumentOutOfRangeException(nameof(postsParams.Core)),
        };


        var postsDto = posts.Select(post => new PostDto
        {
            CreaterId = post.CreaterId,
            CreaterUsername = post.Creater.UserName,
            CreaterKnownAs = post.Creater.KnownAs,
            CreaterPhotoUrl = post.Creater.Photo.FirstOrDefault(x => x.IsMain).Url,
            Content = post.Content,
            PostCreated = post.PostCreated,
            Id = post.Id,
            Photo = post.Photos.Select(p => new PhotoDto
            {
                Id = p.Id,
                Url = p.Url

            }).ToArray()
        });


        return await PagedList<PostDto>.CreateAsync(postsDto,
                  postsParams.PageNumber, postsParams.PageSize);
    }

}
