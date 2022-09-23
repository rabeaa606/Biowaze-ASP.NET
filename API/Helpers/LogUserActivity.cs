
namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    // an services filter by active 
    public async Task OnActionExecutionAsync(ActionExecutingContext context,
    ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.GetUserId();
        var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
        var user = await uow.UserRepository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.UtcNow;  // make user last active equals to now time
        await uow.Complete();
    }

}
