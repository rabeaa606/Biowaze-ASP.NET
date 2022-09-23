namespace API.Controllers;

[Authorize]

public class PostsController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IPhotoService _photoService;


    public PostsController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IPhotoService photoService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _photoService = photoService;

    }

    [HttpGet("get-posts")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetUsersPosts([FromQuery] PostParams PostsParams)
    {
        var username = User.GetUsername();
        PostsParams.Username = username;

        var posts = await _unitOfWork.PostRepository.GetUsersPosts(PostsParams);

        Response.AddPaginationHeader(posts.CurrentPage,
             posts.PageSize, posts.TotalCount, posts.TotalPages);
        return Ok(posts);
    }
    [HttpGet("get-doctor-posts")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetDoctorsPosts([FromQuery] PostParams PostsParams)
    {
        var username = User.GetUsername();
        PostsParams.Username = username;

        var posts = await _unitOfWork.PostRepository.GetDoctorsPosts(PostsParams);

        Response.AddPaginationHeader(posts.CurrentPage,
             posts.PageSize, posts.TotalCount, posts.TotalPages);
        return Ok(posts);
    }


    [HttpDelete("delete-post/{postId}")]
    public async Task<ActionResult> DeletePost(int postId)
    {
        var username = User.GetUsername();

        var post = await _unitOfWork.PostRepository.GetPost(postId);
        if (post == null)
            return BadRequest("Post Not Exist !!");

        if (post.CreaterUsername != username)
            return Unauthorized();

        _unitOfWork.PostRepository.DeletePost(post);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting the post");
    }
    /*    [HttpPost("add-post")]
       public async Task<ActionResult<UserDto>> AddPost(PostDto postContent)
    */
    [HttpPost("add-post")]
    public async Task<ActionResult<UserDto>> AddPost(PostDto postContent)
    {
        var username = User.GetUsername();
        var postededUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        var newpost = new Post
        {
            CreaterUsername = postededUser.UserName.ToLower(),
            CreaterId = postededUser.Id,
            Creater = postededUser,
            Content = postContent.Content,
            Role = "Member"
        };


        if (newpost.Creater.UserName != username)
            return Unauthorized();



        _unitOfWork.PostRepository.AddPost(newpost);


        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to add post");
    }

    [HttpPost("add-doctor-post/{content}")]
    public async Task<ActionResult<int>> AddDoctorPost(string content)
    {
        var username = User.GetUsername();
        var postededUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        var newpost = new Post
        {
            CreaterUsername = postededUser.UserName.ToLower(),
            CreaterId = postededUser.Id,
            Creater = postededUser,
            Content = content,
            Role = "Doctor"
        };


        if (newpost.Creater.UserName != username)
            return Unauthorized();

        _unitOfWork.PostRepository.AddPost(newpost);


        if (await _unitOfWork.Complete()) return Ok(newpost.Id);

        return BadRequest("Failed to add post");
    }

    [HttpPost("add-post-photo")]
    public async Task<ActionResult<PhotoDto>> AddPostPhoto(IFormFile file)
    {
        var username = User.GetUsername();
        var postededUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        var post = await _unitOfWork.PostRepository.GetlastPost(username);
        if (post == null)
            return BadRequest("Post Not Exist !!");


        var result = await _photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);

        var newpostPhotos = new PostPhoto
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            SourcePost = post,
            SourcePostId = post.Id
        };

        post.Photos.Add(newpostPhotos);

        var postDto = new PhotoDto
        {
            Id = newpostPhotos.Id,
            Url = result.SecureUrl.AbsoluteUri,
        };

        if (await _unitOfWork.Complete()) return Ok(postDto);

        return BadRequest("Failed to add post");
    }


}
