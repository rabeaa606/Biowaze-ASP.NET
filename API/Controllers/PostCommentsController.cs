namespace API.Controllers;

public class PostCommentsController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    public PostCommentsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    [HttpGet("users-commented-post/{postId}")]
    public async Task<ActionResult<IEnumerable<PostCommentDto>>> GetUsersCommentedPost(int postId)
    {

        return Ok(await _unitOfWork.PostCommentsRepository.GetcommentOFPost(postId));
    }

    [HttpGet("user-commentes")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsCommentByUser([FromQuery] PostCommentsParams PostCommentsParams)
    {
        var posts = await _unitOfWork.PostCommentsRepository.GetPostsCommentedByUser(PostCommentsParams);

        Response.AddPaginationHeader(posts.CurrentPage,
            posts.PageSize, posts.TotalCount, posts.TotalPages);

        return Ok(posts);
    }
    [HttpGet("user-doctor-comments")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetDoctorPostsLikedByUser([FromQuery] PostCommentsParams PostCommentsParams)
    {
        var posts = await _unitOfWork.PostCommentsRepository.GetDocPostsCommentedByUser(PostCommentsParams);

        Response.AddPaginationHeader(posts.CurrentPage,
            posts.PageSize, posts.TotalCount, posts.TotalPages);

        return Ok(posts);
    }
    [HttpDelete("delete-comment/{commentId}")]
    public async Task<ActionResult> DeletePost(int commentId)
    {
        var username = User.GetUsername();

        var comment = await _unitOfWork.PostCommentsRepository.GetComment(commentId);
        if (comment == null)
            return BadRequest("Post Not Exist !!");


        _unitOfWork.PostCommentsRepository.DeletComment(comment);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting the post");
    }
}


