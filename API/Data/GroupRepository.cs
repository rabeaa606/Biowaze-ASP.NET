namespace API.Data;

public class GroupRepository : IGroupsRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public GroupRepository(DataContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }
    public async void CreateGroup(MemberGroup group, UserGroup userGroup)
    {

        await _context.UserGroup.AddAsync(userGroup);
        await _context.MemberGroup.AddAsync(group);

    }
    public void AddGroupPost(MemberGroup group, Post post)
    {
        group.GroupPosts.Add(post);
    }
    public async Task<GroupDto> GetGroupAsync(string username, string groupname)
    {
        return await _context.MemberGroup
             .Where(x => x.GroupName == groupname && x.AdminUsername == username)
              .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
             .SingleOrDefaultAsync();
    }
    public async Task<PagedList<GroupDto>> GetGroups(GroupParams groupParams, AppUser user)
    {
        var usersgroups = _context.UserGroup
        .Include(g => g.Group)
        .ThenInclude(u => u.Users)
          .AsQueryable();
        var groups = _context.MemberGroup
             .Include(g => g.Users)
            .AsQueryable();

        switch (groupParams.Core)
        {
            case "My":
                usersgroups = usersgroups.Where(u => u.UserId == user.Id && u.UserRole == GroupRpg.Admin);
                break;
            case "Joined":
                usersgroups = usersgroups.Where(u => u.UserId == user.Id && u.UserRole == GroupRpg.Member);
                break;
            case "Request":
                usersgroups = usersgroups.Where(u => u.UserId == user.Id && u.UserRole == GroupRpg.Request);
                break;
            case "Others":
                groups = groups.Where(m => m.AdminId != user.Id);

                groups = groups.Where(m => m.Users.Any(u => u.UserId != user.Id));

                var temp = groups.Select(group => new GroupDto
                {
                    GroupName = group.GroupName,
                    Specialization = group.Specialization,
                    GroupId = group.Id
                });

                return await PagedList<GroupDto>.CreateAsync(temp,
                              groupParams.PageNumber, groupParams.PageSize);
            default:
                throw new ArgumentOutOfRangeException(nameof(groupParams.Core));
        };

        var groupsDto = usersgroups.Select(group => new GroupDto
        {
            GroupName = group.Group.GroupName,
            Specialization = group.Group.Specialization,
            GroupId = group.GroupId
        });


        return await PagedList<GroupDto>.CreateAsync(groupsDto,
                      groupParams.PageNumber, groupParams.PageSize);

    }
    public Boolean CheckGroupMember(PostGroupDto postGroupDto, AppUser user)
    {
        var groups = _context.MemberGroup
                          .AsQueryable();


        groups = postGroupDto.Core switch
        {
            "My" => groups.Where(m => m.Users.Any(u => u.UserId == user.Id && u.UserRole == GroupRpg.Admin)),
            "Joined" => groups.Where(m => m.Users.Any(u => u.UserId == user.Id && u.UserRole == GroupRpg.Member)),
            "Request" => null,
            "Others" => null,
            _ => throw new ArgumentOutOfRangeException(nameof(postGroupDto.Core)),
        };
        if (groups != null)

            return true;
        else
            return false;



    }
    public async Task<PagedList<MemberDto>> GetGroupMembers(GroupParams groupParams)
    {

        var group = await GetGroupBynameAsync(groupParams.Groupname);

        var members = _context.UserGroup
        .Where(m => m.GroupId == group.Id && m.UserRole == GroupRpg.Member)
        .Include(u => u.User)
        .AsQueryable();

        var membersDto = members.Select(mem => new MemberDto
        {
            Id = mem.User.Id,
            Username = mem.User.UserName,
            PhotoUrl = mem.User.Photo.FirstOrDefault(x => x.IsMain).Url,
            Age = mem.User.DateOfBirth.CalculateAge(),
            KnownAs = mem.User.KnownAs,
            Created = mem.User.Created,
            LastActive = mem.User.LastActive,
            Gender = mem.User.Gender,
            Introduction = mem.User.Introduction,
            LookingFor = mem.User.LookingFor,
            Interests = mem.User.Interests,
            City = mem.User.City,
            Country = mem.User.Country,
        });
        return await PagedList<MemberDto>.CreateAsync(membersDto,
           groupParams.PageNumber, groupParams.PageSize);
    }

    public async Task<PagedList<MemberDto>> GetGrouprequests(GroupParams groupParams)
    {

        var group = await GetGroupBynameAsync(groupParams.Groupname);

        var members = _context.UserGroup
        .Where(m => m.GroupId == group.Id && m.UserRole == GroupRpg.Request)
        .Include(u => u.User)
        .AsQueryable();

        var membersDto = members.Select(mem => new MemberDto
        {
            Id = mem.User.Id,
            Username = mem.User.UserName,
            PhotoUrl = mem.User.Photo.FirstOrDefault(x => x.IsMain).Url,
            Age = mem.User.DateOfBirth.CalculateAge(),
            KnownAs = mem.User.KnownAs,
            Created = mem.User.Created,
            LastActive = mem.User.LastActive,
            Gender = mem.User.Gender,
            Introduction = mem.User.Introduction,
            LookingFor = mem.User.LookingFor,
            Interests = mem.User.Interests,
            City = mem.User.City,
            Country = mem.User.Country,
        });
        return await PagedList<MemberDto>.CreateAsync(membersDto,
           groupParams.PageNumber, groupParams.PageSize);
    }

    public async Task<UserGroup> GetGroupAdmin(PostGroupDto postGroupDto)
    {

        var group = _context.MemberGroup
        .Where(x => x.GroupName == postGroupDto.Groupname)
        .Select(x => x.Id).FirstOrDefaultAsync();


        return await _context.UserGroup
                .SingleOrDefaultAsync(m => m.GroupId == group.Result && m.UserRole == GroupRpg.Admin);
    }

    public void DeleteGroupPost(Post post)
    {
        _context.Posts.Remove(post);
    }

    public async Task<Post> GetGroupPost(int id)
    {
        return await _context.Posts
        .Include(u => u.Creater)
        .Include(c => c.Creater.Photo)
        .Include(c => c.Photos)
        .SingleOrDefaultAsync(x => x.Id == id);
    }
    public async Task<Post> GetlastUserPost(string creater)
    {
        return await _context.Posts
        .Where(c => c.CreaterUsername == creater && c.Role == "Group")
        .Include(u => u.Creater)
        .Include(c => c.Creater.Photo)
        .Include(c => c.Photos)
        .FirstOrDefaultAsync(p => p.PostCreated == _context.Posts.Max(x => x.PostCreated));
    }

    public async Task<PagedList<PostDto>> GetGroupPosts(GroupParams groupParams, AppUser User)
    {
        var posts = _context.Posts
        .OrderByDescending(m => m.PostCreated)
       .AsQueryable();

        posts = posts.Where(u => u.SourceGroupName == groupParams.Groupname && u.Role == "Group")
              .OrderByDescending(m => m.PostCreated);

        var GroupPosts = posts.Select(post => new PostDto
        {
            CreaterUsername = post.CreaterUsername,
            CreaterKnownAs = post.Creater.KnownAs,
            CreaterPhotoUrl = post.Creater.Photo.FirstOrDefault(p => p.IsMain).Url,
            CreaterId = post.CreaterId,
            Content = post.Content,
            PostCreated = post.PostCreated,
            Id = post.Id
        });

        return await PagedList<PostDto>.CreateAsync(GroupPosts,
            groupParams.PageNumber, groupParams.PageSize);
    }
    public async Task AsktoJoin(PostGroupDto postGroupDto, AppUser user)
    {
        var Group = await GetGroupBynameAsync(postGroupDto.Groupname);
        var groupRequest = new UserGroup
        {
            User = user,
            UserId = user.Id,
            Group = Group,
            GroupId = Group.Id,
            UserRole = GroupRpg.Request,
        };
        await _context.UserGroup.AddAsync(groupRequest);

    }
    public async Task DeleteJoinAsync(PostGroupDto postGroupDto, AppUser user)
    {
        var Group = await GetGroupBynameAsync(postGroupDto.Groupname);

        var userGroup = await _context.UserGroup
      .SingleOrDefaultAsync(x => x.UserId == user.Id && x.GroupId == Group.Id);

        Group.Users.Remove(userGroup);
        user.Groups.Remove(userGroup);

        _context.UserGroup.Remove(userGroup);
    }


    public async Task AcceptUser(MemberGroup group, AppUser user)
    {
        var mygroup = await _context.UserGroup
               .SingleOrDefaultAsync(x => x.UserId == user.Id && x.GroupId == group.Id && x.UserRole == GroupRpg.Request);

        if (mygroup != null)
        {
            mygroup.UserRole = GroupRpg.Member;

            _context.UserGroup.Update(mygroup);
        }

    }


    public async Task<MemberGroup> GetGroupBynameAsync(string groupname)
    {
        return await _context.MemberGroup
      .SingleOrDefaultAsync(x => x.GroupName == groupname);
    }
    public async Task<bool> GroupExists(string groupname)
    {
        return await _context.MemberGroup.AnyAsync(x => x.GroupName == groupname.ToLower());
    }


}
