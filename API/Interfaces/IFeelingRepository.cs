namespace API.Interfaces;

public interface IFeelingRepository
{
    Task<PagedList<FeelingDto>> GetUserFeelings(HealthParams HealthParams);
    FeelingDto[] GetFeelings();
    void AddFeeling(Feeling feel);
    void DeleteFeeling(Feeling feel);
}
