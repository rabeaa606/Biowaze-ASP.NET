namespace API.Interfaces;
public interface IFoodsRepository
{
    Task<PagedList<FoodDto>> GetUserFoods(HealthParams HealthParams);
    FoodDto[] GetFoodsAsync();

    void AddFood(Food food);
    void DeleteFood(Food food);
}
