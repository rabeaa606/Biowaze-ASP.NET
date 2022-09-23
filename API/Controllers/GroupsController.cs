namespace API.Controllers;

public class GroupsController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IPhotoService _photoService;

    public GroupsController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, IPhotoService photoService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _photoService = photoService;

    }
    [HttpPost("create")]
    public async Task<ActionResult<UserDto>> CreateGroup(GroupDto GroupDto)
    {
        var username = User.GetUsername();
        var admin = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        if (await _unitOfWork.GroupRepository.GroupExists(GroupDto.GroupName)) return BadRequest("Group Name is taken");

        var newGroup = new MemberGroup
        {
            GroupName = GroupDto.GroupName.ToLower(),
            Admin = admin,
            AdminId = admin.Id,
            AdminUsername = admin.UserName.ToLower(),
            Specialization = GroupDto.Specialization,
        };
        var userGroup = new UserGroup
        {
            UserId = admin.Id,
            User = admin,
            UserRole = GroupRpg.Admin,
            GroupId = newGroup.Id,
            Group = newGroup

        };
        if (newGroup.Admin.UserName != username)
            return Unauthorized();

        _unitOfWork.GroupRepository.CreateGroup(newGroup, userGroup);
        if (await _unitOfWork.Complete()) return Ok();
        return BadRequest("Failed to add post");
    }
    [HttpPost("add-group-post")]
    public async Task<ActionResult> AddGroupPost(PostGroupDto postGroupDto)
    {
        var username = User.GetUsername();
        var postededUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var group = await _unitOfWork.GroupRepository.GetGroupBynameAsync(postGroupDto.Groupname);

        var newpost = new Post
        {
            CreaterUsername = postededUser.UserName.ToLower(),
            CreaterId = postededUser.Id,
            Creater = postededUser,
            Content = postGroupDto.Content,
            Role = "Group",
            Group = group,
            SourceGroupId = group.Id,
            SourceGroupName = group.GroupName
        };

        if (newpost.Creater.UserName != username)
            return Unauthorized();
        if (!_unitOfWork.GroupRepository.CheckGroupMember(postGroupDto, postededUser))
        {
            return Unauthorized();
        }

        _unitOfWork.PostRepository.AddPost(newpost);
        _unitOfWork.GroupRepository.AddGroupPost(group, newpost);
        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to add post");
    }
    [HttpPost("add-group-post-photo")]
    public async Task<ActionResult<PhotoDto>> AddPostPhoto(IFormFile file)
    {
        var username = User.GetUsername();
        var postededUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        var post = await _unitOfWork.GroupRepository.GetlastUserPost(username);
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
    [HttpGet("get-group-posts")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetGroupPosts([FromQuery] GroupParams groupParams)
    {
        var username = User.GetUsername();
        var member = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        var posts = await _unitOfWork.GroupRepository.GetGroupPosts(groupParams, member);

        Response.AddPaginationHeader(posts.CurrentPage,
             posts.PageSize, posts.TotalCount, posts.TotalPages);
        return Ok(posts);
    }
    [HttpGet("get-groups")]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups([FromQuery] GroupParams groupParams)
    {
        var username = User.GetUsername();
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        groupParams.Username = username;
        var groups = await _unitOfWork.GroupRepository.GetGroups(groupParams, user);

        Response.AddPaginationHeader(groups.CurrentPage,
             groups.PageSize, groups.TotalCount, groups.TotalPages);
        return Ok(groups);
    }
    [HttpGet("GetGroup/{groupname}")]
    public async Task<ActionResult<GroupDto>> GetGroup(string groupname)
    {
        var currentUsername = User.GetUsername();
        var group = await _unitOfWork.GroupRepository.GetGroupBynameAsync(groupname);
        if (group == null) return BadRequest("Group Not Found");
        var groupDto = new GroupDto
        {
            GroupName = group.GroupName,
            Specialization = group.Specialization,
        };

        return groupDto;

    }
    [HttpGet("get-group-members")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetGroupMembers([FromQuery] GroupParams groupParams)
    {

        var members = await _unitOfWork.GroupRepository.GetGroupMembers(groupParams);

        Response.AddPaginationHeader(members.CurrentPage,
             members.PageSize, members.TotalCount, members.TotalPages);
        return Ok(members);
    }
    [HttpGet("get-group-requests")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetGrouprequests([FromQuery] GroupParams groupParams)
    {
        var username = User.GetUsername();
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var group = await _unitOfWork.GroupRepository.GetGroupBynameAsync(groupParams.Groupname);

        if (group.AdminUsername.ToLower() == username)
        {
            var requests = await _unitOfWork.GroupRepository.GetGrouprequests(groupParams);

            Response.AddPaginationHeader(requests.CurrentPage,
                 requests.PageSize, requests.TotalCount, requests.TotalPages);

            return Ok(requests);
        }
        else
        {
            return Unauthorized("Only Admin Can Accept Requests ");
        }


    }
    [HttpPost("group-join")]
    public async Task<ActionResult> AsktoJoin(PostGroupDto postGroupDto)
    {
        var username = User.GetUsername();
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        if (postGroupDto.Content != username) //Ass Username
            return Unauthorized();

        await _unitOfWork.GroupRepository.AsktoJoin(postGroupDto, user);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to add requests");
    }
    [HttpPost("group-remove-join")]
    public async Task<ActionResult> RemoveJoin(PostGroupDto postGroupDto)
    {
        var username = User.GetUsername();
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        if (postGroupDto.Content != username) //Ass Username
            return Unauthorized();

        await _unitOfWork.GroupRepository.DeleteJoinAsync(postGroupDto, user);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to add requests");
    }

    [HttpPost("group-Accept-User")]
    public async Task<ActionResult> AcceptUser(PostGroupDto postGroupDto)
    {
        var username = User.GetUsername();
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(postGroupDto.Content);
        var group = await _unitOfWork.GroupRepository.GetGroupBynameAsync(postGroupDto.Groupname);

        if (username.ToLower() != group.AdminUsername.ToLower())
            return Unauthorized("Only Admin Can Accept Requests ");

        await _unitOfWork.GroupRepository.AcceptUser(group, user);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to add requests");
    }

    [HttpPost("get-group-admin")]
    public async Task<ActionResult<MemberDto>> GetGroupAdmin(PostGroupDto postGroupDto)
    {

        var groupAdmin = await _unitOfWork.GroupRepository.GetGroupAdmin(postGroupDto);
        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(groupAdmin.UserId);
        var admin = new MemberDto
        {
            Id = user.Id,
            Username = user.UserName,

        };


        return Ok(admin);

    }
}
