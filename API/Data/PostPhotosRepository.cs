namespace API.Data;

public class PostPhotosRepository : IPostPhotosRepository
{
    private readonly DataContext _context;
    public PostPhotosRepository(DataContext context)
    {
        _context = context;
    }

    public void AddPhoto(PostPhoto postPhoto)
    {
        _context.PostsPhoto.Add(postPhoto);
    }

    public async Task<PostPhoto> GetPhoto(int id)
    {
        return await _context.PostsPhoto
                        .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PostPhotoDto>> GetPhotosOFPost(int postId)
    {
        var postPhotos = _context.PostsPhoto.AsQueryable();


        postPhotos = postPhotos.Where(img => img.SourcePostId == postId);

        var postphotos = await postPhotos.Select(p => new PostPhotoDto
        {
            Id = p.Id,
            Url = p.Url

        }).ToListAsync();

        return postphotos;
    }
}
