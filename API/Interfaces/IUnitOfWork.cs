namespace API.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IMessageRepository MessageRepository { get; }
    IPhotoRepository PhotoRepository { get; }
    IPostsRepository PostRepository { get; }
    IFeelingRepository FeelingRepository { get; }
    IFoodsRepository FoodsRepository { get; }

    IPostLikesRepository PostLikesRepository { get; }
    IPostCommentsRepository PostCommentsRepository { get; }
    IPostPhotosRepository PostPhotosRepository { get; }
    IGroupsRepository GroupRepository { get; }

    Task<bool> Complete();
    bool HasChanges();
}
