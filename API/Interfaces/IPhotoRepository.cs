namespace API.Interfaces;

public interface IPhotoRepository
{
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedLicenses();

    Task<Photo> GetPhotoById(int id);
    void RemovePhoto(Photo photo);
}
