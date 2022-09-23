namespace API.Data;

public class FeelingRepository : IFeelingRepository
{
    private readonly DataContext _context;

    public FeelingRepository(DataContext context)
    {
        _context = context;

    }

    public void AddFeeling(Feeling feel)
    {
        _context.Feelings.Add(feel);
    }

    public void DeleteFeeling(Feeling feel)
    {
        _context.Feelings.Remove(feel);
    }

    public async Task<PagedList<FeelingDto>> GetUserFeelings(HealthParams HealthParams)
    {
        var feelings = _context.Feelings
           .OrderByDescending(m => m.FeelingDate).Where(m => m.CreaterUsername == HealthParams.Username)
            .AsQueryable();



        var feelingsDto = feelings.Select(food => new FeelingDto
        {
            Id = food.Id,
            CreaterId = food.CreaterId,
            CreaterUsername = food.Creater.UserName,
            Value = food.Value,
            FeelingDate = food.FeelingDate,
        });

        return await PagedList<FeelingDto>.CreateAsync(feelingsDto,
                  HealthParams.PageNumber, HealthParams.PageSize);
    }
    public FeelingDto[] GetFeelings()
    {
        var feelings = _context.Feelings
           .OrderByDescending(m => m.FeelingDate)
            .AsQueryable();



        var feelingsDto = feelings.Select(food => new FeelingDto
        {
            Id = food.Id,
            CreaterId = food.CreaterId,
            CreaterUsername = food.Creater.UserName,
            Value = food.Value,
            FeelingDate = food.FeelingDate,
        }).ToArray();

        return feelingsDto;
    }
}
