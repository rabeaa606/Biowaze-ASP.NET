namespace API.Data
{
    public class FoodsRepository : IFoodsRepository
    {
        private readonly DataContext _context;

        public FoodsRepository(DataContext context)
        {
            _context = context;

        }

        public void AddFood(Food food)
        {
            _context.Foods.Add(food);
        }

        public void DeleteFood(Food food)
        {
            _context.Foods.Remove(food);
        }

        public async Task<PagedList<FoodDto>> GetUserFoods(HealthParams HealthParams)
        {
            var foods = _context.Foods.Where(m => m.CreaterUsername == HealthParams.Username)
                              .OrderByDescending(m => m.FoodDate)
                              .AsQueryable();



            var foodsDto = foods.Select(food => new FoodDto
            {
                Id = food.Id,
                CreaterId = food.CreaterId,
                CreaterUsername = food.Creater.UserName,
                Value = food.Value,
                Kind = food.Kind,
                FoodDate = food.FoodDate,
            });

            return await PagedList<FoodDto>.CreateAsync(foodsDto,
                      HealthParams.PageNumber, HealthParams.PageSize);
        }

        public FoodDto[] GetFoodsAsync()
        {
            var foods = _context.Foods.OrderByDescending(m => m.FoodDate)
                              .AsQueryable();



            var foodsDto = foods.Select(food => new FoodDto
            {
                Id = food.Id,
                CreaterId = food.CreaterId,
                CreaterUsername = food.Creater.UserName,
                Value = food.Value,
                Kind = food.Kind,
                FoodDate = food.FoodDate,
            }).ToArray();

            return foodsDto;
        }
    }
}