namespace API.Interfaces;

public interface IPostPhotosRepository
{
    void AddPhoto(PostPhoto postPhoto);
    // void DeletePhoto(PostPhoto postPhoto);
    Task<PostPhoto> GetPhoto(int id);
    Task<IEnumerable<PostPhotoDto>> GetPhotosOFPost(int postId);
}
