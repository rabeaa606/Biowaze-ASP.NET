namespace API.Controllers;

[Authorize]
public class HealthController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public HealthController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;

    }
    [HttpPost("add-food/{food}")]
    public async Task<ActionResult<UserDto>> AddFood(string food)
    {
        var username = User.GetUsername();
        var foodUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);


        if (food == "undefined")
        {
            return BadRequest("Failed to Empty Food");

        }
        var newfood = new Food
        {
            CreaterUsername = foodUser.UserName.ToLower(),
            CreaterId = foodUser.Id,
            Creater = foodUser,
            Value = food,
            Kind = food
        };


        if (newfood.Creater.UserName != username)
            return Unauthorized();

        _unitOfWork.FoodsRepository.AddFood(newfood);
        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to add Food");
    }
    [HttpGet("get-foods")]
    public async Task<ActionResult<IEnumerable<FoodDto>>> GetUsersFoods([FromQuery] HealthParams HealthParams)
    {
        var username = User.GetUsername();
        HealthParams.Username = username;

        var foods = await _unitOfWork.FoodsRepository.GetUserFoods(HealthParams);

        Response.AddPaginationHeader(foods.CurrentPage,
             foods.PageSize, foods.TotalCount, foods.TotalPages);
        return Ok(foods);
    }

    [HttpPost("add-feeling/{feeling}")]
    public async Task<ActionResult<UserDto>> AddFeeling(string feeling)
    {
        var username = User.GetUsername();
        var foodUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        if (feeling == "undefined")
        {
            return BadRequest("Failed to Empty feeling");

        }
        var newfeeling = new Feeling
        {
            CreaterUsername = foodUser.UserName.ToLower(),
            CreaterId = foodUser.Id,
            Creater = foodUser,
            Value = feeling,
        };


        if (newfeeling.Creater.UserName != username)
            return Unauthorized();

        _unitOfWork.FeelingRepository.AddFeeling(newfeeling);
        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to add Feeling");
    }
    [HttpGet("get-feelings")]
    public async Task<ActionResult<IEnumerable<FeelingDto>>> GetUsersFeelings([FromQuery] HealthParams HealthParams)
    {
        var username = User.GetUsername();
        HealthParams.Username = username;

        var feelings = await _unitOfWork.FeelingRepository.GetUserFeelings(HealthParams);

        Response.AddPaginationHeader(feelings.CurrentPage,
             feelings.PageSize, feelings.TotalCount, feelings.TotalPages);
        return Ok(feelings);
    }
    [HttpPost("check-food-feeling/{food}")]
    public ActionResult<IEnumerable<HealthDto>> CheckFeelingsAfterFood(string food)
    {
        List<HealthDto> Feelings = new List<HealthDto>();
        var HealthFeeling = "";



        var foods = _unitOfWork.FoodsRepository.GetFoodsAsync();
        var feelings = _unitOfWork.FeelingRepository.GetFeelings();

        var n1 = 0;
        var n2 = 0;
        var n3 = 0;
        var n4 = 0;
        var Exp_n1 = 0;
        var Exp_n2 = 0;
        var Exp_n3 = 0;
        var Exp_n4 = 0;
        var Sum_Row_1 = 0;
        var Sum_Row_2 = 0;
        var Sum_Col_1 = 0;
        var Sum_Col_2 = 0;
        var Sum = 0;
        var Result = 0;

        for (int l = 0; l < feelings.Length; l++)
        {
            HealthFeeling = "";
            HealthFeeling = feelings[l].Value;
            n1 = 0;
            n2 = 0;
            n3 = 0;
            n4 = 0;
            Exp_n1 = 0;
            Exp_n2 = 0;
            Exp_n3 = 0;
            Exp_n4 = 0;
            Sum_Row_1 = 0;
            Sum_Row_2 = 0;
            Sum_Col_1 = 0;
            Sum_Col_2 = 0;
            Sum = 0;
            Result = 0;

            for (int i = 0; i < foods.Length; i++)
            {
                for (int j = 0; j < feelings.Length; j++)
                {
                    string feelyear = DateTime.Parse(feelings[j].FeelingDate.ToString()).Year.ToString();
                    string foodyear = DateTime.Parse(foods[i].FoodDate.ToString()).Year.ToString();

                    string feelMonth = DateTime.Parse(feelings[j].FeelingDate.ToString()).Month.ToString();
                    string foodMonth = DateTime.Parse(foods[i].FoodDate.ToString()).Month.ToString();

                    string feelDay = DateTime.Parse(feelings[j].FeelingDate.ToString()).Day.ToString();
                    string foodDay = DateTime.Parse(foods[i].FoodDate.ToString()).Day.ToString();
                    // Console.WriteLine("foods[i].Value ---------->" + foods[i].Value);
                    // Console.WriteLine("food ---------->" + food);
                    // Console.WriteLine("feelings[j].Value ---------->" + feelings[j].Value);
                    // Console.WriteLine("HealthFeeling ---------->" + HealthFeeling);

                    if (feelings[j].CreaterUsername.ToString() == foods[i].CreaterUsername.ToString())
                        if (feelyear == foodyear)
                            if (feelMonth == foodMonth)
                                if ((Convert.ToInt32(feelDay) - Convert.ToInt32(foodDay)) <= 2 && (Convert.ToInt32(feelDay) - Convert.ToInt32(foodDay)) >= 0)
                                {
                                    if (foods[i].Value == food && feelings[j].Value == HealthFeeling)
                                        n1++;

                                    if (foods[i].Value != food && feelings[j].Value == HealthFeeling)
                                        n2++;

                                    if (foods[i].Value == food && feelings[j].Value != HealthFeeling)
                                        n3++;

                                    if (foods[i].Value != food && feelings[j].Value != HealthFeeling)
                                        n4++;
                                }
                }
            }



            Sum_Row_1 = n1 + n2;
            Sum_Row_2 = n3 + n4;
            Sum_Col_1 = n1 + n3;
            Sum_Col_2 = n2 + n4;

            Sum = Sum_Col_1 + Sum_Col_2;

            ///// (col * row) / sum
            if (Sum != 0)
            {
                Exp_n1 = (Sum_Col_1 * Sum_Row_1) / Sum;
                Exp_n2 = (Sum_Col_2 * Sum_Row_1) / Sum;
                Exp_n3 = (Sum_Col_1 * Sum_Row_2) / Sum;
                Exp_n4 = (Sum_Col_2 * Sum_Row_2) / Sum;
            }



            //// CHI Squre 
            if (Exp_n1 != 0 && Exp_n2 != 0 && Exp_n3 != 0 && Exp_n4 != 0)
            {
                Result = ((n1 - Exp_n1) * (n1 - Exp_n1)) / Exp_n1 +
               ((n2 - Exp_n2) * (n2 - Exp_n2)) / Exp_n2 +
               ((n3 - Exp_n3) * (n3 - Exp_n3)) / Exp_n3 +
               ((n4 - Exp_n4) * (n4 - Exp_n4)) / Exp_n4;
            }



            if (Result >= 2.706)
            {
                var AnFeeling = new HealthDto
                {
                    Value = HealthFeeling,
                };

                Feelings.Add(AnFeeling);
                AnFeeling = null;
            }
        }

        return Ok(Feelings);
    }

    [HttpPost("check")]
    public ActionResult<IEnumerable<HealthDto>> CheckDependency(HealthCareDto healthCareDto)
    {

        var foods = _unitOfWork.FoodsRepository.GetFoodsAsync();
        var feelings = _unitOfWork.FeelingRepository.GetFeelings();

        var n1 = 0;
        var n2 = 0;
        var n3 = 0;
        var n4 = 0;
        var Exp_n1 = 0;
        var Exp_n2 = 0;
        var Exp_n3 = 0;
        var Exp_n4 = 0;
        var Sum_Row_1 = 0;
        var Sum_Row_2 = 0;
        var Sum_Col_1 = 0;
        var Sum_Col_2 = 0;
        var Sum = 0;
        var Result = 0;


        for (int i = 0; i < foods.Length; i++)
            for (int j = 0; j < feelings.Length; j++)
            {
                string feelyear = DateTime.Parse(feelings[j].FeelingDate.ToString()).Year.ToString();
                string foodyear = DateTime.Parse(foods[i].FoodDate.ToString()).Year.ToString();

                string feelMonth = DateTime.Parse(feelings[j].FeelingDate.ToString()).Month.ToString();
                string foodMonth = DateTime.Parse(foods[i].FoodDate.ToString()).Month.ToString();

                string feelDay = DateTime.Parse(feelings[j].FeelingDate.ToString()).Day.ToString();
                string foodDay = DateTime.Parse(foods[i].FoodDate.ToString()).Day.ToString();


                if (feelings[j].CreaterUsername.ToString() == foods[i].CreaterUsername.ToString())
                    if (feelyear == foodyear)
                        if (feelMonth == foodMonth)
                            if ((Convert.ToInt32(feelDay) - Convert.ToInt32(foodDay)) <= 2 && (Convert.ToInt32(feelDay) - Convert.ToInt32(foodDay)) >= 0)
                            {
                                if (foods[i].Value == healthCareDto.Food && feelings[j].Value == healthCareDto.Feeling)
                                    n1++;

                                if (foods[i].Value != healthCareDto.Food && feelings[j].Value == healthCareDto.Feeling)
                                    n2++;

                                if (foods[i].Value == healthCareDto.Food && feelings[j].Value != healthCareDto.Feeling)
                                    n3++;

                                if (foods[i].Value != healthCareDto.Food && feelings[j].Value != healthCareDto.Feeling)
                                    n4++;
                            }
            }

        Sum_Row_1 = n1 + n2;
        Sum_Row_2 = n3 + n4;
        Sum_Col_1 = n1 + n3;
        Sum_Col_2 = n2 + n4;

        Sum = Sum_Col_1 + Sum_Col_2;

        ///// (col * row) / sum
        if (Sum != 0)
        {
            Exp_n1 = (Sum_Col_1 * Sum_Row_1) / Sum;
            Exp_n2 = (Sum_Col_2 * Sum_Row_1) / Sum;
            Exp_n3 = (Sum_Col_1 * Sum_Row_2) / Sum;
            Exp_n4 = (Sum_Col_2 * Sum_Row_2) / Sum;
        }



        //// CHI Squre 
        if (Exp_n1 != 0 && Exp_n2 != 0 && Exp_n3 != 0 && Exp_n4 != 0)
        {
            Result = ((n1 - Exp_n1) * (n1 - Exp_n1)) / Exp_n1 +
           ((n2 - Exp_n2) * (n2 - Exp_n2)) / Exp_n2 +
           ((n3 - Exp_n3) * (n3 - Exp_n3)) / Exp_n3 +
           ((n4 - Exp_n4) * (n4 - Exp_n4)) / Exp_n4;
        }

        var res = new HealthDto
        {
            Value = "Not Checked"
        };

        if (Result == 0) return Ok(res);
        else
        {
            if (Result >= 2.706)
            {
                res.Value = "true";
                return Ok(res);
            }
            else
            {
                res.Value = "false";
                return Ok(res);
            }
        }



    }
}
