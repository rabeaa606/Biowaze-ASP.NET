namespace API.Data;
public class UnitOfWork : IUnitOfWork
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly UserManager<AppUser> _userManager;


    public UnitOfWork(UserManager<AppUser> userManager, DataContext context, IMapper mapper)
    {
        _userManager = userManager;
        _context = context;
        _mapper = mapper;
    }

    public IUserRepository UserRepository => new UserRepository(_context, _mapper);
    public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);
    public IPhotoRepository PhotoRepository => new PhotoRepository(_context);
    public IPostsRepository PostRepository => new PostRepository(_context);
    public IPostLikesRepository PostLikesRepository => new PostLikesRepository(_context);
    public IPostCommentsRepository PostCommentsRepository => new PostCommentsRepository(_context);
    public IPostPhotosRepository PostPhotosRepository => new PostPhotosRepository(_context);
    public IFeelingRepository FeelingRepository => new FeelingRepository(_context);
    public IFoodsRepository FoodsRepository => new FoodsRepository(_context);
    public IGroupsRepository GroupRepository => new GroupRepository(_context, _mapper);

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}
